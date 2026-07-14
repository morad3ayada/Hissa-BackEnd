using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Common.Interfaces;

/// <summary>In-app notification creation, usable from any handler across modules. Stores the
/// notification directly in the database (no email/push/SMS delivery). Stages the Notification
/// only; the caller's handler must still call SaveChangesAsync.</summary>
public interface INotificationService
{
    Task CreateAsync(
        Guid userId, NotificationType type, string title, string message,
        string? actionUrl = null, CancellationToken cancellationToken = default);
}
