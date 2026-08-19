using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.TeacherProfiles.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.TeacherProfiles.Handlers;

public class UpdateBookingStatusCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateBookingStatusCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var profile = await unitOfWork.Repository<TeacherProfile>()
            .GetTrackedAsync(t => t.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Teacher profile not found.");

        if (profile.VerificationStatus != TeacherVerificationStatus.Approved)
            throw new ForbiddenException("Only approved teachers can toggle booking status.");

        profile.AcceptingBookings = request.AcceptingBookings;
        unitOfWork.Repository<TeacherProfile>().Update(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var status = request.AcceptingBookings ? "enabled" : "disabled";
        return ApiResponse.Success($"Booking reception {status} successfully.");
    }
}
