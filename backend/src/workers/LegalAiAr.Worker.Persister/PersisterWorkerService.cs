using System.Diagnostics;
using System.Text;
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
using Microsoft.Extensions.Options;

namespace LegalAiAr.Worker.Persister;

/// <summary>
/// Polls the persister queue, downloads PersisterPayload from blob,
/// resolves <see cref="IPersistStrategy"/>, persists to SQL, advances the <see cref="Document"/>,
/// and publishes <see cref="IndexerMessage"/>.
/// </summary>
/// <remarks>
/// Runs <b>one message at a time</b> (no overlapping persists): avoids races on shared dimension rows
/// (Person, Keyword, Court, …) and on EF graphs. <see cref="WorkerOptions.BatchSize"/> only caps how many
/// queue messages are received per poll; each is still awaited sequentially.
/// On startup, documents stuck in <see cref="DocumentStatus.Processing"/> at this stage are reset to
/// <see cref="DocumentStatus.Pending"/> (same pattern as Parser/Enricher). Then
/// <see cref="EntityCacheService.WarmUpAsync"/> loads keywords, statutes, courts, persons and thesaurus.
/// </remarks>
public sealed class PersisterWorkerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IQueuePublisher _publisher;
    private readonly IQueueReceiver _receiver;
    private readonly PipelineQueueNames _queueNames;
    private readonly EntityCacheService _entityCache;
    private readonly WorkerOptions _options;
    private readonly IWorkerGate _workerGate;
    private readonly ILogger<PersisterWorkerService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public PersisterWorkerService(
        IServiceScopeFactory scopeFactory,
        IQueuePublisher publisher,
        IQueueReceiver receiver,
        PipelineQueueNames queueNames,
        EntityCacheService entityCache,
        IOptions<WorkerOptions> options,
        IWorkerGate workerGate,
        ILogger<PersisterWorkerService> logger)
    {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
        _receiver = receiver;
        _queueNames = queueNames;
        _entityCache = entityCache;
        _options = options.Value;
        _workerGate = workerGate;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "PersisterWorkerService starting (sequential processing, receive batch={BatchSize})...",
            _options.BatchSize);

        using (var recoveryScope = _scopeFactory.CreateScope())
        {
            var documentRepo = recoveryScope.ServiceProvider.GetRequiredService<IDocumentRepository>();
            var recoverySw = Stopwatch.StartNew();
            var reset = await documentRepo.ResetProcessingToPendingAsync(PipelineStage.Persister, stoppingToken);
            recoverySw.Stop();
            if (reset > 0)
                _logger.LogWarning(
                    "Startup recovery: reset {Count} Persister documents from Processing to Pending ({ElapsedMs}ms)",
                    reset, recoverySw.ElapsedMilliseconds);
            else
                _logger.LogDebug(
                    "Persister phase {Phase} {ElapsedMs}ms ResetRows={ResetRows}",
                    "StartupRecovery", recoverySw.ElapsedMilliseconds, reset);
        }

        using (var warmUpScope = _scopeFactory.CreateScope())
        {
            var warmSw = Stopwatch.StartNew();
            await _entityCache.WarmUpAsync(warmUpScope.ServiceProvider, stoppingToken);
            warmSw.Stop();
            _logger.LogInformation(
                "Persister phase {Phase} {ElapsedMs}ms",
                "EntityCacheWarmUp", warmSw.ElapsedMilliseconds);
        }

        var pollInterval = TimeSpan.FromSeconds(_options.PollIntervalSeconds);
        var emptyPollInterval = TimeSpan.FromSeconds(_options.EmptyPollIntervalSeconds);
        var visibilityTimeout = TimeSpan.FromMinutes(_options.VisibilityTimeoutMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            var gateSw = Stopwatch.StartNew();
            await _workerGate.WaitIfPausedAsync(stoppingToken);
            gateSw.Stop();
            if (gateSw.ElapsedMilliseconds > 0)
                _logger.LogDebug(
                    "Persister phase {Phase} {ElapsedMs}ms",
                    "WaitIfPaused", gateSw.ElapsedMilliseconds);

            var processedAny = false;

            try
            {
                var receiveSw = Stopwatch.StartNew();
                var messages = await _receiver.ReceiveAsync(
                    _queueNames.Persister, maxMessages: _options.BatchSize,
                    visibilityTimeout: visibilityTimeout,
                    cancellationToken: stoppingToken);
                receiveSw.Stop();

                _logger.LogDebug(
                    "Persister phase {Phase} {ElapsedMs}ms MessageCount={MessageCount}",
                    "ReceiveAsync", receiveSw.ElapsedMilliseconds, messages.Count);

                if (messages.Count > 0)
                {
                    processedAny = true;

                    foreach (var msg in messages)
                    {
                        if (stoppingToken.IsCancellationRequested) break;

                        if (msg.DequeueCount > _options.MaxDequeueCount)
                        {
                            _logger.LogWarning("Message exceeded max dequeue ({Count}), moving to DLQ",
                                msg.DequeueCount);
                            await _publisher.PublishRawAsync(_queueNames.PersisterDlq, msg.Body, stoppingToken);
                            await _receiver.DeleteMessageAsync(
                                _queueNames.Persister, msg.MessageId, msg.PopReceipt, stoppingToken);
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
                _logger.LogError(ex, "Error polling queue {Queue}", _queueNames.Persister);
            }

            var delay = processedAny ? pollInterval : emptyPollInterval;
            _logger.LogDebug(
                "Persister phase {Phase} {ElapsedMs}ms ProcessedAny={ProcessedAny}",
                "BetweenPollsDelay", (int)delay.TotalMilliseconds, processedAny);
            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("PersisterWorkerService stopped.");
    }

    private async Task ProcessMessageAsync(
        QueueMessage msg,
        CancellationToken ct)
    {
        PersisterMessage? persisterMessage;
        try
        {
            var deserializeSw = Stopwatch.StartNew();
            persisterMessage = JsonSerializer.Deserialize<PersisterMessage>(msg.Body, JsonOptions);
            deserializeSw.Stop();
            if (persisterMessage is null) throw new JsonException("Deserialized to null");

            var queueBodyBytes = Encoding.UTF8.GetByteCount(msg.Body);
            _logger.LogDebug(
                "Persister phase {Phase} {ElapsedMs}ms QueueBodyBytes={QueueBodyBytes}",
                "DeserializeQueueMessage", deserializeSw.ElapsedMilliseconds, queueBodyBytes);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize PersisterMessage");
            await _publisher.PublishToDlqAsync(_queueNames.PersisterDlq, msg.Body, ex, ct);
            await _receiver.DeleteMessageAsync(_queueNames.Persister, msg.MessageId, msg.PopReceipt, ct);
            return;
        }

        _logger.LogInformation(
            "Processing PersisterMessage: DocId={DocId} Entity={Entity} Source={Source} Hash={Hash}",
            persisterMessage.DocumentId, persisterMessage.EntityType, persisterMessage.SourceId,
            persisterMessage.ContentHash?[..Math.Min(8, persisterMessage.ContentHash?.Length ?? 0)]);

        var startedAt = DateTime.UtcNow;
        var processSw = Stopwatch.StartNew();
        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        try
        {
            var documentRepo = sp.GetRequiredService<IDocumentRepository>();
            var jobRepo = sp.GetRequiredService<IIngestionJobRepository>();
            var strategyResolver = sp.GetRequiredService<IStrategyResolver<IPersistStrategy>>();
            var strategy = strategyResolver.Resolve(persisterMessage.EntityType, persisterMessage.SourceId);

            var setProcSw = Stopwatch.StartNew();
            var setProcessingOk = await documentRepo.SetProcessingAsync(
                persisterMessage.DocumentId, PipelineStage.Persister, ct);
            setProcSw.Stop();
            _logger.LogDebug(
                "Persister phase {Phase} {ElapsedMs}ms DocId={DocumentId} Ok={Ok}",
                "SetProcessingAsync", setProcSw.ElapsedMilliseconds, persisterMessage.DocumentId, setProcessingOk);

            if (!setProcessingOk)
            {
                _logger.LogWarning("Document {DocId} not at Persister/Pending, skipping (stage or status mismatch)",
                    persisterMessage.DocumentId);
                processSw.Stop();
                await _receiver.DeleteMessageAsync(_queueNames.Persister, msg.MessageId, msg.PopReceipt, ct);
                return;
            }

            var persistSw = Stopwatch.StartNew();
            var result = await strategy.PersistAsync(persisterMessage, ct);
            persistSw.Stop();
            _logger.LogInformation(
                "Persister phase {Phase} {ElapsedMs}ms DocId={DocumentId} EntityType={EntityType} EntityId={EntityId}",
                "PersistAsync", persistSw.ElapsedMilliseconds, persisterMessage.DocumentId,
                persisterMessage.EntityType, result.EntityId);

            var postSqlSw = Stopwatch.StartNew();
            Guid? rulingId = persisterMessage.EntityType == EntityType.Ruling ? result.EntityId : null;
            int? statuteId = persisterMessage.EntityType == EntityType.Statute
                ? BitConverter.ToInt32(result.EntityId.ToByteArray(), 0)
                : null;
            await documentRepo.SetEntityIdAsync(persisterMessage.DocumentId, rulingId, statuteId, ct);
            await documentRepo.AdvanceStageAsync(persisterMessage.DocumentId, PipelineStage.Indexer, ct);

            if (persisterMessage.IngestionJobId.HasValue)
                await jobRepo.IncrementDocumentsPersistedAsync(persisterMessage.IngestionJobId.Value, ct);
            postSqlSw.Stop();
            _logger.LogDebug(
                "Persister phase {Phase} {ElapsedMs}ms DocId={DocumentId}",
                "PostPersistSql", postSqlSw.ElapsedMilliseconds, persisterMessage.DocumentId);

            var indexerMessage = BuildIndexerMessage(persisterMessage, result);
            var publishSw = Stopwatch.StartNew();
            await _publisher.PublishAsync(_queueNames.Indexer, indexerMessage, ct);
            publishSw.Stop();
            _logger.LogDebug(
                "Persister phase {Phase} {ElapsedMs}ms DocId={DocumentId}",
                "PublishIndexer", publishSw.ElapsedMilliseconds, persisterMessage.DocumentId);

            _logger.LogInformation("Persisted {EntityType} {EntityId}, published to indexer",
                persisterMessage.EntityType, result.EntityId);

            var deleteSw = Stopwatch.StartNew();
            await _receiver.DeleteMessageAsync(_queueNames.Persister, msg.MessageId, msg.PopReceipt, ct);
            deleteSw.Stop();
            _logger.LogDebug(
                "Persister phase {Phase} {ElapsedMs}ms DocId={DocumentId}",
                "DeleteQueueMessage", deleteSw.ElapsedMilliseconds, persisterMessage.DocumentId);

            var completedAt = DateTime.UtcNow;
            processSw.Stop();
            _logger.LogInformation(
                "Persister phase {Phase} {ElapsedMs}ms DocId={DocumentId} EntityType={EntityType}",
                "ProcessMessageAsync", processSw.ElapsedMilliseconds, persisterMessage.DocumentId,
                persisterMessage.EntityType);

            var logStageSw = Stopwatch.StartNew();
            await sp.GetRequiredService<IDocumentStageLogRepository>().LogStageAsync(new DocumentStageLog
            {
                DocumentId = persisterMessage.DocumentId,
                Stage = PipelineStage.Persister,
                StartedAt = startedAt,
                CompletedAt = completedAt,
                DurationMs = (int)(completedAt - startedAt).TotalMilliseconds,
                WorkerInstanceId = Environment.MachineName
            }, CancellationToken.None);
            logStageSw.Stop();
            _logger.LogDebug(
                "Persister phase {Phase} {ElapsedMs}ms DocId={DocumentId}",
                "LogStageAsync", logStageSw.ElapsedMilliseconds, persisterMessage.DocumentId);
        }
        catch (Exception ex)
        {
            var rootEx = ex;
            while (rootEx.InnerException != null) rootEx = rootEx.InnerException;
            var errorMsg = rootEx == ex ? ex.Message : $"{ex.Message} → {rootEx.Message}";

            _logger.LogError(ex, "Failed to persist document {DocId}: {RootCause}",
                persisterMessage.DocumentId, rootEx.Message);

            using var errorScope = _scopeFactory.CreateScope();
            var errorSp = errorScope.ServiceProvider;

            await errorSp.GetRequiredService<IDocumentRepository>()
                .SetFailedAsync(persisterMessage.DocumentId, errorMsg, ex.GetType().Name, ct);

            if (persisterMessage.IngestionJobId.HasValue)
            {
                var failJobRepo = errorSp.GetRequiredService<IIngestionJobRepository>();
                await failJobRepo.IncrementDocumentsFailedAsync(persisterMessage.IngestionJobId.Value, ct);
                await failJobRepo.TryCompleteIfDoneAsync(persisterMessage.IngestionJobId.Value, ct);
            }

            await _publisher.PublishToDlqAsync(_queueNames.PersisterDlq, persisterMessage, ex, ct);
            await _receiver.DeleteMessageAsync(_queueNames.Persister, msg.MessageId, msg.PopReceipt, ct);

            var completedAt = DateTime.UtcNow;
            processSw.Stop();
            _logger.LogInformation(
                "Persister phase {Phase} {ElapsedMs}ms DocId={DocumentId} EntityType={EntityType} (failed)",
                "ProcessMessageAsync", processSw.ElapsedMilliseconds, persisterMessage.DocumentId,
                persisterMessage.EntityType);

            await errorSp.GetRequiredService<IDocumentStageLogRepository>().LogStageAsync(new DocumentStageLog
            {
                DocumentId = persisterMessage.DocumentId,
                Stage = PipelineStage.Persister,
                StartedAt = startedAt,
                CompletedAt = completedAt,
                DurationMs = (int)(completedAt - startedAt).TotalMilliseconds,
                WorkerInstanceId = Environment.MachineName,
                ErrorMessage = errorMsg.Length > 500 ? errorMsg[..500] : errorMsg
            }, CancellationToken.None);
        }
    }

    private static IndexerMessage BuildIndexerMessage(PersisterMessage msg, PersistResult result)
    {
        return new IndexerMessage(
            DocumentId: msg.DocumentId.ToString(),
            ContentHash: msg.ContentHash,
            SourceId: msg.SourceId,
            Ruling: new RulingData("", default, null, null, null, null, null, null, null, false, null, null, "", ""),
            Persons: [],
            Keywords: [],
            Statutes: [],
            Citations: [],
            Chunks: [],
            IngestionJobId: msg.IngestionJobId,
            PayloadBlobPath: msg.PayloadBlobPath,
            Reprocess: msg.Reprocess,
            PipelineDocumentId: msg.DocumentId,
            EntityType: msg.EntityType,
            EntityId: result.EntityId);
    }
}
