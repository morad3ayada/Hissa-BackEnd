namespace LearningPlatform.Application.Common.Interfaces;

/// <summary>In-memory real-time presence tracking for chat (connection count per user).</summary>
public interface IPresenceTracker
{
    Task AddConnectionAsync(Guid userId, string connectionId);
    Task RemoveConnectionAsync(Guid userId, string connectionId);
    Task<bool> IsOnlineAsync(Guid userId);
    Task<int> GetConnectionCountAsync(Guid userId);
    Task<IReadOnlyCollection<Guid>> GetOnlineUserIdsAsync();
}
