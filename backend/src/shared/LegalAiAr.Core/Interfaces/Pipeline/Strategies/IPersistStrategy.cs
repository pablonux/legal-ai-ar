using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Pipeline.Strategies;

/// <summary>
/// Persists the enriched entity graph to the Knowledge Base (SQL).
/// Handles ruling/statute + all related entities (persons, keywords, statutes, citations, etc.)
/// within a single database transaction.
/// </summary>
public interface IPersistStrategy
{
    /// <summary>
    /// Persists the entity and all its relations to SQL.
    /// Returns the result containing the persisted entity ID for the Indexer.
    /// </summary>
    Task<PersistResult> PersistAsync(
        PersisterMessage message,
        CancellationToken cancellationToken = default);
}
