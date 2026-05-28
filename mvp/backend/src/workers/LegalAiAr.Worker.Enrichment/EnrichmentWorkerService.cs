using System.Text.Json;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Worker.Enrichment;

public sealed class EnrichmentWorkerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IQueuePublisher _publisher;
    private readonly IQueueReceiver _receiver;
    private readonly PipelineQueueNames _queueNames;
    private readonly ILogger<EnrichmentWorkerService> _logger;
    private readonly WorkerOptions _options;
    private readonly IWorkerGate _workerGate;
    private readonly SemaphoreSlim _semaphore;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public EnrichmentWorkerService(
        IServiceScopeFactory scopeFactory,
        IQueuePublisher publisher,
        IQueueReceiver receiver,
        PipelineQueueNames queueNames,
        ILogger<EnrichmentWorkerService> logger,
        IWorkerGate workerGate,
        IOptions<WorkerOptions> options)
    {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
        _receiver = receiver;
        _queueNames = queueNames;
        _logger = logger;
        _workerGate = workerGate;
        _options = options.Value;
        _semaphore = new SemaphoreSlim(_options.MaxConcurrency);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EnrichmentWorkerService starting...");

        using (var scope = _scopeFactory.CreateScope())
        {
            var documentRepo = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
            var reset = await documentRepo.ResetProcessingToPendingAsync(PipelineStage.Enricher, stoppingToken);
            if (reset > 0)
                _logger.LogWarning("Startup recovery: reset {Count} Enricher documents from Processing to Pending", reset);
        }

        var pollInterval = TimeSpan.FromSeconds(_options.PollIntervalSeconds);
        var emptyPollInterval = TimeSpan.FromSeconds(_options.EmptyPollIntervalSeconds);
        var visibilityTimeout = TimeSpan.FromMinutes(_options.VisibilityTimeoutMinutes);
        while (!stoppingToken.IsCancellationRequested)
        {
            await _workerGate.WaitIfPausedAsync(stoppingToken);
            var processedAny = false;

            try
            {
                var messages = await _receiver.ReceiveAsync(
                    _queueNames.Enricher, maxMessages: _options.BatchSize,
                    visibilityTimeout: visibilityTimeout,
                    cancellationToken: stoppingToken);

                if (messages.Count > 0)
                {
                    processedAny = true;
                    var tasks = new List<Task>(messages.Count);
                    foreach (var msg in messages)
                    {
                        if (stoppingToken.IsCancellationRequested) break;

                        if (msg.DequeueCount > _options.MaxDequeueCount)
                        {
                            _logger.LogWarning("Message exceeded max dequeue ({Count}), moving to DLQ",
                                msg.DequeueCount);
                            await _publisher.PublishRawAsync(_queueNames.EnricherDlq, msg.Body, stoppingToken);
                            await _receiver.DeleteMessageAsync(
                                _queueNames.Enricher, msg.MessageId, msg.PopReceipt, stoppingToken);
                            continue;
                        }

                        tasks.Add(ProcessMessageWithThrottleAsync(msg, stoppingToken));
                    }

                    await Task.WhenAll(tasks);
                }
            }
            catch (Azure.RequestFailedException ex) when (ex.ErrorCode == "QueueNotFound")
            {
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error polling queue {Queue}", _queueNames.Enricher);
            }

            var delay = processedAny ? pollInterval : emptyPollInterval;
            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("EnrichmentWorkerService stopped.");
    }

    private async Task ProcessMessageWithThrottleAsync(
        QueueMessage msg,
        CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            await ProcessMessageAsync(msg, ct);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task ProcessMessageAsync(
        QueueMessage msg,
        CancellationToken ct)
    {
        EnrichmentMessage? enrichmentMessage;
        try
        {
            enrichmentMessage = JsonSerializer.Deserialize<EnrichmentMessage>(msg.Body, JsonOptions);
            if (enrichmentMessage is null) throw new JsonException("Deserialized to null");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize EnrichmentMessage");
            await _publisher.PublishToDlqAsync(_queueNames.EnricherDlq, msg.Body, ex, ct);
            await _receiver.DeleteMessageAsync(_queueNames.Enricher, msg.MessageId, msg.PopReceipt, ct);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;
        var stageStart = DateTime.UtcNow;

        try
        {
            var documentRepo = sp.GetRequiredService<IDocumentRepository>();
            var jobRepo = sp.GetRequiredService<IIngestionJobRepository>();
            var strategyResolver = sp.GetRequiredService<IStrategyResolver<IEnrichStrategy>>();
            var blobStorage = sp.GetRequiredService<IBlobStorageService>();
            var strategy = strategyResolver.Resolve(enrichmentMessage.EntityType, enrichmentMessage.SourceId);

            if (enrichmentMessage.PipelineDocumentId.HasValue &&
                !await documentRepo.SetProcessingAsync(
                    enrichmentMessage.PipelineDocumentId.Value, PipelineStage.Enricher, ct))
            {
                _logger.LogWarning("Document {DocId} not at Enricher/Pending, skipping",
                    enrichmentMessage.PipelineDocumentId);
                await _receiver.DeleteMessageAsync(_queueNames.Enricher, msg.MessageId, msg.PopReceipt, ct);
                return;
            }

            var hydratedMessage = await HydrateNormalizedTextAsync(blobStorage, enrichmentMessage, ct);
            hydratedMessage = await HydrateMetadataAsync(blobStorage, hydratedMessage, ct);

            var persisterMessage = await strategy.EnrichAsync(hydratedMessage, ct);

            if (enrichmentMessage.PipelineDocumentId.HasValue)
                await documentRepo.AdvanceStageAsync(
                    enrichmentMessage.PipelineDocumentId.Value, PipelineStage.Persister, ct);

            await _publisher.PublishAsync(_queueNames.Persister, persisterMessage, ct);

            if (enrichmentMessage.IngestionJobId.HasValue)
                await jobRepo.IncrementDocumentsEnrichedAsync(enrichmentMessage.IngestionJobId.Value, ct);

            if (enrichmentMessage.PipelineDocumentId.HasValue)
            {
                var stageLog = sp.GetRequiredService<IDocumentStageLogRepository>();
                await stageLog.LogStageAsync(new DocumentStageLog
                {
                    DocumentId = enrichmentMessage.PipelineDocumentId.Value,
                    Stage = PipelineStage.Enricher,
                    StartedAt = stageStart,
                    CompletedAt = DateTime.UtcNow,
                    DurationMs = (int)(DateTime.UtcNow - stageStart).TotalMilliseconds,
                    WorkerInstanceId = Environment.MachineName
                }, ct);
            }

            _logger.LogInformation("Enriched {DocumentId}, published to persister", enrichmentMessage.DocumentId);
            await _receiver.DeleteMessageAsync(_queueNames.Enricher, msg.MessageId, msg.PopReceipt, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enrich document {DocumentId}", enrichmentMessage.DocumentId);

            if (enrichmentMessage.PipelineDocumentId.HasValue)
            {
                await sp.GetRequiredService<IDocumentRepository>()
                    .SetFailedAsync(
                        enrichmentMessage.PipelineDocumentId.Value,
                        ex.Message,
                        ex.GetType().Name,
                        CancellationToken.None);

                var stageLog = sp.GetRequiredService<IDocumentStageLogRepository>();
                await stageLog.LogStageAsync(new DocumentStageLog
                {
                    DocumentId = enrichmentMessage.PipelineDocumentId.Value,
                    Stage = PipelineStage.Enricher,
                    StartedAt = stageStart,
                    CompletedAt = DateTime.UtcNow,
                    DurationMs = (int)(DateTime.UtcNow - stageStart).TotalMilliseconds,
                    WorkerInstanceId = Environment.MachineName,
                    ErrorMessage = ex.Message
                }, CancellationToken.None);
            }

            if (enrichmentMessage.IngestionJobId.HasValue)
            {
                await sp.GetRequiredService<IIngestionJobRepository>()
                    .IncrementDocumentsFailedAsync(enrichmentMessage.IngestionJobId.Value, CancellationToken.None);
            }

            await _publisher.PublishToDlqAsync(_queueNames.EnricherDlq, enrichmentMessage, ex, CancellationToken.None);
            await _receiver.DeleteMessageAsync(_queueNames.Enricher, msg.MessageId, msg.PopReceipt, CancellationToken.None);
        }
    }

    private async Task<EnrichmentMessage> HydrateNormalizedTextAsync(
        IBlobStorageService blobStorage,
        EnrichmentMessage message,
        CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(message.NormalizedText) || string.IsNullOrEmpty(message.TextBlobPath))
            return message;

        using var stream = await blobStorage.DownloadAsync(message.TextBlobPath, ct);
        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
        var text = await reader.ReadToEndAsync(ct);

        _logger.LogDebug("Hydrated text from blob {Path} ({Length} chars)", message.TextBlobPath, text.Length);
        return message with { NormalizedText = text };
    }

    private async Task<EnrichmentMessage> HydrateMetadataAsync(
        IBlobStorageService blobStorage,
        EnrichmentMessage message,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(message.MetadataBlobPath))
            return message;

        using var stream = await blobStorage.DownloadAsync(message.MetadataBlobPath, ct);
        var metadata = await JsonSerializer.DeserializeAsync<ExtractedMetadata>(stream, JsonOptions, ct);

        if (metadata is null)
        {
            _logger.LogWarning("Failed to deserialize metadata from {Path}", message.MetadataBlobPath);
            return message;
        }

        _logger.LogDebug("Hydrated metadata from blob {Path}", message.MetadataBlobPath);
        return message with { ExtractedMetadata = metadata };
    }
}
