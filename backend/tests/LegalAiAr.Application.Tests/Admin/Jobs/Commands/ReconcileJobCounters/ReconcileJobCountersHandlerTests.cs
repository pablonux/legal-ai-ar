using LegalAiAr.Application.Admin.Jobs.Commands.ReconcileJobCounters;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Jobs.Commands.ReconcileJobCounters;

public class ReconcileJobCountersHandlerTests
{
    private static readonly Guid JobId = Guid.Parse("24396DE6-74C4-4366-BFF0-EE6CCBA014E8");

    [Fact]
    public async Task Handle_WhenOutstandingDocuments_ThrowsDomainException()
    {
        var jobs = Substitute.For<IIngestionJobRepository>();
        jobs.GetByIdAsync(JobId, Arg.Any<CancellationToken>())
            .Returns(new IngestionJob
            {
                Id = JobId,
                SourceId = 1,
                Status = "processing",
                DocumentsCrawled = 1,
                DocumentsParsed = 1,
                DocumentsEnriched = 1,
                DocumentsPersisted = 1,
                DocumentsIndexed = 0,
                DocumentsFailed = 9,
            });

        var documents = Substitute.For<IDocumentRepository>();
        documents.HasPendingDocumentsAsync(JobId, Arg.Any<CancellationToken>()).Returns(true);

        var sut = new ReconcileJobCountersHandler(jobs, documents);

        await Assert.ThrowsAsync<DomainException>(() =>
            sut.Handle(new ReconcileJobCountersCommand(JobId), CancellationToken.None));

        await jobs.DidNotReceive().ReconcilePipelineCountersFromDocumentsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoOutstanding_ReconcilesAndReturnsSnapshots()
    {
        var jobs = Substitute.For<IIngestionJobRepository>();
        jobs.GetByIdAsync(JobId, Arg.Any<CancellationToken>())
            .Returns(new IngestionJob
            {
                Id = JobId,
                SourceId = 1,
                Status = "processing",
                DocumentsDiscovered = 1819,
                DocumentsSkipped = 0,
                DocumentsCrawled = 1815,
                DocumentsParsed = 1814,
                DocumentsEnriched = 1814,
                DocumentsPersisted = 1813,
                DocumentsIndexed = 1813,
                DocumentsFailed = 9,
            });

        jobs.ReconcilePipelineCountersFromDocumentsAsync(JobId, Arg.Any<CancellationToken>())
            .Returns(new IngestionPipelineCounters(1815, 1814, 1814, 1813, 1813, 6));

        jobs.TryCompleteIfDoneAsync(JobId, Arg.Any<CancellationToken>()).Returns(true);

        var documents = Substitute.For<IDocumentRepository>();
        documents.HasPendingDocumentsAsync(JobId, Arg.Any<CancellationToken>()).Returns(false);

        var sut = new ReconcileJobCountersHandler(jobs, documents);
        var result = await sut.Handle(new ReconcileJobCountersCommand(JobId), CancellationToken.None);

        Assert.Equal(9, result.Previous.DocumentsFailed);
        Assert.Equal(6, result.Updated.DocumentsFailed);
        Assert.True(result.JobCompletionApplied);
        await jobs.Received(1).ReconcilePipelineCountersFromDocumentsAsync(JobId, Arg.Any<CancellationToken>());
        await jobs.Received(1).TryCompleteIfDoneAsync(JobId, Arg.Any<CancellationToken>());
    }
}
