using LegalAiAr.Application.Admin.Jobs.Commands.RequeueFetcherPending;
using LegalAiAr.Application.Admin.Jobs.Commands.RequeueMissingPipelineMessages;
using LegalAiAr.Application.Admin.Jobs.Commands.ResumeDiscovery;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Application.Admin.Jobs.Commands.RecoverJobFromInfra;

public record RecoverJobFromInfraCommand(
    Guid JobId,
    bool RequireStorageProbe = true,
    bool ClearInfrastructureDegraded = true,
    bool BroadcastRecovered = true,
    bool ResumeDiscovery = false,
    bool RequeueFetcherPending = true,
    bool RequeueAllPipelineStages = false) : IRequest<RecoverJobFromInfraResultDto>;

public record RecoverJobFromInfraResultDto(
    bool StorageProbeOk,
    string? StorageProbeError,
    bool ClearedDegraded,
    bool DiscoveryQueued,
    int RequeueMessagesPublished);

public sealed class RecoverJobFromInfraHandler : IRequestHandler<RecoverJobFromInfraCommand, RecoverJobFromInfraResultDto>
{
    private readonly IIngestionJobRepository _jobs;
    private readonly IQueueMetricsService _queueMetrics;
    private readonly PipelineQueueNames _queueNames;
    private readonly IInfraRecoveryEvents _infraRecoveryEvents;
    private readonly IMediator _mediator;

    public RecoverJobFromInfraHandler(
        IIngestionJobRepository jobs,
        IQueueMetricsService queueMetrics,
        PipelineQueueNames queueNames,
        IInfraRecoveryEvents infraRecoveryEvents,
        IMediator mediator)
    {
        _jobs = jobs;
        _queueMetrics = queueMetrics;
        _queueNames = queueNames;
        _infraRecoveryEvents = infraRecoveryEvents;
        _mediator = mediator;
    }

    public async Task<RecoverJobFromInfraResultDto> Handle(
        RecoverJobFromInfraCommand request,
        CancellationToken cancellationToken)
    {
        _ = await _jobs.GetByIdAsync(request.JobId, cancellationToken)
            ?? throw new NotFoundException($"IngestionJob {request.JobId} not found.");

        var storageOk = true;
        string? storageError = null;
        if (request.RequireStorageProbe)
        {
            var d = await _queueMetrics.TryProbeQueueAsync(_queueNames.Discoverer, cancellationToken);
            var f = await _queueMetrics.TryProbeQueueAsync(_queueNames.Fetcher, cancellationToken);
            storageOk = d.Ok && f.Ok;
            storageError = !d.Ok ? d.Error : !f.Ok ? f.Error : null;
            if (!storageOk)
            {
                return new RecoverJobFromInfraResultDto(
                    StorageProbeOk: false,
                    StorageProbeError: storageError,
                    ClearedDegraded: false,
                    DiscoveryQueued: false,
                    RequeueMessagesPublished: 0);
            }
        }

        var cleared = false;
        if (request.ClearInfrastructureDegraded)
        {
            await _jobs.SetInfrastructureDegradedAsync(
                request.JobId,
                degraded: false,
                degradedSinceUtc: null,
                reason: null,
                cancellationToken);
            cleared = true;
        }

        if (request.BroadcastRecovered)
            await _infraRecoveryEvents.BroadcastInfraRecoveredAsync(request.JobId, null, cancellationToken);

        var requeuePublished = 0;
        if (request.RequeueFetcherPending)
        {
            if (request.RequeueAllPipelineStages)
            {
                var all = await _mediator.Send(
                    new RequeueMissingPipelineMessagesCommand(request.JobId, Stage: null),
                    cancellationToken);
                requeuePublished = all.TotalPublished;
            }
            else
            {
                var rq = await _mediator.Send(new RequeueFetcherPendingCommand(request.JobId), cancellationToken);
                requeuePublished = rq.MessagesPublished;
            }
        }

        var discoveryQueued = false;
        if (request.ResumeDiscovery)
        {
            await _mediator.Send(new ResumeDiscoveryCommand(request.JobId), cancellationToken);
            discoveryQueued = true;
        }

        return new RecoverJobFromInfraResultDto(
            StorageProbeOk: storageOk,
            StorageProbeError: storageError,
            ClearedDegraded: cleared,
            DiscoveryQueued: discoveryQueued,
            RequeueMessagesPublished: requeuePublished);
    }
}
