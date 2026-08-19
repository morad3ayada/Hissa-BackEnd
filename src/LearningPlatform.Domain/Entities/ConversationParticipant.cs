using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

public class ConversationParticipant : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public bool IsMuted { get; set; }
    public bool IsHidden { get; set; }
    public DateTime? HiddenAt { get; set; }
    public Guid? LastReadMessageId { get; set; }
}
