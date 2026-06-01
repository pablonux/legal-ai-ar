namespace LegalAiAr.Core.Entities;

public class LegalDoctrine
{
    public int Id { get; set; }
    public Guid RulingId { get; set; }
    public string Statement { get; set; } = string.Empty;
    public string? Topic { get; set; }
    public bool IsOverruled { get; set; }
    public Guid? OverruledByRulingId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ruling Ruling { get; set; } = null!;
    public Ruling? OverruledByRuling { get; set; }
}
