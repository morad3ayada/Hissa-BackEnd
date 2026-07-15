using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class LiveSessionAttendance : BaseEntity
{
    public Guid LiveSessionId { get; set; }
    public LiveSession LiveSession { get; set; } = null!;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public DateTime JoinedAt { get; set; }
}
