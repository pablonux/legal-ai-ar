using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Groups rulings that belong to the same judicial case across different court instances.
/// Linked by CaseNumber + JurisdictionArea.
/// </summary>
public class JudicialProceeding
{
    public int Id { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? JurisdictionArea { get; set; }
    public int RulingCount { get; set; }
    public DateOnly? FirstRulingDate { get; set; }
    public DateOnly? LastRulingDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public ProcessType? ProcessType { get; set; }
    public string? ProcessSubtype { get; set; }
    public int? CourtId { get; set; }
    public LegalBranch? LegalBranch { get; set; }
    public ProcessStatus? Status { get; set; }

    public Court? Court { get; set; }
    public ICollection<Ruling> Rulings { get; set; } = new List<Ruling>();
    public ICollection<ProceedingParty> Parties { get; set; } = new List<ProceedingParty>();
    public ICollection<LegalRepresentation> LegalRepresentations { get; set; } = new List<LegalRepresentation>();
}
