namespace LearningPlatform.Application.Features.Progress.DTOs;

public class ContinueLearningDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string? CourseThumbnailUrl { get; set; }
    public Guid LessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public Guid SectionId { get; set; }
    public string SectionTitle { get; set; } = string.Empty;
    public int CurrentSecond { get; set; }
    public decimal WatchPercentage { get; set; }
    public DateTime LastWatchedAt { get; set; }
}
