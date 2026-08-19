using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Chat.Commands;

public record BlockUserCommand(Guid UserId) : IRequest<ApiResponse>;
