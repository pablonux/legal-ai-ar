using LegalAiAr.Application.Admin.Crawlers.Queries.GetCrawlers;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Crawlers.Queries.GetCrawlers;

public class GetCrawlersHandlerTests
{
    [Fact]
    public async Task Handle_WhenSourceIdNull_ReturnsAllCrawlers()
    {
        var configRepo = Substitute.For<ICrawlerConfigRepository>();

        var configs = new List<CrawlerConfig>
        {
            new()
            {
                SourceId = 1,
                IsEnabled = true,
                Source = new Source { Id = 1, Name = "CSJN" }
            },
            new()
            {
                SourceId = 2,
                IsEnabled = false,
                Source = new Source { Id = 2, Name = "SAIJ" }
            }
        };
        configRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(configs);

        var sut = new GetCrawlersHandler(configRepo);
        var query = new GetCrawlersQuery(SourceId: null);

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].SourceId);
        Assert.Equal("CSJN", result[0].SourceName);
        Assert.True(result[0].IsEnabled);
        Assert.Equal(2, result[1].SourceId);
        Assert.Equal("SAIJ", result[1].SourceName);
        Assert.False(result[1].IsEnabled);

        await configRepo.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        await configRepo.DidNotReceive().GetBySourceIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSourceIdProvidedAndExists_ReturnsSingleCrawler()
    {
        var configRepo = Substitute.For<ICrawlerConfigRepository>();

        var config = new CrawlerConfig
        {
            SourceId = 1,
            IsEnabled = true,
            LastCrawledAt = new DateTime(2024, 1, 15),
            LastCrawledStatus = "success",
            LastDocumentCount = 42,
            Source = new Source { Id = 1, Name = "CSJN" }
        };
        configRepo.GetBySourceIdAsync(1, Arg.Any<CancellationToken>()).Returns(config);

        var sut = new GetCrawlersHandler(configRepo);
        var query = new GetCrawlersQuery(SourceId: 1);

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(1, result[0].SourceId);
        Assert.Equal("CSJN", result[0].SourceName);
        Assert.Equal(42, result[0].LastDocumentCount);

        await configRepo.Received(1).GetBySourceIdAsync(1, Arg.Any<CancellationToken>());
        await configRepo.DidNotReceive().GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSourceIdProvidedAndNotFound_ThrowsNotFoundException()
    {
        var configRepo = Substitute.For<ICrawlerConfigRepository>();
        configRepo.GetBySourceIdAsync(99, Arg.Any<CancellationToken>()).Returns((CrawlerConfig?)null);

        var sut = new GetCrawlersHandler(configRepo);
        var query = new GetCrawlersQuery(SourceId: 99);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            sut.Handle(query, CancellationToken.None));

        Assert.Contains("99", ex.Message);
    }
}
