namespace LearningPlatform.Application.Features.Quizzes.DTOs;

public class QuizResultDto
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public bool IsPassed { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<ReviewAnswerDto> Answers { get; set; } = [];
}

/// <summary>Review-mode breakdown of a single answered question.</summary>
public class ReviewAnswerDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public Guid? StudentSelectedAnswerId { get; set; }
    public string? StudentAnswerText { get; set; }
    public Guid? CorrectAnswerId { get; set; }
    public string? CorrectAnswerText { get; set; }
    public bool IsCorrect { get; set; }
    public string? Explanation { get; set; }
}
