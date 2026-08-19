using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Shared.Wrappers;

namespace LearningPlatform.Application.Features.Chat.Interfaces;

public interface IChatQueryService
{
    Task<PaginatedResponse<ConversationDto>> GetMyConversationsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<ConversationDto> GetConversationAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken);

    Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken);

    Task<MessageDto?> GetMessageAsync(Guid messageId, Guid userId, CancellationToken cancellationToken);
}
