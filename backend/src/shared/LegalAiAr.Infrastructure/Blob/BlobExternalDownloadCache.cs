using System.Threading;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Blob;

/// <summary>
/// Azure Blob-backed implementation of <see cref="IExternalDownloadCache"/>.
/// Stores cached external downloads (PDFs, API JSON responses) under _cache/ prefix
/// in the same container used for pipeline data.
/// </summary>
public class BlobExternalDownloadCache : IExternalDownloadCache
{
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<BlobExternalDownloadCache> _logger;
    private readonly SemaphoreSlim _ensureContainerLock = new(1, 1);
    private bool _containerEnsured;

    public BlobExternalDownloadCache(
        IOptions<AzureBlobOptions> options,
        ILogger<BlobExternalDownloadCache> logger)
    {
        var opts = options.Value;
        if (string.IsNullOrWhiteSpace(opts.ConnectionString))
            throw new InvalidOperationException("AzureBlob ConnectionString must be configured.");

        var blobServiceClient = new BlobServiceClient(opts.ConnectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(opts.ContainerName);
        _logger = logger;
    }

    private async Task EnsureContainerExistsAsync(CancellationToken cancellationToken)
    {
        if (_containerEnsured)
            return;

        await _ensureContainerLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_containerEnsured)
                return;

            await _containerClient.CreateIfNotExistsAsync(
                cancellationToken: cancellationToken).ConfigureAwait(false);
            _containerEnsured = true;
            _logger.LogInformation(
                "Blob download cache ensured container '{Container}' exists.", _containerClient.Name);
        }
        finally
        {
            _ensureContainerLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<byte[]?> GetAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cacheKey))
            return null;

        await EnsureContainerExistsAsync(cancellationToken).ConfigureAwait(false);

        var blobClient = _containerClient.GetBlobClient(cacheKey);

        try
        {
            var exists = await blobClient.ExistsAsync(cancellationToken);
            if (!exists.Value)
                return null;

            var response = await blobClient.DownloadContentAsync(cancellationToken);
            var bytes = response.Value.Content.ToArray();
            _logger.LogDebug("Cache HIT: {CacheKey} ({Size} bytes)", cacheKey, bytes.Length);
            return bytes;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync(string cacheKey, byte[] content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cacheKey))
            throw new ArgumentException("Cache key cannot be empty.", nameof(cacheKey));

        await EnsureContainerExistsAsync(cancellationToken).ConfigureAwait(false);

        var blobClient = _containerClient.GetBlobClient(cacheKey);
        var contentType = cacheKey.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? "application/json"
            : "application/octet-stream";

        using var stream = new MemoryStream(content);
        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = contentType },
            conditions: null,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Cache SET: {CacheKey} ({Size} bytes)", cacheKey, content.Length);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cacheKey))
            return false;

        await EnsureContainerExistsAsync(cancellationToken).ConfigureAwait(false);

        var blobClient = _containerClient.GetBlobClient(cacheKey);
        var response = await blobClient.ExistsAsync(cancellationToken);
        return response.Value;
    }
}
