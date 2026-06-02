using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Outbox;

public sealed class OutboxMessageProcessor : IOutboxMessageProcessor
{
    private readonly AppDbContext _db;
    private readonly IDomainEventsDispatcher _dispatcher;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxMessageProcessor> _logger;

    public OutboxMessageProcessor(
        AppDbContext db,
        IDomainEventsDispatcher dispatcher,
        IOptions<OutboxOptions> options,
        ILogger<OutboxMessageProcessor> logger)
    {
        _db = db;
        _dispatcher = dispatcher;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<int> ProcessPendingAsync(CancellationToken cancellationToken = default)
    {
        var batch = await _db.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null && m.RetryCount < _options.MaxRetries)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(_options.BatchSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var processed = 0;

        foreach (var message in batch)
        {
            if (await TryProcessMessageAsync(message, cancellationToken).ConfigureAwait(false))
                processed++;
        }

        if (processed > 0)
            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return processed;
    }

    private async Task<bool> TryProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var domainEvent = DomainEventsDispatcher.Deserialize(message.EventType, message.Payload);
            if (domainEvent is null)
            {
                await RecordFailureAsync(
                    message,
                    $"Unable to deserialize event type '{message.EventType}'.",
                    cancellationToken).ConfigureAwait(false);
                return false;
            }

            await _dispatcher.DispatchAsync(domainEvent, cancellationToken).ConfigureAwait(false);
            message.ProcessedOnUtc = DateTime.UtcNow;
            message.Error = null;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Outbox message {OutboxMessageId} ({EventType}) dispatch failed (retry {RetryCount})",
                message.Id,
                message.EventType,
                message.RetryCount);

            await RecordFailureAsync(message, ex.Message, cancellationToken).ConfigureAwait(false);
            return false;
        }
    }

    private Task RecordFailureAsync(OutboxMessage message, string error, CancellationToken cancellationToken)
    {
        message.RetryCount++;
        message.Error = error.Length > 4000 ? error[..4000] : error;
        return Task.CompletedTask;
    }
}
