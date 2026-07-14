using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Queries;

public record GetLessonQuizzesQuery(Guid LessonId) : IRequest<ApiResponse<List<QuizSummaryDto>>>;
