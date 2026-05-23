using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Legal norm (law, decree, resolution, etc.) cited in rulings.
/// </summary>
public class Statute
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }

    public NormType? NormType { get; set; }
    public NormativeLevel? NormativeLevel { get; set; }
    public LegalBranch? LegalBranch { get; set; }
    public string? IssuingBody { get; set; }
    public DateOnly? SanctionDate { get; set; }
    public DateOnly? PublicationDate { get; set; }
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public string? OfficialUrl { get; set; }

    public string? FullText { get; set; }
    public string? SaijId { get; set; }
    public string? IssuingBodyName { get; set; }
    public int? IssuingOrganId { get; set; }
    public StatuteStatus? Status { get; set; }

    public bool IsVigente => Status == StatuteStatus.Vigente
        || (Status == null && (EffectiveTo == null || EffectiveTo > DateOnly.FromDateTime(DateTime.UtcNow)));

    public StateOrgan? IssuingOrgan { get; set; }
    public ICollection<RulingStatute> RulingStatutes { get; set; } = new List<RulingStatute>();
    public ICollection<NormRelation> OutboundNormRelations { get; set; } = new List<NormRelation>();
    public ICollection<NormRelation> InboundNormRelations { get; set; } = new List<NormRelation>();
}
