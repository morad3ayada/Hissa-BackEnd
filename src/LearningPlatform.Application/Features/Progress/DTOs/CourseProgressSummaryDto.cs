namespace LearningPlatform.Application.Features.Progress.DTOs;

/// <summary>
/// Aggregated progress for one student within one course. Built by handlers from
/// multiple CourseProgress rows, not a direct entity projection.
/// </summary>
public class CourseProgressSummaryDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string? CourseThumbnailUrl { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }
    public decimal CompletionPercentage { get; set; }
    public decimal OverallWatchPercentage { get; set; }
    public int TotalWatchedSeconds { get; set; }
    public DateTime? LastWatchedAt { get; set; }
}
