using LegalAiAr.Application.Admin.RulingReprocess.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Application.Admin.RulingReprocess.Commands.EnqueueRulingReprocess;

public sealed class EnqueueRulingReprocessHandler : IRequestHandler<EnqueueRulingReprocessCommand, EnqueueRulingReprocessResult>
{
    private readonly IRulingRepository _rulings;
    private readonly IDocumentRepository _documents;
    private readonly IRulingReprocessRequestRepository _requests;
    private readonly ISearchIndexService _searchIndex;
    private readonly IQueuePublisher _publisher;
    private readonly PipelineQueueNames _queueNames;

    public EnqueueRulingReprocessHandler(
        IRulingRepository rulings,
        IDocumentRepository documents,
        IRulingReprocessRequestRepository requests,
        ISearchIndexService searchIndex,
        IQueuePublisher publisher,
        PipelineQueueNames queueNames)
    {
        _rulings = rulings;
        _documents = documents;
        _requests = requests;
        _searchIndex = searchIndex;
        _publisher = publisher;
        _queueNames = queueNames;
    }

    public async Task<EnqueueRulingReprocessResult> Handle(
        EnqueueRulingReprocessCommand request,
        CancellationToken cancellationToken)
    {
        var ruling = await _rulings.GetByIdAsync(request.RulingId, cancellationToken)
            ?? throw new NotFoundException("Ruling not found.");

        if (ruling.Status is RulingStatus.Reprocessing)
            throw new DomainException("This ruling is already being reprocessed.");

        var active = await _requests.GetActiveByRulingIdAsync(request.RulingId, cancellationToken);
        if (active is not null)
            throw new DomainException("An active reprocess request already exists for this ruling.");

        var document = await _documents.GetLatestByRulingIdAsync(request.RulingId, cancellationToken)
            ?? await _documents.GetByExternalIdAsync(ruling.SourceId, ruling.ExternalId, cancellationToken)
            ?? throw new DomainException(
                "No pipeline document found for this ruling. Run ingestion before reprocessing.");

        if (string.IsNullOrWhiteSpace(document.AnalysisId) && ruling.SourceId == 1)
            throw new DomainException("Document is missing AnalysisId required for CSJN reprocess.");

        var reprocessRequest = new RulingReprocessRequest
        {
            Id = Guid.NewGuid(),
            RulingId = ruling.Id,
            DocumentId = document.Id,
            Status = RulingReprocessRequestStatus.Queued,
            UseCache = request.UseCache,
            RequestedBy = request.RequestedBy,
            RequestedAt = DateTime.UtcNow
        };

        await _searchIndex.DeleteRulingAsync(ruling.Id, cancellationToken);
        await _rulings.UpdateStatusAsync(ruling.Id, RulingStatus.Reprocessing, cancellationToken);

        await _documents.ResetForReprocessAsync(document.Id, cancellationToken);

        await _requests.AddAsync(reprocessRequest, cancellationToken);

        reprocessRequest.Status = RulingReprocessRequestStatus.Running;
        reprocessRequest.StartedAt = DateTime.UtcNow;
        await _requests.SaveChangesAsync(cancellationToken);

        var fetcherMessage = new FetcherMessage(
            DocumentId: document.Id,
            EntityType: document.EntityType,
            SourceId: document.SourceId,
            ExternalId: document.ExternalId,
            AnalysisId: document.AnalysisId ?? ruling.AnalysisId,
            IngestionJobId: document.IngestionJobId,
            UseCache: request.UseCache,
            Reprocess: true);

        await _publisher.PublishAsync(_queueNames.Fetcher, fetcherMessage, cancellationToken);

        return new EnqueueRulingReprocessResult(
            reprocessRequest.Id,
            "Ruling queued for full reprocess (Fetcher → Indexer). It is hidden from search until complete.");
    }
}
