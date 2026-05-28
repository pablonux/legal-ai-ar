using LegalAiAr.Core.Messages;

namespace LegalAiAr.Core.Interfaces.Pipeline.Strategies;

/// <summary>
/// Generates embeddings, indexes the entity in Azure AI Search, extracts chunk mentions,
/// and resolves cross-entity citations.
/// </summary>
public interface IIndexStrategy
{
    /// <summary>
    /// Indexes the persisted entity: generates embeddings, uploads to search index,
    /// extracts chunk-level mentions, and resolves citations.
    /// </summary>
    Task IndexAsync(
        IndexerMessage message,
        CancellationToken cancellationToken = default);
}
