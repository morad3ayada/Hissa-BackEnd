using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Queries;

public record GetCourseStudentsQuery : IRequest<PaginatedResponse<CourseStudentDto>>
{
    public Guid CourseId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
