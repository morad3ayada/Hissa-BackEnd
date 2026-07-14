using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class StudentReward : BaseEntity
{
    public DateTime EarnedAt { get; set; }

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public Guid RewardId { get; set; }
    public Reward Reward { get; set; } = null!;

    public Guid? SourceChallengeId { get; set; }
    public Challenge? SourceChallenge { get; set; }
}
