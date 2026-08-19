using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Common;
using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Application.Features.Chat.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Chat.Services;

public class ChatQueryService(
    IUnitOfWork unitOfWork,
    IPresenceTracker presenceTracker)
    : IChatQueryService
{
    public async Task<PaginatedResponse<ConversationDto>> GetMyConversationsAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var participantQuery = unitOfWork.Repository<ConversationParticipant>().AsQueryable()
            .Where(p => p.UserId == userId && !p.IsHidden);

        var totalCount = await participantQuery.CountAsync(cancellationToken);

        var page = await participantQuery
            .OrderByDescending(p => p.Conversation.LastMessageAt ?? p.Conversation.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new { p.ConversationId, p.IsMuted })
            .ToListAsync(cancellationToken);

        if (page.Count == 0)
        {
            return PaginatedResponse<ConversationDto>.Create(
                new PaginatedList<ConversationDto>([], 0, pageNumber, pageSize));
        }

        var conversationIds = page.Select(x => x.ConversationId).ToList();

        var conversations = await unitOfWork.Repository<Conversation>().AsQueryable()
            .Where(c => conversationIds.Contains(c.Id))
            .Include(c => c.Participants)
            .ThenInclude(p => p.User)
            .ToListAsync(cancellationToken);

        var latest = await unitOfWork.Repository<ChatMessage>().AsQueryable()
            .Where(m => conversationIds.Contains(m.ConversationId))
            .Where(m => m.DeletedForEveryoneAt == null)
            .Where(m => !(m.IsDeletedForSender && m.SenderId == userId))
            .Where(m => !(m.IsDeletedForRecipient && m.SenderId != userId))
            .GroupBy(m => m.ConversationId)
            .Select(g => new
            {
                ConversationId = g.Key,
                MessageId = g.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                    .Select(x => x.Id).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var lastMessageIds = latest.Where(x => x.MessageId != Guid.Empty).Select(x => x.MessageId).ToList();

        var lastMessages = await LoadMessagesAsync(lastMessageIds, cancellationToken);

        var unreadCounts = await unitOfWork.Repository<ChatMessage>().AsQueryable()
            .Where(m => conversationIds.Contains(m.ConversationId) && m.SenderId != userId && m.Status != ChatMessageStatus.Read)
            .Where(m => m.DeletedForEveryoneAt == null)
            .GroupBy(m => m.ConversationId)
            .Select(g => new { ConversationId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var onlineIds = await presenceTracker.GetOnlineUserIdsAsync();
        var onlineSet = onlineIds.ToHashSet();
        var lastMessagesById = lastMessages.ToDictionary(m => m.Id);
        var latestByConversation = latest.ToDictionary(x => x.ConversationId);
        var unreadByConversation = unreadCounts.ToDictionary(x => x.ConversationId);

        var items = conversations
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .Select(c =>
            {
                var mine = c.Participants.First(p => p.UserId == userId);
                var other = c.Participants.First(p => p.UserId != userId);
                var messageId = latestByConversation.GetValueOrDefault(c.Id)?.MessageId ?? Guid.Empty;
                var lastMessage = messageId != Guid.Empty && lastMessagesById.TryGetValue(messageId, out var m)
                    ? ChatMapper.ToMessageDto(m)
                    : null;

                return new ConversationDto
                {
                    Id = c.Id,
                    OtherUser = ChatMapper.ToChatUserDto(other.User, onlineSet.Contains(other.UserId)),
                    LastMessage = lastMessage,
                    LastMessageAt = c.LastMessageAt,
                    UnreadCount = unreadByConversation.GetValueOrDefault(c.Id)?.Count ?? 0,
                    IsMuted = mine.IsMuted,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                };
            })
            .ToList();

        return PaginatedResponse<ConversationDto>.Create(
            new PaginatedList<ConversationDto>(items, totalCount, pageNumber, pageSize));
    }

    public async Task<ConversationDto> GetConversationAsync(
        Guid conversationId, Guid userId, CancellationToken cancellationToken)
    {
        var conversation = await unitOfWork.Repository<Conversation>().AsQueryable()
            .Include(c => c.Participants)
            .ThenInclude(p => p.User)
            .Where(c => c.Id == conversationId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Conversation), conversationId);

        if (!conversation.Participants.Any(p => p.UserId == userId))
            throw new ForbiddenException("You are not a participant of this conversation.");

        var mine = conversation.Participants.First(p => p.UserId == userId);
        var other = conversation.Participants.First(p => p.UserId != userId);

        var lastMessage = await GetLatestMessageAsync(conversationId, userId, cancellationToken);
        var unreadCount = await GetUnreadCountAsync(conversationId, userId, cancellationToken);
        var isOnline = await presenceTracker.IsOnlineAsync(other.UserId);

        return new ConversationDto
        {
            Id = conversation.Id,
            OtherUser = ChatMapper.ToChatUserDto(other.User, isOnline),
            LastMessage = lastMessage,
            LastMessageAt = conversation.LastMessageAt,
            UnreadCount = unreadCount,
            IsMuted = mine.IsMuted,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt
        };
    }

    public async Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken) =>
        await unitOfWork.Repository<ChatMessage>().AsQueryable()
            .CountAsync(
                m => m.ConversationId == conversationId
                    && m.SenderId != userId
                    && m.Status != ChatMessageStatus.Read
                    && m.DeletedForEveryoneAt == null,
                cancellationToken);

    public async Task<MessageDto?> GetMessageAsync(Guid messageId, Guid userId, CancellationToken cancellationToken)
    {
        var message = await unitOfWork.Repository<ChatMessage>().AsQueryable()
            .Include(m => m.Sender)
            .Include(m => m.ReplyToMessage).ThenInclude(r => r!.Sender)
            .Include(m => m.Attachments)
            .Where(m => m.Id == messageId)
            .FirstOrDefaultAsync(cancellationToken);

        if (message is null || message.DeletedForEveryoneAt is not null
            || (message.IsDeletedForSender && message.SenderId == userId)
            || (message.IsDeletedForRecipient && message.SenderId != userId))
            return null;

        return ChatMapper.ToMessageDto(message);
    }

    private async Task<MessageDto?> GetLatestMessageAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken)
    {
        var message = await unitOfWork.Repository<ChatMessage>().AsQueryable()
            .Where(m => m.ConversationId == conversationId)
            .Where(m => m.DeletedForEveryoneAt == null)
            .Where(m => !(m.IsDeletedForSender && m.SenderId == userId))
            .Where(m => !(m.IsDeletedForRecipient && m.SenderId != userId))
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Select(m => m.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (message == Guid.Empty)
            return null;

        return await GetMessageAsync(message, userId, cancellationToken);
    }

    private async Task<List<ChatMessage>> LoadMessagesAsync(List<Guid> messageIds, CancellationToken cancellationToken)
    {
        if (messageIds.Count == 0)
            return [];

        return await unitOfWork.Repository<ChatMessage>().AsQueryable()
            .Where(m => messageIds.Contains(m.Id))
            .Include(m => m.Sender)
            .Include(m => m.ReplyToMessage).ThenInclude(r => r!.Sender)
            .Include(m => m.Attachments)
            .ToListAsync(cancellationToken);
    }
}
