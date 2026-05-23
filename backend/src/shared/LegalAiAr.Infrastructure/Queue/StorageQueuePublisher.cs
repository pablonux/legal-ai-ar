using System.Collections.Concurrent;
using System.Text.Json;
using Azure.Storage.Queues;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Queue;

/// <summary>
/// Publishes messages to Azure Storage Queues.
/// Caches QueueClient instances per queue name and auto-creates queues on first use.
/// </summary>
public class StorageQueuePublisher : IQueuePublisher
{
    private const int MaxErrorMessageLength = 2000;
    private const int MaxConcurrentSends = 10;
    private readonly StorageQueueOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ConcurrentDictionary<string, QueueClient> _clients = new();
    private readonly ConcurrentDictionary<string, bool> _ensured = new();

    public StorageQueuePublisher(IOptions<StorageQueueOptions> options)
    {
        _options = options.Value;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <inheritdoc />
    public async Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class
    {
        var client = await GetOrCreateClientAsync(queueName, cancellationToken);
        var json = JsonSerializer.Serialize(message, _jsonOptions);
        await client.SendMessageAsync(json, cancellationToken);
    }

    /// <inheritdoc />
    public async Task PublishBatchAsync<T>(string queueName, IReadOnlyList<T> messages, CancellationToken cancellationToken = default) where T : class
    {
        if (messages.Count == 0) return;

        var client = await GetOrCreateClientAsync(queueName, cancellationToken);
        using var semaphore = new SemaphoreSlim(MaxConcurrentSends);

        var tasks = messages.Select(async msg =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var json = JsonSerializer.Serialize(msg, _jsonOptions);
                await client.SendMessageAsync(json, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    /// <inheritdoc />
    public async Task PublishRawAsync(string queueName, string rawBody, CancellationToken cancellationToken = default)
    {
        var client = await GetOrCreateClientAsync(queueName, cancellationToken);
        await client.SendMessageAsync(rawBody, cancellationToken);
    }

    /// <inheritdoc />
    public async Task PublishToDlqAsync<T>(string dlqQueueName, T originalMessage, Exception ex, CancellationToken cancellationToken = default) where T : class
    {
        var client = await GetOrCreateClientAsync(dlqQueueName, cancellationToken);

        var errorMessage = Truncate(ex?.Message ?? string.Empty, MaxErrorMessageLength);
        var envelope = new
        {
            originalMessage,
            error = new
            {
                message = errorMessage,
                type = ex?.GetType().Name ?? "Exception",
                timestamp = DateTime.UtcNow.ToString("O")
            }
        };

        var json = JsonSerializer.Serialize(envelope, _jsonOptions);
        await client.SendMessageAsync(json, cancellationToken);
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

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
