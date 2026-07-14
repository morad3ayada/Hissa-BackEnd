using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Queries;

public record GetMyEnrollmentsQuery : IRequest<PaginatedResponse<EnrollmentDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
