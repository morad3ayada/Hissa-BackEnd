using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Notifications.Commands;

public record MarkAsReadCommand(Guid Id) : IRequest<ApiResponse>;
