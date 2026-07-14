using LearningPlatform.Application.Common.Extensions;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.Commands;
using LearningPlatform.Application.Features.LiveSessions.DTOs;
using LearningPlatform.Application.Features.LiveSessions.Mappings;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.LiveSessions.Handlers;

public class UpdateLiveSessionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<UpdateLiveSessionCommand, ApiResponse<LiveSessionDto>>
{
    public async Task<ApiResponse<LiveSessionDto>> Handle(UpdateLiveSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await unitOfWork.Repository<LiveSession>().AsQueryable()
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(LiveSession), request.Id);

        currentUser.EnsureCanManageCourse(session.Course);

        // Only enforce "not in the past" when this is a genuine reschedule — editing other
        // fields on an already-elapsed session shouldn't be blocked by its own historical time.
        if (request.StartDateTime != session.StartDateTime && request.StartDateTime <= DateTime.UtcNow)
            throw new BadRequestException("Session start time cannot be in the past.");

        var hasOverlap = await unitOfWork.Repository<LiveSession>().ExistsAsync(
            s => s.Id != session.Id &&
                 s.InstructorId == session.InstructorId &&
                 s.Status != LiveSessionStatus.Cancelled &&
                 s.StartDateTime < request.EndDateTime &&
                 request.StartDateTime < s.EndDateTime,
            cancellationToken);

        if (hasOverlap)
            throw new ConflictException("This instructor already has another live session scheduled during this time.");

        session.Title = request.Title;
        session.Description = request.Description;
        session.MeetingPlatform = request.MeetingPlatform;
        session.MeetingLink = request.MeetingLink;
        session.MeetingPassword = request.MeetingPassword;
        session.StartDateTime = request.StartDateTime;
        session.EndDateTime = request.EndDateTime;

        if (request.Status.HasValue)
            session.Status = request.Status.Value;

        unitOfWork.Repository<LiveSession>().Update(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var loaded = await unitOfWork.Repository<LiveSession>().AsQueryable()
            .Include(s => s.Course)
            .Include(s => s.Instructor)
            .FirstAsync(s => s.Id == session.Id, cancellationToken);

        var dto = LiveSessionDtoBuilder.Build(loaded);

        return ApiResponse<LiveSessionDto>.Success(dto, "Live session updated successfully.");
    }
}
