using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Indexer.Steps;

/// <summary>
/// Indexes ruling and chunks in Azure AI Search (rulings-by-ruling, rulings-by-chunk).
/// MergeOrUpload for idempotency. Per E056.
/// </summary>
public class IndexSearchStep
{
    private readonly ISearchIndexService _searchIndex;
    private readonly ILogger<IndexSearchStep> _logger;

    public IndexSearchStep(
        ISearchIndexService searchIndex,
        ILogger<IndexSearchStep> logger)
    {
        _searchIndex = searchIndex;
        _logger = logger;
    }

    /// <summary>
    /// Indexes ruling and chunks in Azure AI Search.
    /// </summary>
    /// <param name="courtName">Resolved court name from PersistRulingStep (actual name, not Instance).</param>
    /// <param name="courtMetadata">Optional court ontology metadata (type, fuero, instanceLevel).</param>
    public async Task ExecuteAsync(
        Guid rulingId,
        IndexerMessage message,
        EmbeddingsResult embeddingsResult,
        string? courtName = null,
        CourtMetadata? courtMetadata = null,
        CancellationToken cancellationToken = default)
    {
        var ruling = message.Ruling;
        var keywords = message.Keywords.Select(k => k.Description).ToList();
        var persons = message.Persons.Select(p => $"{p.LastName}, {p.FirstName}").ToList();
        var statutes = message.Statutes
            .Select(s => string.IsNullOrWhiteSpace(s.Articles) ? $"{s.Number} {s.Name}" : $"{s.Number} {s.Name} {s.Articles}")
            .ToList();

        var rulingInput = new RulingIndexInput(
            RulingId: rulingId,
            CaseTitle: ruling.CaseTitle,
            Summary: ruling.Summary,
            Holding: ruling.Holding,
            CaseNumber: ruling.CaseNumber,
            RulingDate: ruling.RulingDate,
            JurisdictionArea: ruling.JurisdictionArea,
            Instance: ruling.Instance,
            Court: courtName ?? ruling.Instance,
            Keywords: keywords,
            Persons: persons,
            Statutes: statutes,
            RulingDirection: ruling.RulingDirection,
            Embedding: embeddingsResult.RulingEmbedding,
            SubjectArea: ruling.SubjectArea,
            LegalBranch: ruling.LegalBranch,
            PrecedentWeight: ruling.PrecedentWeight,
            IsPlenario: ruling.IsPlenario,
            IsLeadingCase: ruling.IsLeadingCase,
            ResourceType: ruling.ResourceType,
            IsUnconstitutional: ruling.IsUnconstitutional,
            CourtType: courtMetadata?.CourtType,
            Fuero: courtMetadata?.Fuero,
            InstanceLevel: courtMetadata?.InstanceLevel,
            ActionType: ruling.ActionType,
            OfficialReference: ruling.OfficialReference,
            FederalQuestion: ruling.FederalQuestion,
            HasDictamen: ruling.HasDictamen);

        await _searchIndex.IndexRulingAsync(rulingInput, cancellationToken);

        var chunkInputs = embeddingsResult.ChunkEmbeddings
            .Select(c => new ChunkIndexInput(
                rulingId,
                c.Index,
                c.Text,
                c.ContextualizedText,
                c.Embedding))
            .ToList();

        if (chunkInputs.Count > 0)
        {
            await _searchIndex.IndexChunksAsync(chunkInputs, cancellationToken);
        }

        _logger.LogInformation(
            "Indexed ruling {RulingId} in Azure AI Search: ruling + {ChunkCount} chunks",
            rulingId,
            chunkInputs.Count);
    }
}
