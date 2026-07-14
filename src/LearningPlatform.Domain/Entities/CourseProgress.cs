using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class CourseProgress : BaseEntity
{
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }

    /// <summary>Playback position (seconds) to resume from next time the lesson is opened.</summary>
    public int CurrentSecond { get; set; }

    /// <summary>Furthest watch percentage reached (0-100); never decreases once recorded.</summary>
    public decimal WatchPercentage { get; set; }

    public DateTime? LastWatchedAt { get; set; }

    public Guid EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;

    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;
}
