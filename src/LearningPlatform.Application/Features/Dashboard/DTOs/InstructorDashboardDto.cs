namespace LearningPlatform.Application.Features.Dashboard.DTOs;

public class InstructorDashboardDto
{
    public int CoursesCount { get; set; }
    public int StudentsCount { get; set; }
    public decimal AverageCourseRating { get; set; }
    public decimal StudentCompletionRate { get; set; }
    public List<TopLessonDto> MostWatchedLessons { get; set; } = [];
    public QuizResultsSummaryDto QuizResultsSummary { get; set; } = new();
    public List<UpcomingSessionDto> UpcomingLiveSessions { get; set; } = [];
}
