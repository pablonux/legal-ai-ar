using System.Collections.Concurrent;
using Azure;
using Azure.Storage.Queues;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Queue;

/// <summary>
/// Receives messages from Azure Storage Queues.
/// Caches QueueClient instances per queue name and auto-creates queues on first use.
/// </summary>
public class StorageQueueReceiver : IQueueReceiver
{
    private readonly StorageQueueOptions _options;
    private readonly ILogger<StorageQueueReceiver> _logger;
    private readonly IWorkerInfraNotifier? _infraNotifier;
    private readonly ConcurrentDictionary<string, QueueClient> _clients = new();
    private readonly ConcurrentDictionary<string, bool> _ensured = new();

    public StorageQueueReceiver(
        IOptions<StorageQueueOptions> options,
        ILogger<StorageQueueReceiver> logger,
        IWorkerInfraNotifier? infraNotifier = null)
    {
        _options = options.Value;
        _logger = logger;
        _infraNotifier = infraNotifier;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<QueueMessage>> ReceiveAsync(
        string queueName,
        int maxMessages = 1,
        TimeSpan? visibilityTimeout = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await GetOrCreateClientAsync(queueName, cancellationToken);
            var visibility = visibilityTimeout ?? TimeSpan.FromMinutes(5);

            var response = await client.ReceiveMessagesAsync(
                maxMessages: Math.Clamp(maxMessages, 1, 32),
                visibilityTimeout: visibility,
                cancellationToken);

            var messages = response.Value
                .Select(m => new QueueMessage(
                    m.MessageId,
                    m.PopReceipt,
                    m.Body?.ToString() ?? string.Empty,
                    (int)m.DequeueCount))
                .ToList();

            return messages;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogWarning(ex, "Queue receive failed for {Queue}", queueName);
            if (_infraNotifier is not null)
            {
                await _infraNotifier.NotifyInfrastructureIncidentAsync(
                    queueName,
                    ex.ErrorCode,
                    ex.Message,
                    ingestionJobId: null,
                    cancellationToken);
            }

            return Array.Empty<QueueMessage>();
        }
    }

    /// <inheritdoc />
    public async Task DeleteMessageAsync(string queueName, string messageId, string popReceipt, CancellationToken cancellationToken = default)
    {
        var client = await GetOrCreateClientAsync(queueName, cancellationToken);
        try
        {
            await client.DeleteMessageAsync(messageId, popReceipt, cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == "MessageNotFound")
        {
            _logger.LogWarning("DeleteMessage returned MessageNotFound (message already deleted or popReceipt expired). MessageId={MessageId}", messageId);
        }
    }

    private async Task<QueueClient> GetOrCreateClientAsync(string queueName, CancellationToken ct)
    {
        var client = _clients.GetOrAdd(queueName, name => new QueueClient(_options.ConnectionString, name));

        if (_ensured.TryAdd(queueName, true))
        {
            await client.CreateIfNotExistsAsync(cancellationToken: ct);
        }

        return client;
    }
}
