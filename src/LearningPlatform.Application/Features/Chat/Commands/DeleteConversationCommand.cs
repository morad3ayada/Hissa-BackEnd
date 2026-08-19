using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Commands;

public record DeleteConversationCommand(Guid ConversationId) : IRequest<ApiResponse>;
