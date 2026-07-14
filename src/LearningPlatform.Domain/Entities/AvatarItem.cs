using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class AvatarItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int PriceInPoints { get; set; }
    public bool IsDefault { get; set; }

    public ICollection<Reward> Rewards { get; set; } = [];
    public ICollection<StudentAvatar> StudentAvatars { get; set; } = [];
}
