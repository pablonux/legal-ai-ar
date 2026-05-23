namespace LegalAiAr.Core.Exceptions;

/// <summary>
/// Thrown when CSJN pagination or API response schema has changed (R-001).
/// Required fields missing or type changes indicate a breaking API change.
/// CrawlerWorker moves the message to DLQ and does not update LastCrawledAt.
/// </summary>
public class CsjnSchemaViolationException : DomainException
{
    /// <summary>
    /// Source ID where the violation occurred (e.g. 1 for CSJN).
    /// </summary>
    public int SourceId { get; }

    /// <summary>
    /// Optional context: document ID, page index, or endpoint.
    /// </summary>
    public string? Context { get; }

    public CsjnSchemaViolationException(string message, int sourceId = 1, string? context = null)
        : base(message)
    {
        SourceId = sourceId;
        Context = context;
    }

    public CsjnSchemaViolationException(string message, Exception innerException, int sourceId = 1, string? context = null)
        : base(message, innerException)
    {
        SourceId = sourceId;
        Context = context;
    }
}
