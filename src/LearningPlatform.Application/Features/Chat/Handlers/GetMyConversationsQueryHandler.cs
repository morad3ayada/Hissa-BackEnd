using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Application.Features.Chat.Interfaces;
using LearningPlatform.Application.Features.Chat.Queries;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Handlers;

public class GetMyConversationsQueryHandler(
    ICurrentUserService currentUser,
    IChatQueryService chatQueryService)
    : IRequestHandler<GetMyConversationsQuery, PaginatedResponse<ConversationDto>>
{
    public Task<PaginatedResponse<ConversationDto>> Handle(GetMyConversationsQuery request, CancellationToken cancellationToken) =>
        chatQueryService.GetMyConversationsAsync(
            currentUser.UserId!.Value, request.PageNumber, request.PageSize, cancellationToken);
}
