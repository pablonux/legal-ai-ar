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

namespace LegalAiAr.Worker.Parser;

public sealed class ParserWorkerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IQueuePublisher _publisher;
    private readonly IQueueReceiver _receiver;
    private readonly PipelineQueueNames _queueNames;
    private readonly ILogger<ParserWorkerService> _logger;
    private readonly WorkerOptions _options;
    private readonly IWorkerGate _workerGate;
    private readonly SemaphoreSlim _throttle;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public ParserWorkerService(
        IServiceScopeFactory scopeFactory,
        IQueuePublisher publisher,
        IQueueReceiver receiver,
        PipelineQueueNames queueNames,
        ILogger<ParserWorkerService> logger,
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
        _throttle = new SemaphoreSlim(_options.MaxConcurrency);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ParserWorkerService starting...");

        using (var scope = _scopeFactory.CreateScope())
        {
            var documentRepo = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
            await documentRepo.ResetProcessingToPendingAsync(PipelineStage.Parser, stoppingToken);
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
                    _queueNames.Parser, maxMessages: _options.BatchSize,
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
                            await _publisher.PublishRawAsync(_queueNames.ParserDlq, msg.Body, stoppingToken);
                            await _receiver.DeleteMessageAsync(
                                _queueNames.Parser, msg.MessageId, msg.PopReceipt, stoppingToken);
                            continue;
                        }

                        tasks.Add(ProcessWithThrottleAsync(msg, stoppingToken));
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
                _logger.LogError(ex, "Error polling queue {Queue}", _queueNames.Parser);
            }

            var delay = processedAny ? pollInterval : emptyPollInterval;
            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("ParserWorkerService stopped.");
    }

    private async Task ProcessWithThrottleAsync(
        QueueMessage msg,
        CancellationToken ct)
    {
        await _throttle.WaitAsync(ct);
        try
        {
            await ProcessMessageAsync(msg, ct);
        }
        finally
        {
            _throttle.Release();
        }
    }

    private async Task ProcessMessageAsync(
        QueueMessage msg,
        CancellationToken ct)
    {
        ParserMessage? parserMessage;
        try
        {
            parserMessage = JsonSerializer.Deserialize<ParserMessage>(msg.Body, JsonOptions);
            if (parserMessage is null) throw new JsonException("Deserialized to null");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize ParserMessage");
            await _publisher.PublishToDlqAsync(_queueNames.ParserDlq, msg.Body, ex, ct);
            await _receiver.DeleteMessageAsync(_queueNames.Parser, msg.MessageId, msg.PopReceipt, ct);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;
        var stageStart = DateTime.UtcNow;

        try
        {
            var documentRepo = sp.GetRequiredService<IDocumentRepository>();
            var jobRepo = sp.GetRequiredService<IIngestionJobRepository>();
            var strategyResolver = sp.GetRequiredService<IStrategyResolver<IParseStrategy>>();
            var strategy = strategyResolver.Resolve(parserMessage.EntityType, parserMessage.SourceId);

            if (parserMessage.PipelineDocumentId.HasValue &&
                !await documentRepo.SetProcessingAsync(
                    parserMessage.PipelineDocumentId.Value, PipelineStage.Parser, ct))
            {
                _logger.LogWarning("Document {DocId} not at Parser/Pending, skipping",
                    parserMessage.PipelineDocumentId);
                await _receiver.DeleteMessageAsync(_queueNames.Parser, msg.MessageId, msg.PopReceipt, ct);
                return;
            }

            var result = await strategy.ParseAsync(parserMessage, ct);

            if (parserMessage.PipelineDocumentId.HasValue)
                await documentRepo.AdvanceStageAsync(
                    parserMessage.PipelineDocumentId.Value, PipelineStage.Enricher, ct);

            await _publisher.PublishAsync(_queueNames.Enricher, result.EnrichmentMessage, ct);

            if (parserMessage.IngestionJobId.HasValue)
                await jobRepo.IncrementDocumentsParsedAsync(parserMessage.IngestionJobId.Value, ct);

            if (parserMessage.PipelineDocumentId.HasValue)
            {
                var stageLogRepo = sp.GetRequiredService<IDocumentStageLogRepository>();
                var now = DateTime.UtcNow;
                await stageLogRepo.LogStageAsync(new DocumentStageLog
                {
                    DocumentId = parserMessage.PipelineDocumentId.Value,
                    Stage = PipelineStage.Parser,
                    StartedAt = stageStart,
                    CompletedAt = now,
                    DurationMs = (int)(now - stageStart).TotalMilliseconds,
                    WorkerInstanceId = Environment.MachineName
                }, ct);
            }

            _logger.LogInformation("Parsed {DocumentId}, published to enricher", parserMessage.DocumentId);
            await _receiver.DeleteMessageAsync(_queueNames.Parser, msg.MessageId, msg.PopReceipt, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse document {DocumentId}", parserMessage.DocumentId);

            if (parserMessage.PipelineDocumentId.HasValue)
            {
                await sp.GetRequiredService<IDocumentRepository>()
                    .SetFailedAsync(
                        parserMessage.PipelineDocumentId.Value,
                        ex.Message,
                        ex.GetType().Name,
                        CancellationToken.None);

                var stageLogRepo = sp.GetRequiredService<IDocumentStageLogRepository>();
                var now = DateTime.UtcNow;
                await stageLogRepo.LogStageAsync(new DocumentStageLog
                {
                    DocumentId = parserMessage.PipelineDocumentId.Value,
                    Stage = PipelineStage.Parser,
                    StartedAt = stageStart,
                    CompletedAt = now,
                    DurationMs = (int)(now - stageStart).TotalMilliseconds,
                    WorkerInstanceId = Environment.MachineName,
                    ErrorMessage = ex.Message
                }, CancellationToken.None);
            }

            if (parserMessage.IngestionJobId.HasValue)
            {
                await sp.GetRequiredService<IIngestionJobRepository>()
                    .IncrementDocumentsFailedAsync(parserMessage.IngestionJobId.Value, CancellationToken.None);
            }

            await _publisher.PublishToDlqAsync(_queueNames.ParserDlq, parserMessage, ex, CancellationToken.None);
            await _receiver.DeleteMessageAsync(_queueNames.Parser, msg.MessageId, msg.PopReceipt, CancellationToken.None);
        }
    }
}
