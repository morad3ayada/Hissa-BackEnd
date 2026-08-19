using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.TeacherProfiles.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.TeacherProfiles.Handlers;

public class RejectTeacherCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<RejectTeacherCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(RejectTeacherCommand request, CancellationToken cancellationToken)
    {
        var adminId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var profile = await unitOfWork.Repository<TeacherProfile>()
            .GetTrackedAsync(t => t.Id == request.TeacherProfileId, cancellationToken)
            ?? throw new NotFoundException("Teacher profile not found.");

        var oldStatus = profile.VerificationStatus;

        profile.VerificationStatus = TeacherVerificationStatus.Rejected;
        profile.RejectionReason = request.Reason;
        profile.AcceptingBookings = false;

        unitOfWork.Repository<TeacherProfile>().Update(profile);

        await unitOfWork.Repository<TeacherVerificationHistory>().AddAsync(new TeacherVerificationHistory
        {
            TeacherProfileId = profile.Id,
            OldStatus = oldStatus,
            NewStatus = TeacherVerificationStatus.Rejected,
            Reason = request.Reason,
            ChangedByUserId = adminId
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Teacher rejected successfully.");
    }
}
