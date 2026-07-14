using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class StudentAnswer : BaseEntity
{
    public string? TextResponse { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime AnsweredAt { get; set; }

    public Guid QuizResultId { get; set; }
    public QuizResult QuizResult { get; set; } = null!;

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public Guid? SelectedAnswerId { get; set; }
    public Answer? SelectedAnswer { get; set; }

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public ErrorBank? ErrorBankEntry { get; set; }
}
