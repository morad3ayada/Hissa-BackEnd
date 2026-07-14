using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Courses.Queries;

public record GetAllCoursesQuery : IRequest<PaginatedResponse<CourseSummaryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
    public string? Category { get; init; }
}
