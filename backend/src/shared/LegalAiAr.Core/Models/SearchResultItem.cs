namespace LegalAiAr.Core.Models;

/// <summary>
/// Single item from a hybrid search result.
/// </summary>
/// <param name="RulingId">ID of the ruling.</param>
/// <param name="CaseTitle">Case title.</param>
/// <param name="Summary">Summary text.</param>
/// <param name="Holding">Main holding text.</param>
/// <param name="Highlight">Highlighted fragment from chunk (if from chunk search).</param>
/// <param name="RulingDate">Date of the ruling.</param>
/// <param name="JurisdictionArea">Jurisdiction area.</param>
/// <param name="Instance">Instance (e.g. CSJN, Cámara).</param>
/// <param name="Court">Court name.</param>
/// <param name="Keywords">Keyword descriptions.</param>
/// <param name="RulingDirection">Ruling direction.</param>
/// <param name="Score">Relevance score.</param>
public record SearchResultItem(
    Guid RulingId,
    string CaseTitle,
    string? Summary,
    string? Holding,
    string? Highlight,
    DateOnly RulingDate,
    string? JurisdictionArea,
    string? Instance,
    string? Court,
    IReadOnlyList<string> Keywords,
    string? RulingDirection,
    double Score,
    string? LegalBranch = null,
    string? PrecedentWeight = null,
    bool IsPlenario = false,
    bool IsLeadingCase = false);
