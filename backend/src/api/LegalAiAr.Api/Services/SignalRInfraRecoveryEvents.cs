using LegalAiAr.Api.Hubs;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace LegalAiAr.Api.Services;

/// <summary>
/// Broadcasts infra recovery to SignalR clients (admin UI).
/// </summary>
public sealed class SignalRInfraRecoveryEvents : IInfraRecoveryEvents
{
    private readonly IHubContext<WorkerControlHub, IWorkerControlClient> _hubContext;

    public SignalRInfraRecoveryEvents(IHubContext<WorkerControlHub, IWorkerControlClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task BroadcastInfraRecoveredAsync(Guid? ingestionJobId, string? detail, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.All.InfraRecoveredAsync(ingestionJobId, detail);
    }
}
