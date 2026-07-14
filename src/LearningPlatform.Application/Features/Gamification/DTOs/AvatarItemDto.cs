namespace LearningPlatform.Application.Features.Gamification.DTOs;

public class AvatarItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int PriceInPoints { get; set; }
    public bool IsDefault { get; set; }
    public bool Owned { get; set; }
    public bool Equipped { get; set; }
}
