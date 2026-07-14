using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Application.Features.LiveSessions.Mappings;
using LearningPlatform.Application.Features.LiveSessions.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.LiveSessions.Handlers;

public class GetInstructorSessionsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetInstructorSessionsQuery, PaginatedResponse<LiveSessionDto>>
{
    public async Task<PaginatedResponse<LiveSessionDto>> Handle(GetInstructorSessionsQuery request, CancellationToken cancellationToken)
    {
        var instructorId = currentUser.UserId!.Value;

        var query = unitOfWork.Repository<LiveSession>().AsQueryable()
            .Include(s => s.Course)
            .Include(s => s.Instructor)
            .Where(s => s.InstructorId == instructorId);

        var totalCount = await query.CountAsync(cancellationToken);

        var sessions = await query
            .OrderByDescending(s => s.StartDateTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = sessions.Select(LiveSessionDtoBuilder.Build).ToList();

        var paginatedList = new PaginatedList<LiveSessionDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        return PaginatedResponse<LiveSessionDto>.Create(paginatedList);
    }
}
