using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Common;
using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Application.Features.Chat.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class GetConversationMessagesQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<GetConversationMessagesQuery, PaginatedResponse<MessageDto>>
{
    public async Task<PaginatedResponse<MessageDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var isParticipant = await unitOfWork.Repository<ConversationParticipant>()
            .ExistsAsync(
                p => p.ConversationId == request.ConversationId && p.UserId == userId && !p.IsHidden,
                cancellationToken);

        if (!isParticipant)
            throw new NotFoundException(nameof(Conversation), request.ConversationId);

        var query = unitOfWork.Repository<ChatMessage>().AsQueryable()
            .Where(m => m.ConversationId == request.ConversationId)
            .Where(m => m.DeletedForEveryoneAt == null)
            .Where(m => !(m.IsDeletedForSender && m.SenderId == userId))
            .Where(m => !(m.IsDeletedForRecipient && m.SenderId != userId));

        if (request.BeforeMessageId is not null)
        {
            var before = await query
                .Where(m => m.Id == request.BeforeMessageId.Value)
                .Select(m => m.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (before != default)
                query = query.Where(m => m.CreatedAt < before);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(m => m.Sender)
            .Include(m => m.ReplyToMessage)
            .ThenInclude(r => r!.Sender)
            .Include(m => m.Attachments)
            .ToListAsync(cancellationToken);

        var items = messages.Select(ChatMapper.ToMessageDto).ToList();

        return PaginatedResponse<MessageDto>.Create(
            new PaginatedList<MessageDto>(items, totalCount, request.PageNumber, request.PageSize));
    }
}
