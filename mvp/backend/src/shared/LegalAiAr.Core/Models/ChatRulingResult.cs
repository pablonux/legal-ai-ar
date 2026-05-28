namespace LegalAiAr.Core.Models;

/// <summary>
/// Ruling metadata from rulings-by-ruling index for RAG context construction.
/// </summary>
/// <param name="RulingId">ID of the ruling.</param>
/// <param name="CaseTitle">Case title.</param>
/// <param name="Summary">Ruling summary.</param>
/// <param name="Holding">Main holding.</param>
/// <param name="RulingDate">Date of the ruling.</param>
/// <param name="JurisdictionArea">Jurisdiction area.</param>
/// <param name="Instance">Instance (e.g. CSJN, Cámara).</param>
/// <param name="Court">Court name.</param>
/// <param name="Score">Relevance score.</param>
public record ChatRulingResult(
    Guid RulingId,
    string CaseTitle,
    string? Summary,
    string? Holding,
    DateOnly RulingDate,
    string? JurisdictionArea,
    string? Instance,
    string? Court,
    double Score);
