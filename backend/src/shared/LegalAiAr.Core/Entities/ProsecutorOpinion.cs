namespace LegalAiAr.Core.Entities;

/// <summary>
/// Extracted opinion of the Procurador General from a CSJN ruling.
/// One ruling has at most one prosecutor opinion.
/// </summary>
public class ProsecutorOpinion
{
    public int Id { get; set; }
    public Guid RulingId { get; set; }
    public int? PersonId { get; set; }
    public string ProsecutorName { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? RecommendedDirection { get; set; }
    public bool AgreedWithCourt { get; set; }
    public string? DocumentBlobPath { get; set; }
    public DateTime ExtractedAt { get; set; }

    public Ruling Ruling { get; set; } = null!;
    public Person? Person { get; set; }
}
