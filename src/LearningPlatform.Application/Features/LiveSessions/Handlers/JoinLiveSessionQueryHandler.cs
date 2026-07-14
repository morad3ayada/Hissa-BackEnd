using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Application.Features.LiveSessions.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.Mappings;
using LearningPlatform.Application.Features.LiveSessions.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.LiveSessions.Handlers;

public class JoinLiveSessionQueryHandler(IUnitOfWork unitOfWork, ILiveSessionAccessService accessService)
    : IRequestHandler<JoinLiveSessionQuery, ApiResponse<LiveSessionDto>>
{
    public async Task<ApiResponse<LiveSessionDto>> Handle(JoinLiveSessionQuery request, CancellationToken cancellationToken)
    {
        var session = await unitOfWork.Repository<LiveSession>().AsQueryable()
            .Include(s => s.Course)
            .Include(s => s.Instructor)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(LiveSession), request.Id);

        await accessService.EnsureCanViewAsync(session, cancellationToken);

        if (LiveSessionDtoBuilder.EffectiveStatus(session) == LiveSessionStatus.Cancelled)
            throw new BadRequestException("This live session has been cancelled.");

        // The frontend is solely responsible for opening MeetingLink — this endpoint just
        // returns the session and meeting details after confirming the caller may attend.
        var dto = LiveSessionDtoBuilder.Build(session);

        return ApiResponse<LiveSessionDto>.Success(dto);
    }
}
