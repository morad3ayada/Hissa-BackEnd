namespace LearningPlatform.Application.Features.Gamification.DTOs;

public class StudentRewardDto
{
    public Guid Id { get; set; }
    public Guid RewardId { get; set; }
    public string RewardName { get; set; } = string.Empty;
    public string? RewardDescription { get; set; }
    public string RewardType { get; set; } = string.Empty;
    public int PointsValue { get; set; }
    public string? IconUrl { get; set; }
    public string? TriggerType { get; set; }
    public DateTime EarnedAt { get; set; }
    public Guid? SourceChallengeId { get; set; }
    public string? SourceChallengeTitle { get; set; }
}
