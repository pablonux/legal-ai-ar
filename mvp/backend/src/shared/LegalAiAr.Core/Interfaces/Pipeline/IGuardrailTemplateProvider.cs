using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Pipeline;

/// <summary>
/// Provides rejection response templates per guardrail classification category.
/// </summary>
public interface IGuardrailTemplateProvider
{
    string GetTemplate(GuardrailCategory category);
}
