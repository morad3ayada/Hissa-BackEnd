using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.ErrorBanks.DTOs;
using LearningPlatform.Application.Features.ErrorBanks.Queries;
using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Application.Features.Quizzes.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.ErrorBanks.Handlers;

public class GetMyErrorsQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IQuizAuthorizationService quizAuthorization)
    : IRequestHandler<GetMyErrorsQuery, ApiResponse<List<ErrorBankEntryDto>>>
{
    public async Task<ApiResponse<List<ErrorBankEntryDto>>> Handle(GetMyErrorsQuery request, CancellationToken cancellationToken)
    {
        var studentId = currentUser.UserId!.Value;

        var entriesQuery = unitOfWork.Repository<ErrorBank>().AsQueryable()
            .Include(e => e.Question).ThenInclude(q => q.Answers)
            .Include(e => e.Question).ThenInclude(q => q.Quiz)
            .Where(e => e.StudentId == studentId);

        if (!request.IncludeResolved)
            entriesQuery = entriesQuery.Where(e => !e.IsResolved);

        var entries = await entriesQuery
            .OrderByDescending(e => e.LastMistakeAt)
            .ToListAsync(cancellationToken);

        var courseCache = new Dictionary<Guid, Course>();

        var dtos = new List<ErrorBankEntryDto>();
        foreach (var entry in entries)
        {
            var quiz = entry.Question.Quiz;
            if (!courseCache.TryGetValue(quiz.Id, out var course))
            {
                course = await quizAuthorization.GetQuizCourseAsync(quiz, cancellationToken);
                courseCache[quiz.Id] = course;
            }

            dtos.Add(new ErrorBankEntryDto
            {
                Id = entry.Id,
                QuestionId = entry.QuestionId,
                QuestionText = entry.Question.Text,
                Answers = entry.Question.Answers
                    .OrderBy(a => a.Order)
                    .Select(a => new AnswerDto { Id = a.Id, Text = a.Text, Order = a.Order, IsCorrect = null })
                    .ToList(),
                MistakeCount = entry.MistakeCount,
                LastMistakeAt = entry.LastMistakeAt,
                IsResolved = entry.IsResolved,
                ResolvedAt = entry.ResolvedAt,
                LessonId = entry.LessonId,
                CourseId = course.Id,
                CourseTitle = course.Title
            });
        }

        return ApiResponse<List<ErrorBankEntryDto>>.Success(dtos);
    }
}
