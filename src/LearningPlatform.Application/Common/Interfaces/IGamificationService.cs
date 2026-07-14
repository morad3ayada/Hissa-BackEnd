using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Common.Interfaces;

public interface IGamificationService
{
    /// <summary>
    /// Awards points, idempotent per (studentId, reason, sourceId) when sourceId is supplied —
    /// the same lesson/course/quiz/challenge/reward can never grant points twice. Recalculates
    /// the student's level and, on level-up, grants the LevelUp achievement automatically.
    /// Stages changes only; the caller's handler must still call SaveChangesAsync.
    /// </summary>
    Task AwardPointsAsync(
        Guid studentId, int points, PointsReason reason, Guid? sourceId,
        string? notes = null, CancellationToken cancellationToken = default);

    /// <summary>Awards the daily-login bonus at most once per calendar day (UTC). Returns
    /// false if today's bonus was already claimed. Stages changes only.</summary>
    Task<bool> TryAwardDailyLoginAsync(Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>Grants the Reward configured for this trigger, once per student ever. No-op if
    /// no Reward is configured for the trigger or the student already has it. Stages changes only.</summary>
    Task CheckAchievementAsync(Guid studentId, AchievementTriggerType trigger, CancellationToken cancellationToken = default);
}
