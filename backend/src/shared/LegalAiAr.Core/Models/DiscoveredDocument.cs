namespace LegalAiAr.Core.Models;

/// <summary>
/// Represents a document discovered during crawl. Used as the yield type of <see cref="Interfaces.Pipeline.ICrawlerSource.DiscoverAsync"/>.
/// </summary>
/// <param name="DocumentId">External document ID in the source (e.g. Codigo from CSJN pagination).</param>
/// <param name="AnalysisId">Source-specific analysis ID. For CSJN: idAnalisis used for API calls. Null for sources that do not use it.</param>
/// <param name="AcuerdoDate">For CSJN: the acuerdo (court session) date that is the authoritative ruling date. Null when unknown.</param>
public record DiscoveredDocument(
    string DocumentId,
    string? AnalysisId,
    DateOnly? AcuerdoDate = null,
    string? CaseNumber = null);
