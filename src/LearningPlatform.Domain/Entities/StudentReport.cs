using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class StudentReport : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public Guid InstructorId { get; set; }
    public ApplicationUser Instructor { get; set; } = null!;

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;
}
