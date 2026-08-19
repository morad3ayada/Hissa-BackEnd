using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.TeacherProfiles.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.TeacherProfiles.Handlers;

public class ApproveTeacherCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<ApproveTeacherCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ApproveTeacherCommand request, CancellationToken cancellationToken)
    {
        var adminId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var profile = await unitOfWork.Repository<TeacherProfile>()
            .GetTrackedAsync(t => t.Id == request.TeacherProfileId, cancellationToken)
            ?? throw new NotFoundException("Teacher profile not found.");

        var oldStatus = profile.VerificationStatus;

        profile.VerificationStatus = TeacherVerificationStatus.Approved;
        profile.RejectionReason = null;
        profile.AcceptingBookings = true;

        unitOfWork.Repository<TeacherProfile>().Update(profile);

        await AddHistoryAsync(profile.Id, oldStatus, TeacherVerificationStatus.Approved, null, adminId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Teacher approved successfully.");
    }

    private async Task AddHistoryAsync(
        Guid profileId, TeacherVerificationStatus oldStatus, TeacherVerificationStatus newStatus,
        string? reason, Guid changedBy, CancellationToken ct)
    {
        await unitOfWork.Repository<TeacherVerificationHistory>().AddAsync(new TeacherVerificationHistory
        {
            TeacherProfileId = profileId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            Reason = reason,
            ChangedByUserId = changedBy
        }, ct);
    }
}
