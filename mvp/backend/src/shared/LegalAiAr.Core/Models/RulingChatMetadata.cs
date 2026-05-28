namespace LegalAiAr.Core.Models;

/// <summary>
/// Minimal ruling fields needed to build the RAG chat context.
/// Avoids loading the full entity graph (judges, keywords, statutes, citations).
/// </summary>
public record RulingChatMetadata(
    string CaseTitle,
    string? Summary,
    string? Holding,
    DateOnly RulingDate,
    string? JurisdictionArea,
    string? Instance,
    string? CourtName);
