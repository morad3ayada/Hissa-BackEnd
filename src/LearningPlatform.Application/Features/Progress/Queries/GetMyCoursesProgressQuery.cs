using LearningPlatform.Application.Features.Progress.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Progress.Queries;

public record GetMyCoursesProgressQuery : IRequest<PaginatedResponse<CourseProgressSummaryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
