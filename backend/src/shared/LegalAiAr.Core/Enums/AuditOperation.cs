namespace LegalAiAr.Core.Enums;

/// <summary>
/// Type of operation recorded in the entity audit log.
/// </summary>
public enum AuditOperation
{
    Created,
    Updated,
    Deleted,
    Restored,
    Reprocessed
}
