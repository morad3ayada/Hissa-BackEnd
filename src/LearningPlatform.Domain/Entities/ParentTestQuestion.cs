using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class ParentTestQuestion : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public int Order { get; set; }
    public string? CorrectAnswerText { get; set; }

    public Guid ParentTestId { get; set; }
    public ParentTest ParentTest { get; set; } = null!;
}
