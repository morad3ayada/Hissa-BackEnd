using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Courses.Queries;

public record GetCoursesForAdminQuery : IRequest<ApiResponse<List<CourseSummaryDto>>>;