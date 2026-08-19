using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Commands;

public record UnblockUserCommand(Guid UserId) : IRequest<ApiResponse>;
