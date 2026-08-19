using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class Conversation : BaseEntity
{
    public DateTime? LastMessageAt { get; set; }
    public Guid? LastMessageId { get; set; }

    public ICollection<ConversationParticipant> Participants { get; set; } = [];
    public ICollection<ChatMessage> Messages { get; set; } = [];
}
