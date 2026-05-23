namespace LegalAiAr.Core.Enums;

/// <summary>
/// Type of judicial process by subject matter.
/// Maps to the ontological class ProcesoJudicial → tipoProceso.
/// </summary>
public enum ProcessType
{
    Civil,
    Penal,
    Laboral,
    ContenciosoAdministrativo,
    Familia,
    Constitucional
}
