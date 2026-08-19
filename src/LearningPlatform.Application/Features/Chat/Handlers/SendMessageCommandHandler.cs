using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Application.Features.Chat.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class SendMessageCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider,
    IChatQueryService chatQueryService,
    INotificationService notificationService)
    : IRequestHandler<SendMessageCommand, ApiResponse<MessageDto>>
{
    public async Task<ApiResponse<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;
        var isTextOnly = string.IsNullOrWhiteSpace(request.Content) && request.Attachments.Count == 0;

        if (isTextOnly)
            throw new BadRequestException("Message content or at least one attachment is required.");

        var messageRepo = unitOfWork.Repository<ChatMessage>();
        var participantRepo = unitOfWork.Repository<ConversationParticipant>();

        var participants = await participantRepo.AsQueryable()
            .Where(p => p.ConversationId == request.ConversationId)
            .ToListAsync(cancellationToken);

        var mine = participants.FirstOrDefault(p => p.UserId == userId)
            ?? throw new ForbiddenException("You are not a participant of this conversation.");

        var other = participants.FirstOrDefault(p => p.UserId != userId)
            ?? throw new NotFoundException("The other participant of this conversation was not found.");

        if (mine.IsHidden)
            throw new NotFoundException(nameof(Conversation), request.ConversationId);

        var isBlockedByOther = await unitOfWork.Repository<BlockedUser>()
            .ExistsAsync(b => b.UserId == other.UserId && b.BlockedUserId == userId, cancellationToken);

        if (isBlockedByOther)
            throw new ForbiddenException("You are blocked by this user and cannot send messages.");

        if (request.ReplyToMessageId is not null)
        {
            var replyExists = await messageRepo.ExistsAsync(
                m => m.Id == request.ReplyToMessageId && m.ConversationId == request.ConversationId, cancellationToken);

            if (!replyExists)
                throw new BadRequestException("The replied-to message does not exist in this conversation.");
        }

        var content = request.Content?.Trim();
        if (content is { Length: > 5000 })
            throw new BadRequestException("Message content cannot exceed 5000 characters.");

        var messageType = ResolveMessageType(request.Attachments);
        var message = new ChatMessage
        {
            ConversationId = request.ConversationId,
            SenderId = userId,
            Content = content,
            MessageType = messageType,
            Status = ChatMessageStatus.Sent,
            ReplyToMessageId = request.ReplyToMessageId,
            Attachments = request.Attachments
                .Where(a => !string.IsNullOrWhiteSpace(a.FileUrl))
                .Select(a => new MessageAttachment
                {
                    FileName = a.FileName,
                    FileUrl = a.FileUrl,
                    ContentType = a.ContentType,
                    FileSize = a.FileSize,
                    AttachmentType = a.AttachmentType
                })
                .ToList()
        };

        await messageRepo.AddAsync(message, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var conversation = await unitOfWork.Repository<Conversation>()
            .GetTrackedAsync(c => c.Id == request.ConversationId, cancellationToken);

        if (conversation is not null)
        {
            conversation.LastMessageAt = dateTimeProvider.UtcNow;
            conversation.LastMessageId = message.Id;
        }

        var senderParticipant = await participantRepo
            .GetTrackedAsync(p => p.ConversationId == request.ConversationId && p.UserId == userId, cancellationToken);

        if (senderParticipant is not null)
            senderParticipant.LastReadMessageId = message.Id;

        var otherParticipant = await participantRepo
            .GetTrackedAsync(p => p.ConversationId == request.ConversationId && p.UserId == other.UserId, cancellationToken);

        if (otherParticipant is { IsHidden: true })
        {
            otherParticipant.IsHidden = false;
            otherParticipant.HiddenAt = null;
        }

        if (!other.IsMuted)
        {
            await notificationService.CreateAsync(
                other.UserId,
                NotificationType.Info,
                "New message",
                BuildNotificationPreview(message, content),
                $"/chat/{request.ConversationId}",
                cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await chatQueryService.GetMessageAsync(message.Id, userId, cancellationToken)
            ?? throw new NotFoundException(nameof(ChatMessage), message.Id);

        return ApiResponse<MessageDto>.Success(dto, "Message sent.");
    }

    private static ChatMessageType ResolveMessageType(IReadOnlyList<MessageAttachmentInput> attachments) =>
        attachments.Count == 0
            ? ChatMessageType.Text
            : attachments[0].AttachmentType switch
            {
                AttachmentType.Image => ChatMessageType.Image,
                AttachmentType.Video => ChatMessageType.Video,
                AttachmentType.Audio => ChatMessageType.Audio,
                _ => ChatMessageType.File
            };

    private static string BuildNotificationPreview(ChatMessage message, string? content)
    {
        if (!string.IsNullOrWhiteSpace(content))
            return content.Length > 120 ? content[..120] + "..." : content;

        return message.MessageType switch
        {
            ChatMessageType.Image => "sent an image",
            ChatMessageType.Video => "sent a video",
            ChatMessageType.Audio => "sent an audio message",
            _ => "sent a file"
        };
    }
}
