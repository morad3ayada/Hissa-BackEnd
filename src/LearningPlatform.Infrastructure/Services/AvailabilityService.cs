using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Infrastructure.Services;

public class AvailabilityService(IUnitOfWork unitOfWork) : IAvailabilityService
{
    public async Task<List<AvailableSlot>> GetAvailableSlotsAsync(
        Guid teacherId,
        DateOnly date,
        int durationInMinutes,
        CancellationToken cancellationToken = default)
    {
        var dayOfWeek = (int)date.DayOfWeek;

        var availability = await unitOfWork.Repository<TeacherAvailability>()
            .AsQueryable()
            .Where(a => a.TeacherId == teacherId && a.DayOfWeek == dayOfWeek)
            .OrderBy(a => a.StartTime)
            .ToListAsync(cancellationToken);

        if (availability.Count == 0)
            return [];

        var unavailableSlots = await unitOfWork.Repository<TeacherUnavailableSlot>()
            .AsQueryable()
            .Where(s => s.TeacherId == teacherId && s.Date == date)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        var existingBookings = await unitOfWork.Repository<Booking>()
            .AsQueryable()
            .Where(b => b.TeacherId == teacherId
                && b.Date == date
                && b.Status != Domain.Enums.BookingStatus.Cancelled)
            .OrderBy(b => b.StartTime)
            .ToListAsync(cancellationToken);

        var result = new List<AvailableSlot>();

        foreach (var avail in availability)
        {
            var currentStart = avail.StartTime;

            while (currentStart.AddMinutes(durationInMinutes) <= avail.EndTime)
            {
                var candidateEnd = currentStart.AddMinutes(durationInMinutes);

                var overlapsUnavailable = unavailableSlots.Any(u =>
                    currentStart < u.EndTime && candidateEnd > u.StartTime);

                var overlapsBooking = existingBookings.Any(b =>
                    currentStart < b.EndTime && candidateEnd > b.StartTime);

                if (!overlapsUnavailable && !overlapsBooking)
                {
                    result.Add(new AvailableSlot
                    {
                        Date = date,
                        StartTime = currentStart,
                        EndTime = candidateEnd
                    });
                }

                currentStart = currentStart.AddMinutes(durationInMinutes);
            }
        }

        return result;
    }

    public async Task<bool> IsSlotAvailableAsync(
        Guid teacherId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken = default)
    {
        var slots = await GetAvailableSlotsAsync(teacherId, date,
            (int)(endTime - startTime).TotalMinutes, cancellationToken);

        return slots.Any(s => s.StartTime == startTime && s.EndTime == endTime);
    }
}
