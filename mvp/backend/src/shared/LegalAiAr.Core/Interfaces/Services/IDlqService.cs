namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Service for admin operations on Dead Letter Queues (Azure Storage Queues).
/// DLQ queues follow {prefix}-{stage}-dlq convention (e.g. pipeline-parser-dlq).
/// </summary>
public interface IDlqService
{
    /// <summary>
    /// Valid queue names: crawler, parser, enrichment, indexer.
    /// </summary>
    IReadOnlySet<string> ValidQueueNames { get; }

    /// <summary>
    /// Peeks messages from a DLQ without removing them. Max 32 messages.
    /// </summary>
    /// <param name="queueName">Queue name without prefix: crawler, parser, enrichment, or indexer.</param>
    /// <param name="maxMessages">Max messages to peek (1-32).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<DlqPeekResult> PeekMessagesAsync(
        string queueName,
        int maxMessages = 32,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Requeues a message from DLQ to the origin queue. Receives, publishes to origin, deletes from DLQ.
    /// </summary>
    /// <param name="queueName">Queue name: crawler, parser, enrichment, or indexer.</param>
    /// <param name="messageId">Message ID as returned by PeekMessagesAsync.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<RequeueResult> RequeueMessageAsync(
        string queueName,
        string messageId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of peeking DLQ messages.
/// </summary>
/// <param name="Queue">Queue name (crawler, parser, etc.).</param>
/// <param name="MessageCount">Total count of messages peeked.</param>
/// <param name="Messages">Peeked messages with id, insertedOn, dequeueCount, bodyPreview.</param>
public record DlqPeekResult(
    string Queue,
    int MessageCount,
    IReadOnlyList<DlqMessageInfo> Messages);

/// <summary>
/// DLQ message info for admin display.
/// </summary>
/// <param name="Id">Azure Storage Queue message ID.</param>
/// <param name="InsertedOn">When the message was added.</param>
/// <param name="DequeueCount">Delivery attempts.</param>
/// <param name="BodyPreview">First 200 chars of message body.</param>
/// <param name="Error">Error info when message uses envelope format; null for legacy messages.</param>
public record DlqMessageInfo(
    string Id,
    DateTimeOffset InsertedOn,
    int DequeueCount,
    string BodyPreview,
    DlqMessageErrorInfo? Error = null);

/// <summary>
/// Error metadata from DLQ envelope.
/// </summary>
/// <param name="Message">Exception message.</param>
/// <param name="Type">Exception type name.</param>
public record DlqMessageErrorInfo(string Message, string Type);

/// <summary>
/// Result of requeuing a message.
/// </summary>
/// <param name="Success">Whether the requeue succeeded.</param>
/// <param name="Message">Human-readable message.</param>
public record RequeueResult(bool Success, string Message);
