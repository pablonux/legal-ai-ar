using System.Text.Json;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Core.Pipeline;
using LegalAiAr.Infrastructure.Control;
using LegalAiAr.Worker.Parser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace LegalAiAr.Worker.Parser.Tests;

public class ParserWorkerServiceTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>Must match <see cref="PipelineQueueNames"/> prefix used in <see cref="BuildSut"/>.</summary>
    private static readonly PipelineQueueNames TestQueueNames = new("pipeline");

    private static string TestQueue => TestQueueNames.Parser;
    private static string TestDlqQueue => TestQueueNames.ParserDlq;

    private static ParserMessage CreateParserMessage(Guid? pipelineDocumentId = null) =>
        new(
            SourceId: 1,
            DocumentId: "8048522",
            AnalysisId: "804852",
            BlobPathPdf: "csjn/2024/8048522.pdf",
            ContentHash: "abc123",
            ApiMetadata: null,
            PipelineDocumentId: pipelineDocumentId,
            EntityType: EntityType.Ruling);

    private static EnrichmentMessage CreateEnrichmentMessage() =>
        new(
            DocumentId: "8048522",
            SourceId: 1,
            NormalizedText: "",
            ExtractedMetadata: new ExtractedMetadata(
                "Test Case", default, null, null, null, null, null,
                null, null, [], []),
            MissingFields: []);

    private (ParserWorkerService Sut, IParseStrategy Strategy, IQueueReceiver Receiver,
        IQueuePublisher Publisher, IDocumentRepository DocRepo, IIngestionJobRepository JobRepo)
        BuildSut(IReadOnlyList<QueueMessage>? firstBatch = null)
    {
        var receiver = Substitute.For<IQueueReceiver>();
        var publisher = Substitute.For<IQueuePublisher>();
        var strategy = Substitute.For<IParseStrategy>();
        var resolver = Substitute.For<IStrategyResolver<IParseStrategy>>();
        var docRepo = Substitute.For<IDocumentRepository>();
        var jobRepo = Substitute.For<IIngestionJobRepository>();
        var stageLogRepo = Substitute.For<IDocumentStageLogRepository>();

        var workerGate = Substitute.For<IWorkerGate>();
        workerGate.WaitIfPausedAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        docRepo.ResetProcessingToPendingAsync(Arg.Any<PipelineStage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(0));
        stageLogRepo.LogStageAsync(Arg.Any<DocumentStageLog>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        docRepo.AdvanceStageAsync(Arg.Any<Guid>(), Arg.Any<PipelineStage>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        docRepo.SetFailedAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        jobRepo.IncrementDocumentsParsedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        jobRepo.IncrementDocumentsFailedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        publisher.PublishAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        publisher.PublishToDlqAsync(
                Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Exception>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        publisher.PublishRawAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        strategy.ParseAsync(Arg.Any<ParserMessage>(), Arg.Any<CancellationToken>())
            .Returns(new ParseResult(CreateEnrichmentMessage()));
        resolver.Resolve(Arg.Any<EntityType>(), Arg.Any<int>()).Returns(strategy);
        docRepo.SetProcessingAsync(Arg.Any<Guid>(), Arg.Any<PipelineStage>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Default: all queues return empty
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

        var services = new ServiceCollection();
        services.AddSingleton(receiver);
        services.AddSingleton(publisher);
        services.AddScoped(_ => resolver);
        services.AddScoped(_ => docRepo);
        services.AddScoped(_ => jobRepo);
        services.AddScoped(_ => stageLogRepo);
        services.AddSingleton<ILogger<ParserWorkerService>>(Substitute.For<ILogger<ParserWorkerService>>());
        var provider = services.BuildServiceProvider();

        var sut = new ParserWorkerService(
            provider.GetRequiredService<IServiceScopeFactory>(),
            provider.GetRequiredService<IQueuePublisher>(),
            provider.GetRequiredService<IQueueReceiver>(),
            new LegalAiAr.Core.Pipeline.PipelineQueueNames("pipeline"),
            provider.GetRequiredService<ILogger<ParserWorkerService>>(),
            workerGate,
            Microsoft.Extensions.Options.Options.Create(new LegalAiAr.Core.Pipeline.WorkerOptions()));

        return (sut, strategy, receiver, publisher, docRepo, jobRepo);
    }

    [Fact]
    public async Task ExecuteAsync_ValidMessage_CallsStrategyAndDeletesMessage()
    {
        var docId = Guid.NewGuid();
        var message = CreateParserMessage(docId);
        var body = JsonSerializer.Serialize(message, JsonOptions);
        var queueMsg = new QueueMessage("msg-1", "pop-1", body, 1);

        var (sut, strategy, receiver, _, docRepo, _) = BuildSut([queueMsg]);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await sut.StartAsync(cts.Token);
        await Task.Delay(2000, cts.Token);
        await sut.StopAsync(cts.Token);

        await strategy.Received(1).ParseAsync(
            Arg.Is<ParserMessage>(m => m.DocumentId == "8048522"),
            Arg.Any<CancellationToken>());

        await receiver.Received().DeleteMessageAsync(
            TestQueue, "msg-1", "pop-1", Arg.Any<CancellationToken>());

        await docRepo.Received(1).AdvanceStageAsync(
            docId, PipelineStage.Enricher, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_InvalidJson_PublishesToDlqAndDeletesMessage()
    {
        var queueMsg = new QueueMessage("msg-1", "pop-1", "{ invalid json }", 1);
        var (sut, strategy, receiver, publisher, _, _) = BuildSut([queueMsg]);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await sut.StartAsync(cts.Token);
        await Task.Delay(2000, cts.Token);
        await sut.StopAsync(cts.Token);

        await strategy.DidNotReceive().ParseAsync(
            Arg.Any<ParserMessage>(), Arg.Any<CancellationToken>());

        await receiver.Received().DeleteMessageAsync(
            TestQueue, "msg-1", "pop-1", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_StrategyThrows_SetsDocumentFailedAndPublishesToDlq()
    {
        var docId = Guid.NewGuid();
        var message = CreateParserMessage(docId);
        var body = JsonSerializer.Serialize(message, JsonOptions);
        var queueMsg = new QueueMessage("msg-1", "pop-1", body, 1);

        var (sut, strategy, receiver, publisher, docRepo, _) = BuildSut([queueMsg]);

        strategy.ParseAsync(Arg.Any<ParserMessage>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Simulated failure"));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await sut.StartAsync(cts.Token);
        await Task.Delay(2000, cts.Token);
        await sut.StopAsync(cts.Token);

        await publisher.Received(1).PublishToDlqAsync(
            TestDlqQueue,
            Arg.Is<ParserMessage>(m => m.DocumentId == "8048522"),
            Arg.Any<Exception>(),
            Arg.Any<CancellationToken>());

        await receiver.Received().DeleteMessageAsync(
            TestQueue, "msg-1", "pop-1", Arg.Any<CancellationToken>());

        await docRepo.Received(1).SetFailedAsync(
            docId, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_MaxDequeueExceeded_MovesToDlqWithoutProcessing()
    {
        var message = CreateParserMessage();
        var body = JsonSerializer.Serialize(message, JsonOptions);
        var queueMsg = new QueueMessage("msg-1", "pop-1", body, DequeueCount: 5);

        var (sut, strategy, receiver, publisher, _, _) = BuildSut([queueMsg]);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await sut.StartAsync(cts.Token);
        await Task.Delay(2000, cts.Token);
        await sut.StopAsync(cts.Token);

        await strategy.DidNotReceive().ParseAsync(
            Arg.Any<ParserMessage>(), Arg.Any<CancellationToken>());

        await publisher.Received(1).PublishRawAsync(
            TestDlqQueue, body, Arg.Any<CancellationToken>());

        await receiver.Received().DeleteMessageAsync(
            TestQueue, "msg-1", "pop-1", Arg.Any<CancellationToken>());
    }
}
