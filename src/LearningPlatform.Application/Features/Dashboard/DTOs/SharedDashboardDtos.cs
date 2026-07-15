namespace LearningPlatform.Application.Features.Dashboard.DTOs;

public class PaymentStatusBreakdownDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

public class TopCourseDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TopInstructorDto
{
    public Guid InstructorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CoursesCount { get; set; }
    public int StudentsCount { get; set; }
}

public class TopLessonDto
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public int ViewCount { get; set; }
}

public class QuizResultsSummaryDto
{
    public int TotalAttempts { get; set; }
    public int PassedCount { get; set; }
    public decimal PassRate { get; set; }
    public decimal AverageScore { get; set; }
}

public class UpcomingSessionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
}
