using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class Reward : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RewardType Type { get; set; }
    public int PointsValue { get; set; }
    public string? IconUrl { get; set; }

    /// <summary>Non-null for milestone badges that GamificationService auto-grants the first
    /// time a student reaches the trigger; null for challenge-only or manually-granted rewards.</summary>
    public AchievementTriggerType? TriggerType { get; set; }

    public Guid? AvatarItemId { get; set; }
    public AvatarItem? AvatarItem { get; set; }

    public ICollection<Challenge> Challenges { get; set; } = [];
    public ICollection<StudentReward> StudentRewards { get; set; } = [];
}
