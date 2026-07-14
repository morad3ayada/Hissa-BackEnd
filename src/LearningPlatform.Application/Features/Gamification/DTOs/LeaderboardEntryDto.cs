namespace LearningPlatform.Application.Features.Gamification.DTOs;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public int TotalPoints { get; set; }
    public int CurrentLevel { get; set; }
    public int CompletedCoursesCount { get; set; }
    public int PassedQuizzesCount { get; set; }
}
