namespace LearningPlatform.Application.Features.Gamification.DTOs;

public class ChallengeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid ChallengerId { get; set; }
    public string ChallengerName { get; set; } = string.Empty;

    public Guid OpponentId { get; set; }
    public string OpponentName { get; set; } = string.Empty;

    public Guid QuizId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;

    public int DurationInMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int PointsReward { get; set; }

    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }

    public decimal? MyScore { get; set; }
    public decimal? OpponentScore { get; set; }
}
