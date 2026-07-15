using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Commands;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Quizzes.Handlers;

public class SubmitQuizCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IQuizAuthorizationService quizAuthorization,
    IGamificationService gamificationService,
    ICourseCompletionService courseCompletionService)
    : IRequestHandler<SubmitQuizCommand, ApiResponse<QuizResultDto>>
{
    private static readonly TimeSpan TimeLimitGrace = TimeSpan.FromSeconds(60);
    private const int QuizPassedPoints = 15;
    private const int FinalExamPassedPoints = 100;

    public async Task<ApiResponse<QuizResultDto>> Handle(SubmitQuizCommand request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var quiz = await unitOfWork.Repository<Quiz>().AsQueryable()
            .Include(q => q.Questions).ThenInclude(qs => qs.Answers)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken)
            ?? throw new NotFoundException(nameof(Quiz), request.QuizId);

        if (!quiz.IsPublished)
            throw new BadRequestException("This quiz is not published yet.");

        await quizAuthorization.EnsureCanTakeQuizAsync(quiz, cancellationToken);

        var previousAttempts = await unitOfWork.Repository<QuizResult>()
            .FindAsync(r => r.QuizId == quiz.Id && r.StudentId == studentId, cancellationToken);

        if (previousAttempts.Count >= (quiz.MaxAttempts ?? 1))
            throw new BadRequestException("You have reached the maximum number of attempts allowed for this quiz.");

        if (quiz.TimeLimitInMinutes.HasValue)
        {
            var elapsed = DateTime.UtcNow - request.StartedAt;
            if (elapsed > TimeSpan.FromMinutes(quiz.TimeLimitInMinutes.Value) + TimeLimitGrace)
                throw new BadRequestException("The time limit for this quiz attempt has been exceeded.");
        }

        var quizQuestionIds = quiz.Questions.Select(q => q.Id).ToHashSet();
        if (request.Answers.Any(a => !quizQuestionIds.Contains(a.QuestionId)))
            throw new BadRequestException("One or more submitted answers do not belong to this quiz.");

        var submittedByQuestion = request.Answers.ToDictionary(a => a.QuestionId, a => a.SelectedAnswerId);

        var result = new QuizResult
        {
            QuizId = quiz.Id,
            StudentId = studentId,
            AttemptNumber = previousAttempts.Count + 1,
            StartedAt = request.StartedAt,
            CompletedAt = DateTime.UtcNow,
            TotalQuestions = quiz.Questions.Count
        };

        var totalPoints = quiz.Questions.Sum(q => q.Points);
        var earnedPoints = 0;
        var correctCount = 0;

        foreach (var question in quiz.Questions)
        {
            submittedByQuestion.TryGetValue(question.Id, out var selectedAnswerId);
            var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);
            var isCorrect = selectedAnswerId.HasValue && selectedAnswerId.Value == correctAnswer?.Id;

            // QuestionId (scalar FK), not Question (navigation): quiz/question came from an
            // AsNoTracking() query, so assigning the navigation would drag that untracked graph
            // into this AddAsync's reachability set and EF would try to re-insert the existing Quiz.
            var studentAnswer = new StudentAnswer
            {
                QuestionId = question.Id,
                SelectedAnswerId = selectedAnswerId,
                StudentId = studentId,
                IsCorrect = isCorrect,
                AnsweredAt = DateTime.UtcNow
            };

            result.StudentAnswers.Add(studentAnswer);

            if (isCorrect)
            {
                correctCount++;
                earnedPoints += question.Points;
                continue;
            }

            var errorBankEntry = (await unitOfWork.Repository<ErrorBank>()
                .FindAsync(e => e.StudentId == studentId && e.QuestionId == question.Id, cancellationToken))
                .FirstOrDefault();

            if (errorBankEntry is null)
            {
                errorBankEntry = new ErrorBank
                {
                    StudentId = studentId,
                    QuestionId = question.Id,
                    LessonId = quiz.LessonId,
                    MistakeCount = 1,
                    LastMistakeAt = DateTime.UtcNow,
                    IsResolved = false,
                    StudentAnswer = studentAnswer
                };
                await unitOfWork.Repository<ErrorBank>().AddAsync(errorBankEntry, cancellationToken);
            }
            else
            {
                errorBankEntry.MistakeCount++;
                errorBankEntry.LastMistakeAt = DateTime.UtcNow;
                errorBankEntry.IsResolved = false;
                errorBankEntry.ResolvedAt = null;
                errorBankEntry.StudentAnswer = studentAnswer;
                unitOfWork.Repository<ErrorBank>().Update(errorBankEntry);
            }
        }

        result.CorrectAnswers = correctCount;
        result.Score = totalPoints > 0 ? Math.Round(earnedPoints * 100m / totalPoints, 2) : 0;
        result.IsPassed = result.Score >= quiz.PassingScore;

        await unitOfWork.Repository<QuizResult>().AddAsync(result, cancellationToken);

        if (quiz.IsFinalExam && result.IsPassed && quiz.CourseId.HasValue)
            await courseCompletionService.TryGrantCourseCompletionAsync(studentId, quiz.CourseId.Value, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Gamification hooks run after the save above so the queries they issue (fresh,
        // AsNoTracking) see this attempt as committed, not still-pending.
        if (result.IsPassed)
        {
            await gamificationService.AwardPointsAsync(
                studentId, QuizPassedPoints, PointsReason.QuizPassed, quiz.Id, cancellationToken: cancellationToken);

            var passedQuizzesCount = await unitOfWork.Repository<QuizResult>()
                .ExistsAsync(r => r.StudentId == studentId && r.IsPassed && r.Id != result.Id, cancellationToken);

            if (!passedQuizzesCount)
                await gamificationService.CheckAchievementAsync(studentId, AchievementTriggerType.FirstQuizPassed, cancellationToken);

            if (result.Score >= 100)
                await gamificationService.CheckAchievementAsync(studentId, AchievementTriggerType.PerfectQuizScore, cancellationToken);

            if (quiz.IsFinalExam)
            {
                await gamificationService.AwardPointsAsync(
                    studentId, FinalExamPassedPoints, PointsReason.FinalExamPassed, quiz.Id, cancellationToken: cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Fix up navigations for DTO construction only, now that SaveChanges is done — safe
        // because there are no more pending inserts for these reference assignments to corrupt.
        foreach (var studentAnswer in result.StudentAnswers)
        {
            var question = quiz.Questions.First(q => q.Id == studentAnswer.QuestionId);
            studentAnswer.Question = question;
            studentAnswer.SelectedAnswer = studentAnswer.SelectedAnswerId.HasValue
                ? question.Answers.FirstOrDefault(a => a.Id == studentAnswer.SelectedAnswerId.Value)
                : null;
        }

        var dto = QuizDtoBuilder.BuildResult(result, quiz.Title);

        return ApiResponse<QuizResultDto>.Success(dto, result.IsPassed ? "Quiz submitted — you passed!" : "Quiz submitted.");
    }
}
