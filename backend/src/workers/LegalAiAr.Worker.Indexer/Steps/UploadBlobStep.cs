using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Indexer.Steps;

/// <summary>
/// Verifies the PDF blob exists at ruling.blobPath.
/// Phase 1: CrawlerWorker uploads PDF; this step only verifies. No upload from IndexerWorker.
/// Per E056 — if blob missing, fails and message moves to DLQ.
/// </summary>
public class UploadBlobStep
{
    private readonly IBlobStorageService _blobStorage;
    private readonly ILogger<UploadBlobStep> _logger;

    public UploadBlobStep(
        IBlobStorageService blobStorage,
        ILogger<UploadBlobStep> logger)
    {
        _blobStorage = blobStorage;
        _logger = logger;
    }

    /// <summary>
    /// Verifies the blob exists at ruling.blobPath.
    /// </summary>
    /// <param name="message">IndexerMessage with ruling.blobPath.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The confirmed blob path.</returns>
    /// <exception cref="DomainException">When blob does not exist.</exception>
    public async Task<string> ExecuteAsync(
        IndexerMessage message,
        CancellationToken cancellationToken = default)
    {
        var blobPath = message.Ruling.BlobPath;

        if (string.IsNullOrWhiteSpace(blobPath))
        {
            _logger.LogError("Blob path is empty for document {DocumentId}", message.DocumentId);
            throw new DomainException($"Blob path is required for document {message.DocumentId}.");
        }

        var exists = await _blobStorage.ExistsAsync(blobPath, cancellationToken);

        if (!exists)
        {
            _logger.LogError("Blob not found at {BlobPath} for document {DocumentId}", blobPath, message.DocumentId);
            throw new DomainException($"Blob not found at {blobPath} for document {message.DocumentId}. CrawlerWorker should have uploaded the PDF.");
        }

        _logger.LogDebug("Blob verified at {BlobPath} for document {DocumentId}", blobPath, message.DocumentId);
        return blobPath;
    }
}
