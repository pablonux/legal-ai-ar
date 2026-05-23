namespace LegalAiAr.Core.Enums;

/// <summary>
/// Type of citation relationship between rulings.
/// Inferred by GPT-4o from textual context. Default: CITES.
/// </summary>
public enum CitationType
{
    UPHOLDS,
    OVERRULES,
    DISTINGUISHES,
    CITES,
    FOLLOWS,
    DISSENTS_FROM
}
