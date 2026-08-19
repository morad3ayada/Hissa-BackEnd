using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class TeacherStudentNote : BaseEntity
{
    public Guid TeacherId { get; set; }
    public ApplicationUser Teacher { get; set; } = null!;

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public string Note { get; set; } = string.Empty;
}
