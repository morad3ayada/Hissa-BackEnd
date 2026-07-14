using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.LiveSessions.Queries;

public record GetPreviousLiveSessionsQuery : IRequest<PaginatedResponse<LiveSessionDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
