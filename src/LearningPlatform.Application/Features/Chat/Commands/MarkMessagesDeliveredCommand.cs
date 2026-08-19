using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Commands;

public record MarkMessagesDeliveredCommand(Guid ConversationId, List<Guid> MessageIds) : IRequest<ApiResponse<int>>;
