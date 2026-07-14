namespace LearningPlatform.Application.Features.Quizzes.DTOs;

public class QuizSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsFinalExam { get; set; }
    public int? TimeLimitInMinutes { get; set; }
    public int PassingScore { get; set; }
    public int? MaxAttempts { get; set; }
    public bool IsPublished { get; set; }
    public int QuestionsCount { get; set; }
}
