namespace LearningPlatform.Application.Features.Gamification.DTOs;

public class GamificationProfileDto
{
    public int TotalPoints { get; set; }
    public int CurrentLevel { get; set; }
    public string LevelTitle { get; set; } = string.Empty;
    public int? PointsToNextLevel { get; set; }
    public int Rank { get; set; }
    public string AvatarGender { get; set; } = string.Empty;
    public List<AvatarItemDto> EquippedItems { get; set; } = [];
}
