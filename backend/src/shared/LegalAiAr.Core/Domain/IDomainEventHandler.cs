namespace LegalAiAr.Core.Domain;

/// <summary>
/// Handles a single domain event type after it is read from the outbox.
/// Handlers must be idempotent — the same event may be delivered more than once.
/// </summary>
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
