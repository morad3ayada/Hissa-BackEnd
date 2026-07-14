using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class StudentAvatar : BaseEntity
{
    public bool IsEquipped { get; set; }
    public DateTime AcquiredAt { get; set; }

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public Guid AvatarItemId { get; set; }
    public AvatarItem AvatarItem { get; set; } = null!;
}
