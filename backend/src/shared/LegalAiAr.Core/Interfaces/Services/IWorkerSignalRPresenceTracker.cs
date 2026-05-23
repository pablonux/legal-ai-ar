namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Counts pipeline worker processes connected to the admin worker-control SignalR hub (per worker type).
/// </summary>
public interface IWorkerSignalRPresenceTracker
{
    void RegisterConnection(string connectionId, string workerType);

    void UnregisterConnection(string connectionId);

    /// <summary>Worker type → number of active SignalR connections (usually 0 or 1 per type).</summary>
    IReadOnlyDictionary<string, int> GetConnectedCountsByWorkerType();
}
