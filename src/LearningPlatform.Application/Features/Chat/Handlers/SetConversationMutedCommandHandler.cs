using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class SetConversationMutedCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<SetConversationMutedCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(SetConversationMutedCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var participant = await unitOfWork.Repository<ConversationParticipant>()
            .GetTrackedAsync(p => p.ConversationId == request.ConversationId && p.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Conversation), request.ConversationId);

        participant.IsMuted = request.IsMuted;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success(request.IsMuted ? "Conversation muted." : "Conversation unmuted.");
    }
}
