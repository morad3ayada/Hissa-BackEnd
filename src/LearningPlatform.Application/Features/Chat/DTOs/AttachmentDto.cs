namespace LearningPlatform.Application.Features.Chat.DTOs;

public class AttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string AttachmentType { get; set; } = string.Empty;
}
