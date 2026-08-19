using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Queries;

public record GetConversationMessagesQuery : IRequest<PaginatedResponse<MessageDto>>
{
    public Guid ConversationId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 30;
    public Guid? BeforeMessageId { get; init; }
}
