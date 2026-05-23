using System.Text.Json;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Core.Pipeline;
using LegalAiAr.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Indexer;

/// <summary>
/// Polls the indexer queue. For each message:
/// - Resolves IIndexStrategy(EntityType, SourceId) → RulingIndexStrategy or StatuteIndexStrategy
/// - Tracks progress via the Document entity and IngestionJob counters
/// </summary>
public class IndexerWorkerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IQueueReceiver _queueReceiver;
    private readonly IQueuePublisher _queuePublisher;
    private readonly PipelineQueueNames _queueNames;
    private readonly EntityCacheService _entityCache;
    private readonly ILogger<IndexerWorkerService> _logger;
    private readonly IWorkerGate _workerGate;

    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(2);
    private readonly TimeSpan _emptyPollInterval = TimeSpan.FromSeconds(15);
    private readonly TimeSpan _visibilityTimeout = TimeSpan.FromMinutes(30);
    private const int MaxDequeueCount = 3;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public IndexerWorkerService(
        IServiceScopeFactory scopeFactory,
        IQueueReceiver queueReceiver,
        IQueuePublisher queuePublisher,
        PipelineQueueNames queueNames,
        EntityCacheService entityCache,
        ILogger<IndexerWorkerService> logger,
        IWorkerGate workerGate)
    {
        _scopeFactory = scopeFactory;
        _queueReceiver = queueReceiver;
        _queuePublisher = queuePublisher;
        _queueNames = queueNames;
        _entityCache = entityCache;
        _logger = logger;
        _workerGate = workerGate;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("IndexerWorkerService starting...");

        using (var recoveryScope = _scopeFactory.CreateScope())
        {
            var documentRepo = recoveryScope.ServiceProvider.GetRequiredService<IDocumentRepository>();
            var reset = await documentRepo.ResetProcessingToPendingAsync(PipelineStage.Indexer, stoppingToken);
            if (reset > 0)
                _logger.LogWarning(
                    "Startup recovery: reset {Count} Indexer documents from Processing to Pending",
                    reset);
        }

        _logger.LogInformation("IndexerWorkerService warming up entity cache...");
        using (var warmUpScope = _scopeFactory.CreateScope())
        {
            await _entityCache.WarmUpAsync(warmUpScope.ServiceProvider, stoppingToken);
        }

        _logger.LogInformation("IndexerWorkerService started, polling queue {Queue}", _queueNames.Indexer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await _workerGate.WaitIfPausedAsync(stoppingToken);
            var processedAny = false;

            try
            {
                var messages = await _queueReceiver.ReceiveAsync(
                    _queueNames.Indexer,
                    maxMessages: 1,
                    visibilityTimeout: _visibilityTimeout,
                    stoppingToken);

                if (messages.Count > 0)
                {
                    processedAny = true;

                    foreach (var msg in messages)
                    {
                        if (stoppingToken.IsCancellationRequested) break;

                        if (msg.DequeueCount > MaxDequeueCount)
                        {
                            _logger.LogWarning("Message exceeded max dequeue ({Count}), moving to DLQ",
                                msg.DequeueCount);
                            await _queuePublisher.PublishRawAsync(_queueNames.IndexerDlq, msg.Body, stoppingToken);
                            await _queueReceiver.DeleteMessageAsync(
                                _queueNames.Indexer, msg.MessageId, msg.PopReceipt, stoppingToken);
                            continue;
                        }

                        await ProcessMessageAsync(msg, stoppingToken);
                    }
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
                _logger.LogError(ex, "Error polling queue {Queue}", _queueNames.Indexer);
            }

            var delay = processedAny ? _pollInterval : _emptyPollInterval;
            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("IndexerWorkerService stopped.");
    }

    private async Task ProcessMessageAsync(QueueMessage msg, CancellationToken ct)
    {
        IndexerMessage? indexerMessage;
        try
        {
            indexerMessage = JsonSerializer.Deserialize<IndexerMessage>(msg.Body, JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize IndexerMessage");
            await _queuePublisher.PublishToDlqAsync(_queueNames.IndexerDlq, msg.Body, ex, ct);
            await _queueReceiver.DeleteMessageAsync(_queueNames.Indexer, msg.MessageId, msg.PopReceipt, ct);
            return;
        }

        if (indexerMessage is null || string.IsNullOrEmpty(indexerMessage.DocumentId))
        {
            _logger.LogWarning("Received invalid IndexerMessage (null or missing DocumentId), discarding");
            await _queueReceiver.DeleteMessageAsync(_queueNames.Indexer, msg.MessageId, msg.PopReceipt, ct);
            return;
        }

        if (indexerMessage.PipelineDocumentId is null)
        {
            _logger.LogWarning("Received IndexerMessage without PipelineDocumentId (legacy format), moving to DLQ");
            await _queuePublisher.PublishToDlqAsync(_queueNames.IndexerDlq, indexerMessage,
                new InvalidOperationException("Legacy IndexerMessage format is no longer supported"), ct);
            await _queueReceiver.DeleteMessageAsync(_queueNames.Indexer, msg.MessageId, msg.PopReceipt, ct);
            return;
        }

        var startedAt = DateTime.UtcNow;
        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        try
        {
            var documentRepo = sp.GetRequiredService<IDocumentRepository>();

            if (!await documentRepo.SetProcessingAsync(
                    indexerMessage.PipelineDocumentId.Value, PipelineStage.Indexer, ct))
            {
                _logger.LogWarning("Document {DocId} not at Indexer/Pending, skipping",
                    indexerMessage.PipelineDocumentId);
                await _queueReceiver.DeleteMessageAsync(_queueNames.Indexer, msg.MessageId, msg.PopReceipt, ct);
                return;
            }

            var strategyResolver = sp.GetRequiredService<IStrategyResolver<IIndexStrategy>>();
            var strategy = strategyResolver.Resolve(indexerMessage.EntityType, indexerMessage.SourceId);
            await strategy.IndexAsync(indexerMessage, ct);

            await documentRepo.SetCompletedAsync(indexerMessage.PipelineDocumentId.Value, ct);

            await sp.GetRequiredService<IRulingReprocessCompletionService>()
                .OnPipelineSucceededAsync(indexerMessage.PipelineDocumentId.Value, ct);

            if (indexerMessage.IngestionJobId.HasValue)
            {
                var jobRepo = sp.GetRequiredService<IIngestionJobRepository>();
                await jobRepo.IncrementDocumentsIndexedAsync(indexerMessage.IngestionJobId.Value, ct);
                await jobRepo.TryCompleteIfDoneAsync(indexerMessage.IngestionJobId.Value, ct);
            }

            _logger.LogInformation("Indexed {EntityType} document {DocId}, entity {EntityId}",
                indexerMessage.EntityType, indexerMessage.PipelineDocumentId, indexerMessage.EntityId);
            await _queueReceiver.DeleteMessageAsync(_queueNames.Indexer, msg.MessageId, msg.PopReceipt, ct);

            var completedAt = DateTime.UtcNow;
            await sp.GetRequiredService<IDocumentStageLogRepository>().LogStageAsync(new DocumentStageLog
            {
                DocumentId = indexerMessage.PipelineDocumentId.Value,
                Stage = PipelineStage.Indexer,
                StartedAt = startedAt,
                CompletedAt = completedAt,
                DurationMs = (int)(completedAt - startedAt).TotalMilliseconds,
                WorkerInstanceId = Environment.MachineName
            }, CancellationToken.None);
        }
        catch (Exception ex)
        {
            var rootEx = ex;
            while (rootEx.InnerException != null) rootEx = rootEx.InnerException;
            var errorMsg = rootEx == ex ? ex.Message : $"{ex.Message} → {rootEx.Message}";

            _logger.LogError(ex, "Indexing failed for document {DocId}: {RootCause}",
                indexerMessage.PipelineDocumentId, rootEx.Message);

            await sp.GetRequiredService<IDocumentRepository>()
                .SetFailedAsync(indexerMessage.PipelineDocumentId.Value, errorMsg, ex.GetType().Name, ct);

            await sp.GetRequiredService<IRulingReprocessCompletionService>()
                .OnPipelineFailedAsync(indexerMessage.PipelineDocumentId.Value, errorMsg, ct);

            if (indexerMessage.IngestionJobId.HasValue)
            {
                var jobRepo = sp.GetRequiredService<IIngestionJobRepository>();
                await jobRepo.IncrementDocumentsFailedAsync(indexerMessage.IngestionJobId.Value, ct);
                await jobRepo.TryCompleteIfDoneAsync(indexerMessage.IngestionJobId.Value, ct);
            }

            await _queuePublisher.PublishToDlqAsync(_queueNames.IndexerDlq, indexerMessage, ex, ct);
            await _queueReceiver.DeleteMessageAsync(_queueNames.Indexer, msg.MessageId, msg.PopReceipt, ct);

            var completedAt = DateTime.UtcNow;
            await sp.GetRequiredService<IDocumentStageLogRepository>().LogStageAsync(new DocumentStageLog
            {
                DocumentId = indexerMessage.PipelineDocumentId.Value,
                Stage = PipelineStage.Indexer,
                StartedAt = startedAt,
                CompletedAt = completedAt,
                DurationMs = (int)(completedAt - startedAt).TotalMilliseconds,
                WorkerInstanceId = Environment.MachineName,
                ErrorMessage = errorMsg.Length > 500 ? errorMsg[..500] : errorMsg
            }, CancellationToken.None);
        }
    }
}
