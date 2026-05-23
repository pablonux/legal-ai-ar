namespace LegalAiAr.Core.Enums;

/// <summary>
/// Status of a document within its current pipeline stage.
/// </summary>
public enum DocumentStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Discarded,
    Cancelled,
}
