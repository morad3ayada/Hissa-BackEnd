using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Notifications.Commands;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Notifications.Handlers;

public class MarkAllAsReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<MarkAllAsReadCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var unread = await unitOfWork.Repository<Notification>().AsQueryable()
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var notification in unread)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
            unitOfWork.Repository<Notification>().Update(notification);
        }

        if (unread.Count > 0)
            await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success($"{unread.Count} notification(s) marked as read.");
    }
}
