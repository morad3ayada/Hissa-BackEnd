namespace LearningPlatform.Application.Features.Chat.DTOs;

public class ConversationDto
{
    public Guid Id { get; set; }
    public ChatUserDto OtherUser { get; set; } = new();
    public MessageDto? LastMessage { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
    public bool IsMuted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
