using LearningPlatform.Application.Features.Dashboard.DTOs;

namespace LearningPlatform.Application.Features.Reports.DTOs;

public class CoursesReportDto
{
    public List<TopCourseDto> TopEnrolledCourses { get; set; } = [];
    public List<TopCourseDto> TopViewedCourses { get; set; } = [];
    public List<TopCourseDto> LeastActiveCourses { get; set; } = [];
}
