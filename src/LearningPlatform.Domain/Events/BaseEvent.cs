using MediatR;

namespace LearningPlatform.Domain.Events;

public abstract class BaseEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
