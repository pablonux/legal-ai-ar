using System.Text.Json;
using LegalAiAr.Core.Domain;
using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LegalAiAr.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Persists aggregate domain events to <see cref="OutboxMessage"/> rows in the same database transaction.
/// </summary>
public sealed class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        PersistDomainEvents(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        PersistDomainEvents(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void PersistDomainEvents(DbContext? context)
    {
        if (context is null)
            return;

        var aggregateEntries = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .ToList();

        if (aggregateEntries.Count == 0)
            return;

        var now = DateTime.UtcNow;

        foreach (var entry in aggregateEntries)
        {
            var aggregate = entry.Entity;
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                var eventType = domainEvent.GetType();
                context.Set<OutboxMessage>().Add(new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    EventType = eventType.AssemblyQualifiedName ?? eventType.FullName ?? eventType.Name,
                    Payload = JsonSerializer.Serialize(domainEvent, eventType, SerializerOptions),
                    OccurredOnUtc = now,
                });
            }

            aggregate.ClearDomainEvents();
        }
    }
}
