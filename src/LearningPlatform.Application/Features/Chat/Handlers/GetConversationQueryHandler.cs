using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Application.Features.Chat.Interfaces;
using LearningPlatform.Application.Features.Chat.Queries;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class GetConversationQueryHandler(
    ICurrentUserService currentUser,
    IChatQueryService chatQueryService)
    : IRequestHandler<GetConversationQuery, ApiResponse<ConversationDto>>
{
    public async Task<ApiResponse<ConversationDto>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
    {
        var dto = await chatQueryService.GetConversationAsync(
            request.ConversationId, currentUser.UserId!.Value, cancellationToken);

        return ApiResponse<ConversationDto>.Success(dto);
    }
}
