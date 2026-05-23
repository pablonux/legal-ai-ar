using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Pipeline.Strategies;

/// <summary>
/// Extracts structured metadata and normalized text from fetched content.
/// Produces an EnrichmentMessage for the next stage.
/// </summary>
public interface IParseStrategy
{
    /// <summary>
    /// Parses the fetched document (PDF text extraction, API metadata, HTML scraping)
    /// and returns the result to be forwarded to the enrichment stage.
    /// </summary>
    Task<ParseResult> ParseAsync(
        ParserMessage message,
        CancellationToken cancellationToken = default);
}
