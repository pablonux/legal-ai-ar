using System.Text.Json;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;
namespace LegalAiAr.Application.Admin.Pipeline.Commands.RequeueDocument;

/// <summary>
/// Handles RequeueDocumentCommand. Publishes to parser, enrichment, or indexer queue.
/// </summary>
public class RequeueDocumentHandler : IRequestHandler<RequeueDocumentCommand, RequeueDocumentResult>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly IRulingRepository _rulings;
    private readonly IQueuePublisher _publisher;
    private readonly ITextChunkingService _chunking;
    private readonly PipelineQueueNames _queueNames;

    public RequeueDocumentHandler(
        IRulingRepository rulings,
        IQueuePublisher publisher,
        ITextChunkingService chunking,
        PipelineQueueNames queueNames)
    {
        _rulings = rulings;
        _publisher = publisher;
        _chunking = chunking;
        _queueNames = queueNames;
    }

    public async Task<RequeueDocumentResult> Handle(RequeueDocumentCommand request, CancellationToken cancellationToken)
    {
        var stage = request.Stage.ToLowerInvariant();

        if (request.Message.HasValue)
        {
            return await PublishFromMessageAsync(stage, request.Message!.Value, cancellationToken);
        }

        if (request.RulingId.HasValue)
        {
            return await PublishFromRulingAsync(stage, request.RulingId.Value, cancellationToken);
        }

        return new RequeueDocumentResult(false, stage);
    }

    private async Task<RequeueDocumentResult> PublishFromMessageAsync(string stage, JsonElement json, CancellationToken cancellationToken)
    {
        switch (stage)
        {
            case "parser":
                var parserMsg = JsonSerializer.Deserialize<ParserMessage>(json.GetRawText(), JsonOptions);
                if (parserMsg is null) throw new InvalidOperationException("Invalid ParserMessage payload.");
                var parserQueue = _queueNames.Parser;
                await _publisher.PublishAsync(parserQueue, parserMsg, cancellationToken);
                break;
            case "enrichment":
                var enrichMsg = JsonSerializer.Deserialize<EnrichmentMessage>(json.GetRawText(), JsonOptions);
                if (enrichMsg is null) throw new InvalidOperationException("Invalid EnrichmentMessage payload.");
                var enrichQueue = _queueNames.Enricher;
                await _publisher.PublishAsync(enrichQueue, enrichMsg, cancellationToken);
                break;
            case "indexer":
                var indexMsg = JsonSerializer.Deserialize<IndexerMessage>(json.GetRawText(), JsonOptions);
                if (indexMsg is null) throw new InvalidOperationException("Invalid IndexerMessage payload.");
                var indexQueue = _queueNames.Indexer;
                await _publisher.PublishAsync(indexQueue, indexMsg, cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"Unknown stage: {stage}");
        }
        return new RequeueDocumentResult(true, stage);
    }

    private async Task<RequeueDocumentResult> PublishFromRulingAsync(string stage, Guid rulingId, CancellationToken cancellationToken)
    {
        var ruling = await _rulings.GetByIdAsync(rulingId, cancellationToken);
        if (ruling is null)
            throw new NotFoundException($"Ruling with id '{rulingId}' was not found.");

        switch (stage)
        {
            case "parser":
                await PublishParserFromRulingAsync(ruling, cancellationToken);
                break;
            case "enrichment":
                await PublishEnrichmentFromRulingAsync(ruling, cancellationToken);
                break;
            case "indexer":
                await PublishIndexerFromRulingAsync(ruling, cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"Unknown stage: {stage}");
        }
        return new RequeueDocumentResult(true, stage);
    }

    private async Task PublishParserFromRulingAsync(Ruling ruling, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(ruling.BlobPath))
            throw new InvalidOperationException("Cannot reconstruct ParserMessage: Ruling has no BlobPath.");

        var msg = new ParserMessage(
            ruling.SourceId,
            ruling.ExternalId,
            ruling.AnalysisId,
            ruling.BlobPath,
            ruling.ContentHash,
            null,
            IngestionJobId: null);
        await _publisher.PublishAsync(_queueNames.Parser, msg, cancellationToken);
    }

    private async Task PublishEnrichmentFromRulingAsync(Ruling ruling, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(ruling.FullText))
            throw new InvalidOperationException("Cannot reconstruct EnrichmentMessage: Ruling has no FullText.");

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
            ruling.RulingKeywords.OrderBy(rk => rk.SortOrder).Select(rk => new ExtractedKeywordDto(rk.Keyword.ExternalCode, rk.Keyword.Description)).ToList(),
            ruling.OutboundCitations.Select(c => new ExtractedCitationDto(c.ExternalAlias, c.CsjnSummaryId)).ToList());

        var missingFields = new List<string> { "judges", "cited_statutes", "citation_types" };
        var msg = new EnrichmentMessage(
            ruling.ExternalId,
            ruling.SourceId,
            ruling.FullText,
            metadata,
            missingFields,
            ruling.BlobPath,
            ruling.ContentHash,
            IngestionJobId: null);
        await _publisher.PublishAsync(_queueNames.Enricher, msg, cancellationToken);
    }

    private async Task PublishIndexerFromRulingAsync(Ruling ruling, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(ruling.FullText))
            throw new InvalidOperationException("Cannot reconstruct IndexerMessage: Ruling has no FullText.");
        if (string.IsNullOrEmpty(ruling.BlobPath))
            throw new InvalidOperationException("Cannot reconstruct IndexerMessage: Ruling has no BlobPath.");

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
        await _publisher.PublishAsync(_queueNames.Indexer, msg, cancellationToken);
    }
}
