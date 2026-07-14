using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class Quiz : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public QuizScope Scope { get; set; }
    public int? TimeLimitInMinutes { get; set; }
    public int PassingScore { get; set; } = 60;
    public int? MaxAttempts { get; set; }
    public bool IsPublished { get; set; }

    /// <summary>True for the single course-completion final exam (Scope = Course).</summary>
    public bool IsFinalExam { get; set; }

    // Exactly one of Course / Lesson / Challenge is set, matching Scope.
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }

    public Guid? LessonId { get; set; }
    public Lesson? Lesson { get; set; }

    public Guid? ChallengeId { get; set; }
    public Challenge? Challenge { get; set; }

    public ICollection<Question> Questions { get; set; } = [];
    public ICollection<QuizResult> QuizResults { get; set; } = [];
}
