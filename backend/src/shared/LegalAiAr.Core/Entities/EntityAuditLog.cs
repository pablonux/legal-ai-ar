using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Log of operations on KB entities (creation, update, deletion, restore).
/// </summary>
public class EntityAuditLog
{
    public long Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public AuditOperation Operation { get; set; }
    public Guid? IngestionJobId { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public string? ChangeSummary { get; set; }
    public string? FieldsChanged { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public IngestionJob? IngestionJob { get; set; }
}
