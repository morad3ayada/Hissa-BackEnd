using System.Collections.Concurrent;
using LearningPlatform.Application.Common.Interfaces;

namespace LearningPlatform.Infrastructure.Chat;

/// <summary>
/// Tracks which users currently have an active SignalR connection, keyed by user id
/// with a set of connection ids per user (a user may be online on many devices).
/// </summary>
public class PresenceTracker : IPresenceTracker
{
    private readonly ConcurrentDictionary<Guid, HashSet<string>> _connections = new();

    public Task AddConnectionAsync(Guid userId, string connectionId)
    {
        _connections.AddOrUpdate(
            userId,
            _ => new HashSet<string> { connectionId },
            (_, connections) =>
            {
                lock (connections)
                {
                    connections.Add(connectionId);
                }
                return connections;
            });

        return Task.CompletedTask;
    }

    public Task RemoveConnectionAsync(Guid userId, string connectionId)
    {
        if (_connections.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                connections.Remove(connectionId);

                if (connections.Count == 0)
                    _connections.TryRemove(userId, out _);
            }
        }

        return Task.CompletedTask;
    }

    public Task<bool> IsOnlineAsync(Guid userId) =>
        Task.FromResult(_connections.ContainsKey(userId));

    public Task<int> GetConnectionCountAsync(Guid userId) =>
        Task.FromResult(
            _connections.TryGetValue(userId, out var connections)
                ? connections.Count
                : 0);

    public Task<IReadOnlyCollection<Guid>> GetOnlineUserIdsAsync()
    {
        IReadOnlyCollection<Guid> onlineIds = _connections.Keys.ToList();
        return Task.FromResult(onlineIds);
    }
}
