namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Receives messages from Azure Storage Queue.
/// Used by workers to consume pipeline messages.
/// </summary>
public interface IQueueReceiver
{
    /// <summary>
    /// Receives up to the specified number of messages from the queue.
    /// Messages are invisible to other consumers for the visibility timeout.
    /// </summary>
    /// <param name="queueName">Name of the queue (e.g. "queue-crawler", "queue-parser").</param>
    /// <param name="maxMessages">Maximum number of messages to receive (1-32 for Storage Queues).</param>
    /// <param name="visibilityTimeout">How long the message is invisible after receipt. Default 5 minutes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Received messages with their receipt handles for deletion.</returns>
    Task<IReadOnlyList<QueueMessage>> ReceiveAsync(
        string queueName,
        int maxMessages = 1,
        TimeSpan? visibilityTimeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a message after successful processing.
    /// </summary>
    Task DeleteMessageAsync(string queueName, string messageId, string popReceipt, CancellationToken cancellationToken = default);
}

/// <summary>
/// A message received from the queue.
/// </summary>
public record QueueMessage(
    string MessageId,
    string PopReceipt,
    string Body,
    int DequeueCount);
