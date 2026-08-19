namespace LearningPlatform.Application.Features.Chat.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeletedForEveryone { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    public string? ReplyToContent { get; set; }
    public string? ReplyToSenderName { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<AttachmentDto> Attachments { get; set; } = [];
}
