using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Common.Interfaces;

public interface IBookingEligibilityService
{
    bool CanReceiveBookings(TeacherProfile profile);
    Task<bool> CanReceiveBookingsAsync(Guid instructorId, CancellationToken cancellationToken = default);
}
