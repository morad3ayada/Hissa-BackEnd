using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Queries;

public record GetCourseQuizzesQuery(Guid CourseId) : IRequest<ApiResponse<List<QuizSummaryDto>>>;
