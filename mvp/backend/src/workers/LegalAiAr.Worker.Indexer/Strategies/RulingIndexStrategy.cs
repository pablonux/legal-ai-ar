using System.Text.Json;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Worker.Indexer.Chunking;
using LegalAiAr.Worker.Indexer.Steps;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Indexer.Strategies;

/// <summary>
/// Indexes a ruling that was already persisted by the Persister worker.
/// Steps: HydratePayload → HydrateFullText → UploadBlob → Chunk → Embeddings → IndexSearch →
///        TrackEmbeddingState → ExtractChunkMentions → ResolveCitations.
/// </summary>
public sealed class RulingIndexStrategy : IIndexStrategy
{
    private readonly IBlobStorageService _blobStorage;
    private readonly IRulingRepository _rulingRepository;
    private readonly ICourtRepository _courtRepository;
    private readonly IEmbeddingConfigRepository _embeddingConfigRepo;
    private readonly TextChunkingService _chunkingService;
    private readonly UploadBlobStep _uploadBlobStep;
    private readonly GenerateEmbeddingsStep _generateEmbeddingsStep;
    private readonly IndexSearchStep _indexSearchStep;
    private readonly ExtractChunkMentionsStep _extractMentionsStep;
    private readonly ResolveCitationsStep _resolveCitationsStep;
    private readonly ILogger<RulingIndexStrategy> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public RulingIndexStrategy(
        IBlobStorageService blobStorage,
        IRulingRepository rulingRepository,
        ICourtRepository courtRepository,
        IEmbeddingConfigRepository embeddingConfigRepo,
        TextChunkingService chunkingService,
        UploadBlobStep uploadBlobStep,
        GenerateEmbeddingsStep generateEmbeddingsStep,
        IndexSearchStep indexSearchStep,
        ExtractChunkMentionsStep extractMentionsStep,
        ResolveCitationsStep resolveCitationsStep,
        ILogger<RulingIndexStrategy> logger)
    {
        _blobStorage = blobStorage;
        _rulingRepository = rulingRepository;
        _courtRepository = courtRepository;
        _embeddingConfigRepo = embeddingConfigRepo;
        _chunkingService = chunkingService;
        _uploadBlobStep = uploadBlobStep;
        _generateEmbeddingsStep = generateEmbeddingsStep;
        _indexSearchStep = indexSearchStep;
        _extractMentionsStep = extractMentionsStep;
        _resolveCitationsStep = resolveCitationsStep;
        _logger = logger;
    }

    public async Task IndexAsync(IndexerMessage message, CancellationToken cancellationToken = default)
    {
        var rulingId = message.EntityId
                       ?? throw new InvalidOperationException("IndexerMessage.EntityId is required for RulingIndexStrategy");

        message = await HydratePayloadAsync(message, cancellationToken);
        message = await HydrateFullTextAsync(message, cancellationToken);

        await _uploadBlobStep.ExecuteAsync(message, cancellationToken);

        var chunks = _chunkingService.GetEffectiveChunks(message);

        var activeConfig = await _embeddingConfigRepo.EnsureSeededAsync(cancellationToken);
        var embeddingsResult = await _generateEmbeddingsStep.ExecuteAsync(
            message, chunks, activeConfig, cancellationToken);

        var courtName = message.Ruling.Court ?? message.Ruling.Instance ?? "CSJN";
        var court = await _courtRepository.GetOrCreateAsync(courtName, "Federal", "Nacional",
            message.Ruling.Instance ?? "Única", cancellationToken);
        PersistRulingStep.ClassifyCourtIfNeeded(court, message.Ruling.Instance);
        var courtMeta = new CourtMetadata(court.CourtCategory?.ToString(), court.Fuero?.ToString(), court.InstanceLevel);

        await _indexSearchStep.ExecuteAsync(rulingId, message, embeddingsResult, courtName, courtMeta, cancellationToken);

        try
        {
            await _embeddingConfigRepo.UpsertEmbeddingStateAsync(rulingId, activeConfig.Id,
                embeddingsResult.ChunkEmbeddings.Count, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to track embedding state for ruling {RulingId}, continuing", rulingId);
        }

        await _extractMentionsStep.ExecuteAsync(rulingId, message, chunks, cancellationToken);
        await _resolveCitationsStep.ExecuteAsync(rulingId, message, cancellationToken);

        _logger.LogInformation("Indexed ruling {RulingId} (doc {DocumentId})", rulingId, message.DocumentId);
    }

    private async Task<IndexerMessage> HydratePayloadAsync(IndexerMessage message, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(message.PayloadBlobPath))
            return message;

        await using var stream = await _blobStorage.DownloadAsync(message.PayloadBlobPath, ct);
        var fullMessage = await JsonSerializer.DeserializeAsync<IndexerMessage>(stream, JsonOptions, ct);
        if (fullMessage is null)
            return message;

        return fullMessage with
        {
            PayloadBlobPath = message.PayloadBlobPath,
            IngestionJobId = message.IngestionJobId ?? fullMessage.IngestionJobId,
            PipelineDocumentId = message.PipelineDocumentId,
            EntityType = message.EntityType,
            EntityId = message.EntityId
        };
    }

    private async Task<IndexerMessage> HydrateFullTextAsync(IndexerMessage message, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(message.Ruling.FullText) || string.IsNullOrEmpty(message.TextBlobPath))
            return message;

        await using var stream = await _blobStorage.DownloadAsync(message.TextBlobPath, ct);
        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
        var fullText = await reader.ReadToEndAsync(ct);

        var hydratedRuling = message.Ruling with { FullText = fullText };
        var chunks = _chunkingService.Chunk(fullText);

        return message with { Ruling = hydratedRuling, Chunks = chunks };
    }
}
