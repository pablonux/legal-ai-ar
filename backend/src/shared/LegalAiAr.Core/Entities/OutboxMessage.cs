namespace LegalAiAr.Core.Entities;

/// <summary>
/// Serialized domain event row written in the same transaction as the aggregate save.
/// </summary>
public class OutboxMessage
{
    public Guid Id { get; set; }

    /// <summary>Fully qualified CLR type name used to deserialize <see cref="Payload"/>.</summary>
    public string EventType { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public DateTime OccurredOnUtc { get; set; }

    public DateTime? ProcessedOnUtc { get; set; }

    public string? Error { get; set; }

    public int RetryCount { get; set; }
}
