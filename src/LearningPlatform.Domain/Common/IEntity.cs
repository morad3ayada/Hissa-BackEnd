namespace LearningPlatform.Domain.Common;

public interface IEntity<out TId>
{
    TId Id { get; }
}
