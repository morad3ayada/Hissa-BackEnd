namespace LearningPlatform.Application.Features.Instructors.DTOs;

public class InstructorDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public int CoursesCount { get; set; }
    public List<InstructorCourseDto> Courses { get; set; } = [];
}
