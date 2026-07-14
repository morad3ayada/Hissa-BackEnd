using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class Answer : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int Order { get; set; }

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
}
