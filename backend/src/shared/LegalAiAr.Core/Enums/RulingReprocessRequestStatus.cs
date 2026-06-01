namespace LegalAiAr.Core.Enums;

/// <summary>
/// Lifecycle of an admin-initiated full ruling reprocess request.
/// </summary>
public enum RulingReprocessRequestStatus
{
    Queued,
    Running,
    Completed,
    Failed,
    Cancelled
}
