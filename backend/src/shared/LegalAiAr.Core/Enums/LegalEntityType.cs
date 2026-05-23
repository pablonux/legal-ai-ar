namespace LegalAiAr.Core.Enums;

/// <summary>
/// Specific type of legal entity (persona jurídica).
/// Only applicable when <see cref="PersonType"/> is not <see cref="PersonType.Physical"/>.
/// </summary>
public enum LegalEntityType
{
    SA,
    SRL,
    SAS,
    CivilAssociation,
    Foundation,
    Cooperative,
    Mutual,
    State,
    Other
}
