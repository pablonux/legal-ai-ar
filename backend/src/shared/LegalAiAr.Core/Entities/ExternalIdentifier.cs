namespace LegalAiAr.Core.Entities;

/// <summary>
/// Identifier of a local entity in an external system.
/// Generic pattern for IDs from multiple sources (CSJN, MJN, SAIJ, etc.).
/// </summary>
public class ExternalIdentifier
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public int SourceId { get; set; }
    public string ExternalCode { get; set; } = string.Empty;
    public string? Label { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Source Source { get; set; } = null!;
}
