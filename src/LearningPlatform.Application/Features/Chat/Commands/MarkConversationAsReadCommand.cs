using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Commands;

public record MarkConversationAsReadCommand(Guid ConversationId, Guid? LastMessageId = null) : IRequest<ApiResponse<int>>;
