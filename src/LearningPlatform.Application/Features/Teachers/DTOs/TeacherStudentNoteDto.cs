namespace LearningPlatform.Application.Features.Teachers.DTOs;

public class TeacherStudentNoteDto
{
    public Guid NoteId { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
