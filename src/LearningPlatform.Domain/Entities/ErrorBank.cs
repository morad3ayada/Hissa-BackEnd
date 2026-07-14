using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class ErrorBank : BaseEntity
{
    public int MistakeCount { get; set; } = 1;
    public DateTime LastMistakeAt { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public Guid? LessonId { get; set; }
    public Lesson? Lesson { get; set; }

    public Guid? StudentAnswerId { get; set; }
    public StudentAnswer? StudentAnswer { get; set; }
}
