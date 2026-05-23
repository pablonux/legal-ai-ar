using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Jobs.Commands.RetryJob;

/// <summary>
/// Looks up a partial/failed IngestionJob and re-publishes its CrawlerMessage.
/// The new crawl will skip already-indexed documents thanks to ExistsByExternalId.
/// </summary>
public class RetryJobHandler : IRequestHandler<RetryJobCommand, RetryJobResult>
{
    private readonly IIngestionJobRepository _jobs;
    private readonly IQueuePublisher _publisher;
    private readonly PipelineQueueNames _queueNames;

    public RetryJobHandler(IIngestionJobRepository jobs, IQueuePublisher publisher, PipelineQueueNames queueNames)
    {
        _jobs = jobs;
        _publisher = publisher;
        _queueNames = queueNames;
    }

    public async Task<RetryJobResult> Handle(RetryJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobs.GetByIdAsync(request.JobId, cancellationToken)
            ?? throw new NotFoundException($"IngestionJob {request.JobId} not found.");

        if (job.EntityType == EntityType.Thesaurus)
        {
            throw new DomainException(
                "Los jobs de tesauro no se reintentan por este flujo. Iniciá una nueva ingesta de tesauro desde el panel.");
        }

        if (job.Status is not ("partial" or "failed" or "pending"))
            throw new DomainException($"Only partial, failed or pending jobs can be retried. Current status: {job.Status}.");

        var message = new DiscovererMessage(
            EntityType: job.EntityType,
            SourceId: job.SourceId,
            Type: job.Type,
            Since: null,
            DateFrom: job.DateFrom,
            DateTo: job.DateTo,
            IngestionJobId: job.Id);

        await _publisher.PublishAsync(_queueNames.Discoverer, message, cancellationToken);

        return new RetryJobResult(
            Success: true,
            Message: $"Retry queued for job {request.JobId} (source {job.Source?.Name ?? job.SourceId.ToString()}, type {job.Type}).");
    }
}
