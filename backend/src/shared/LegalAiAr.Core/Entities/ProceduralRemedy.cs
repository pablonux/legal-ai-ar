using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

public class ProceduralRemedy
{
    public int Id { get; set; }
    public RemedyType RemedyType { get; set; }
    public DateOnly? FilingDate { get; set; }
    public DateOnly? ResolutionDate { get; set; }
    public string? Outcome { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? ResolvingRulingId { get; set; }
    public Guid? AppealedRulingId { get; set; }
    public int? CourtAQuoId { get; set; }
    public int? CourtAdQuemId { get; set; }
    public int? JudicialProceedingId { get; set; }

    public Ruling? ResolvingRuling { get; set; }
    public Ruling? AppealedRuling { get; set; }
    public Court? CourtAQuo { get; set; }
    public Court? CourtAdQuem { get; set; }
    public JudicialProceeding? JudicialProceeding { get; set; }
}
