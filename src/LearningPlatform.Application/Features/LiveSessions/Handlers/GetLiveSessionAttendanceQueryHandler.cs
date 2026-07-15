using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Application.Features.LiveSessions.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.LiveSessions.Handlers;

public class GetLiveSessionAttendanceQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetLiveSessionAttendanceQuery, ApiResponse<List<LiveSessionAttendanceDto>>>
{
    public async Task<ApiResponse<List<LiveSessionAttendanceDto>>> Handle(GetLiveSessionAttendanceQuery request, CancellationToken cancellationToken)
    {
        var session = await unitOfWork.Repository<LiveSession>().AsQueryable()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(LiveSession), request.Id);

        var attendances = await unitOfWork.Repository<LiveSessionAttendance>().AsQueryable()
            .Include(a => a.User)
            .Where(a => a.LiveSessionId == request.Id)
            .OrderBy(a => a.JoinedAt)
            .Select(a => new LiveSessionAttendanceDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = $"{a.User.FirstName} {a.User.LastName}",
                UserEmail = a.User.Email,
                JoinedAt = a.JoinedAt
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<LiveSessionAttendanceDto>>.Success(attendances);
    }
}