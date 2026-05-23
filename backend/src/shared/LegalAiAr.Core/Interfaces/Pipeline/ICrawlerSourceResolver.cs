namespace LegalAiAr.Core.Interfaces.Pipeline;

/// <summary>
/// Resolves the appropriate crawler source for a given source ID.
/// </summary>
public interface ICrawlerSourceResolver
{
    /// <summary>
    /// Gets the crawler source for the given source ID and optional crawl type.
    /// When type is "fallos-destacados", returns the curated-collection source for CSJN.
    /// </summary>
    /// <param name="sourceId">Source ID (CSJN=1, SAIJ=2, PJN=3, SCBA=4).</param>
    /// <param name="type">Optional crawl type that may influence which source is returned.</param>
    /// <returns>The crawler source, or null if the source is not implemented.</returns>
    ICrawlerSource? GetSource(int sourceId, string? type = null);
}
