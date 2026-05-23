using LegalAiAr.Application.Admin.Dlq.Commands.RequeueMessage;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Services;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Dlq.Commands.RequeueMessage;

public class RequeueMessageHandlerTests
{
    [Fact]
    public async Task Handle_WhenMessageFound_ReturnsSuccess()
    {
        var dlqService = Substitute.For<IDlqService>();
        dlqService.ValidQueueNames.Returns(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "crawler", "parser", "enrichment", "indexer" });

        var expected = new RequeueResult(true, "Message requeued to pipeline-parser");
        dlqService.RequeueMessageAsync("parser", "msg-123", Arg.Any<CancellationToken>()).Returns(expected);

        var sut = new RequeueMessageHandler(dlqService);
        var command = new RequeueMessageCommand("parser", "msg-123");

        var result = await sut.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Contains("requeued", result.Message);

        await dlqService.Received(1).RequeueMessageAsync("parser", "msg-123", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenMessageNotFound_ThrowsNotFoundException()
    {
        var dlqService = Substitute.For<IDlqService>();
        dlqService.ValidQueueNames.Returns(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "crawler", "parser", "enrichment", "indexer" });

        dlqService.RequeueMessageAsync("parser", "nonexistent", Arg.Any<CancellationToken>())
            .Returns(Task.FromException<RequeueResult>(new NotFoundException("Message nonexistent not found in DLQ pipeline-parser-dlq.")));

        var sut = new RequeueMessageHandler(dlqService);
        var command = new RequeueMessageCommand("parser", "nonexistent");

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            sut.Handle(command, CancellationToken.None));

        Assert.Contains("nonexistent", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenInvalidQueue_ThrowsDomainException()
    {
        var dlqService = Substitute.For<IDlqService>();
        dlqService.ValidQueueNames.Returns(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "crawler", "parser", "enrichment", "indexer" });

        var sut = new RequeueMessageHandler(dlqService);
        var command = new RequeueMessageCommand("invalid", "msg-123");

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            sut.Handle(command, CancellationToken.None));

        Assert.Contains("Invalid queue", ex.Message);
        await dlqService.DidNotReceive().RequeueMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
