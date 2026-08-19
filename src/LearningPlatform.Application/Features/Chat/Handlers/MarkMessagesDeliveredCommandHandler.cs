using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class MarkMessagesDeliveredCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<MarkMessagesDeliveredCommand, ApiResponse<int>>
{
    public async Task<ApiResponse<int>> Handle(MarkMessagesDeliveredCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        if (request.MessageIds.Count == 0)
            return ApiResponse<int>.Success(0);

        var isParticipant = await unitOfWork.Repository<ConversationParticipant>()
            .ExistsAsync(
                p => p.ConversationId == request.ConversationId && p.UserId == userId,
                cancellationToken);

        if (!isParticipant)
            throw new ForbiddenException("You are not a participant of this conversation.");

        var messageIds = request.MessageIds.Distinct().ToList();

        var messages = await unitOfWork.Repository<ChatMessage>().GetTrackedListAsync(
            m => m.ConversationId == request.ConversationId
                && messageIds.Contains(m.Id)
                && m.SenderId != userId
                && m.Status == ChatMessageStatus.Sent,
            cancellationToken);

        var now = dateTimeProvider.UtcNow;

        foreach (var message in messages)
        {
            message.Status = ChatMessageStatus.Delivered;
            message.DeliveredAt = now;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Success(messages.Count, "Messages marked as delivered.");
    }
}
