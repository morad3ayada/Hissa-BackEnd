using LearningPlatform.Application.Features.Teachers.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Commands;

public record CreateUnavailableSlotCommand : IRequest<ApiResponse<UnavailableSlotDto>>
{
    public string Date { get; init; } = string.Empty;
    public string StartTime { get; init; } = string.Empty;
    public string EndTime { get; init; } = string.Empty;
    public string? Reason { get; init; }
}
