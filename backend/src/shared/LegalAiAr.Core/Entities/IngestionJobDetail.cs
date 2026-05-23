namespace LegalAiAr.Core.Entities;

/// <summary>
/// Detailed metrics for an ingestion job broken down by entity type.
/// </summary>
public class IngestionJobDetail
{
    public int Id { get; set; }
    public Guid IngestionJobId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntitiesCreated { get; set; }
    public int EntitiesUpdated { get; set; }
    public int EntitiesDeleted { get; set; }
    public int FieldsUpdated { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public IngestionJob IngestionJob { get; set; } = null!;
}
