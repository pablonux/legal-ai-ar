namespace LegalAiAr.Infrastructure.Blob;

/// <summary>
/// Builds blob paths for the external download cache.
/// All cache entries live under _cache/ to separate them from pipeline data (legal-ai-ar-kb/).
/// Keys are immutable by sourceId + documentId/analysisId, independent of the PDF's final blob path.
/// </summary>
public static class CachePathHelper
{
    private const string Root = "_cache";

    /// <summary>
    /// Cache key for a downloaded PDF: _cache/{source}/pdf/{documentId}.pdf
    /// </summary>
    public static string PdfCacheKey(int sourceId, string documentId)
    {
        var source = ResolveSourceName(sourceId);
        return $"{Root}/{source}/pdf/{documentId}.pdf";
    }

    /// <summary>
    /// Cache key for a CSJN API JSON response: _cache/{source}/api/{endpoint}/{id}.json
    /// </summary>
    /// <param name="sourceId">Source identifier (1=csjn, etc.).</param>
    /// <param name="endpoint">API endpoint name (e.g. "abrirAnalisis", "getCitas").</param>
    /// <param name="id">The ID parameter used in the call (analysisId or documentId depending on endpoint).</param>
    public static string ApiCacheKey(int sourceId, string endpoint, string id)
    {
        var source = ResolveSourceName(sourceId);
        return $"{Root}/{source}/api/{endpoint}/{id}.json";
    }

    private static string ResolveSourceName(int sourceId) => sourceId switch
    {
        1 => "csjn",
        2 => "saij",
        3 => "pjn",
        4 => "scba",
        _ => "unknown"
    };
}
