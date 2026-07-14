using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Notifications.Commands;

public record DeleteNotificationCommand(Guid Id) : IRequest<ApiResponse>;
