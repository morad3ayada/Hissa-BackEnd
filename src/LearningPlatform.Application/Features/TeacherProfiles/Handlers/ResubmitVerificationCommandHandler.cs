using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.TeacherProfiles.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.TeacherProfiles.Handlers;

public class ResubmitVerificationCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<ResubmitVerificationCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(ResubmitVerificationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var profile = await unitOfWork.Repository<TeacherProfile>()
            .GetTrackedAsync(t => t.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Teacher profile not found.");

        if (profile.VerificationStatus != TeacherVerificationStatus.Rejected)
            throw new BadRequestException("You can only resubmit after your profile has been rejected.");

        if (string.IsNullOrWhiteSpace(profile.RealName) || string.IsNullOrWhiteSpace(profile.Specialization))
            throw new BadRequestException("Please complete your profile before resubmitting.");

        if (profile.Certificates.Count == 0 && profile.Qualifications.Count == 0 && profile.RequiredDocuments.Count == 0)
            throw new BadRequestException("Please upload at least one document before resubmitting.");

        var oldStatus = profile.VerificationStatus;

        profile.VerificationStatus = TeacherVerificationStatus.UnderReview;
        profile.RejectionReason = null;

        unitOfWork.Repository<TeacherProfile>().Update(profile);

        await unitOfWork.Repository<TeacherVerificationHistory>().AddAsync(new TeacherVerificationHistory
        {
            TeacherProfileId = profile.Id,
            OldStatus = oldStatus,
            NewStatus = TeacherVerificationStatus.UnderReview,
            Reason = "Resubmitted by instructor",
            ChangedByUserId = userId
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success("Your profile has been resubmitted for review.");
    }
}
