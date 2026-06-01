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

namespace LegalAiAr.Worker.Discoverer;

/// <summary>
/// Background service that polls all discoverer queues, discovers documents from sources,
/// creates Document entities for tracking, and publishes FetcherMessages.
/// Replaces the discovery portion of the legacy CrawlerWorkerService.
/// </summary>
public sealed class DiscovererWorkerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IQueuePublisher _publisher;
    private readonly IQueueReceiver _receiver;
    private readonly PipelineQueueNames _queueNames;
    private readonly ILogger<DiscovererWorkerService> _logger;
    private readonly IWorkerGate _workerGate;
    private readonly IWorkerInfraNotifier? _infraNotifier;
    private readonly WorkerOptions _options;
    private readonly SemaphoreSlim _concurrencySemaphore;

    private readonly TimeSpan _pollInterval;
    private readonly TimeSpan _emptyPollInterval;
    private readonly TimeSpan _visibilityTimeout;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public DiscovererWorkerService(
        IServiceScopeFactory scopeFactory,
        IQueuePublisher publisher,
        IQueueReceiver receiver,
        PipelineQueueNames queueNames,
        ILogger<DiscovererWorkerService> logger,
        IWorkerGate workerGate,
        IOptions<WorkerOptions> options,
        IWorkerInfraNotifier? infraNotifier = null)
    {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
        _receiver = receiver;
        _queueNames = queueNames;
        _logger = logger;
        _workerGate = workerGate;
        _options = options.Value;
        _infraNotifier = infraNotifier;
        _concurrencySemaphore = new SemaphoreSlim(_options.MaxConcurrency);
        _pollInterval = TimeSpan.FromSeconds(_options.PollIntervalSeconds);
        _emptyPollInterval = TimeSpan.FromSeconds(_options.EmptyPollIntervalSeconds);
        _visibilityTimeout = TimeSpan.FromMinutes(_options.VisibilityTimeoutMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DiscovererWorkerService starting...");

        await RecoverProcessingDocumentsAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            await _workerGate.WaitIfPausedAsync(stoppingToken);
            var processedAny = false;

            try
            {
                var messages = await _receiver.ReceiveAsync(
                    _queueNames.Discoverer, maxMessages: _options.BatchSize,
                    visibilityTimeout: _visibilityTimeout,
                    cancellationToken: stoppingToken);

                if (messages.Count > 0)
                {
                    processedAny = true;
                    var tasks = messages.Select(msg => ProcessWithSemaphoreAsync(msg, stoppingToken));
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
                _logger.LogError(ex, "Error polling queue {Queue}", _queueNames.Discoverer);
            }

            var delay = processedAny ? _pollInterval : _emptyPollInterval;
            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("DiscovererWorkerService stopped.");
    }

    private async Task RecoverProcessingDocumentsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var documentRepo = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
        var recovered = await documentRepo.ResetProcessingToPendingAsync(PipelineStage.Discoverer);
        if (recovered > 0)
            _logger.LogInformation("Recovered {Count} documents from Processing back to Pending (Discoverer stage)", recovered);
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
                _logger.LogWarning("Message exceeded max dequeue count ({Count}), moving to DLQ. Queue={Queue}",
                    msg.DequeueCount, _queueNames.Discoverer);
                await _publisher.PublishRawAsync(_queueNames.DiscovererDlq, msg.Body, CancellationToken.None);
                await _receiver.DeleteMessageAsync(_queueNames.Discoverer, msg.MessageId, msg.PopReceipt, CancellationToken.None);
                return;
            }

            await ProcessMessageAsync(msg, ct);
            await _receiver.DeleteMessageAsync(_queueNames.Discoverer, msg.MessageId, msg.PopReceipt, CancellationToken.None);
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
        DiscovererMessage? discovererMessage;
        try
        {
            discovererMessage = JsonSerializer.Deserialize<DiscovererMessage>(msg.Body, JsonOptions);
            if (discovererMessage is null) throw new JsonException("Deserialized to null");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize DiscovererMessage from queue {Queue}", _queueNames.Discoverer);
            await _publisher.PublishToDlqAsync(_queueNames.DiscovererDlq, msg.Body, ex, CancellationToken.None);
            return;
        }

        _logger.LogInformation(
            "Processing discovery: EntityType={EntityType} Source={SourceId} Type={Type} DateFrom={DateFrom} DateTo={DateTo}",
            discovererMessage.EntityType, discovererMessage.SourceId, discovererMessage.Type,
            discovererMessage.DateFrom, discovererMessage.DateTo);

        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        try
        {
            await ProcessDiscoveryAsync(discovererMessage, sp, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Discovery failed for EntityType={EntityType} Source={SourceId}",
                discovererMessage.EntityType, discovererMessage.SourceId);
            await _publisher.PublishToDlqAsync(_queueNames.DiscovererDlq, discovererMessage, ex, ct);
        }
    }

    private async Task ProcessDiscoveryAsync(
        DiscovererMessage message,
        IServiceProvider sp,
        CancellationToken ct)
    {
        var jobRepo = sp.GetRequiredService<IIngestionJobRepository>();
        var documentRepo = sp.GetRequiredService<IDocumentRepository>();
        var strategyResolver = sp.GetRequiredService<IStrategyResolver<IDiscoverStrategy>>();
        var strategy = strategyResolver.Resolve(message.EntityType, message.SourceId);

        // Check for overlapping active jobs (allow resuming the same IngestionJobId).
        var hasOverlap = message.IngestionJobId is { } existingJobId
            ? await jobRepo.HasActiveJobOtherThanAsync(message.EntityType, message.SourceId, existingJobId, ct)
            : await jobRepo.HasActiveJobAsync(message.EntityType, message.SourceId, ct);
        if (hasOverlap)
        {
            _logger.LogWarning("Active job already exists for EntityType={EntityType} Source={SourceId}, skipping",
                message.EntityType, message.SourceId);
            return;
        }

        // Create or resume the IngestionJob
        IngestionJob job;
        if (message.IngestionJobId.HasValue)
        {
            job = await jobRepo.GetByIdAsync(message.IngestionJobId.Value, ct)
                ?? throw new InvalidOperationException($"IngestionJob {message.IngestionJobId.Value} not found");

            if (job.Status is "pending")
                await jobRepo.UpdateStatusAsync(job.Id, "running", ct);

            if (!message.PreserveProgressCounters)
                await jobRepo.ResetDocumentsDiscoveredAsync(job.Id, ct);
        }
        else
        {
            job = new IngestionJob
            {
                Id = Guid.NewGuid(),
                SourceId = message.SourceId,
                EntityType = message.EntityType,
                Type = message.Type,
                DateFrom = message.DateFrom,
                DateTo = message.DateTo,
                TriggeredBy = "admin",
                StartedAt = DateTime.UtcNow,
                Status = "running",
            };
            await jobRepo.AddAsync(job, ct);
        }

        await jobRepo.SetDiscoveryBatchPublishedAsync(job.Id, false, ct);

        _logger.LogInformation("Job {JobId}: Starting discovery for {EntityType}/{SourceId}",
            job.Id, message.EntityType, message.SourceId);

        var documentsCreated = 0;
        var documentsSkipped = message is { IngestionJobId: not null, PreserveProgressCounters: true }
            ? job.DocumentsSkipped
            : 0;
        string? lastError = null;
        var totalDiscovered = 0;
        var totalProcessed = 0;
        var skipRemaining = message.SkipDocuments ?? 0;
        var fetcherMessagesEnqueuedTotal = 0;
        var fetcherPipelineStarted = false;

        async Task<bool> TryEnqueueFetcherBatchAsync(List<FetcherMessage> batchMessages)
        {
            if (batchMessages.Count == 0)
                return true;

            var batchSize = batchMessages.Count;
            try
            {
                await _publisher.PublishBatchAsync(_queueNames.Fetcher, batchMessages, ct);
                fetcherMessagesEnqueuedTotal += batchSize;
                batchMessages.Clear();

                if (!fetcherPipelineStarted)
                {
                    fetcherPipelineStarted = true;
                    await jobRepo.SetDiscoveryBatchPublishedAsync(job.Id, true, ct);
                    await jobRepo.UpdateStatusAsync(job.Id, "processing", ct);
                    _logger.LogInformation(
                        "Job {JobId}: Fetcher pipeline started (incremental enqueue). First batch enqueued={Count} message(s).",
                        job.Id, batchSize);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job {JobId}: Failed to enqueue documents to Fetcher queue", job.Id);
                if (_infraNotifier is not null)
                {
                    var code = ex is Azure.RequestFailedException rfe ? rfe.ErrorCode : null;
                    await _infraNotifier.NotifyInfrastructureIncidentAsync(
                        _queueNames.Fetcher,
                        code,
                        ex.Message,
                        job.Id,
                        ct);
                }

                await jobRepo.UpdateCompletionAsync(job.Id, documentsCreated, "failed", $"Fetcher enqueue: {ex.Message}", ct);
                return false;
            }
        }

        try
        {
            await foreach (var batch in strategy.DiscoverAsync(message, ct).WithCancellation(ct))
            {
                ct.ThrowIfCancellationRequested();

                var batchFetcherMessages = new List<FetcherMessage>();
                var batchProcessed = 0;
                foreach (var doc in batch)
                {
                    totalDiscovered++;

                    if (skipRemaining > 0)
                    {
                        skipRemaining--;
                        if (!message.PreserveProgressCounters)
                            documentsSkipped++;
                        continue;
                    }

                    if (message.MaxDocuments.HasValue && totalProcessed >= message.MaxDocuments.Value)
                    {
                        if (batchProcessed > 0)
                            await jobRepo.IncrementDocumentsDiscoveredAsync(job.Id, batchProcessed, ct);

                        if (documentsSkipped > 0)
                            await jobRepo.SetDocumentsSkippedAsync(job.Id, documentsSkipped, ct);

                        if (!await TryEnqueueFetcherBatchAsync(batchFetcherMessages))
                            return;

                        _logger.LogInformation("MaxDocuments limit ({Max}) reached (skipped {Skip}, processed {Processed})",
                            message.MaxDocuments.Value, message.SkipDocuments ?? 0, totalProcessed);
                        goto discoveryComplete;
                    }

                    batchProcessed++;

                    ct.ThrowIfCancellationRequested();
                    totalProcessed++;

                    try
                    {
                        Document document;
                        var existing = message.Reprocess
                            ? await documentRepo.GetByExternalIdAsync(message.SourceId, doc.DocumentId, ct)
                            : null;

                        if (existing is not null)
                        {
                            existing.IngestionJobId = job.Id;
                            existing.CurrentStage = PipelineStage.Fetcher;
                            existing.Status = DocumentStatus.Pending;
                            existing.ErrorMessage = null;
                            existing.ErrorType = null;
                            existing.RetryCount = 0;
                            existing.RulingId = null;
                            existing.StatuteId = null;
                            await documentRepo.UpdateAsync(existing, ct);
                            document = existing;
                        }
                        else
                        {
                            if (!message.Reprocess
                                && await documentRepo.ExistsByExternalIdAsync(message.SourceId, doc.DocumentId, ct))
                            {
                                documentsSkipped++;
                                continue;
                            }

                            document = new Document
                            {
                                Id = Guid.NewGuid(),
                                IngestionJobId = job.Id,
                                EntityType = message.EntityType,
                                SourceId = message.SourceId,
                                ExternalId = doc.DocumentId,
                                AnalysisId = doc.AnalysisId,
                                CurrentStage = PipelineStage.Fetcher,
                                Status = DocumentStatus.Pending,
                            };
                            await documentRepo.CreateAsync(document, ct);
                        }
                        documentsCreated++;

                        batchFetcherMessages.Add(new FetcherMessage(
                            DocumentId: document.Id,
                            EntityType: message.EntityType,
                            SourceId: message.SourceId,
                            ExternalId: doc.DocumentId,
                            AnalysisId: doc.AnalysisId,
                            IngestionJobId: job.Id,
                            UseCache: message.UseCache,
                            Reprocess: message.Reprocess,
                            AcuerdoDate: doc.AcuerdoDate,
                            CaseNumber: doc.CaseNumber,
                            FetchPdfTimeoutSeconds: document.FetchPdfTimeoutSeconds));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to create document entry for {ExternalId}", doc.DocumentId);
                        lastError = ex.Message;
                    }
                }

                if (batchProcessed > 0)
                    await jobRepo.IncrementDocumentsDiscoveredAsync(job.Id, batchProcessed, ct);

                if (documentsSkipped > 0)
                    await jobRepo.SetDocumentsSkippedAsync(job.Id, documentsSkipped, ct);

                if (!await TryEnqueueFetcherBatchAsync(batchFetcherMessages))
                    return;

                _logger.LogInformation("Job {JobId}: Discovered batch — created: {Created}, skipped: {Skipped}",
                    job.Id, documentsCreated, documentsSkipped);
            }
        discoveryComplete:

            if (strategy.LastTotalSearchResults.HasValue)
                await jobRepo.SetTotalSearchResultsAsync(job.Id, strategy.LastTotalSearchResults.Value, ct);

            if (documentsSkipped > 0)
                await jobRepo.SetDocumentsSkippedAsync(job.Id, documentsSkipped, ct);

            if (lastError != null)
            {
                await jobRepo.UpdateCompletionAsync(job.Id, documentsCreated, "failed", lastError, ct);
                _logger.LogWarning(
                    "Job {JobId}: Discovery completed with errors. Created={Created} Skipped={Skipped}. " +
                    "{FetcherEnqueued} Fetcher message(s) were already enqueued; later batches may be missing from the queue.",
                    job.Id, documentsCreated, documentsSkipped, fetcherMessagesEnqueuedTotal);
                return;
            }

            if (fetcherMessagesEnqueuedTotal > 0)
            {
                _logger.LogInformation(
                    "Job {JobId}: Discovery complete. Created={Created} Skipped={Skipped} Enqueued={Enqueued} Status=processing (awaiting pipeline)",
                    job.Id, documentsCreated, documentsSkipped, fetcherMessagesEnqueuedTotal);
            }
            else
            {
                await jobRepo.UpdateStatusAsync(job.Id, "discovered", ct);
                await jobRepo.UpdateCompletionAsync(job.Id, documentsCreated, "success", null, ct);
                _logger.LogInformation(
                    "Job {JobId}: Discovery complete. Created={Created} Skipped={Skipped} Status=success (nothing to process)",
                    job.Id, documentsCreated, documentsSkipped);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            await jobRepo.UpdateCompletionAsync(job.Id, documentsCreated, "cancelled", "Cancelled by host shutdown", CancellationToken.None);
            throw;
        }
        catch (Exception ex)
        {
            await jobRepo.UpdateCompletionAsync(job.Id, documentsCreated, "failed", ex.Message, CancellationToken.None);
            throw;
        }
    }
}
