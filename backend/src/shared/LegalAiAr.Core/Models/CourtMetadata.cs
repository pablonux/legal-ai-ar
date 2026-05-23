namespace LegalAiAr.Core.Models;

/// <summary>
/// Ontology metadata for a court, used to enrich search index documents.
/// </summary>
public record CourtMetadata(
    string? CourtType,
    string? Fuero,
    int? InstanceLevel);
