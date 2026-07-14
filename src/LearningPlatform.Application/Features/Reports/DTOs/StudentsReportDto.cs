namespace LearningPlatform.Application.Features.Reports.DTOs;

public class StudentsReportDto
{
    public decimal AverageGradeAcrossAllQuizzes { get; set; }
    public List<CourseStudentDistributionDto> StudentDistribution { get; set; } = [];
}

public class CourseStudentDistributionDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int StudentsCount { get; set; }
}
