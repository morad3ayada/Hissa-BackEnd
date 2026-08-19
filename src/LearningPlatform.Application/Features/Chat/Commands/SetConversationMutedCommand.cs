using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Commands;

public record SetConversationMutedCommand(Guid ConversationId, bool IsMuted) : IRequest<ApiResponse>;
