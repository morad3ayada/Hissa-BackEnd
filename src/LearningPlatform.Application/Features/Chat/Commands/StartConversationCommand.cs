using LearningPlatform.Application.Features.Chat.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Commands;

public record StartConversationCommand(Guid OtherUserId) : IRequest<ApiResponse<ConversationDto>>;
