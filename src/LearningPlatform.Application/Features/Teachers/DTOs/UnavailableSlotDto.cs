namespace LearningPlatform.Application.Features.Teachers.DTOs;

public class UnavailableSlotDto
{
    public Guid Id { get; set; }
    public string Date { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class CreateUnavailableSlotRequest
{
    public string Date { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
