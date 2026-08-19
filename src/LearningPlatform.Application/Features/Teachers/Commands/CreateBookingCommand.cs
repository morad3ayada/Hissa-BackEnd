using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Teachers.Commands;

public record CreateBookingCommand : IRequest<ApiResponse>
{
    public Guid TeacherId { get; init; }
    public string Date { get; init; } = string.Empty;
    public string StartTime { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public int DurationInMinutes { get; init; } = 60;
    public string? Notes { get; init; }
}
