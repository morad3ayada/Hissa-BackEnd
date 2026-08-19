using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities;

public class MessageAttachment : BaseEntity
{
    public Guid MessageId { get; set; }
    public ChatMessage Message { get; set; } = null!;

    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public AttachmentType AttachmentType { get; set; }
}
