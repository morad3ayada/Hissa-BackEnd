using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Common.Services;

public class GamificationService(IUnitOfWork unitOfWork, INotificationService notificationService) : IGamificationService
{
    private const int DailyLoginPoints = 5;

    public async Task AwardPointsAsync(
        Guid studentId, int points, PointsReason reason, Guid? sourceId,
        string? notes = null, CancellationToken cancellationToken = default)
    {
        if (sourceId.HasValue &&
            await unitOfWork.Repository<PointsTransaction>().ExistsAsync(
                t => t.StudentId == studentId && t.Reason == reason && t.SourceId == sourceId, cancellationToken))
            return;

        var (profile, isNew) = await LoadOrCreateProfileAsync(studentId, cancellationToken);

        // Always true here: recursion safety is enforced at the single internal call site in
        // ApplyPointsAsync that awards the LevelUp reward's own bonus points (capped there with
        // allowLevelUpAchievement: false), not by the reason of the top-level call — otherwise
        // a level-up triggered BY an achievement bonus (e.g. FirstCourseCompleted) would silently
        // skip granting LevelUp too, even though it's a different, legitimate achievement.
        await ApplyPointsAsync(profile, points, reason, sourceId, notes, allowLevelUpAchievement: true, cancellationToken);

        await SaveProfileAsync(profile, isNew, cancellationToken);
    }

    public async Task<bool> TryAwardDailyLoginAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var (profile, isNew) = await LoadOrCreateProfileAsync(studentId, cancellationToken);

        if (profile.LastDailyLoginRewardAt?.Date == DateTime.UtcNow.Date)
        {
            if (isNew)
                await SaveProfileAsync(profile, isNew, cancellationToken);

            return false;
        }

        profile.LastDailyLoginRewardAt = DateTime.UtcNow;

        await ApplyPointsAsync(profile, DailyLoginPoints, PointsReason.DailyLogin, null, "Daily login bonus", allowLevelUpAchievement: true, cancellationToken);

        await SaveProfileAsync(profile, isNew, cancellationToken);

        return true;
    }

    public async Task CheckAchievementAsync(Guid studentId, AchievementTriggerType trigger, CancellationToken cancellationToken = default)
    {
        var reward = (await unitOfWork.Repository<Reward>()
            .FindAsync(r => r.TriggerType == trigger, cancellationToken)).FirstOrDefault();

        if (reward is null)
            return;

        var alreadyGranted = await unitOfWork.Repository<StudentReward>().ExistsAsync(
            sr => sr.StudentId == studentId && sr.RewardId == reward.Id, cancellationToken);

        if (alreadyGranted)
            return;

        await unitOfWork.Repository<StudentReward>().AddAsync(new StudentReward
        {
            StudentId = studentId,
            RewardId = reward.Id,
            EarnedAt = DateTime.UtcNow
        }, cancellationToken);

        await notificationService.CreateAsync(
            studentId, NotificationType.Achievement, "Achievement unlocked!",
            $"You've earned the \"{reward.Name}\" achievement.", cancellationToken: cancellationToken);

        if (reward.PointsValue > 0)
            await AwardPointsAsync(studentId, reward.PointsValue, PointsReason.Achievement, reward.Id, $"Achievement unlocked: {reward.Name}", cancellationToken);
    }

    /// <summary>
    /// Adds the PointsTransaction, bumps TotalPoints, and recalculates CurrentLevel on the
    /// given (already loaded, not-yet-saved) profile instance — never re-fetches, so this is
    /// safe to call from within a profile session that already has pending, unsaved changes.
    /// When a level-up occurs and allowLevelUpAchievement is true, grants the LevelUp
    /// achievement's bonus points onto the SAME profile instance, capped at one extra hop so a
    /// level-up triggered by an achievement bonus can never itself trigger another.
    /// </summary>
    private async Task ApplyPointsAsync(
        GamificationProfile profile, int points, PointsReason reason, Guid? sourceId, string? notes,
        bool allowLevelUpAchievement, CancellationToken cancellationToken)
    {
        await unitOfWork.Repository<PointsTransaction>().AddAsync(new PointsTransaction
        {
            StudentId = profile.StudentId,
            Points = points,
            Reason = reason,
            SourceId = sourceId,
            Notes = notes
        }, cancellationToken);

        profile.TotalPoints += points;

        var leveledUp = await RecalculateLevelAsync(profile, cancellationToken);
        if (!leveledUp)
            return;

        await notificationService.CreateAsync(
            profile.StudentId, NotificationType.Achievement, "Level up!",
            $"You've reached level {profile.CurrentLevel}.", cancellationToken: cancellationToken);

        if (!allowLevelUpAchievement)
            return;

        var reward = (await unitOfWork.Repository<Reward>()
            .FindAsync(r => r.TriggerType == AchievementTriggerType.LevelUp, cancellationToken)).FirstOrDefault();

        if (reward is null)
            return;

        var alreadyGranted = await unitOfWork.Repository<StudentReward>().ExistsAsync(
            sr => sr.StudentId == profile.StudentId && sr.RewardId == reward.Id, cancellationToken);

        if (alreadyGranted)
            return;

        await unitOfWork.Repository<StudentReward>().AddAsync(new StudentReward
        {
            StudentId = profile.StudentId,
            RewardId = reward.Id,
            EarnedAt = DateTime.UtcNow
        }, cancellationToken);

        await notificationService.CreateAsync(
            profile.StudentId, NotificationType.Achievement, "Achievement unlocked!",
            $"You've earned the \"{reward.Name}\" achievement.", cancellationToken: cancellationToken);

        if (reward.PointsValue > 0)
        {
            await ApplyPointsAsync(
                profile, reward.PointsValue, PointsReason.Achievement, reward.Id,
                $"Achievement unlocked: {reward.Name}", allowLevelUpAchievement: false, cancellationToken);
        }
    }

    private async Task<(GamificationProfile Profile, bool IsNew)> LoadOrCreateProfileAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var existing = await unitOfWork.Repository<GamificationProfile>()
            .GetTrackedAsync(p => p.StudentId == studentId, cancellationToken);

        if (existing is not null)
            return (existing, false);

        // First-time onboarding: auto-grant (free, pre-equipped) the default starter avatar so
        // a brand-new profile isn't blank. Staged here, persisted whenever the caller saves.
        var starterItem = (await unitOfWork.Repository<AvatarItem>()
            .FindAsync(a => a.Category == "Base" && a.IsDefault, cancellationToken))
            .OrderBy(a => a.Name)
            .FirstOrDefault();

        if (starterItem is not null)
        {
            await unitOfWork.Repository<StudentAvatar>().AddAsync(new StudentAvatar
            {
                StudentId = studentId,
                AvatarItemId = starterItem.Id,
                IsEquipped = true,
                AcquiredAt = DateTime.UtcNow
            }, cancellationToken);
        }

        return (new GamificationProfile { StudentId = studentId, TotalPoints = 0, CurrentLevel = 1 }, true);
    }

    private async Task<bool> RecalculateLevelAsync(GamificationProfile profile, CancellationToken cancellationToken)
    {
        var levels = await unitOfWork.Repository<GamificationLevel>().GetAllAsync(cancellationToken);

        var newLevel = levels
            .Where(l => l.RequiredPoints <= profile.TotalPoints)
            .OrderByDescending(l => l.LevelNumber)
            .Select(l => (int?)l.LevelNumber)
            .FirstOrDefault();

        if (newLevel is null || newLevel <= profile.CurrentLevel)
            return false;

        profile.CurrentLevel = newLevel.Value;
        return true;
    }

    private async Task SaveProfileAsync(GamificationProfile profile, bool isNew, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<GamificationProfile>();

        if (isNew)
            await repository.AddAsync(profile, cancellationToken);
        else
            repository.Update(profile);
    }
}
