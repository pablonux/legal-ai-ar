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
/// SAIJ legislation enrichment: no AI calls needed (self-contained data).
/// Builds a <see cref="PersisterPayload"/> with <see cref="StatutePayloadData"/> from
/// the parsed metadata, produces text chunks, uploads to blob, and returns PersisterMessage.
/// </summary>
public sealed class SaijLegislationEnrichStrategy : IEnrichStrategy
{
    private readonly TextChunkingService _chunkingService;
    private readonly IBlobStorageService _blobStorage;
    private readonly ILogger<SaijLegislationEnrichStrategy> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public SaijLegislationEnrichStrategy(
        TextChunkingService chunkingService,
        IBlobStorageService blobStorage,
        ILogger<SaijLegislationEnrichStrategy> logger)
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

        var statute = new StatutePayloadData(
            Number: meta.CaseNumber ?? message.DocumentId,
            Name: meta.CaseTitle,
            NormType: null,
            NormativeLevel: null,
            IssuingBody: meta.Court,
            SanctionDate: meta.RulingDate != default ? meta.RulingDate : null,
            PublicationDate: null,
            Status: null,
            FullText: fullText,
            SaijId: message.DocumentId,
            OfficialUrl: null);

        var chunks = !string.IsNullOrWhiteSpace(fullText)
            ? _chunkingService.Chunk(fullText)
            : Array.Empty<ChunkData>();

        var payload = new PersisterPayload(
            Ruling: null,
            Persons: [],
            Keywords: [],
            Statutes: [],
            Citations: [],
            Chunks: chunks,
            Statute: statute);

        var blobPath = $"legal-ai-ar-kb/statute/saij/{DateTime.UtcNow:yyyy-MM}/{message.DocumentId}.payload.json";
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        {
            await _blobStorage.UploadAsync(blobPath, stream, cancellationToken);
        }

        _logger.LogInformation("SAIJ Legislation enriched: {Name}, payload at {BlobPath}",
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
