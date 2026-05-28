using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Pipeline;

/// <summary>
/// LLM-based query enricher for ambiguous queries where rule-based
/// extraction returned insufficient confidence.
/// </summary>
public interface IQueryEnricher
{
    Task<QueryEnrichment?> EnrichAsync(
        string query, CancellationToken cancellationToken = default);
}
