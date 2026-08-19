using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class MarkConversationAsReadCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<MarkConversationAsReadCommand, ApiResponse<int>>
{
    public async Task<ApiResponse<int>> Handle(MarkConversationAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;
        var messageRepo = unitOfWork.Repository<ChatMessage>();

        var participant = await unitOfWork.Repository<ConversationParticipant>()
            .GetTrackedAsync(p => p.ConversationId == request.ConversationId && p.UserId == userId, cancellationToken)
            ?? throw new ForbiddenException("You are not a participant of this conversation.");

        var upToId = request.LastMessageId;

        if (upToId is null)
        {
            upToId = await messageRepo.AsQueryable()
                .Where(m => m.ConversationId == request.ConversationId)
                .OrderByDescending(m => m.CreatedAt)
                .ThenByDescending(m => m.Id)
                .Select(m => m.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (upToId is null || upToId == Guid.Empty)
            return ApiResponse<int>.Success(0, "No messages to mark as read.");

        var target = await messageRepo.AsQueryable()
            .Where(m => m.Id == upToId.Value && m.ConversationId == request.ConversationId)
            .Select(m => m.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (target == default)
            throw new BadRequestException("The message does not belong to this conversation.");

        var now = dateTimeProvider.UtcNow;

        var unread = await messageRepo.GetTrackedListAsync(
            m => m.ConversationId == request.ConversationId
                && m.SenderId != userId
                && m.Status != ChatMessageStatus.Read
                && m.DeletedForEveryoneAt == null
                && m.CreatedAt <= target,
            cancellationToken);

        foreach (var message in unread)
        {
            message.Status = ChatMessageStatus.Read;
            message.ReadAt = now;
        }

        participant.LastReadMessageId = upToId.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Success(unread.Count, "Messages marked as read.");
    }
}
