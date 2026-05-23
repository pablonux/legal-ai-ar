using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Infrastructure.Blob;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Enrichment.Strategies;

/// <summary>
/// Adapter that wraps the existing <see cref="CsjnEnrichmentStrategy"/> (IEnrichmentStrategy)
/// and converts its output to <see cref="PersisterMessage"/> (IEnrichStrategy).
/// The full enriched payload is uploaded to blob; the PersisterMessage carries only the blob path.
/// </summary>
public sealed class CsjnRulingEnrichStrategy : IEnrichStrategy
{
    private readonly CsjnEnrichmentStrategy _inner;
    private readonly IBlobStorageService _blobStorage;
    private readonly ILogger<CsjnRulingEnrichStrategy> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public CsjnRulingEnrichStrategy(
        CsjnEnrichmentStrategy inner,
        IBlobStorageService blobStorage,
        ILogger<CsjnRulingEnrichStrategy> logger)
    {
        _inner = inner;
        _blobStorage = blobStorage;
        _logger = logger;
    }

    public async Task<PersisterMessage> EnrichAsync(
        EnrichmentMessage message,
        CancellationToken cancellationToken = default)
    {
        if (message.UseCache && !string.IsNullOrEmpty(message.BlobPath))
        {
            var cachedBlobPath = BlobPathHelper.ToIndexerPayloadPath(message.BlobPath);
            if (await _blobStorage.ExistsAsync(cachedBlobPath, cancellationToken))
            {
                _logger.LogInformation(
                    "Reusing cached enriched payload at {BlobPath} for document {DocumentId}",
                    cachedBlobPath, message.DocumentId);
                return new PersisterMessage(
                    DocumentId: message.PipelineDocumentId ?? Guid.Empty,
                    EntityType: message.EntityType,
                    SourceId: message.SourceId,
                    ContentHash: message.ContentHash ?? "",
                    IngestionJobId: message.IngestionJobId,
                    PayloadBlobPath: cachedBlobPath,
                    Reprocess: message.Reprocess);
            }
        }

        var indexerMessage = await _inner.EnrichAsync(message, cancellationToken);

        var payload = new PersisterPayload(
            Ruling: indexerMessage.Ruling,
            Persons: indexerMessage.Persons,
            Keywords: indexerMessage.Keywords,
            Statutes: indexerMessage.Statutes,
            Citations: indexerMessage.Citations,
            Chunks: indexerMessage.Chunks,
            AnalysisId: indexerMessage.AnalysisId,
            CitedBy: indexerMessage.CitedBy,
            ProsecutorOpinion: indexerMessage.ProsecutorOpinion,
            Votes: indexerMessage.Votes,
            Sumarios: indexerMessage.Sumarios,
            Syntheses: indexerMessage.Syntheses,
            Links: indexerMessage.Links,
            Parties: indexerMessage.Parties,
            LegalRepresentations: indexerMessage.LegalRepresentations);

        var blobPath = BlobPathHelper.ToIndexerPayloadPath(indexerMessage.Ruling.BlobPath);
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        {
            await _blobStorage.UploadAsync(blobPath, stream, cancellationToken);
        }

        _logger.LogInformation("Uploaded enriched payload to {BlobPath} ({Size} bytes)",
            blobPath, json.Length);

        return new PersisterMessage(
            DocumentId: message.PipelineDocumentId ?? Guid.Empty,
            EntityType: message.EntityType,
            SourceId: message.SourceId,
            ContentHash: message.ContentHash ?? "",
            IngestionJobId: message.IngestionJobId,
            PayloadBlobPath: blobPath,
            Reprocess: message.Reprocess);
    }
}
