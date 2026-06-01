using System.Text.Json;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;
using LegalAiAr.Infrastructure.Crawling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Worker.Fetcher;

public sealed class FetcherWorkerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IQueuePublisher _publisher;
    private readonly IQueueReceiver _receiver;
    private readonly PipelineQueueNames _queueNames;
    private readonly ILogger<FetcherWorkerService> _logger;
    private readonly WorkerOptions _options;
    private readonly IWorkerGate _workerGate;
    private readonly SemaphoreSlim _concurrencySemaphore;

    private readonly TimeSpan _pollInterval;
    private readonly TimeSpan _emptyPollInterval;
    private readonly TimeSpan _visibilityTimeout;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public FetcherWorkerService(
        IServiceScopeFactory scopeFactory,
        IQueuePublisher publisher,
        IQueueReceiver receiver,
        PipelineQueueNames queueNames,
        ILogger<FetcherWorkerService> logger,
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
        _concurrencySemaphore = new SemaphoreSlim(_options.MaxConcurrency);
        _pollInterval = TimeSpan.FromSeconds(_options.PollIntervalSeconds);
        _emptyPollInterval = TimeSpan.FromSeconds(_options.EmptyPollIntervalSeconds);
        _visibilityTimeout = TimeSpan.FromMinutes(_options.VisibilityTimeoutMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("FetcherWorkerService starting...");

        await RecoverProcessingDocumentsAsync();

        using var shutdownCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await _workerGate.WaitIfPausedAsync(stoppingToken);
            var processedAny = false;

            try
            {
                var messages = await _receiver.ReceiveAsync(
                    _queueNames.Fetcher, maxMessages: _options.BatchSize,
                    visibilityTimeout: _visibilityTimeout,
                    cancellationToken: stoppingToken);

                if (messages.Count > 0)
                {
                    processedAny = true;
                    var tasks = messages.Select(msg => ProcessWithSemaphoreAsync(msg, shutdownCts.Token));
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
                _logger.LogError(ex, "Error polling queue {Queue}", _queueNames.Fetcher);
            }

            var delay = processedAny ? _pollInterval : _emptyPollInterval;
            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("FetcherWorkerService stopped.");
    }

    private async Task ProcessWithSemaphoreAsync(
        QueueMessage msg,
        CancellationToken ct)
    {
        await _concurrencySemaphore.WaitAsync(ct);
        try
        {
            if (msg.DequeueCount > _options.MaxDequeueCount)
            {
                _logger.LogWarning("Message exceeded max dequeue ({Count}), moving to DLQ", msg.DequeueCount);
                await _publisher.PublishRawAsync(_queueNames.FetcherDlq, msg.Body, CancellationToken.None);
                await _receiver.DeleteMessageAsync(_queueNames.Fetcher, msg.MessageId, msg.PopReceipt, CancellationToken.None);
                return;
            }

            await ProcessMessageAsync(msg, ct);
        }
        finally
        {
            _concurrencySemaphore.Release();
        }
    }

    private async Task ProcessMessageAsync(
        QueueMessage msg,
        CancellationToken ct)
    {
        FetcherMessage? fetcherMessage;
        try
        {
            fetcherMessage = JsonSerializer.Deserialize<FetcherMessage>(msg.Body, JsonOptions);
            if (fetcherMessage is null) throw new JsonException("Deserialized to null");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize FetcherMessage");
            await _publisher.PublishToDlqAsync(_queueNames.FetcherDlq, msg.Body, ex, CancellationToken.None);
            await _receiver.DeleteMessageAsync(_queueNames.Fetcher, msg.MessageId, msg.PopReceipt, CancellationToken.None);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;
        var startedAt = DateTime.UtcNow;

        try
        {
            var documentRepo = sp.GetRequiredService<IDocumentRepository>();
            var jobRepo = sp.GetRequiredService<IIngestionJobRepository>();
            var strategyResolver = sp.GetRequiredService<IStrategyResolver<IFetchStrategy>>();
            var strategy = strategyResolver.Resolve(fetcherMessage.EntityType, fetcherMessage.SourceId);

            if (!await documentRepo.SetProcessingAsync(fetcherMessage.DocumentId, PipelineStage.Fetcher, ct))
            {
                _logger.LogWarning("Document {DocId} not at Fetcher/Pending stage, skipping", fetcherMessage.DocumentId);
                await _receiver.DeleteMessageAsync(_queueNames.Fetcher, msg.MessageId, msg.PopReceipt, CancellationToken.None);
                return;
            }

            if (fetcherMessage.EntityType == EntityType.Ruling && fetcherMessage.SourceId == 1 &&
                string.IsNullOrWhiteSpace(fetcherMessage.AnalysisId))
            {
                throw new InvalidOperationException(
                    $"AnalysisId is required for CSJN ruling fetch (document {fetcherMessage.ExternalId}).");
            }

            var result = await strategy.FetchAsync(fetcherMessage, fetcherMessage.UseCache, ct);

            if (fetcherMessage.EntityType == EntityType.Ruling && fetcherMessage.SourceId == 1)
            {
                var sjTransport = sp.GetRequiredService<CsjnSjconsultaJsonTransport>();
                await sjTransport.PrimeSjconsultaCachesAsync(
                    fetcherMessage.ExternalId,
                    fetcherMessage.AnalysisId!,
                    fetcherMessage.UseCache,
                    ct).ConfigureAwait(false);
                _logger.LogInformation(
                    "Primed CSJN sjconsulta JSON cache for document {ExternalId}, analysis {AnalysisId}",
                    fetcherMessage.ExternalId,
                    fetcherMessage.AnalysisId);
            }

            await documentRepo.SetFetchResultAsync(fetcherMessage.DocumentId, result.BlobPath, result.ContentHash, ct);
            await documentRepo.AdvanceStageAsync(fetcherMessage.DocumentId, PipelineStage.Parser, ct);

            var parserMessage = new ParserMessage(
                SourceId: fetcherMessage.SourceId,
                DocumentId: fetcherMessage.ExternalId,
                AnalysisId: fetcherMessage.AnalysisId,
                BlobPathPdf: result.BlobPath,
                ContentHash: result.ContentHash,
                ApiMetadata: null,
                IngestionJobId: fetcherMessage.IngestionJobId,
                UseCache: fetcherMessage.UseCache,
                Reprocess: fetcherMessage.Reprocess,
                RulingDateHint: fetcherMessage.AcuerdoDate,
                CaseNumberHint: fetcherMessage.CaseNumber,
                PipelineDocumentId: fetcherMessage.DocumentId,
                EntityType: fetcherMessage.EntityType);

            await _publisher.PublishAsync(_queueNames.Parser, parserMessage, ct);

            if (fetcherMessage.IngestionJobId.HasValue)
                await jobRepo.IncrementDocumentsCrawledAsync(fetcherMessage.IngestionJobId.Value, 1, ct);

            _logger.LogInformation("Fetched {ExternalId} -> {BlobPath}, published to parser",
                fetcherMessage.ExternalId, result.BlobPath);

            await _receiver.DeleteMessageAsync(_queueNames.Fetcher, msg.MessageId, msg.PopReceipt, CancellationToken.None);

            var completedAt = DateTime.UtcNow;
            var stageLogRepo = sp.GetRequiredService<IDocumentStageLogRepository>();
            await stageLogRepo.LogStageAsync(new DocumentStageLog
            {
                DocumentId = fetcherMessage.DocumentId,
                Stage = PipelineStage.Fetcher,
                StartedAt = startedAt,
                CompletedAt = completedAt,
                DurationMs = (int)(completedAt - startedAt).TotalMilliseconds,
                WorkerInstanceId = Environment.MachineName,
                ErrorMessage = null
            }, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch document {ExternalId}", fetcherMessage.ExternalId);

            var documentRepo = sp.GetRequiredService<IDocumentRepository>();
            await documentRepo.SetFailedAsync(
                fetcherMessage.DocumentId,
                ex.Message,
                ex.GetType().Name,
                CancellationToken.None);

            if (fetcherMessage.IngestionJobId.HasValue)
            {
                await sp.GetRequiredService<IIngestionJobRepository>()
                    .IncrementDocumentsFailedAsync(fetcherMessage.IngestionJobId.Value, CancellationToken.None);
            }

            await _publisher.PublishToDlqAsync(_queueNames.FetcherDlq, fetcherMessage, ex, CancellationToken.None);
            await _receiver.DeleteMessageAsync(_queueNames.Fetcher, msg.MessageId, msg.PopReceipt, CancellationToken.None);

            var completedAt = DateTime.UtcNow;
            var stageLogRepo = sp.GetRequiredService<IDocumentStageLogRepository>();
            await stageLogRepo.LogStageAsync(new DocumentStageLog
            {
                DocumentId = fetcherMessage.DocumentId,
                Stage = PipelineStage.Fetcher,
                StartedAt = startedAt,
                CompletedAt = completedAt,
                DurationMs = (int)(completedAt - startedAt).TotalMilliseconds,
                WorkerInstanceId = Environment.MachineName,
                ErrorMessage = ex.Message.Length > 500 ? ex.Message[..500] : ex.Message
            }, CancellationToken.None);
        }
    }

    private async Task RecoverProcessingDocumentsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var documentRepo = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
        var recovered = await documentRepo.ResetProcessingToPendingAsync(PipelineStage.Fetcher);
        if (recovered > 0)
            _logger.LogInformation("Recovered {Count} documents from Processing back to Pending", recovered);
    }
}
