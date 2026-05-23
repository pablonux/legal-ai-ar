using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Application.Admin.Jobs.Commands.ResumeDiscovery;

public record ResumeDiscoveryCommand(Guid JobId) : IRequest<ResumeDiscoveryResultDto>;

public record ResumeDiscoveryResultDto(int SkipDocumentsQueued);

public sealed class ResumeDiscoveryHandler : IRequestHandler<ResumeDiscoveryCommand, ResumeDiscoveryResultDto>
{
    private static readonly HashSet<string> ResumableStatuses =
    [
        "running", "pending", "partial", "processing", "discovered",
    ];

    private readonly IIngestionJobRepository _jobs;
    private readonly IDocumentRepository _documents;
    private readonly IQueuePublisher _publisher;
    private readonly PipelineQueueNames _queueNames;

    public ResumeDiscoveryHandler(
        IIngestionJobRepository jobs,
        IDocumentRepository documents,
        IQueuePublisher publisher,
        PipelineQueueNames queueNames)
    {
        _jobs = jobs;
        _documents = documents;
        _publisher = publisher;
        _queueNames = queueNames;
    }

    public async Task<ResumeDiscoveryResultDto> Handle(ResumeDiscoveryCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobs.GetByIdAsync(request.JobId, cancellationToken)
            ?? throw new NotFoundException($"IngestionJob {request.JobId} not found.");

        if (!ResumableStatuses.Contains(job.Status))
        {
            throw new DomainException(
                $"El job no admite reanudar discovery con estado actual '{job.Status}'.");
        }

        await _jobs.ResumeProcessingIfTerminalAsync(request.JobId, cancellationToken);

        var docCount = await _documents.CountByJobAsync(request.JobId, null, null, cancellationToken);
        var skip = Math.Max(job.DocumentsDiscovered, docCount);

        var message = new DiscovererMessage(
            EntityType: job.EntityType,
            SourceId: job.SourceId,
            Type: job.Type,
            Since: null,
            DateFrom: job.DateFrom,
            DateTo: job.DateTo,
            IngestionJobId: job.Id,
            UseCache: false,
            Reprocess: false,
            MaxDocuments: null,
            SkipDocuments: skip,
            PreserveProgressCounters: true);

        await _publisher.PublishAsync(_queueNames.Discoverer, message, cancellationToken);

        return new ResumeDiscoveryResultDto(skip);
    }
}
