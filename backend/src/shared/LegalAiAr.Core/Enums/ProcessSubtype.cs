namespace LegalAiAr.Core.Enums;

/// <summary>
/// Subtype of judicial process by procedural form.
/// Refines <see cref="ProcessType"/> with the specific procedural vehicle.
/// </summary>
public enum ProcessSubtype
{
    Ordinario,
    Sumarisimo,
    Ejecutivo,
    Amparo,
    HabeasCorpus,
    HabeasData,
    AccionDeclarativa
}
