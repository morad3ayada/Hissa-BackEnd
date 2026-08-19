using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Chat.Common;

public static class ChatMapper
{
    public static MessageDto ToMessageDto(ChatMessage message)
    {
        var reply = message.ReplyToMessage;

        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderName = GetDisplayName(message.Sender),
            Content = message.Content,
            MessageType = message.MessageType.ToString(),
            Status = message.Status.ToString(),
            DeliveredAt = message.DeliveredAt,
            ReadAt = message.ReadAt,
            IsEdited = message.IsEdited,
            IsDeletedForEveryone = message.DeletedForEveryoneAt.HasValue,
            ReplyToMessageId = message.ReplyToMessageId,
            ReplyToContent = reply?.Content,
            ReplyToSenderName = GetDisplayName(reply?.Sender),
            CreatedAt = message.CreatedAt,
            Attachments = message.Attachments
                .Where(a => message.DeletedForEveryoneAt is null)
                .Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FileUrl = a.FileUrl,
                    ContentType = a.ContentType,
                    FileSize = a.FileSize,
                    AttachmentType = a.AttachmentType.ToString()
                })
                .ToList()
        };
    }

    public static ChatUserDto ToChatUserDto(ApplicationUser user, bool isOnline = false) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        ProfilePictureUrl = user.ProfilePictureUrl,
        IsOnline = isOnline
    };

    public static string GetDisplayName(ApplicationUser? user) =>
        user is null ? string.Empty : $"{user.FirstName} {user.LastName}".Trim();
}
