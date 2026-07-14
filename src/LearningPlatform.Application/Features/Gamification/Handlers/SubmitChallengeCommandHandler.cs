using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Gamification.Commands;
using LearningPlatform.Application.Features.Gamification.DTOs;
using LearningPlatform.Application.Features.Gamification.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Gamification.Handlers;

public class SubmitChallengeCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IGamificationService gamificationService,
    INotificationService notificationService)
    : IRequestHandler<SubmitChallengeCommand, ApiResponse<ChallengeDto>>
{
    public async Task<ApiResponse<ChallengeDto>> Handle(SubmitChallengeCommand request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var challenge = await unitOfWork.Repository<Challenge>().AsQueryable()
            .Include(c => c.Challenger)
            .Include(c => c.Opponent)
            .Include(c => c.Winner)
            .Include(c => c.Quiz).ThenInclude(q => q.Questions).ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(c => c.Id == request.ChallengeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Challenge), request.ChallengeId);

        if (challenge.ChallengerId != studentId && challenge.OpponentId != studentId)
            throw new ForbiddenException("You are not a participant in this challenge.");

        if (challenge.Status != ChallengeStatus.InProgress)
            throw new BadRequestException("This challenge is not currently active.");

        if (challenge.AcceptedAt.HasValue && DateTime.UtcNow > challenge.AcceptedAt.Value.AddMinutes(challenge.DurationInMinutes))
        {
            challenge.Status = ChallengeStatus.Expired;
            unitOfWork.Repository<Challenge>().Update(challenge);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new BadRequestException("The time limit for this challenge has expired.");
        }

        var myStudentChallenge = await unitOfWork.Repository<StudentChallenge>().GetTrackedAsync(
            sc => sc.ChallengeId == challenge.Id && sc.StudentId == studentId, cancellationToken)
            ?? throw new NotFoundException(nameof(StudentChallenge), studentId);

        if (myStudentChallenge.Status == ChallengeStatus.Completed)
            throw new BadRequestException("You have already submitted your answers for this challenge.");

        var quizQuestionIds = challenge.Quiz.Questions.Select(q => q.Id).ToHashSet();
        if (request.Answers.Any(a => !quizQuestionIds.Contains(a.QuestionId)))
            throw new BadRequestException("One or more submitted answers do not belong to this challenge's quiz.");

        var submittedByQuestion = request.Answers.ToDictionary(a => a.QuestionId, a => a.SelectedAnswerId);
        var totalPoints = challenge.Quiz.Questions.Sum(q => q.Points);
        var earnedPoints = 0;

        foreach (var question in challenge.Quiz.Questions)
        {
            submittedByQuestion.TryGetValue(question.Id, out var selectedAnswerId);
            var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);
            if (selectedAnswerId.HasValue && selectedAnswerId.Value == correctAnswer?.Id)
                earnedPoints += question.Points;
        }

        var myScore = totalPoints > 0 ? Math.Round(earnedPoints * 100m / totalPoints, 2) : 0;

        myStudentChallenge.Score = myScore;
        myStudentChallenge.Status = ChallengeStatus.Completed;
        myStudentChallenge.Progress = 100;
        myStudentChallenge.CompletedAt = DateTime.UtcNow;
        unitOfWork.Repository<StudentChallenge>().Update(myStudentChallenge);

        var otherStudentId = studentId == challenge.ChallengerId ? challenge.OpponentId : challenge.ChallengerId;
        var opponentStudentChallenge = (await unitOfWork.Repository<StudentChallenge>().FindAsync(
            sc => sc.ChallengeId == challenge.Id && sc.StudentId == otherStudentId, cancellationToken)).FirstOrDefault();

        decimal? opponentScore = null;

        if (opponentStudentChallenge is { Status: ChallengeStatus.Completed })
        {
            opponentScore = opponentStudentChallenge.Score;

            Guid? winnerId = myScore > opponentStudentChallenge.Score ? studentId
                : opponentStudentChallenge.Score > myScore ? otherStudentId
                : null; // a tie: no winner, no bonus points awarded

            challenge.Status = ChallengeStatus.Completed;
            challenge.CompletedAt = DateTime.UtcNow;
            challenge.WinnerId = winnerId;
            // DTO-only fixup: Update() persists via the WinnerId scalar, not this navigation —
            // it just needs to be set so ChallengeDtoBuilder can read WinnerName below.
            challenge.Winner = winnerId == challenge.ChallengerId ? challenge.Challenger
                : winnerId == challenge.OpponentId ? challenge.Opponent
                : null;
            unitOfWork.Repository<Challenge>().Update(challenge);

            if (winnerId.HasValue)
            {
                await gamificationService.AwardPointsAsync(
                    winnerId.Value, challenge.PointsReward, PointsReason.ChallengeWon, challenge.Id,
                    cancellationToken: cancellationToken);

                await notificationService.CreateAsync(
                    winnerId.Value, NotificationType.Achievement, "You won a challenge!",
                    $"You won the challenge \"{challenge.Title}\" and earned {challenge.PointsReward} points.",
                    cancellationToken: cancellationToken);

                if (challenge.RewardId.HasValue)
                {
                    var alreadyGranted = await unitOfWork.Repository<StudentReward>().ExistsAsync(
                        sr => sr.StudentId == winnerId.Value && sr.SourceChallengeId == challenge.Id, cancellationToken);

                    if (!alreadyGranted)
                    {
                        await unitOfWork.Repository<StudentReward>().AddAsync(new StudentReward
                        {
                            StudentId = winnerId.Value,
                            RewardId = challenge.RewardId.Value,
                            SourceChallengeId = challenge.Id,
                            EarnedAt = DateTime.UtcNow
                        }, cancellationToken);
                    }
                }
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = ChallengeDtoBuilder.Build(challenge, myScore, opponentScore);

        return ApiResponse<ChallengeDto>.Success(dto, "Challenge submission recorded.");
    }
}
