using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Hybrid search (vector + fulltext) over Azure AI Search.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Performs hybrid semantic search over rulings (vector + keyword).
    /// When <paramref name="queryEmbedding"/> is null, performs a filter-only search sorted by date descending.
    /// </summary>
    /// <param name="queryEmbedding">Embedding of the search query (3072 dims), or null for filter-only search.</param>
    /// <param name="searchText">Original query text for keyword/fulltext search. When provided, enables hybrid search.</param>
    Task<PagedSearchResult> SearchAsync(
        float[]? queryEmbedding,
        string? searchText,
        SearchFilters? filters,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds rulings semantically similar to the given ruling.
    /// </summary>
    Task<IReadOnlyList<SearchResultItem>> SearchRelatedAsync(
        Guid rulingId,
        int topK,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Hybrid search over rulings-by-chunk index for RAG context. Returns top-K chunks by similarity.
    /// Combines vector search with optional BM25 text search on contextualized chunks.
    /// </summary>
    Task<IReadOnlyList<ChatChunkResult>> SearchChunksAsync(
        float[] queryEmbedding,
        int topK,
        string? searchText = null,
        Guid? rulingId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Vector search over rulings-by-ruling index for RAG context. Returns top-K rulings by similarity.
    /// </summary>
    Task<IReadOnlyList<ChatRulingResult>> SearchRulingsForRagAsync(
        float[] queryEmbedding,
        int topK,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns facet values for filterable fields (jurisdictionArea, instance, court, subjectArea, resourceType).
    /// </summary>
    Task<SearchFacets> GetFacetsAsync(CancellationToken cancellationToken = default);
}
