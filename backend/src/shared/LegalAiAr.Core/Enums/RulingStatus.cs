namespace LegalAiAr.Core.Enums;

/// <summary>
/// Status of a ruling in the indexing pipeline.
/// </summary>
public enum RulingStatus
{
    Indexed,
    Reprocessing,
    Error,
    Pending
}
