using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Master/reference entity for a person (physical or legal) in the Knowledge Base.
/// Treated as nomenclated data: stable identity, verified from official sources.
/// The person's role is always contextual -- determined by relationship entities
/// (JudicialOffice, RulingParticipation, ProceedingParty, LegalRepresentation).
/// </summary>
public class Person
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public PersonType PersonType { get; set; } = PersonType.Physical;
    public LegalEntityType? LegalEntityType { get; set; }
    public int? CsjnMinistroId { get; set; }
    public bool IsVerified { get; set; }

    public ICollection<RulingParticipation> RulingParticipations { get; set; } = new List<RulingParticipation>();
    public ICollection<JudicialOffice> JudicialOffices { get; set; } = new List<JudicialOffice>();
    public ICollection<ProceedingParty> ProceedingParties { get; set; } = new List<ProceedingParty>();
}
