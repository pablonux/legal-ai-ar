using System.Collections.Concurrent;
using LegalAiAr.Api.Hubs;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace LegalAiAr.Api.Services;

/// <summary>
/// Persists infra-degraded flags on jobs when a worker reports an incident with a job id,
/// and broadcasts <see cref="IWorkerControlClient.InfraDegradedAsync"/> to all hub clients.
/// </summary>
public sealed class InfraIncidentCoordinator
{
    private static readonly TimeSpan DegradedBroadcastDedupeWindow = TimeSpan.FromSeconds(45);

    private readonly IHubContext<WorkerControlHub, IWorkerControlClient> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<InfraIncidentCoordinator> _logger;
    private readonly ConcurrentDictionary<string, long> _degradedBroadcastTicks = new();

    public InfraIncidentCoordinator(
        IHubContext<WorkerControlHub, IWorkerControlClient> hubContext,
        IServiceScopeFactory scopeFactory,
        ILogger<InfraIncidentCoordinator> logger)
    {
        _hubContext = hubContext;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task HandleWorkerReportAsync(InfraIncidentReport report, CancellationToken cancellationToken)
    {
        if (report.IngestionJobId is { } jobId)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var jobs = scope.ServiceProvider.GetRequiredService<IIngestionJobRepository>();
                var reason =
                    $"{report.Category} {report.ErrorCode} {report.QueueName}: {report.Detail}".Trim();
                await jobs.SetInfrastructureDegradedAsync(
                    jobId,
                    degraded: true,
                    DateTime.UtcNow,
                    reason,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist infrastructure degraded for job {JobId}", jobId);
            }
        }

        if (!ShouldBroadcastDegraded(report))
            return;

        try
        {
            await _hubContext.Clients.All.InfraDegradedAsync(report);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "InfraDegraded broadcast failed");
        }
    }

    private bool ShouldBroadcastDegraded(InfraIncidentReport r)
    {
        var key = $"{r.WorkerType}\u001f{r.QueueName}\u001f{r.ErrorCode}\u001f{r.IngestionJobId}";
        var now = DateTime.UtcNow.Ticks;
        var window = DegradedBroadcastDedupeWindow.Ticks;

        while (true)
        {
            if (_degradedBroadcastTicks.TryGetValue(key, out var prev))
            {
                if (now - prev < window)
                    return false;
                if (_degradedBroadcastTicks.TryUpdate(key, now, prev))
                    return true;
            }
            else if (_degradedBroadcastTicks.TryAdd(key, now))
                return true;
        }
    }
}
