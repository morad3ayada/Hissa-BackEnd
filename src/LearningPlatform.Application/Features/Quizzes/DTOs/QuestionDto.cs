namespace LearningPlatform.Application.Features.Quizzes.DTOs;

public class QuestionDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Order { get; set; }
    public int Points { get; set; }

    /// <summary>Null when returned to a student attempting the quiz (no spoilers).</summary>
    public string? Explanation { get; set; }

    public List<AnswerDto> Answers { get; set; } = [];
}
