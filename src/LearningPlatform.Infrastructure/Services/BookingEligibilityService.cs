using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Infrastructure.Services;

public class BookingEligibilityService(IUnitOfWork unitOfWork) : IBookingEligibilityService
{
    public bool CanReceiveBookings(TeacherProfile profile) =>
        profile.VerificationStatus == TeacherVerificationStatus.Approved && profile.AcceptingBookings;

    public async Task<bool> CanReceiveBookingsAsync(Guid instructorId, CancellationToken cancellationToken = default)
    {
        var profile = await unitOfWork.Repository<TeacherProfile>()
            .AsQueryable()
            .FirstOrDefaultAsync(t => t.UserId == instructorId, cancellationToken);

        return profile is not null && CanReceiveBookings(profile);
    }
}
