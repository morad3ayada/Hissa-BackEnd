using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class Booking : BaseEntity
{
    public Guid TeacherId { get; set; }
    public ApplicationUser Teacher { get; set; } = null!;

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public string Subject { get; set; } = string.Empty;
    public int DurationInMinutes { get; set; }
    public decimal Price { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
}
