namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Optional hook for workers to notify the API hub about infrastructure failures (e.g. Storage 403).
/// </summary>
public interface IWorkerInfraNotifier
{
    /// <summary>
    /// Best-effort: no-op when SignalR is not connected.
    /// </summary>
    ValueTask NotifyInfrastructureIncidentAsync(
        string queueName,
        string? errorCode,
        string? detail,
        Guid? ingestionJobId = null,
        CancellationToken cancellationToken = default);
}
