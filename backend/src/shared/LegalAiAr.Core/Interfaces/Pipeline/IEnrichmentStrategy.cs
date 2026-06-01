using LegalAiAr.Core.Messages;

namespace LegalAiAr.Core.Interfaces.Pipeline;

/// <summary>
/// Contract for enrichment strategies (CSJN gap-filling, SAIJ/PJN/SCBA full enrichment).
/// Each strategy receives EnrichmentMessage and returns IndexerMessage ready for queue-indexer.
/// </summary>
public interface IEnrichmentStrategy
{
    /// <summary>
    /// Enriches the message by extracting missing fields via GPT-4o (or other means),
    /// merges with extracted metadata, produces chunks, and builds the complete IndexerMessage.
    /// </summary>
    /// <param name="message">Incoming message from queue-enrichment with extracted metadata and missing fields.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Complete IndexerMessage ready for queue-indexer.</returns>
    Task<IndexerMessage> EnrichAsync(
        EnrichmentMessage message,
        CancellationToken cancellationToken = default);
}
