using LegalAiAr.Application.Admin.Jobs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Application.Admin.Jobs.Commands.RewindParserFailedToFetcher;

public sealed class RewindParserFailedToFetcherHandler
    : IRequestHandler<RewindParserFailedToFetcherCommand, RewindParserFailedToFetcherResultDto>
{
    private readonly IIngestionJobRepository _jobs;
    private readonly IDocumentRepository _documents;
    private readonly IQueuePublisher _publisher;
    private readonly PipelineQueueNames _queueNames;

    public RewindParserFailedToFetcherHandler(
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

    public async Task<RewindParserFailedToFetcherResultDto> Handle(
        RewindParserFailedToFetcherCommand request,
        CancellationToken cancellationToken)
    {
        _ = await _jobs.GetByIdAsync(request.JobId, cancellationToken)
            ?? throw new NotFoundException($"IngestionJob {request.JobId} not found.");

        if (request.MaxDocuments is < 1 or > 20_000)
            throw new DomainException("MaxDocuments must be between 1 and 20000.");

        if (!request.OnlyCsjnCacheMiss
            && string.IsNullOrWhiteSpace(request.ErrorMessageContains)
            && !request.SourceId.HasValue)
        {
            throw new DomainException(
                "When OnlyCsjnCacheMiss is false, set ErrorMessageContains and/or SourceId so the rewind set is bounded.");
        }

        var docs = await _documents.TakeParserFailedRewindToFetcherPendingAsync(
            request.JobId,
            request.OnlyCsjnCacheMiss,
            request.ErrorMessageContains,
            request.SourceId,
            request.MaxDocuments,
            cancellationToken);

        if (docs.Count == 0)
        {
            return new RewindParserFailedToFetcherResultDto(
                0,
                0,
                "No matching Parser/Failed documents for this job and filters.");
        }

        await _jobs.DecrementDocumentsFailedAsync(request.JobId, docs.Count, cancellationToken);
        await _jobs.ResumeProcessingIfTerminalAsync(request.JobId, cancellationToken);

        var published = 0;
        foreach (var doc in docs)
        {
            if (await DocumentStageQueuePublisher.TryPublishForStageAsync(
                    doc,
                    PipelineStage.Fetcher,
                    _publisher,
                    _queueNames,
                    cancellationToken))
                published++;
        }

        var msg =
            $"Rewound {docs.Count} document(s) to Fetcher/Pending and published {published} Fetcher message(s).";
        return new RewindParserFailedToFetcherResultDto(docs.Count, published, msg);
    }
}
