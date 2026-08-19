using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Commands;

public record UpdateAvailabilityCommand : IRequest<ApiResponse>
{
    public List<TeacherAvailabilityDto> Availability { get; init; } = [];
}
