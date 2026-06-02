using LegalAiAr.Core.Domain;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Tests.Outbox;

public class OutboxInfrastructureTests
{
    [Fact]
    public async Task ProcessPendingAsync_dispatches_handler_and_marks_message_processed()
    {
        var dbName = Guid.NewGuid().ToString();
        var handlerState = new PingHandlerState();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(handlerState);
        services.AddScoped<PingDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<PingDomainEvent>>(sp => sp.GetRequiredService<PingDomainEventHandler>());
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();

        await using var provider = services.BuildServiceProvider();
        await using var context = OutboxTestDbContextFactory.Create(dbName);

        var aggregateId = Guid.NewGuid();
        context.TestPingAggregates.Add(TestPingAggregate.Create(aggregateId));
        await context.SaveChangesAsync();

        Assert.Equal(1, await context.OutboxMessages.CountAsync());

        var dispatcher = provider.GetRequiredService<IDomainEventsDispatcher>();
        var processor = new OutboxMessageProcessor(
            context,
            dispatcher,
            Options.Create(new OutboxOptions { BatchSize = 10, MaxRetries = 5 }),
            NullLogger<OutboxMessageProcessor>.Instance);

        var processed = await processor.ProcessPendingAsync();

        Assert.Equal(1, processed);
        Assert.Single(handlerState.HandledAggregateIds);
        Assert.Equal(aggregateId, handlerState.HandledAggregateIds[0]);

        var message = await context.OutboxMessages.SingleAsync();
        Assert.NotNull(message.ProcessedOnUtc);
        Assert.Null(message.Error);
    }
}
