namespace LegalAiAr.Core.Models;

/// <summary>
/// Result of downloading a document from a crawler source. Contains the content hash and raw PDF bytes.
/// Returned by <see cref="Interfaces.Pipeline.ICrawlerSource.GetContentHashAsync"/> to avoid
/// downloading the PDF twice (once for hash/deduplication, once for Blob upload).
/// </summary>
/// <param name="ContentHash">SHA-256 hash of the PDF content (64-char hex, lowercase).</param>
/// <param name="PdfBytes">Raw PDF binary content for Blob Storage upload.</param>
public record CrawlerDocumentContent(
    string ContentHash,
    byte[] PdfBytes);
