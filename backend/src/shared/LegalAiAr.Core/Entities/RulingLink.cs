namespace LegalAiAr.Core.Entities;

/// <summary>
/// Link to a related document (MPF opinion PDF, external doc, etc.).
/// Source: getEnlacesAnalisis (CSJN).
/// </summary>
public class RulingLink
{
    public int Id { get; set; }
    public Guid RulingId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? LinkType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ruling Ruling { get; set; } = null!;
}
