namespace LegalAiAr.Infrastructure.Search;

/// <summary>
/// Configuration options for Azure AI Search.
/// </summary>
public class AzureSearchOptions
{
    public const string SectionName = "AzureSearch";

    /// <summary>
    /// Azure AI Search endpoint (e.g. https://legal-ai-search.search.windows.net).
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// API key for authentication.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Ruling-level index name.
    /// </summary>
    public string RulingIndexName { get; set; } = "rulings-by-ruling";

    /// <summary>
    /// Chunk-level index name.
    /// </summary>
    public string ChunkIndexName { get; set; } = "rulings-by-chunk";

    /// <summary>
    /// Statute-level index name for legislation search.
    /// </summary>
    public string StatuteIndexName { get; set; } = "statutes";

    /// <summary>
    /// Minimum relevance score (0.0–1.0) for hybrid search results.
    /// Results below this threshold are discarded. Set to 0 to disable filtering.
    /// </summary>
    public double MinRelevanceScore { get; set; } = 0.02;
}
