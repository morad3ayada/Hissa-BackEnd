using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Application.Features.LiveSessions.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.Mappings;
using LearningPlatform.Application.Features.LiveSessions.Queries;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.LiveSessions.Handlers;

public class GetUpcomingLiveSessionsQueryHandler(ILiveSessionAccessService accessService)
    : IRequestHandler<GetUpcomingLiveSessionsQuery, PaginatedResponse<LiveSessionDto>>
{
    public async Task<PaginatedResponse<LiveSessionDto>> Handle(GetUpcomingLiveSessionsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var query = (await accessService.GetVisibleSessionsQueryAsync(cancellationToken))
            .Where(s => s.EndDateTime >= now && s.Status != LiveSessionStatus.Cancelled);

        var totalCount = await query.CountAsync(cancellationToken);

        var sessions = await query
            .OrderBy(s => s.StartDateTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = sessions.Select(LiveSessionDtoBuilder.Build).ToList();

        var paginatedList = new PaginatedList<LiveSessionDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        return PaginatedResponse<LiveSessionDto>.Create(paginatedList);
    }
}
