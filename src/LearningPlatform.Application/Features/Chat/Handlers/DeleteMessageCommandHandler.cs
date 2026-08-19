using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class DeleteMessageCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<DeleteMessageCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;
        var messageRepo = unitOfWork.Repository<ChatMessage>();

        var message = await messageRepo.GetTrackedAsync(m => m.Id == request.MessageId, cancellationToken)
            ?? throw new NotFoundException(nameof(ChatMessage), request.MessageId);

        var isParticipant = await unitOfWork.Repository<ConversationParticipant>()
            .ExistsAsync(
                p => p.ConversationId == message.ConversationId && p.UserId == userId,
                cancellationToken);

        if (!isParticipant)
            throw new ForbiddenException("You are not a participant of this conversation.");

        if (request.ForEveryone)
        {
            if (message.SenderId != userId)
                throw new ForbiddenException("You can only delete your own messages for everyone.");

            if (message.DeletedForEveryoneAt is not null)
                return ApiResponse.Success("Message already deleted.");

            message.DeletedForEveryoneAt = dateTimeProvider.UtcNow;
            message.Content = null;
        }
        else
        {
            if (message.SenderId == userId)
            {
                if (message.IsDeletedForSender)
                    return ApiResponse.Success("Message already deleted.");
                message.IsDeletedForSender = true;
            }
            else
            {
                if (message.IsDeletedForRecipient)
                    return ApiResponse.Success("Message already deleted.");
                message.IsDeletedForRecipient = true;
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success(request.ForEveryone ? "Message deleted for everyone." : "Message deleted.");
    }
}
