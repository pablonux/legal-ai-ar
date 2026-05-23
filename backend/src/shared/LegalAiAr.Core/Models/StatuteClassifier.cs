using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Models;

/// <summary>
/// Rule-based classifier for Argentine legal norms. Sets NormType and NormativeLevel
/// based on name/number patterns when not already assigned.
/// </summary>
public static class StatuteClassifier
{
    public static void ClassifyIfNeeded(Statute statute)
    {
        if (statute.NormType is not null)
            return;

        var name = statute.Name;
        var number = statute.Number;

        if (name.Contains("Constitución", StringComparison.OrdinalIgnoreCase)
            || number.Equals("CN", StringComparison.OrdinalIgnoreCase))
        {
            statute.NormType = NormType.CONSTITUTION;
            statute.NormativeLevel = NormativeLevel.CONSTITUTIONAL;
        }
        else if (name.StartsWith("DNU", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Decreto de Necesidad y Urgencia", StringComparison.OrdinalIgnoreCase))
        {
            statute.NormType = NormType.DNU;
            statute.NormativeLevel = NormativeLevel.LEGAL;
        }
        else if (name.Contains("Tratado", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Convención", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Convencion", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Pacto", StringComparison.OrdinalIgnoreCase))
        {
            statute.NormType = NormType.TREATY;
            statute.NormativeLevel = NormativeLevel.SUPRALEGAL;
        }
        else if (name.Contains("Decreto", StringComparison.OrdinalIgnoreCase)
            || number.Contains("/", StringComparison.Ordinal))
        {
            statute.NormType = NormType.DECREE;
            statute.NormativeLevel = NormativeLevel.REGULATORY;
        }
        else if (name.Contains("Resolución", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Resolucion", StringComparison.OrdinalIgnoreCase))
        {
            statute.NormType = NormType.RESOLUTION;
            statute.NormativeLevel = NormativeLevel.REGULATORY;
        }
        else if (name.Contains("Acordada", StringComparison.OrdinalIgnoreCase))
        {
            statute.NormType = NormType.ACORDADA;
            statute.NormativeLevel = NormativeLevel.REGULATORY;
        }
        else if (name.Contains("Ordenanza", StringComparison.OrdinalIgnoreCase))
        {
            statute.NormType = NormType.ORDINANCE;
            statute.NormativeLevel = NormativeLevel.REGULATORY;
        }
        else if (name.Contains("Ley", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Código", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Codigo", StringComparison.OrdinalIgnoreCase))
        {
            statute.NormType = NormType.LAW;
            statute.NormativeLevel = NormativeLevel.LEGAL;
        }
    }
}
