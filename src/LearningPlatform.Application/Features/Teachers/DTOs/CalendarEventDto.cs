using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Teachers.DTOs;

public class CalendarEventDto
{
    public Guid BookingId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? StudentImage { get; set; }
    public string Subject { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public decimal Price { get; set; }
    public BookingStatus Status { get; set; }
}
