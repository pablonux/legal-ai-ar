namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Publishes messages to Azure Storage Queues.
/// </summary>
public interface IQueuePublisher
{
    /// <summary>
    /// Publishes a single message to the specified queue.
    /// </summary>
    Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publishes multiple messages to the specified queue.
    /// More efficient than calling PublishAsync in a loop: uses concurrent sends.
    /// </summary>
    Task PublishBatchAsync<T>(string queueName, IReadOnlyList<T> messages, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publishes a raw message body without serialization. Used for requeuing from DLQ.
    /// </summary>
    Task PublishRawAsync(string queueName, string rawBody, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a message to a DLQ with an envelope containing the original message and error metadata.
    /// </summary>
    Task PublishToDlqAsync<T>(string dlqQueueName, T originalMessage, Exception ex, CancellationToken cancellationToken = default) where T : class;
}
