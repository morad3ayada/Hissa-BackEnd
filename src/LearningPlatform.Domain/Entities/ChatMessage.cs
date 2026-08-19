using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public Guid SenderId { get; set; }
    public ApplicationUser Sender { get; set; } = null!;

    public string? Content { get; set; }
    public ChatMessageType MessageType { get; set; } = ChatMessageType.Text;
    public ChatMessageStatus Status { get; set; } = ChatMessageStatus.Sent;

    public Guid? ReplyToMessageId { get; set; }
    public ChatMessage? ReplyToMessage { get; set; }

    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeletedForSender { get; set; }
    public bool IsDeletedForRecipient { get; set; }
    public DateTime? DeletedForEveryoneAt { get; set; }

    public ICollection<MessageAttachment> Attachments { get; set; } = [];
}
