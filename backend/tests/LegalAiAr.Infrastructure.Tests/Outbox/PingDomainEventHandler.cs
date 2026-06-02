using LegalAiAr.Core.Domain;

namespace LegalAiAr.Infrastructure.Tests.Outbox;

internal sealed class PingDomainEventHandler(PingHandlerState state) : IDomainEventHandler<PingDomainEvent>
{
    public Task HandleAsync(PingDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        state.HandledAggregateIds.Add(domainEvent.AggregateId);
        return Task.CompletedTask;
    }
}

internal sealed class PingHandlerState
{
    public List<Guid> HandledAggregateIds { get; } = [];
}
