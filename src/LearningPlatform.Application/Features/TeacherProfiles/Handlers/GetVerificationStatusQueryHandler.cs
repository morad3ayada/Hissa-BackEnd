using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.TeacherProfiles.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.TeacherProfiles.Handlers;

public class GetVerificationStatusQueryHandler(
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<Queries.GetVerificationStatusQuery, ApiResponse<VerificationStatusDto>>
{
    public async Task<ApiResponse<VerificationStatusDto>> Handle(
        Queries.GetVerificationStatusQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var profile = await unitOfWork.Repository<TeacherProfile>()
            .GetTrackedAsync(t => t.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Teacher profile not found.");

        var dto = new VerificationStatusDto
        {
            Status = profile.VerificationStatus,
            CanReceiveBookings = profile.CanReceiveBookings,
            RejectionReason = profile.RejectionReason,
            AcceptingBookings = profile.AcceptingBookings
        };

        return ApiResponse<VerificationStatusDto>.Success(dto);
    }
}
