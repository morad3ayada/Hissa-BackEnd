using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Teachers.DTOs;

public class StudentDetailDto
{
    public Guid StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public List<LessonHistoryDto> PreviousLessons { get; set; } = [];
    public List<LessonHistoryDto> UpcomingLessons { get; set; } = [];
}

public class LessonHistoryDto
{
    public Guid BookingId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public decimal Price { get; set; }
    public BookingStatus Status { get; set; }
}
