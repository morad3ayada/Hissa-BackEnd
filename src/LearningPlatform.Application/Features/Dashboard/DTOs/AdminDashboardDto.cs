namespace LearningPlatform.Application.Features.Dashboard.DTOs;

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalParents { get; set; }

    public int TotalCourses { get; set; }
    public int PublishedCourses { get; set; }
    public int PendingCourses { get; set; }
    public int RejectedCourses { get; set; }

    public int TotalEnrollments { get; set; }
    public List<PaymentStatusBreakdownDto> PaymentsByStatus { get; set; } = [];

    public List<TopCourseDto> TopEnrolledCourses { get; set; } = [];
    public List<TopCourseDto> TopViewedCourses { get; set; } = [];
    public List<TopInstructorDto> MostActiveInstructors { get; set; } = [];

    public int TotalQuizzes { get; set; }
    public int TotalLiveSessions { get; set; }
    public decimal AveragePassRate { get; set; }
    public int ActiveUsersCount { get; set; }
}
