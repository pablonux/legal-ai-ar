namespace LegalAiAr.Core.Enums;

/// <summary>
/// Catalog list slice for persons: same underlying <see cref="Entities.Person"/>, different entry paths in the UI.
/// </summary>
public enum PersonListView
{
    All = 0,
    /// <summary>Ruling participation as tribunal formation (signatory, dissent, concurrence, majority author).</summary>
    Magistrates = 1,
    /// <summary>Persons with at least one <see cref="Entities.ProceedingParty"/> row.</summary>
    Parties = 2
}
