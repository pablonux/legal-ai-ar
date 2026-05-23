using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Parser.Parsers;

/// <summary>
/// Extracts normalized text from PDFs stored in Azure Blob Storage.
/// Downloads the blob and passes the stream to PdfTextExtractor.
/// </summary>
public class BlobPdfExtractor : IBlobPdfExtractor
{
    private readonly IBlobStorageService _blobStorage;
    private readonly PdfTextExtractor _pdfExtractor;
    private readonly ILogger<BlobPdfExtractor> _logger;

    public BlobPdfExtractor(
        IBlobStorageService blobStorage,
        PdfTextExtractor pdfExtractor,
        ILogger<BlobPdfExtractor> logger)
    {
        _blobStorage = blobStorage;
        _pdfExtractor = pdfExtractor;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> ExtractTextAsync(string blobPathPdf, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobPathPdf))
            throw new ArgumentException("Blob path cannot be empty.", nameof(blobPathPdf));

        var exists = await _blobStorage.ExistsAsync(blobPathPdf, cancellationToken);
        if (!exists)
            throw new DomainException($"Blob not found: {blobPathPdf}");

        await using var stream = await _blobStorage.DownloadAsync(blobPathPdf, cancellationToken);

        try
        {
            var text = await _pdfExtractor.ExtractAsync(stream, cancellationToken);
            _logger.LogDebug("Extracted {Length} chars from blob {BlobPath}", text.Length, blobPathPdf);
            return text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract text from blob {BlobPath}", blobPathPdf);
            throw new DomainException($"Failed to extract text from blob {blobPathPdf}", ex);
        }
    }
}
