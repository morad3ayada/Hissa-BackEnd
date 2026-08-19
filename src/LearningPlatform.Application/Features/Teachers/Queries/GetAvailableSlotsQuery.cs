using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Queries;

public record GetAvailableSlotsQuery : IRequest<ApiResponse<List<AvailableSlotDto>>>
{
    public Guid TeacherId { get; init; }
    public string Date { get; init; } = string.Empty;
    public int DurationInMinutes { get; init; } = 60;
}
