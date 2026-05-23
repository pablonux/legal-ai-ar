namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Generates LLM-based summaries for GraphRAG communities.
/// </summary>
public interface ICommunitySummaryService
{
    /// <summary>
    /// Generates Summary, KeyFindings, and a refined Title for all
    /// communities that have empty summaries.
    /// Returns the number of communities summarized.
    /// </summary>
    Task<int> GenerateSummariesAsync(CancellationToken cancellationToken = default);
}
