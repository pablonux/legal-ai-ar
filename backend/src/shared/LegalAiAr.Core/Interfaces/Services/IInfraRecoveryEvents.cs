namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Optional broadcast when infrastructure recovery is acknowledged (API → SignalR clients).
/// </summary>
public interface IInfraRecoveryEvents
{
    Task BroadcastInfraRecoveredAsync(Guid? ingestionJobId, string? detail, CancellationToken cancellationToken = default);
}
