namespace LearningPlatform.Application.Features.Quizzes.DTOs;

public class AnswerDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }

    /// <summary>Null when returned to a student attempting the quiz (no spoilers).</summary>
    public bool? IsCorrect { get; set; }
}
