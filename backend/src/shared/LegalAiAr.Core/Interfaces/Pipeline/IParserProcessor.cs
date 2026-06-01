using LegalAiAr.Core.Messages;

namespace LegalAiAr.Core.Interfaces.Pipeline;

/// <summary>
/// Processes ParserMessage: fetches metadata, extracts PDF text, and publishes EnrichmentMessage.
/// Phase 1: CSJN only. Other sources in Phase 2.
/// </summary>
public interface IParserProcessor
{
    /// <summary>
    /// Processes a single ParserMessage: fetches API metadata (CSJN), extracts text from Blob,
    /// builds EnrichmentMessage and publishes to queue-enrichment.
    /// </summary>
    /// <param name="message">Incoming message from queue-parser.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ProcessAsync(ParserMessage message, CancellationToken cancellationToken = default);
}
