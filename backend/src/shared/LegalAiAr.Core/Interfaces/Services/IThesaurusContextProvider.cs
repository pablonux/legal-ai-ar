namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Provides thesaurus context for a search query to enrich LLM preprocessing.
/// </summary>
public interface IThesaurusContextProvider
{
    /// <summary>
    /// Looks up the SAIJ thesaurus for terms matching the query and returns
    /// a formatted context block with descriptors, synonyms, broader/narrower
    /// and related terms. Returns null if no relevant terms found.
    /// </summary>
    Task<string?> GetContextAsync(string query, CancellationToken cancellationToken = default);
}
