using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Infra;

/// <summary>
/// Default no-op for hosts that do not broadcast SignalR (workers, tests override in API).
/// </summary>
public sealed class NoOpInfraRecoveryEvents : IInfraRecoveryEvents
{
    public Task BroadcastInfraRecoveredAsync(Guid? ingestionJobId, string? detail, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
