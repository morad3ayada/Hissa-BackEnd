using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Notifications.DTOs;
using LearningPlatform.Application.Features.Notifications.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Notifications.Handlers;

public class GetMyNotificationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetMyNotificationsQuery, PaginatedResponse<NotificationDto>>
{
    public async Task<PaginatedResponse<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var query = unitOfWork.Repository<Notification>().AsQueryable()
            .Where(n => n.UserId == userId);

        if (request.UnreadOnly)
            query = query.Where(n => !n.IsRead);

        var totalCount = await query.CountAsync(cancellationToken);

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type.ToString(),
                IsRead = n.IsRead,
                ReadAt = n.ReadAt,
                ActionUrl = n.ActionUrl,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedList<NotificationDto>(notifications, totalCount, request.PageNumber, request.PageSize);

        return PaginatedResponse<NotificationDto>.Create(paginatedList);
    }
}
