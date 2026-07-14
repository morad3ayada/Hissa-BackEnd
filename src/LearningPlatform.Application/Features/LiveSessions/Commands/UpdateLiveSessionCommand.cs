using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.LiveSessions.Commands;

public record UpdateLiveSessionCommand : IRequest<ApiResponse<LiveSessionDto>>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public LiveMeetingPlatform MeetingPlatform { get; init; }
    public string MeetingLink { get; init; } = string.Empty;
    public string? MeetingPassword { get; init; }
    public DateTime StartDateTime { get; init; }
    public DateTime EndDateTime { get; init; }

    /// <summary>Only Scheduled or Cancelled may be set explicitly — Completed is always
    /// derived automatically once EndDateTime passes.</summary>
    public LiveSessionStatus? Status { get; init; }
}
