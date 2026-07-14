namespace LearningPlatform.Application.Features.Quizzes.DTOs;

public class QuizDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public Guid? CourseId { get; set; }
    public Guid? LessonId { get; set; }
    public bool IsFinalExam { get; set; }
    public int? TimeLimitInMinutes { get; set; }
    public int PassingScore { get; set; }
    public int? MaxAttempts { get; set; }
    public bool IsPublished { get; set; }
    public int TotalPoints { get; set; }
    public List<QuestionDto> Questions { get; set; } = [];
}
