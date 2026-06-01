using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Infrastructure.Blob;
using LegalAiAr.Worker.Enrichment.Chunking;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Enrichment.Strategies;

/// <summary>
/// SAIJ ruling enrichment: no AI calls needed (self-contained data from SAIJ API).
/// Builds a <see cref="PersisterPayload"/> with <see cref="RulingData"/> from
/// the parsed metadata, produces text chunks, uploads to blob, and returns PersisterMessage.
/// </summary>
public sealed class SaijRulingEnrichStrategy : IEnrichStrategy
{
    private readonly TextChunkingService _chunkingService;
    private readonly IBlobStorageService _blobStorage;
    private readonly ILogger<SaijRulingEnrichStrategy> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public SaijRulingEnrichStrategy(
        TextChunkingService chunkingService,
        IBlobStorageService blobStorage,
        ILogger<SaijRulingEnrichStrategy> logger)
    {
        _chunkingService = chunkingService;
        _blobStorage = blobStorage;
        _logger = logger;
    }

    public async Task<PersisterMessage> EnrichAsync(
        EnrichmentMessage message,
        CancellationToken cancellationToken = default)
    {
        var meta = message.ExtractedMetadata;
        var fullText = message.NormalizedText ?? "";

        var ruling = new RulingData(
            CaseTitle: meta.CaseTitle,
            RulingDate: meta.RulingDate,
            CaseNumber: meta.CaseNumber,
            JurisdictionArea: meta.JurisdictionArea,
            Instance: meta.Instance,
            Jurisdiction: meta.Jurisdiction,
            ResourceType: meta.ResourceType,
            RulingDirection: meta.RulingDirection,
            SubjectArea: meta.SubjectArea,
            IsUnconstitutional: meta.IsUnconstitutional,
            Summary: meta.Summary,
            Holding: meta.Holding,
            FullText: fullText,
            BlobPath: message.BlobPath ?? "",
            Court: meta.Court);

        var chunks = !string.IsNullOrWhiteSpace(fullText)
            ? _chunkingService.Chunk(fullText)
            : Array.Empty<ChunkData>();

        var payload = new PersisterPayload(
            Ruling: ruling,
            Persons: [],
            Keywords: [],
            Statutes: [],
            Citations: [],
            Chunks: chunks);

        var blobPath = $"legal-ai-ar-kb/ruling/saij/{DateTime.UtcNow:yyyy-MM}/{message.DocumentId}.payload.json";
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        {
            await _blobStorage.UploadAsync(blobPath, stream, cancellationToken);
        }

        _logger.LogInformation("SAIJ Ruling enriched: {Title}, payload at {BlobPath}",
            meta.CaseTitle, blobPath);

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
