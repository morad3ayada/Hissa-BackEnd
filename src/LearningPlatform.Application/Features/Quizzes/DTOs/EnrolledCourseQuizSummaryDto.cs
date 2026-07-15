namespace LearningPlatform.Application.Features.Quizzes.DTOs;

public class EnrolledCourseQuizSummaryDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;

    public Guid LessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;

    public Guid QuizId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public int? TimeLimitInMinutes { get; set; }
    public int PassingScore { get; set; }
    public int? MaxAttempts { get; set; }
    public int QuestionsCount { get; set; }
}
