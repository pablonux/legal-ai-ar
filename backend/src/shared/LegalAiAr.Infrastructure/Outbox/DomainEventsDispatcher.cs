using System.Text.Json;
using LegalAiAr.Core.Domain;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Outbox;

public sealed class DomainEventsDispatcher : IDomainEventsDispatcher
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventsDispatcher> _logger;

    public DomainEventsDispatcher(
        IServiceProvider serviceProvider,
        ILogger<DomainEventsDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var eventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
        var handlers = _serviceProvider.GetServices(handlerType).ToList();

        if (handlers.Count == 0)
        {
            _logger.LogWarning(
                "No {HandlerType} registered for domain event {EventType}",
                handlerType.Name,
                eventType.Name);
            return;
        }

        foreach (var handler in handlers)
        {
            if (handler is null)
                continue;

            var method = handler.GetType().GetMethod(
                "HandleAsync",
                [eventType, typeof(CancellationToken)]);

            if (method is null)
                continue;

            var task = method.Invoke(handler, [domainEvent, cancellationToken]) as Task;
            if (task is not null)
                await task.ConfigureAwait(false);
        }
    }

    public static IDomainEvent? Deserialize(string eventTypeName, string payload)
    {
        var eventType = Type.GetType(eventTypeName, throwOnError: false);
        if (eventType is null || !typeof(IDomainEvent).IsAssignableFrom(eventType))
            return null;

        return (IDomainEvent?)JsonSerializer.Deserialize(payload, eventType, SerializerOptions);
    }
}
