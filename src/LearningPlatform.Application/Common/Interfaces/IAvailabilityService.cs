namespace LearningPlatform.Application.Common.Interfaces;

public interface IAvailabilityService
{
    Task<List<AvailableSlot>> GetAvailableSlotsAsync(
        Guid teacherId,
        DateOnly date,
        int durationInMinutes,
        CancellationToken cancellationToken = default);

    Task<bool> IsSlotAvailableAsync(
        Guid teacherId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken = default);
}

public class AvailableSlot
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
