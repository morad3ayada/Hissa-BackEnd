namespace LearningPlatform.Application.Features.Teachers.DTOs;

public class TeacherStudentDto
{
    public Guid StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int LessonsCount { get; set; }
    public List<string> Subjects { get; set; } = [];
}
