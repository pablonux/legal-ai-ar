namespace LegalAiAr.Core.Entities;

/// <summary>
/// Source-specific fields that don't belong in the generic Ruling model.
/// Key-value pairs scoped to a source (e.g., CSJN ActionType, InternalSubject, etc.).
/// </summary>
public class RulingSourceMetadata
{
    public int Id { get; set; }
    public Guid RulingId { get; set; }
    public int SourceId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }

    public Ruling Ruling { get; set; } = null!;
    public Source Source { get; set; } = null!;
}
