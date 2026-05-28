using LegalAiAr.Application.Admin.Dlq.Queries.GetDlqMessages;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Services;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Dlq.Queries.GetDlqMessages;

public class GetDlqMessagesHandlerTests
{
    [Fact]
    public async Task Handle_WhenValidQueue_ReturnsPeekResult()
    {
        var dlqService = Substitute.For<IDlqService>();
        dlqService.ValidQueueNames.Returns(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "crawler", "parser", "enrichment", "indexer" });

        var expected = new DlqPeekResult(
            "parser",
            2,
            new List<DlqMessageInfo>
            {
                new("msg-1", DateTimeOffset.UtcNow, 3, "{\"documentId\":\"123\"...}"),
                new("msg-2", DateTimeOffset.UtcNow, 3, "{\"documentId\":\"456\"...}")
            });
        dlqService.PeekMessagesAsync("parser", 32, Arg.Any<CancellationToken>()).Returns(expected);

        var sut = new GetDlqMessagesHandler(dlqService);
        var query = new GetDlqMessagesQuery("parser", 32);

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.Equal("parser", result.Queue);
        Assert.Equal(2, result.MessageCount);
        Assert.Equal(2, result.Messages.Count);

        await dlqService.Received(1).PeekMessagesAsync("parser", 32, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInvalidQueue_ThrowsDomainException()
    {
        var dlqService = Substitute.For<IDlqService>();
        dlqService.ValidQueueNames.Returns(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "crawler", "parser", "enrichment", "indexer" });

        var sut = new GetDlqMessagesHandler(dlqService);
        var query = new GetDlqMessagesQuery("invalid");

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            sut.Handle(query, CancellationToken.None));

        Assert.Contains("Invalid queue", ex.Message);
        await dlqService.DidNotReceive().PeekMessagesAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }
}
