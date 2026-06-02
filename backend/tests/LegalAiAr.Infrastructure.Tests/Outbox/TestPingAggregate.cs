using LegalAiAr.Core.Domain;

namespace LegalAiAr.Infrastructure.Tests.Outbox;

internal sealed class TestPingAggregate : AggregateRoot
{
    public Guid Id { get; private set; }

    public static TestPingAggregate Create(Guid id)
    {
        var aggregate = new TestPingAggregate { Id = id };
        aggregate.RaiseDomainEvent(new PingDomainEvent(id));
        return aggregate;
    }
}

internal sealed record PingDomainEvent(Guid AggregateId) : IDomainEvent;
