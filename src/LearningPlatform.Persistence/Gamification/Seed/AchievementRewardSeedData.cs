using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Persistence.Gamification.Seed;

/// <summary>Fixed, deterministic achievement Reward rows (badges auto-granted on milestones).
/// See GamificationLevelSeedData for why anonymous objects are used instead of Reward instances.</summary>
public static class AchievementRewardSeedData
{
    private static readonly DateTime SeededAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static object[] Rewards =>
    [
        Build("b0000000-0000-0000-0000-000000000001", "First Quiz", "Passed your very first quiz.", AchievementTriggerType.FirstQuizPassed, 20),
        Build("b0000000-0000-0000-0000-000000000002", "First Course", "Completed your very first course.", AchievementTriggerType.FirstCourseCompleted, 50),
        Build("b0000000-0000-0000-0000-000000000003", "Dedicated Learner", "Completed 10 lessons.", AchievementTriggerType.TenLessonsCompleted, 30),
        Build("b0000000-0000-0000-0000-000000000004", "Perfect Score", "Scored 100% on a quiz.", AchievementTriggerType.PerfectQuizScore, 25),
        Build("b0000000-0000-0000-0000-000000000005", "Level Up", "Reached a new gamification level.", AchievementTriggerType.LevelUp, 10)
    ];

    private static object Build(string id, string name, string description, AchievementTriggerType trigger, int pointsValue) => new
    {
        Id = Guid.Parse(id),
        Name = name,
        Description = description,
        Type = RewardType.Badge,
        PointsValue = pointsValue,
        IconUrl = (string?)null,
        TriggerType = (AchievementTriggerType?)trigger,
        AvatarItemId = (Guid?)null,
        CreatedAt = SeededAt,
        UpdatedAt = (DateTime?)null,
        IsDeleted = false
    };
}
