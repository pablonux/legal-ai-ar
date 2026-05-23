namespace LegalAiAr.Infrastructure.Blob;

/// <summary>
/// Builds blob storage paths for the legal-ai-ar-kb container.
/// Structure: legal-ai-ar-kb / {documentType} / {source} / {yyyy-MM} / {documentId}.pdf
/// Example: legal-ai-ar-kb/rulings/csjn/2024-04/8048522.pdf
/// </summary>
public static class BlobPathHelper
{
    private const string Root = "legal-ai-ar-kb";

    /// <summary>
    /// Builds the PDF blob path: legal-ai-ar-kb/{documentType}/{source}/{yyyy-MM}/{documentId}.pdf
    /// </summary>
    /// <param name="documentType">ruling, dictamen, resolution, etc.</param>
    /// <param name="sourceId">1=csjn, 2=saij, 3=pjn, 4=scba.</param>
    /// <param name="documentId">External document ID.</param>
    /// <param name="yearMonth">Optional year-month (e.g. from crawl DateFrom). Uses current if null.</param>
    public static string BuildPdfPath(string documentType, int sourceId, string documentId, DateOnly? yearMonth = null)
    {
        var docType = string.IsNullOrWhiteSpace(documentType) ? "ruling" : documentType.Trim().ToLowerInvariant();
        var source = sourceId switch
        {
            1 => "csjn",
            2 => "saij",
            3 => "pjn",
            4 => "scba",
            _ => "unknown"
        };
        var yyyyMm = yearMonth.HasValue
            ? $"{yearMonth.Value.Year:D4}-{yearMonth.Value.Month:D2}"
            : $"{DateTime.UtcNow.Year:D4}-{DateTime.UtcNow.Month:D2}";
        return $"{Root}/{docType}/{source}/{yyyyMm}/{documentId}.pdf";
    }

    /// <summary>
    /// Builds the text blob path from a PDF path (replaces .pdf with .txt).
    /// </summary>
    public static string ToTextPath(string pdfPath)
    {
        if (string.IsNullOrEmpty(pdfPath)) return pdfPath;
        return pdfPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
            ? pdfPath[..^4] + ".txt"
            : pdfPath + ".txt";
    }

    /// <summary>
    /// Builds the metadata JSON blob path from a PDF path (replaces .pdf with .metadata.json).
    /// </summary>
    public static string ToMetadataPath(string pdfPath)
    {
        if (string.IsNullOrEmpty(pdfPath)) return pdfPath;
        return pdfPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
            ? pdfPath[..^4] + ".metadata.json"
            : pdfPath + ".metadata.json";
    }

    /// <summary>
    /// Builds the indexer payload blob path from a PDF path (replaces .pdf with .indexer.json).
    /// </summary>
    public static string ToIndexerPayloadPath(string pdfPath)
    {
        if (string.IsNullOrEmpty(pdfPath)) return pdfPath;
        return pdfPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
            ? pdfPath[..^4] + ".indexer.json"
            : pdfPath + ".indexer.json";
    }

    /// <summary>
    /// Extracts document type from a PDF blob path.
    /// Path format: {root}/{documentType}/{source}/{yyyy-MM}/{documentId}.pdf
    /// </summary>
    public static string? GetDocumentTypeFromPath(string pdfPath)
    {
        if (string.IsNullOrWhiteSpace(pdfPath)) return null;
        var parts = pdfPath.Split('/');
        return parts.Length >= 5 ? parts[1] : null;
    }
}
