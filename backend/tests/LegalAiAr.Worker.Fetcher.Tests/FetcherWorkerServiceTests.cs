using System.Text.Json;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Worker.Fetcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace LegalAiAr.Worker.Fetcher.Tests;

/// <summary>
/// Ola 1 (benchmark): al fallar el Fetcher, el job debe incrementar <see cref="IIngestionJobRepository.IncrementDocumentsFailedAsync"/>.
/// </summary>
public class FetcherWorkerServiceTests
{
    private const string TestQueue = "pipeline-fetcher";
    private const string TestDlqQueue = "pipeline-fetcher-dlq";

    private static FetcherMessage CreateFetcherMessage(Guid pipelineDocId, Guid? ingestionJobId = null) =>
        new(
            DocumentId: pipelineDocId,
            EntityType: EntityType.Ruling,
            SourceId: 1,
            ExternalId: "8180461",
            AnalysisId: "818046",
            IngestionJobId: ingestionJobId,
            UseCache: false,
            Reprocess: false);

    private (FetcherWorkerService Sut, IFetchStrategy Strategy, IQueueReceiver Receiver,
        IQueuePublisher Publisher, IDocumentRepository DocRepo, IIngestionJobRepository JobRepo)
        BuildSut(IReadOnlyList<QueueMessage>? firstBatch = null)
    {
        var receiver = Substitute.For<IQueueReceiver>();
        var publisher = Substitute.For<IQueuePublisher>();
        var strategy = Substitute.For<IFetchStrategy>();
        var resolver = Substitute.For<IStrategyResolver<IFetchStrategy>>();
        var docRepo = Substitute.For<IDocumentRepository>();
        var jobRepo = Substitute.For<IIngestionJobRepository>();
        var stageLogRepo = Substitute.For<IDocumentStageLogRepository>();
        var workerGate = Substitute.For<IWorkerGate>();
        workerGate.WaitIfPausedAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        strategy.FetchAsync(Arg.Any<FetcherMessage>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(new FetchedDocument("hash", "csjn/2026/8180461.pdf"));
        resolver.Resolve(Arg.Any<EntityType>(), Arg.Any<int>()).Returns(strategy);
        docRepo.SetProcessingAsync(Arg.Any<Guid>(), Arg.Any<PipelineStage>(), Arg.Any<CancellationToken>())
            .Returns(true);

        receiver.ReceiveAsync(
                Arg.Any<string>(), Arg.Any<int>(), Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<QueueMessage>>([]));

        if (firstBatch is { Count: > 0 })
        {
            var callCount = 0;
            receiver.ReceiveAsync(
                    Arg.Is(TestQueue), Arg.Any<int>(), Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>())
                .Returns(_ =>
                {
                    callCount++;
                    return Task.FromResult(
                        callCount == 1 ? firstBatch : (IReadOnlyList<QueueMessage>)[]);
                });
        }

        stageLogRepo.LogStageAsync(Arg.Any<DocumentStageLog>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddSingleton(receiver);
        services.AddSingleton(publisher);
        services.AddScoped(_ => resolver);
        services.AddScoped(_ => docRepo);
        services.AddScoped(_ => jobRepo);
        services.AddScoped(_ => stageLogRepo);
        services.AddSingleton<ILogger<FetcherWorkerService>>(Substitute.For<ILogger<FetcherWorkerService>>());
        var provider = services.BuildServiceProvider();

        var sut = new FetcherWorkerService(
            provider.GetRequiredService<IServiceScopeFactory>(),
            provider.GetRequiredService<IQueuePublisher>(),
            provider.GetRequiredService<IQueueReceiver>(),
            new LegalAiAr.Core.Pipeline.PipelineQueueNames("pipeline"),
            provider.GetRequiredService<ILogger<FetcherWorkerService>>(),
            workerGate,
            Microsoft.Extensions.Options.Options.Create(new LegalAiAr.Core.Pipeline.WorkerOptions()));

        return (sut, strategy, receiver, publisher, docRepo, jobRepo);
    }

    [Fact]
    public async Task ExecuteAsync_FetchThrows_WithIngestionJobId_IncrementsDocumentsFailed()
    {
        var docId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var message = CreateFetcherMessage(docId, jobId);
        var body = JsonSerializer.Serialize(message);
        var queueMsg = new QueueMessage("msg-1", "pop-1", body, 1);

        var (sut, strategy, receiver, publisher, docRepo, jobRepo) = BuildSut([queueMsg]);

        strategy.FetchAsync(Arg.Any<FetcherMessage>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("simulated blob failure"));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await sut.StartAsync(cts.Token);
        await Task.Delay(2000, cts.Token);
        await sut.StopAsync(cts.Token);

        await docRepo.Received(1).SetFailedAsync(
            docId, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await jobRepo.Received(1).IncrementDocumentsFailedAsync(jobId, Arg.Any<CancellationToken>());
        await publisher.Received(1).PublishToDlqAsync(
            TestDlqQueue,
            Arg.Is<FetcherMessage>(m => m.DocumentId == docId),
            Arg.Any<Exception>(),
            Arg.Any<CancellationToken>());
        await receiver.Received().DeleteMessageAsync(TestQueue, "msg-1", "pop-1", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_FetchThrows_NoIngestionJobId_DoesNotIncrementJobFailed()
    {
        var docId = Guid.NewGuid();
        var message = CreateFetcherMessage(docId, ingestionJobId: null);
        var body = JsonSerializer.Serialize(message);
        var queueMsg = new QueueMessage("msg-2", "pop-2", body, 1);

        var (sut, strategy, receiver, publisher, docRepo, jobRepo) = BuildSut([queueMsg]);

        strategy.FetchAsync(Arg.Any<FetcherMessage>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("simulated failure"));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await sut.StartAsync(cts.Token);
        await Task.Delay(2000, cts.Token);
        await sut.StopAsync(cts.Token);

        await docRepo.Received(1).SetFailedAsync(docId, Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>());
        await jobRepo.DidNotReceive().IncrementDocumentsFailedAsync(Arg.Any<Guid>(),
            Arg.Any<CancellationToken>());
    }
}
