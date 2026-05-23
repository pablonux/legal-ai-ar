namespace LegalAiAr.Core.Enums;

/// <summary>
/// Branches of Argentine law. Organized into public, private, social/family, and digital law.
/// Replaces the free-text SubjectArea field on Ruling with a controlled taxonomy.
/// </summary>
public enum LegalBranch
{
    // Public law
    PUB_CONST,
    PUB_ADMIN,
    PUB_PENAL,
    PUB_PROC_CIV,
    PUB_PROC_PEN,
    PUB_TRIB,
    PUB_INT,

    // Private law
    PRIV_CIVIL,
    PRIV_COM,
    PRIV_LAB,
    PRIV_LAB_COL,
    PRIV_SEG,
    PRIV_PI,

    // Social and family law
    SOC_FAM,
    SOC_PREV,
    SOC_NINEZ,
    SOC_AMB,
    SOC_CONS,

    // Digital law
    DIG_DATOS,
    DIG_CYBER,
    DIG_FIRMA
}
