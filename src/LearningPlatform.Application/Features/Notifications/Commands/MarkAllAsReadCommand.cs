using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Notifications.Commands;

public record MarkAllAsReadCommand : IRequest<ApiResponse>;
