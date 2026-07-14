using LearningPlatform.Application.Features.Notifications.DTOs;
using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.Notifications.Queries;

public record GetMyNotificationsQuery : IRequest<PaginatedResponse<NotificationDto>>
{
    public bool UnreadOnly { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
