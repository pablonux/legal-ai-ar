using LegalAiAr.Core.Messages;

namespace LegalAiAr.Core.Models;

/// <summary>
/// Result of parsing: the EnrichmentMessage to forward to the enrichment stage.
/// </summary>
/// <param name="EnrichmentMessage">Complete message for the enrichment queue.</param>
public record ParseResult(EnrichmentMessage EnrichmentMessage);
