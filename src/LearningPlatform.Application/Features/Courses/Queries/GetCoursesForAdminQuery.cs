using LearningPlatform.Application.Features.Courses.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Courses.Queries;

public record GetCoursesForAdminQuery : IRequest<PaginatedResponse<CourseSummaryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public CourseStatus? Status { get; init; }
    public Guid? InstructorId { get; init; }
    public string? Search { get; init; }
}
