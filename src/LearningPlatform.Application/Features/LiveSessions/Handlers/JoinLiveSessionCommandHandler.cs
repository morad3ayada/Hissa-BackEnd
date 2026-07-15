using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.Commands;
using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Application.Features.LiveSessions.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.LiveSessions.Handlers;

public class JoinLiveSessionCommandHandler(
    IUnitOfWork unitOfWork,
    ILiveSessionAccessService accessService,
    ICurrentUserService currentUser)
    : IRequestHandler<JoinLiveSessionCommand, ApiResponse<LiveSessionDto>>
{
    public async Task<ApiResponse<LiveSessionDto>> Handle(JoinLiveSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await unitOfWork.Repository<LiveSession>().AsQueryable()
            .Include(s => s.Course)
            .Include(s => s.Instructor)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(LiveSession), request.Id);

        await accessService.EnsureCanViewAsync(session, cancellationToken);

        if (LiveSessionDtoBuilder.EffectiveStatus(session) == LiveSessionStatus.Cancelled)
            throw new BadRequestException("This live session has been cancelled.");

        var userGuid = currentUser.UserId;
        if (userGuid is not null)
        {
            var existing = await unitOfWork.Repository<LiveSessionAttendance>()
                .AsQueryable()
                .AnyAsync(a => a.LiveSessionId == request.Id && a.UserId == userGuid, cancellationToken);

            if (!existing)
            {
                var attendance = new LiveSessionAttendance
                {
                    LiveSessionId = request.Id,
                    UserId = userGuid.Value,
                    JoinedAt = DateTime.UtcNow
                };
                await unitOfWork.Repository<LiveSessionAttendance>().AddAsync(attendance, cancellationToken);
                await unitOfWork.CompleteAsync(cancellationToken);
            }
        }

        var dto = LiveSessionDtoBuilder.Build(session);
        return ApiResponse<LiveSessionDto>.Success(dto);
    }
}
