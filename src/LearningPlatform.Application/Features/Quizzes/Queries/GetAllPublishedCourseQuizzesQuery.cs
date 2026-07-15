using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Quizzes.Queries;

/// <summary>Returns all PUBLISHED quizzes for a course across all scopes/lessons — used for challenge creation.</summary>
public record GetAllPublishedCourseQuizzesQuery(Guid CourseId) : IRequest<ApiResponse<List<QuizSummaryDto>>>;
