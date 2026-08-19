using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class TeacherAvailability : BaseEntity
{
    public Guid TeacherId { get; set; }
    public ApplicationUser Teacher { get; set; } = null!;

    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
