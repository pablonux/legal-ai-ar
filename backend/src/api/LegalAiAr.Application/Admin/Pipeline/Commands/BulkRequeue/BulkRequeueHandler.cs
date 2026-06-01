using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;
using Microsoft.Extensions.Logging;
namespace LegalAiAr.Application.Admin.Pipeline.Commands.BulkRequeue;

public class BulkRequeueHandler : IRequestHandler<BulkRequeueCommand, BulkRequeueResult>
{
    private readonly IRulingRepository _rulings;
    private readonly IQueuePublisher _publisher;
    private readonly ITextChunkingService _chunking;
    private readonly ILogger<BulkRequeueHandler> _logger;
    private readonly PipelineQueueNames _queueNames;

    public BulkRequeueHandler(
        IRulingRepository rulings,
        IQueuePublisher publisher,
        ITextChunkingService chunking,
        ILogger<BulkRequeueHandler> logger,
        PipelineQueueNames queueNames)
    {
        _rulings = rulings;
        _publisher = publisher;
        _chunking = chunking;
        _logger = logger;
        _queueNames = queueNames;
    }

    public async Task<BulkRequeueResult> Handle(BulkRequeueCommand request, CancellationToken cancellationToken)
    {
        var stage = request.Stage.ToLowerInvariant();
        int queued = 0, skipped = 0, failed = 0;
        var lastId = Guid.Empty;

        while (!cancellationToken.IsCancellationRequested)
        {
            var batch = await _rulings.GetPageForRequeueAsync(
                lastId, request.BatchSize, request.OnlyMissingOntology, request.SourceId, cancellationToken);

            if (batch.Count == 0)
                break;

            foreach (var ruling in batch)
            {
                lastId = ruling.Id;
                try
                {
                    bool published = stage switch
                    {
                        "enrichment" => await TryPublishEnrichment(ruling, cancellationToken),
                        "indexer" => await TryPublishIndexer(ruling, cancellationToken),
                        _ => false
                    };

                    if (published)
                        queued++;
                    else
                        skipped++;
                }
                catch (Exception ex)
                {
                    failed++;
                    _logger.LogWarning(ex, "Failed to requeue ruling {RulingId} to {Stage}", ruling.Id, stage);
                }
            }

            _logger.LogInformation(
                "Bulk requeue progress: {Queued} queued, {Skipped} skipped, {Failed} failed (last batch {BatchCount})",
                queued, skipped, failed, batch.Count);
        }

        return new BulkRequeueResult(queued, skipped, failed, stage);
    }

    private async Task<bool> TryPublishEnrichment(Ruling ruling, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(ruling.FullText))
            return false;

        var metadata = new ExtractedMetadata(
            ruling.CaseTitle,
            ruling.RulingDate,
            ruling.CaseNumber,
            ruling.Court?.Name,
            ruling.JurisdictionArea,
            ruling.Instance,
            ruling.RulingDirection,
            ruling.Summary,
            ruling.Holding,
            ruling.RulingKeywords.OrderBy(rk => rk.SortOrder)
                .Select(rk => new ExtractedKeywordDto(rk.Keyword.ExternalCode, rk.Keyword.Description)).ToList(),
            ruling.OutboundCitations
                .Select(c => new ExtractedCitationDto(c.ExternalAlias, c.CsjnSummaryId)).ToList());

        var msg = new EnrichmentMessage(
            ruling.ExternalId,
            ruling.SourceId,
            ruling.FullText,
            metadata,
            new List<string> { "judges", "cited_statutes", "citation_types" },
            ruling.BlobPath,
            ruling.ContentHash,
            IngestionJobId: null);

        var queueName = _queueNames.Enricher;
        await _publisher.PublishAsync(queueName, msg, ct);
        return true;
    }

    private async Task<bool> TryPublishIndexer(Ruling ruling, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(ruling.FullText) || string.IsNullOrEmpty(ruling.BlobPath))
            return false;

        var rulingData = new RulingData(
            ruling.CaseTitle,
            ruling.RulingDate,
            ruling.CaseNumber,
            ruling.JurisdictionArea,
            ruling.Instance,
            ruling.Jurisdiction,
            ruling.ResourceType,
            ruling.RulingDirection,
            ruling.SubjectArea,
            ruling.IsUnconstitutional,
            ruling.Summary,
            ruling.Holding,
            ruling.FullText,
            ruling.BlobPath,
            Court: ruling.Court?.Name,
            LegalBranch: ruling.LegalBranch?.ToString(),
            PrecedentWeight: ruling.PrecedentWeight?.ToString(),
            IsPlenario: ruling.IsPlenario,
            IsLeadingCase: ruling.IsLeadingCase,
            ActionType: ruling.ActionType,
            InternalSubject: ruling.InternalSubject,
            OfficialReference: ruling.OfficialReference,
            Observations: ruling.Observations,
            FederalQuestion: ruling.FederalQuestion,
            ProceduralFormula: ruling.ProceduralFormula,
            HasDictamen: ruling.HasDictamen);

        var persons = ruling.RulingParticipations
            .Select(rp => new PersonData(rp.Person.FirstName ?? "", rp.Person.LastName ?? "", rp.Role.ToString(), rp.Person.CsjnMinistroId))
            .ToList();

        var keywords = ruling.RulingKeywords
            .OrderBy(rk => rk.SortOrder)
            .Select(rk => new KeywordData(rk.Keyword.ExternalCode, rk.Keyword.Description, rk.SortOrder))
            .ToList();

        var statutes = ruling.RulingStatutes
            .Select(rs => new StatuteData(rs.Statute.Number, rs.Statute.Name, rs.Articles))
            .ToList();

        var citations = ruling.OutboundCitations
            .Select(c => new CitationData(c.ExternalAlias, c.CsjnSummaryId, c.CitationType.ToString(), c.CsjnFalloId, c.CitationText))
            .ToList();

        var chunks = _chunking.Chunk(ruling.FullText);

        var msg = new IndexerMessage(
            ruling.ExternalId,
            ruling.ContentHash,
            ruling.SourceId,
            rulingData,
            persons,
            keywords,
            statutes,
            citations,
            chunks,
            IngestionJobId: null);

        var queueName = _queueNames.Indexer;
        await _publisher.PublishAsync(queueName, msg, ct);
        return true;
    }
}
