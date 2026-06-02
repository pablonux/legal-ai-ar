namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Processes pending rows in <c>OutboxMessages</c> (dispatch handlers, mark processed or record errors).
/// </summary>
public interface IOutboxMessageProcessor
{
    /// <summary>
    /// Processes a batch of pending outbox messages (batch size configured via <c>Outbox:BatchSize</c>).
    /// </summary>
    /// <returns>Number of messages successfully processed in this call.</returns>
    Task<int> ProcessPendingAsync(CancellationToken cancellationToken = default);
}
