using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Queries;

public record GetAllEnrollmentsQuery : IRequest<PaginatedResponse<EnrollmentDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid? CourseId { get; init; }
    public Guid? StudentId { get; init; }
    public EnrollmentStatus? Status { get; init; }
}
