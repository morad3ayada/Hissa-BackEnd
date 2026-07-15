using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Courses.Queries;

public record GetAllCoursesQuery : IRequest<ApiResponse<List<CourseSummaryDto>>>;