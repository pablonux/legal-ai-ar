namespace LegalAiAr.Core.Domain;

/// <summary>
/// Aggregate root that collects domain events until <see cref="ClearDomainEvents"/> is called
/// (typically by the outbox interceptor on save).
/// </summary>
public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}
