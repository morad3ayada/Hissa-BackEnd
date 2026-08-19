using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class IsConversationParticipantQueryHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<IsConversationParticipantQuery, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(IsConversationParticipantQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var isParticipant = await unitOfWork.Repository<ConversationParticipant>()
            .ExistsAsync(
                p => p.ConversationId == request.ConversationId && p.UserId == userId && !p.IsHidden,
                cancellationToken);

        return ApiResponse<bool>.Success(isParticipant);
    }
}
