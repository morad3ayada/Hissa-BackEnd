using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class QuizResult : BaseEntity
{
    public decimal Score { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public bool IsPassed { get; set; }
    public int AttemptNumber { get; set; } = 1;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
}
