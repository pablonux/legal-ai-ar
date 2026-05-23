using System.Collections.Concurrent;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Infrastructure.Control;

/// <summary>
/// Tracks worker host processes connected to the worker-control SignalR hub (one connection per process).
/// </summary>
public sealed class WorkerSignalRPresenceTracker : IWorkerSignalRPresenceTracker
{
    private readonly ConcurrentDictionary<string, string> _connectionToWorkerType = new();
    private readonly ConcurrentDictionary<string, int> _countsByWorkerType = new();

    public void RegisterConnection(string connectionId, string workerType)
    {
        if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(workerType))
            return;

        workerType = workerType.Trim();
        UnregisterConnection(connectionId);

        _connectionToWorkerType[connectionId] = workerType;
        _countsByWorkerType.AddOrUpdate(workerType, 1, (_, n) => n + 1);
    }

    public void UnregisterConnection(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return;

        if (!_connectionToWorkerType.TryRemove(connectionId, out var workerType))
            return;

        _countsByWorkerType.AddOrUpdate(workerType, 0, (_, n) => Math.Max(0, n - 1));
    }

    public IReadOnlyDictionary<string, int> GetConnectedCountsByWorkerType()
    {
        return new Dictionary<string, int>(_countsByWorkerType.Where(kv => kv.Value > 0));
    }
}
