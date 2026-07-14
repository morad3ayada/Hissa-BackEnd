using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Common.Services;

public class NotificationService(IUnitOfWork unitOfWork) : INotificationService
{
    public async Task CreateAsync(
        Guid userId, NotificationType type, string title, string message,
        string? actionUrl = null, CancellationToken cancellationToken = default)
    {
        await unitOfWork.Repository<Notification>().AddAsync(new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            ActionUrl = actionUrl
        }, cancellationToken);
    }
}
