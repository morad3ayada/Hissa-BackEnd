using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class TeacherUnavailableSlot : BaseEntity
{
    public Guid TeacherId { get; set; }
    public ApplicationUser Teacher { get; set; } = null!;

    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? Reason { get; set; }
}
