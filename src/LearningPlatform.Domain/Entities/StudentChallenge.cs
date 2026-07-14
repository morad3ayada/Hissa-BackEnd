using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class StudentChallenge : BaseEntity
{
    public ChallengeStatus Status { get; set; } = ChallengeStatus.NotStarted;
    public int Progress { get; set; }
    public decimal? Score { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public Guid ChallengeId { get; set; }
    public Challenge Challenge { get; set; } = null!;
}
