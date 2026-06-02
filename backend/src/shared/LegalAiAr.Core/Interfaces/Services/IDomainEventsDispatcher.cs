using LegalAiAr.Core.Domain;

namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Deserializes and dispatches a domain event to registered <see cref="IDomainEventHandler{TEvent}"/> instances.
/// </summary>
public interface IDomainEventsDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
