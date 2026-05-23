namespace LegalAiAr.Application.Admin.Pipeline.Commands.RequeueDocument;

/// <summary>
/// Result of RequeueDocumentCommand.
/// </summary>
/// <param name="Success">Whether the message was published.</param>
/// <param name="Stage">Stage the message was published to.</param>
/// <param name="MessageId">Optional queue message ID if available.</param>
public record RequeueDocumentResult(
    bool Success,
    string Stage,
    string? MessageId = null);
