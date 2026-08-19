using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class TeacherVerificationHistory : BaseEntity
{
    public Guid TeacherProfileId { get; set; }
    public TeacherProfile TeacherProfile { get; set; } = null!;

    public TeacherVerificationStatus OldStatus { get; set; }
    public TeacherVerificationStatus NewStatus { get; set; }
    public string? Reason { get; set; }
    public Guid ChangedByUserId { get; set; }
}
