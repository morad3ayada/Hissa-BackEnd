namespace LearningPlatform.Application.Features.Chat.DTOs;

public class BlockedUserDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? ProfilePictureUrl { get; set; }
    public DateTime BlockedAt { get; set; }
}
