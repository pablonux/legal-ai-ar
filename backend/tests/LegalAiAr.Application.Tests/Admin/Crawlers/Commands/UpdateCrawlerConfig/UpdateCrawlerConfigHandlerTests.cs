using LegalAiAr.Application.Admin.Crawlers.Commands.UpdateCrawlerConfig;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Crawlers.Commands.UpdateCrawlerConfig;

public class UpdateCrawlerConfigHandlerTests
{
    [Fact]
    public async Task Handle_WhenConfigExists_UpdatesAndReturnsDto()
    {
        var configRepo = Substitute.For<ICrawlerConfigRepository>();

        var config = new CrawlerConfig
        {
            SourceId = 1,
            IsEnabled = false,
            LastCrawledAt = new DateTime(2024, 1, 15),
            LastCrawledStatus = "success",
            LastDocumentCount = 42,
            Source = new Source { Id = 1, Name = "CSJN" }
        };
        configRepo.GetBySourceIdAsync(1, Arg.Any<CancellationToken>()).Returns(config);

        var updatedConfig = new CrawlerConfig
        {
            SourceId = 1,
            IsEnabled = true,
            LastCrawledAt = new DateTime(2024, 1, 15),
            LastCrawledStatus = "success",
            LastDocumentCount = 42,
            Source = new Source { Id = 1, Name = "CSJN" }
        };
        configRepo.GetBySourceIdAsync(1, Arg.Any<CancellationToken>()).Returns(config, updatedConfig);

        var sut = new UpdateCrawlerConfigHandler(configRepo);
        var command = new UpdateCrawlerConfigCommand(SourceId: 1, IsEnabled: true);

        var result = await sut.Handle(command, CancellationToken.None);

        Assert.Equal(1, result.SourceId);
        Assert.Equal("CSJN", result.SourceName);
        Assert.True(result.IsEnabled);
        Assert.Equal(42, result.LastDocumentCount);

        await configRepo.Received(1).UpdateIsEnabledAsync(1, true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenConfigNotFound_ThrowsNotFoundException()
    {
        var configRepo = Substitute.For<ICrawlerConfigRepository>();
        configRepo.GetBySourceIdAsync(99, Arg.Any<CancellationToken>()).Returns((CrawlerConfig?)null);

        var sut = new UpdateCrawlerConfigHandler(configRepo);
        var command = new UpdateCrawlerConfigCommand(SourceId: 99, IsEnabled: true);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            sut.Handle(command, CancellationToken.None));

        Assert.Contains("99", ex.Message);
        await configRepo.DidNotReceive().UpdateIsEnabledAsync(Arg.Any<int>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDisabling_UpdatesToFalse()
    {
        var configRepo = Substitute.For<ICrawlerConfigRepository>();

        var config = new CrawlerConfig
        {
            SourceId = 1,
            IsEnabled = true,
            Source = new Source { Id = 1, Name = "CSJN" }
        };
        var updatedConfig = new CrawlerConfig
        {
            SourceId = 1,
            IsEnabled = false,
            Source = new Source { Id = 1, Name = "CSJN" }
        };
        configRepo.GetBySourceIdAsync(1, Arg.Any<CancellationToken>()).Returns(config, updatedConfig);

        var sut = new UpdateCrawlerConfigHandler(configRepo);
        var command = new UpdateCrawlerConfigCommand(SourceId: 1, IsEnabled: false);

        var result = await sut.Handle(command, CancellationToken.None);

        Assert.False(result.IsEnabled);
        await configRepo.Received(1).UpdateIsEnabledAsync(1, false, Arg.Any<CancellationToken>());
    }
}
