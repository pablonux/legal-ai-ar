namespace LegalAiAr.Core.Enums;

/// <summary>
/// Type of entity being processed through the ingestion pipeline.
/// Used as the primary axis for queue naming, strategy resolution, and job scoping.
/// </summary>
public enum EntityType
{
    Ruling = 1,
    Statute = 2,
    /// <summary>SAIJ controlled vocabulary (TemaTres API); not part of the document pipeline.</summary>
    Thesaurus = 3,
}
