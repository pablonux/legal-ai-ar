namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Service for queue metrics (e.g. approximate message count).
/// Used by admin pipeline status.
/// </summary>
public interface IQueueMetricsService
{
    /// <summary>
    /// Gets the approximate message count for the specified queue.
    /// </summary>
    /// <param name="queueName">Full queue name (e.g. pipeline-discoverer).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<int> GetApproximateMessageCountAsync(string queueName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lightweight connectivity check (GetProperties) without dequeuing messages.
    /// </summary>
    Task<(bool Ok, string? Error)> TryProbeQueueAsync(string queueName, CancellationToken cancellationToken = default);
}
