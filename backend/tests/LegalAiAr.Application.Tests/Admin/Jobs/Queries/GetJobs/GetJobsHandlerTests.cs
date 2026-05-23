using System.Collections.Generic;
using LegalAiAr.Application.Admin.Jobs.Queries.GetJobs;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Jobs.Queries.GetJobs;

public class GetJobsHandlerTests
{
    [Fact]
    public async Task Handle_WhenIngestionJobsExist_ReturnsRealJobs()
    {
        var jobs = Substitute.For<IIngestionJobRepository>();
        var configs = Substitute.For<ICrawlerConfigRepository>();

        var ingestionJobs = new List<IngestionJob>
        {
            new()
            {
                Id = Guid.NewGuid(),
                SourceId = 1,
                Type = "incremental",
                TriggeredBy = "admin",
                StartedAt = new DateTime(2024, 3, 15, 14, 0, 0, DateTimeKind.Utc),
                CompletedAt = new DateTime(2024, 3, 15, 14, 30, 0, DateTimeKind.Utc),
                Status = "completed",
                DocumentsDiscovered = 50,
                DocumentsIndexed = 48,
                DocumentsFailed = 2,
                ErrorSummary = null,
                Source = new Source { Id = 1, Name = "CSJN" }
            }
        };
        jobs.GetAllAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(ingestionJobs);

        var documents = Substitute.For<IDocumentRepository>();
        documents.CountOutstandingDocumentsByJobIdsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, int>());

        var sut = new GetJobsHandler(jobs, configs, documents);
        var query = new GetJobsQuery();

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(ingestionJobs[0].Id.ToString(), result[0].Id);
        Assert.Equal(1, result[0].SourceId);
        Assert.Equal("CSJN", result[0].SourceName);
        Assert.Equal("incremental", result[0].Type);
        Assert.Equal("admin", result[0].TriggeredBy);
        Assert.Equal("completed", result[0].Status);
        Assert.Equal(50, result[0].DocumentsDiscovered);
        Assert.Equal(48, result[0].DocumentsIndexed);
        Assert.Equal(2, result[0].DocumentsFailed);
        Assert.Equal(0, result[0].OutstandingDocuments);
    }

    [Fact]
    public async Task Handle_WhenNoIngestionJobs_FallsBackToSyntheticJobs()
    {
        var jobs = Substitute.For<IIngestionJobRepository>();
        var configs = Substitute.For<ICrawlerConfigRepository>();

        jobs.GetAllAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new List<IngestionJob>());

        var documents = Substitute.For<IDocumentRepository>();

        var crawlerConfigs = new List<CrawlerConfig>
        {
            new()
            {
                SourceId = 1,
                LastCrawledAt = new DateTime(2024, 3, 15, 14, 30, 0, DateTimeKind.Utc),
                LastCrawledStatus = "success",
                LastDocumentCount = 42,
                Source = new Source { Id = 1, Name = "CSJN" }
            }
        };
        configs.GetAllAsync(Arg.Any<CancellationToken>()).Returns(crawlerConfigs);

        var sut = new GetJobsHandler(jobs, configs, documents);
        var query = new GetJobsQuery();

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("synthetic-1", result[0].Id);
        Assert.Equal(1, result[0].SourceId);
        Assert.Equal("CSJN", result[0].SourceName);
        Assert.Equal("incremental", result[0].Type);
        Assert.Equal("admin", result[0].TriggeredBy);
        Assert.Equal("success", result[0].Status);
        Assert.Equal(42, result[0].DocumentsIndexed);
    }

    [Fact]
    public async Task Handle_WhenNoIngestionJobsAndNoLastCrawled_ReturnsEmpty()
    {
        var jobs = Substitute.For<IIngestionJobRepository>();
        var configs = Substitute.For<ICrawlerConfigRepository>();

        jobs.GetAllAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new List<IngestionJob>());
        var crawlerConfigs = new List<CrawlerConfig>
        {
            new()
            {
                SourceId = 1,
                LastCrawledAt = null,
                Source = new Source { Id = 1, Name = "CSJN" }
            }
        };
        configs.GetAllAsync(Arg.Any<CancellationToken>()).Returns(crawlerConfigs);

        var documents = Substitute.For<IDocumentRepository>();
        var sut = new GetJobsHandler(jobs, configs, documents);
        var query = new GetJobsQuery();

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }
}
