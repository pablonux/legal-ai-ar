namespace LegalAiAr.Core.Entities;

/// <summary>
/// Synthesis/review document for a ruling. Source: getSintesisAnalisis (CSJN).
/// </summary>
public class RulingSynthesis
{
    public int Id { get; set; }
    public Guid RulingId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ruling Ruling { get; set; } = null!;
}
