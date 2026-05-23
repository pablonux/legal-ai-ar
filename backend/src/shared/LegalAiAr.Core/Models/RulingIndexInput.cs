namespace LegalAiAr.Core.Models;

/// <summary>
/// Input for indexing a ruling in Azure AI Search rulings-by-ruling index.
/// </summary>
public record RulingIndexInput(
    Guid RulingId,
    string CaseTitle,
    string? Summary,
    string? Holding,
    string? CaseNumber,
    DateOnly RulingDate,
    string? JurisdictionArea,
    string? Instance,
    string? Court,
    IReadOnlyList<string> Keywords,
    IReadOnlyList<string> Persons,
    IReadOnlyList<string> Statutes,
    string? RulingDirection,
    float[] Embedding,
    string? SubjectArea = null,
    string? LegalBranch = null,
    string? PrecedentWeight = null,
    bool IsPlenario = false,
    bool IsLeadingCase = false,
    string? ResourceType = null,
    bool IsUnconstitutional = false,
    string? CourtType = null,
    string? Fuero = null,
    int? InstanceLevel = null,
    string? ActionType = null,
    string? OfficialReference = null,
    string? FederalQuestion = null,
    bool HasDictamen = false);
