using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class Lesson : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public LessonType Type { get; set; }
    public string? ContentUrl { get; set; }
    public string? Content { get; set; }
    public int? DurationInSeconds { get; set; }
    public int Order { get; set; }
    public bool IsFreePreview { get; set; }

    public Guid CourseSectionId { get; set; }
    public CourseSection CourseSection { get; set; } = null!;

    public ICollection<CourseProgress> CourseProgresses { get; set; } = [];
    public ICollection<Quiz> Quizzes { get; set; } = [];
    public ICollection<ErrorBank> ErrorBankEntries { get; set; } = [];
}
