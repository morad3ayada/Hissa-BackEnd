namespace LearningPlatform.Application.Features.Teachers.DTOs;

public class AvailableSlotDto
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
