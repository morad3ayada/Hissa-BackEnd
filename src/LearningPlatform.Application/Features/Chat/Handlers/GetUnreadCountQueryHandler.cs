using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Application.Features.Chat.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class GetUnreadCountQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<GetUnreadCountQuery, ApiResponse<UnreadCountDto>>
{
    public async Task<ApiResponse<UnreadCountDto>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var visibleConversationIds = unitOfWork.Repository<ConversationParticipant>().AsQueryable()
            .Where(p => p.UserId == userId && !p.IsHidden)
            .Select(p => p.ConversationId);

        var unreadGroups = await unitOfWork.Repository<ChatMessage>().AsQueryable()
            .Where(m => visibleConversationIds.Contains(m.ConversationId))
            .Where(m => m.SenderId != userId)
            .Where(m => m.Status != ChatMessageStatus.Read)
            .Where(m => m.DeletedForEveryoneAt == null)
            .GroupBy(m => m.ConversationId)
            .Select(g => new { ConversationId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var dto = new UnreadCountDto
        {
            TotalUnreadMessages = unreadGroups.Sum(g => g.Count),
            ConversationsWithUnread = unreadGroups.Count
        };

        return ApiResponse<UnreadCountDto>.Success(dto);
    }
}
