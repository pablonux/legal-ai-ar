using LegalAiAr.Application.Admin.Jobs.Commands.RewindParserFailedToFetcher;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;
using NSubstitute;

namespace LegalAiAr.Application.Tests.Admin.Jobs.Commands.RewindParserFailedToFetcher;

public sealed class RewindParserFailedToFetcherHandlerTests
{
    [Fact]
    public async Task Handle_RewindsAndPublishesFetcher_WhenDocumentsReturned()
    {
        var jobId = Guid.NewGuid();
        var docId = Guid.NewGuid();
        var jobs = Substitute.For<IIngestionJobRepository>();
        jobs.GetByIdAsync(jobId, Arg.Any<CancellationToken>())
            .Returns(new IngestionJob { Id = jobId });

        var doc = new Document
        {
            Id = docId,
            IngestionJobId = jobId,
            EntityType = EntityType.Ruling,
            SourceId = 1,
            ExternalId = "7962861",
            AnalysisId = "a1",
            CurrentStage = PipelineStage.Fetcher,
            Status = DocumentStatus.Pending,
        };

        var documents = Substitute.For<IDocumentRepository>();
        documents.TakeParserFailedRewindToFetcherPendingAsync(
                jobId,
                true,
                null,
                null,
                5000,
                Arg.Any<CancellationToken>())
            .Returns([doc]);

        var publisher = Substitute.For<IQueuePublisher>();
        var queueNames = new PipelineQueueNames("pipeline");

        var sut = new RewindParserFailedToFetcherHandler(jobs, documents, publisher, queueNames);

        var result = await sut.Handle(new RewindParserFailedToFetcherCommand(jobId), CancellationToken.None);

        Assert.Equal(1, result.DocumentsRewound);
        Assert.Equal(1, result.MessagesPublished);
        await jobs.Received(1).DecrementDocumentsFailedAsync(jobId, 1, Arg.Any<CancellationToken>());
        await jobs.Received(1).ResumeProcessingIfTerminalAsync(jobId, Arg.Any<CancellationToken>());
        await publisher.Received(1).PublishAsync(
            queueNames.Fetcher,
            Arg.Is<FetcherMessage>(m =>
                m.DocumentId == docId && m.SourceId == 1 && m.ExternalId == "7962861" && m.AnalysisId == "a1"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ThrowsDomain_WhenMaxDocumentsInvalid()
    {
        var jobId = Guid.NewGuid();
        var jobs = Substitute.For<IIngestionJobRepository>();
        jobs.GetByIdAsync(jobId, Arg.Any<CancellationToken>())
            .Returns(new IngestionJob { Id = jobId });
        var sut = new RewindParserFailedToFetcherHandler(
            jobs,
            Substitute.For<IDocumentRepository>(),
            Substitute.For<IQueuePublisher>(),
            new PipelineQueueNames("pipeline"));

        await Assert.ThrowsAsync<LegalAiAr.Core.Exceptions.DomainException>(() =>
            sut.Handle(new RewindParserFailedToFetcherCommand(jobId, MaxDocuments: 0), CancellationToken.None));
    }
}
