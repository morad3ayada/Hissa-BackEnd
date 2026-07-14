using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

/// <summary>One row per student: the aggregate points/level/avatar-gender state that
/// PointsTransaction entries roll up into. Kept separate from ApplicationUser to isolate
/// gamification state from identity.</summary>
public class GamificationProfile : BaseEntity
{
    public int TotalPoints { get; set; }
    public int CurrentLevel { get; set; } = 1;
    public AvatarGender AvatarGender { get; set; } = AvatarGender.Boy;
    public DateTime? LastDailyLoginRewardAt { get; set; }

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;
}
