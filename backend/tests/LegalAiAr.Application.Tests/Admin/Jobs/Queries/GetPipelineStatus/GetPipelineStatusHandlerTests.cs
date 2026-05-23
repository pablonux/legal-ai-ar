using LegalAiAr.Application.Admin.Jobs.Queries.GetPipelineStatus;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Jobs.Queries.GetPipelineStatus;

public class GetPipelineStatusHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPipelineStatusWithQueueLength()
    {
        var configs = Substitute.For<ICrawlerConfigRepository>();
        var queueMetrics = Substitute.For<IQueueMetricsService>();

        var crawlerConfigs = new List<LegalAiAr.Core.Entities.CrawlerConfig>
        {
            new()
            {
                SourceId = 1,
                LastCrawledAt = new DateTime(2024, 3, 15, 14, 30, 0, DateTimeKind.Utc),
                LastCrawledStatus = "success",
                LastDocumentCount = 42,
                Source = new LegalAiAr.Core.Entities.Source { Id = 1, Name = "CSJN" }
            }
        };
        configs.GetAllAsync(Arg.Any<CancellationToken>()).Returns(crawlerConfigs);
        queueMetrics.GetApproximateMessageCountAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(5);

        var sut = new GetPipelineStatusHandler(configs, queueMetrics, new PipelineQueueNames("pipeline"));
        var query = new GetPipelineStatusQuery();

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.Single(result.Sources);
        Assert.Equal(1, result.Sources[0].SourceId);
        Assert.Equal("CSJN", result.Sources[0].SourceName);
        Assert.Equal(42, result.Sources[0].LastDocumentCount);
        Assert.Equal(5, result.Sources[0].QueueLength);
    }
}
