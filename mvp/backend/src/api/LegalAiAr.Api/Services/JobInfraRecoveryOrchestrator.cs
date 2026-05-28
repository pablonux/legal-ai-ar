using LegalAiAr.Api.Hubs;
using LegalAiAr.Application.Admin.Jobs.Commands.RecoverJobFromInfra;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Entities;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Api.Services;

/// <summary>
/// Runs post-infra recovery (mediator + resume all pipeline workers in SignalR + DB pause state).
/// </summary>
public sealed class JobInfraRecoveryOrchestrator
{
    /// <summary>WorkerType values matching <c>AddWorkerControlGate(..., workerType)</c>.</summary>
    public static readonly string[] PipelineWorkerTypes =
    [
        "Discoverer", "Fetcher", "Parser", "Enricher", "Persister", "Indexer",
    ];

    private readonly IMediator _mediator;
    private readonly IHubContext<WorkerControlHub, IWorkerControlClient> _hubContext;
    private readonly AppDbContext _db;

    public JobInfraRecoveryOrchestrator(
        IMediator mediator,
        IHubContext<WorkerControlHub, IWorkerControlClient> hubContext,
        AppDbContext db)
    {
        _mediator = mediator;
        _hubContext = hubContext;
        _db = db;
    }

    public async Task<RecoverJobFromInfraResultDto> RecoverJobAsync(
        RecoverJobFromInfraCommand command,
        bool resumeAllWorkers,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (resumeAllWorkers && result.StorageProbeOk)
        {
            foreach (var workerType in PipelineWorkerTypes)
            {
                await UpsertPauseStateAsync(workerType, isPaused: false, cancellationToken);
                await _hubContext.Clients.Group(workerType).ResumeAsync();
            }
        }

        return result;
    }

    private async Task UpsertPauseStateAsync(string workerType, bool isPaused, CancellationToken ct)
    {
        var existing = await _db.WorkerPauseStates.FindAsync([workerType], ct);
        if (existing is not null)
        {
            existing.IsPaused = isPaused;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _db.WorkerPauseStates.Add(new WorkerPauseState
            {
                WorkerType = workerType,
                IsPaused = isPaused,
                UpdatedAt = DateTime.UtcNow,
            });
        }

        await _db.SaveChangesAsync(ct);
    }
}
