using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Commands;

public record SendMessageCommand : IRequest<ApiResponse<MessageDto>>
{
    public Guid ConversationId { get; init; }
    public string? Content { get; init; }
    public Guid? ReplyToMessageId { get; init; }
    public List<MessageAttachmentInput> Attachments { get; init; } = [];
}

public record MessageAttachmentInput
{
    public string FileName { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public AttachmentType AttachmentType { get; init; }
}
