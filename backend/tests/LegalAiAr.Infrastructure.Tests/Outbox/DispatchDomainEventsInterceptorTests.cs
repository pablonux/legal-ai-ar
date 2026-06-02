using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Tests.Outbox;

public class DispatchDomainEventsInterceptorTests
{
    [Fact]
    public async Task SaveChanges_persists_outbox_row_and_clears_aggregate_events()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = OutboxTestDbContextFactory.Create(dbName);
        var aggregateId = Guid.NewGuid();
        var aggregate = TestPingAggregate.Create(aggregateId);

        context.TestPingAggregates.Add(aggregate);
        await context.SaveChangesAsync();

        Assert.Empty(aggregate.DomainEvents);

        var outboxRows = await context.OutboxMessages.ToListAsync();
        Assert.Single(outboxRows);
        Assert.Contains(aggregateId.ToString(), outboxRows[0].Payload, StringComparison.Ordinal);
        Assert.Null(outboxRows[0].ProcessedOnUtc);
    }
}
