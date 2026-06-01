using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Blob;

namespace LegalAiAr.Worker.Fetcher.Strategies;

/// <summary>
/// Adapts an existing <see cref="ICrawlerSource.GetContentHashAsync"/> to the new
/// <see cref="IFetchStrategy"/> interface. Downloads PDF, uploads to Blob, returns result.
/// </summary>
public sealed class CrawlerSourceFetchAdapter : IFetchStrategy
{
    private readonly ICrawlerSource _source;
    private readonly IBlobStorageService _blobStorage;

    public CrawlerSourceFetchAdapter(ICrawlerSource source, IBlobStorageService blobStorage)
    {
        _source = source;
        _blobStorage = blobStorage;
    }

    public async Task<FetchedDocument> FetchAsync(
        FetcherMessage message,
        bool useCache = false,
        CancellationToken cancellationToken = default)
    {
        var content = await _source.GetContentHashAsync(
            message.ExternalId,
            message.AnalysisId,
            useCache || message.UseCache,
            message.FetchPdfTimeoutSeconds,
            cancellationToken);

        var entityType = message.EntityType.ToString().ToLowerInvariant();
        var blobPath = BlobPathHelper.BuildPdfPath(
            entityType,
            message.SourceId,
            message.ExternalId,
            message.AcuerdoDate);

        using var stream = new MemoryStream(content.PdfBytes);
        await _blobStorage.UploadAsync(blobPath, stream, cancellationToken);

        return new FetchedDocument(content.ContentHash, blobPath, content.PdfBytes);
    }
}
