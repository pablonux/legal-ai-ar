using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Indexes rulings and chunks in Azure AI Search.
/// Used by IndexerWorker for MergeOrUpload operations.
/// </summary>
public interface ISearchIndexService
{
    /// <summary>
    /// Indexes or updates a ruling document in rulings-by-ruling.
    /// </summary>
    Task IndexRulingAsync(RulingIndexInput input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indexes or updates chunk documents in rulings-by-chunk.
    /// </summary>
    Task IndexChunksAsync(IReadOnlyList<ChunkIndexInput> inputs, CancellationToken cancellationToken = default);

    /// <summary>
    /// Merges only ontology fields into an existing search document (no embedding required).
    /// </summary>
    Task MergeOntologyFieldsAsync(
        Guid rulingId,
        string? legalBranch, string? precedentWeight, bool isPlenario, bool isLeadingCase,
        string? courtType, string? fuero, int? instanceLevel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Indexes or updates a statute document in the statutes index.
    /// </summary>
    Task IndexStatuteAsync(StatuteIndexInput input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a ruling and its chunks from Azure AI Search (admin reprocess).
    /// </summary>
    Task DeleteRulingAsync(Guid rulingId, CancellationToken cancellationToken = default);
}
