using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class BlockedUser : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public Guid BlockedUserId { get; set; }
    public ApplicationUser BlockedUserInfo { get; set; } = null!;
}
