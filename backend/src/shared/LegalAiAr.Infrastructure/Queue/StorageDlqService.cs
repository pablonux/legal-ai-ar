using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Pipeline;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Queue;

/// <summary>
/// DLQ operations for Azure Storage Queues.
/// Queue names are derived from <see cref="PipelineQueueNames"/>.
/// </summary>
public class StorageDlqService : IDlqService
{
    private const int BodyPreviewMaxLength = 200;
    private const int MinVisibilityTimeoutSeconds = 1;

    private static readonly IReadOnlySet<string> ValidNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { "discoverer", "fetcher", "parser", "enricher", "persister", "indexer", "crawler", "enrichment" };

    public IReadOnlySet<string> ValidQueueNames => ValidNames;

    private readonly StorageQueueOptions _options;
    private readonly PipelineQueueNames _queueNames;
    private readonly IQueuePublisher _publisher;
    private readonly IQueueReceiver _receiver;

    public StorageDlqService(
        IOptions<StorageQueueOptions> options,
        PipelineQueueNames queueNames,
        IQueuePublisher publisher,
        IQueueReceiver receiver)
    {
        _options = options.Value;
        _queueNames = queueNames;
        _publisher = publisher;
        _receiver = receiver;
    }

    /// <inheritdoc />
    public async Task<DlqPeekResult> PeekMessagesAsync(
        string queueName,
        int maxMessages = 32,
        CancellationToken cancellationToken = default)
    {
        var dlqName = GetDlqQueueName(queueName);
        var client = new QueueClient(_options.ConnectionString, dlqName);
        await client.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var response = await client.PeekMessagesAsync(
            maxMessages: Math.Clamp(maxMessages, 1, 32),
            cancellationToken);

        var messages = response.Value
            .Select(m => CreateDlqMessageInfo(m))
            .ToList();

        return new DlqPeekResult(
            Queue: queueName.ToLowerInvariant(),
            MessageCount: messages.Count,
            Messages: messages);
    }

    /// <inheritdoc />
    public async Task<RequeueResult> RequeueMessageAsync(
        string queueName,
        string messageId,
        CancellationToken cancellationToken = default)
    {
        var dlqName = GetDlqQueueName(queueName);
        var originName = GetOriginQueueName(queueName);

        var messages = await _receiver.ReceiveAsync(
            dlqName,
            maxMessages: 32,
            visibilityTimeout: TimeSpan.FromSeconds(MinVisibilityTimeoutSeconds),
            cancellationToken);

        var match = messages.FirstOrDefault(m =>
            string.Equals(m.MessageId, messageId, StringComparison.Ordinal));

        if (match is null)
            throw new NotFoundException($"Message {messageId} not found in DLQ {dlqName}.");

        var bodyToPublish = ExtractOriginalMessageBody(match.Body);
        await _publisher.PublishRawAsync(originName, bodyToPublish, cancellationToken);
        await _receiver.DeleteMessageAsync(dlqName, match.MessageId, match.PopReceipt, cancellationToken);

        return new RequeueResult(
            Success: true,
            Message: $"Message requeued to {originName}");
    }

    private string GetDlqQueueName(string queueName) =>
        _queueNames.DlqFor(queueName.ToLowerInvariant());

    private string GetOriginQueueName(string queueName) =>
        _queueNames.QueueFor(queueName.ToLowerInvariant());

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength];

    private static DlqMessageInfo CreateDlqMessageInfo(PeekedMessage m)
    {
        var body = m.Body?.ToString() ?? string.Empty;
        var bodyPreview = Truncate(body, BodyPreviewMaxLength);
        var error = TryParseErrorFromBody(body);
        return new DlqMessageInfo(
            Id: m.MessageId,
            InsertedOn: m.InsertedOn ?? DateTimeOffset.MinValue,
            DequeueCount: (int)m.DequeueCount,
            BodyPreview: bodyPreview,
            Error: error);
    }

    private static DlqMessageErrorInfo? TryParseErrorFromBody(string body)
    {
        if (string.IsNullOrEmpty(body)) return null;

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (!root.TryGetProperty("error", out var errorElement)) return null;

            var message = errorElement.TryGetProperty("message", out var msgProp)
                ? msgProp.GetString() ?? string.Empty
                : string.Empty;
            var type = errorElement.TryGetProperty("type", out var typeProp)
                ? typeProp.GetString() ?? "Exception"
                : "Exception";

            return new DlqMessageErrorInfo(message, type);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// If body is envelope format (has originalMessage), extract and return it. Else return raw body (legacy).
    /// </summary>
    private static string ExtractOriginalMessageBody(string body)
    {
        if (string.IsNullOrEmpty(body)) return body;

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (root.TryGetProperty("originalMessage", out var originalMsgElement))
                return originalMsgElement.GetRawText();
        }
        catch (JsonException)
        {
            // Not valid JSON or envelope — use raw body
        }

        return body;
    }
}
