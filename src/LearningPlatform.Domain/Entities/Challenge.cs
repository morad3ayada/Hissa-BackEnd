using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class Challenge : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PointsReward { get; set; }

    public Guid? RewardId { get; set; }
    public Reward? Reward { get; set; }

    // 1v1 duel fields: exactly two participants competing on a single existing Quiz.
    public Guid ChallengerId { get; set; }
    public ApplicationUser Challenger { get; set; } = null!;

    public Guid OpponentId { get; set; }
    public ApplicationUser Opponent { get; set; } = null!;

    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public int DurationInMinutes { get; set; }
    public ChallengeStatus Status { get; set; } = ChallengeStatus.NotStarted;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Guid? WinnerId { get; set; }
    public ApplicationUser? Winner { get; set; }

    public ICollection<Quiz> Quizzes { get; set; } = [];
    public ICollection<StudentChallenge> StudentChallenges { get; set; } = [];
}
