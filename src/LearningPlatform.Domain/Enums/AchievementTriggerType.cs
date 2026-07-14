namespace LearningPlatform.Domain.Enums;

/// <summary>Milestones that auto-grant a Reward the first time a student reaches them.</summary>
public enum AchievementTriggerType
{
    FirstQuizPassed = 1,
    FirstCourseCompleted = 2,
    TenLessonsCompleted = 3,
    PerfectQuizScore = 4,
    LevelUp = 5
}
