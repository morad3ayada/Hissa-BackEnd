using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Commands;

public record DeleteMessageCommand(Guid MessageId, bool ForEveryone = false) : IRequest<ApiResponse>;
