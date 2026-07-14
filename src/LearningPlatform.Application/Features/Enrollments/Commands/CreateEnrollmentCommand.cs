using LearningPlatform.Application.Features.Enrollments.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Commands;

public record CreateEnrollmentCommand : IRequest<ApiResponse<EnrollmentDto>>
{
    public Guid CourseId { get; init; }
}
