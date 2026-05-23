namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Preprocesses a raw user search query using an LLM to produce optimized
/// keyword and semantic query variants for hybrid search.
/// </summary>
public interface ISearchQueryPreprocessor
{
    /// <summary>
    /// Expands and reformulates the raw query into keyword-optimized and semantic-optimized variants.
    /// Returns null if preprocessing fails (caller should fall back to the original query).
    /// </summary>
    /// <param name="rawQuery">Original user search text.</param>
    /// <param name="thesaurusContext">Optional SAIJ thesaurus context block injected into the LLM prompt.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PreprocessedQuery?> PreprocessAsync(string rawQuery, string? thesaurusContext = null, CancellationToken cancellationToken = default);
}

/// <param name="KeywordQuery">Expanded query for BM25 keyword search (includes synonyms, legal terms).</param>
/// <param name="SemanticQuery">Enriched query for embedding generation (more descriptive, contextual).</param>
public record PreprocessedQuery(string KeywordQuery, string SemanticQuery);
