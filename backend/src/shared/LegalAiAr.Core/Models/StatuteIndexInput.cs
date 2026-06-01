namespace LegalAiAr.Core.Models;

/// <summary>
/// Input for indexing a statute in Azure AI Search.
/// </summary>
public record StatuteIndexInput(
    int StatuteId,
    string Number,
    string Name,
    string? NormType,
    string? NormativeLevel,
    string? LegalBranch,
    string? IssuingBody,
    DateOnly? SanctionDate,
    DateOnly? PublicationDate,
    string? Status,
    bool IsVigente,
    string? FullText,
    string? SaijId,
    int RulingCount);
