using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Queries;

public record IsConversationParticipantQuery(Guid ConversationId) : IRequest<ApiResponse<bool>>;
