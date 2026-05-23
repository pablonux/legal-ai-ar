using System.Threading;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Blob;

/// <summary>
/// Azure Blob Storage implementation for PDF storage.
/// Path structure: {source}/{year}/{documentId}.pdf (e.g. csjn/2024/8048522.pdf).
/// </summary>
public class AzureBlobStorageService : IBlobStorageService
{
    private const string PdfContentType = "application/pdf";
    private readonly BlobContainerClient? _containerClient;
    private readonly ILogger<AzureBlobStorageService> _logger;
    private readonly SemaphoreSlim _ensureContainerLock = new(1, 1);
    private bool _containerEnsured;

    public AzureBlobStorageService(
        IOptions<AzureBlobOptions> options,
        ILogger<AzureBlobStorageService> logger)
    {
        _logger = logger;
        var opts = options.Value;
        if (string.IsNullOrWhiteSpace(opts.ConnectionString))
        {
            _logger.LogWarning("AzureBlob ConnectionString is not configured — blob operations will fail at runtime");
            return;
        }

        var blobServiceClient = new BlobServiceClient(opts.ConnectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(opts.ContainerName);
    }

    private BlobContainerClient Container =>
        _containerClient ?? throw new InvalidOperationException("AzureBlob ConnectionString is not configured.");

    private async Task EnsureContainerExistsAsync(CancellationToken cancellationToken)
    {
        if (_containerEnsured)
            return;

        await _ensureContainerLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_containerEnsured)
                return;

            var client = Container;
            await client.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            _containerEnsured = true;
            _logger.LogInformation("Ensured blob container '{Container}' exists for PDF storage.", client.Name);
        }
        finally
        {
            _ensureContainerLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(string blobPath, Stream content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobPath))
            throw new ArgumentException("Blob path cannot be empty.", nameof(blobPath));

        await EnsureContainerExistsAsync(cancellationToken).ConfigureAwait(false);

        var blobClient = Container.GetBlobClient(blobPath);

        await blobClient.UploadAsync(
            content,
            new BlobHttpHeaders { ContentType = PdfContentType },
            conditions: null,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Uploaded blob {BlobPath}", blobPath);
        return blobPath;
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobPath))
            throw new ArgumentException("Blob path cannot be empty.", nameof(blobPath));

        await EnsureContainerExistsAsync(cancellationToken).ConfigureAwait(false);

        var blobClient = Container.GetBlobClient(blobPath);
        var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);

        return response.Value.Content;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobPath))
            return false;

        await EnsureContainerExistsAsync(cancellationToken).ConfigureAwait(false);

        var blobClient = Container.GetBlobClient(blobPath);
        var response = await blobClient.ExistsAsync(cancellationToken);
        return response.Value;
    }

    /// <inheritdoc />
    public async Task<string> MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
            throw new ArgumentException("Source path cannot be empty.", nameof(sourcePath));
        if (string.IsNullOrWhiteSpace(destinationPath))
            throw new ArgumentException("Destination path cannot be empty.", nameof(destinationPath));
        if (string.Equals(sourcePath, destinationPath, StringComparison.OrdinalIgnoreCase))
            return destinationPath;

        await using var sourceStream = await DownloadAsync(sourcePath, cancellationToken);
        using var buffer = new MemoryStream();
        await sourceStream.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;
        await UploadAsync(destinationPath, buffer, cancellationToken);
        var sourceClient = Container.GetBlobClient(sourcePath);
        await sourceClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Moved blob from {SourcePath} to {DestinationPath}", sourcePath, destinationPath);
        return destinationPath;
    }
}
