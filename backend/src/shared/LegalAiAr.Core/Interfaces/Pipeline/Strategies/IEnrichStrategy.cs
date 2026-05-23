using LegalAiAr.Core.Messages;

namespace LegalAiAr.Core.Interfaces.Pipeline.Strategies;

/// <summary>
/// Enriches parsed metadata using AI (GPT-5 family) or other extraction methods.
/// Fills gaps in structured data (judges, statutes, citation types, etc.).
/// Produces a PersisterMessage for the next stage.
/// </summary>
public interface IEnrichStrategy
{
    /// <summary>
    /// Enriches the document by extracting missing fields and building the complete
    /// entity payload ready for persistence.
    /// </summary>
    Task<PersisterMessage> EnrichAsync(
        EnrichmentMessage message,
        CancellationToken cancellationToken = default);
}
