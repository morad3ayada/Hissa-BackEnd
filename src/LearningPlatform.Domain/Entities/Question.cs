using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class Question : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public int Order { get; set; }
    public int Points { get; set; } = 1;
    public string? Explanation { get; set; }

    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public ICollection<Answer> Answers { get; set; } = [];
    public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
    public ICollection<ErrorBank> ErrorBankEntries { get; set; } = [];
}
