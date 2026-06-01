namespace LegalAiAr.Core.Enums;

/// <summary>
/// SKOS-style relationship types between thesaurus terms.
/// </summary>
public enum ThesaurusRelationType
{
    /// <summary>Broader Term (TG in SAIJ — parent in hierarchy).</summary>
    BT,

    /// <summary>Narrower Term (TE in SAIJ — child in hierarchy).</summary>
    NT,

    /// <summary>Use For — the source term is the preferred form for the target (UP in SAIJ).</summary>
    UF,

    /// <summary>Related Term (TR in SAIJ — associative relationship).</summary>
    RT
}
