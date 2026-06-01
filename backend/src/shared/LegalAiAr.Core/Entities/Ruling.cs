using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Judicial ruling (fallo) indexed in the Knowledge Base.
/// </summary>
public class Ruling
{
    public Guid Id { get; set; }
    public int SourceId { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string? AnalysisId { get; set; }
    public string ContentHash { get; set; } = string.Empty;
    public string CaseTitle { get; set; } = string.Empty;
    public string? CaseNumber { get; set; }
    public DateOnly RulingDate { get; set; }
    public int CourtId { get; set; }
    public string? JurisdictionArea { get; set; }
    public string? Instance { get; set; }
    public string? Jurisdiction { get; set; }
    public string? ResourceType { get; set; }
    public string? RulingDirection { get; set; }
    public string? SubjectArea { get; set; }
    public LegalBranch? LegalBranch { get; set; }
    public PrecedentWeight? PrecedentWeight { get; set; }
    public bool IsPlenario { get; set; }
    public bool IsLeadingCase { get; set; }
    public bool IsUnconstitutional { get; set; }
    public string? Summary { get; set; }
    public string? Holding { get; set; }
    public string? FullText { get; set; }
    public string? BlobPath { get; set; }
    public string? ActionType { get; set; }
    public string? InternalSubject { get; set; }
    public string? OfficialReference { get; set; }
    public string? Observations { get; set; }
    public string? FederalQuestion { get; set; }
    public string? ProceduralFormula { get; set; }
    public string? RatioDecidendi { get; set; }
    public string? DoctrinaLegal { get; set; }
    public bool HasDictamen { get; set; }
    public DateTime IndexedAt { get; set; }
    public RulingStatus Status { get; set; }
    public Guid? IngestionJobId { get; set; }
    public int? JudicialProceedingId { get; set; }

    public Source Source { get; set; } = null!;
    public IngestionJob? IngestionJob { get; set; }
    public Court Court { get; set; } = null!;
    public JudicialProceeding? JudicialProceeding { get; set; }
    public ProsecutorOpinion? ProsecutorOpinion { get; set; }
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    public ICollection<RulingParticipation> RulingParticipations { get; set; } = new List<RulingParticipation>();
    public ICollection<RulingKeyword> RulingKeywords { get; set; } = new List<RulingKeyword>();
    public ICollection<RulingStatute> RulingStatutes { get; set; } = new List<RulingStatute>();
    public ICollection<Citation> OutboundCitations { get; set; } = new List<Citation>();
    public ICollection<Citation> InboundCitations { get; set; } = new List<Citation>();
    public ICollection<Sumario> Sumarios { get; set; } = new List<Sumario>();
    public ICollection<RulingSynthesis> Syntheses { get; set; } = new List<RulingSynthesis>();
    public ICollection<RulingLink> Links { get; set; } = new List<RulingLink>();
    public ICollection<RulingSourceMetadata> SourceMetadata { get; set; } = new List<RulingSourceMetadata>();
    public ICollection<LegalDoctrine> LegalDoctrines { get; set; } = new List<LegalDoctrine>();
}
