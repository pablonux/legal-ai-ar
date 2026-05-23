using LegalAiAr.Application.Admin.Crawlers.Commands.RunCrawler;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Crawlers.Commands.RunCrawler;

public class RunCrawlerHandlerTests
{

    [Fact]
    public async Task Handle_WhenSourceEnabled_PublishesMessageAndReturnsSuccess()
    {
        var publisher = Substitute.For<IQueuePublisher>();
        var configRepo = Substitute.For<ICrawlerConfigRepository>();

        var config = new CrawlerConfig
        {
            SourceId = 1,
            IsEnabled = true,
            LastCrawledAt = new DateTime(2024, 1, 15),
            Source = new Source { Id = 1, Name = "CSJN" }
        };
        configRepo.GetBySourceIdAsync(1, Arg.Any<CancellationToken>()).Returns(config);

        var sut = new RunCrawlerHandler(publisher, configRepo, new PipelineQueueNames("pipeline"));
        var command = new RunCrawlerCommand(SourceId: 1, DocumentType: "ruling", Type: "incremental", Since: null);

        var result = await sut.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Contains("CSJN", result.Message);

        await publisher.Received(1).PublishAsync(
            Arg.Is<string>(q => q.Contains("discoverer")),
            Arg.Any<LegalAiAr.Core.Messages.DiscovererMessage>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSourceDisabled_ThrowsDomainException()
    {
        var publisher = Substitute.For<IQueuePublisher>();
        var configRepo = Substitute.For<ICrawlerConfigRepository>();

        var config = new CrawlerConfig
        {
            SourceId = 1,
            IsEnabled = false,
            Source = new Source { Id = 1, Name = "CSJN" }
        };
        configRepo.GetBySourceIdAsync(1, Arg.Any<CancellationToken>()).Returns(config);

        var sut = new RunCrawlerHandler(publisher, configRepo, new PipelineQueueNames("pipeline"));
        var command = new RunCrawlerCommand(SourceId: 1, DocumentType: "ruling", Type: "by-range", Since: null, DateFrom: new DateOnly(2024, 1, 1), DateTo: new DateOnly(2024, 1, 31));

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            sut.Handle(command, CancellationToken.None));

        Assert.Contains("disabled", ex.Message);
        await publisher.DidNotReceive().PublishAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSourceNotFound_ThrowsNotFoundException()
    {
        var publisher = Substitute.For<IQueuePublisher>();
        var configRepo = Substitute.For<ICrawlerConfigRepository>();

        configRepo.GetBySourceIdAsync(99, Arg.Any<CancellationToken>()).Returns((CrawlerConfig?)null);

        var sut = new RunCrawlerHandler(publisher, configRepo, new PipelineQueueNames("pipeline"));
        var command = new RunCrawlerCommand(SourceId: 99, DocumentType: "ruling", Type: "by-range", Since: null, DateFrom: new DateOnly(2024, 1, 1), DateTo: new DateOnly(2024, 1, 31));

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            sut.Handle(command, CancellationToken.None));

        Assert.Contains("99", ex.Message);
        await publisher.DidNotReceive().PublishAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenIncrementalWithNoLastCrawledAndNoSince_ThrowsDomainException()
    {
        var publisher = Substitute.For<IQueuePublisher>();
        var configRepo = Substitute.For<ICrawlerConfigRepository>();

        var config = new CrawlerConfig
        {
            SourceId = 1,
            IsEnabled = true,
            LastCrawledAt = null,
            Source = new Source { Id = 1, Name = "CSJN" }
        };
        configRepo.GetBySourceIdAsync(1, Arg.Any<CancellationToken>()).Returns(config);

        var sut = new RunCrawlerHandler(publisher, configRepo, new PipelineQueueNames("pipeline"));
        var command = new RunCrawlerCommand(SourceId: 1, DocumentType: "ruling", Type: "incremental", Since: null);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            sut.Handle(command, CancellationToken.None));

        Assert.Contains("since", ex.Message);
        await publisher.DidNotReceive().PublishAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenIncrementalWithSinceProvided_PublishesWithCorrectSince()
    {
        var publisher = Substitute.For<IQueuePublisher>();
        var configRepo = Substitute.For<ICrawlerConfigRepository>();

        var config = new CrawlerConfig
        {
            SourceId = 1,
            IsEnabled = true,
            LastCrawledAt = null,
            Source = new Source { Id = 1, Name = "CSJN" }
        };
        configRepo.GetBySourceIdAsync(1, Arg.Any<CancellationToken>()).Returns(config);

        var sut = new RunCrawlerHandler(publisher, configRepo, new PipelineQueueNames("pipeline"));
        var since = new DateOnly(2024, 1, 1);
        var command = new RunCrawlerCommand(SourceId: 1, DocumentType: "ruling", Type: "incremental", Since: since);

        var result = await sut.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        await publisher.Received(1).PublishAsync(
            Arg.Any<string>(),
            Arg.Is<LegalAiAr.Core.Messages.DiscovererMessage>(m =>
                m.SourceId == 1 && m.Type == "incremental" && m.Since == since),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenByRangeWithDateFromAndDateTo_PublishesWithCorrectDateRange()
    {
        var publisher = Substitute.For<IQueuePublisher>();
        var configRepo = Substitute.For<ICrawlerConfigRepository>();

        var config = new CrawlerConfig
        {
            SourceId = 1,
            IsEnabled = true,
            LastCrawledAt = new DateTime(2024, 1, 15),
            Source = new Source { Id = 1, Name = "CSJN" }
        };
        configRepo.GetBySourceIdAsync(1, Arg.Any<CancellationToken>()).Returns(config);

        var sut = new RunCrawlerHandler(publisher, configRepo, new PipelineQueueNames("pipeline"));
        var dateFrom = new DateOnly(2024, 3, 1);
        var dateTo = new DateOnly(2024, 3, 31);
        var command = new RunCrawlerCommand(SourceId: 1, DocumentType: "ruling", Type: "by-range", Since: null, DateFrom: dateFrom, DateTo: dateTo);

        var result = await sut.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        await publisher.Received(1).PublishAsync(
            Arg.Any<string>(),
            Arg.Is<LegalAiAr.Core.Messages.DiscovererMessage>(m =>
                m.SourceId == 1 && m.Type == "by-range" && m.DateFrom == dateFrom && m.DateTo == dateTo),
            Arg.Any<CancellationToken>());
    }
}
