using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class UserActivity : BaseEntity
{
    // Kept as free-form text (not an enum) because activity types are expected
    // to grow continuously as the platform evolves.
    public string ActivityType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IpAddress { get; set; }
    public DateTime OccurredAt { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}
