namespace LegalAiAr.Core.Interfaces.Pipeline;

/// <summary>
/// Extracts normalized text from a PDF stored in Blob Storage.
/// Used by ParserWorker to read PDFs uploaded by CrawlerWorker.
/// </summary>
public interface IBlobPdfExtractor
{
    /// <summary>
    /// Downloads the PDF from Blob Storage and extracts normalized text.
    /// </summary>
    /// <param name="blobPathPdf">Blob path (e.g. csjn/2024/8048522.pdf).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Normalized full text of the document.</returns>
    Task<string> ExtractTextAsync(string blobPathPdf, CancellationToken cancellationToken = default);
}
