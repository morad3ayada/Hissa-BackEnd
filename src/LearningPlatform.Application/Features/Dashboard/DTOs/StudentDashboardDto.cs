namespace LearningPlatform.Application.Features.Dashboard.DTOs;

public class StudentDashboardDto
{
    public List<EnrolledCourseSummaryDto> EnrolledCourses { get; set; } = [];
    public decimal OverallProgressPercentage { get; set; }
    public LastLessonDto? LastLesson { get; set; }
    public LastQuizDto? LastQuiz { get; set; }
    public int Points { get; set; }
    public int Level { get; set; }
    public int AchievementsCount { get; set; }
    public int CertificatesCount { get; set; }
    public List<UpcomingSessionDto> UpcomingLiveSessions { get; set; } = [];
}

public class EnrolledCourseSummaryDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
}

public class LastLessonDto
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime LastWatchedAt { get; set; }
}

public class LastQuizDto
{
    public Guid QuizId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public bool IsPassed { get; set; }
    public DateTime CompletedAt { get; set; }
}
