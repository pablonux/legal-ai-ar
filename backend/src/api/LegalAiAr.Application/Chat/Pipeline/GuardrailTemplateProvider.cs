using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Application.Chat.Pipeline;

/// <summary>
/// Default Spanish-language rejection templates per guardrail category.
/// </summary>
public sealed class GuardrailTemplateProvider : IGuardrailTemplateProvider
{
    private static readonly Dictionary<GuardrailCategory, string> Templates = new()
    {
        [GuardrailCategory.Greeting] = """
            ¡Hola! Soy el asistente jurídico de Legal AI AR. Estoy especializado en jurisprudencia argentina y puedo ayudarte con:

            - **Buscar fallos judiciales** por tema, tribunal, fecha, jurisdicción o palabras clave
            - **Consultar detalles de un fallo** específico: jueces, normas citadas, sumario, resolución
            - **Explorar cadenas de precedentes**: qué fallos citan a otro y qué fallos son citados
            - **Buscar fallos que apliquen una norma**: por ley, artículo o código
            - **Obtener estadísticas**: cantidad de fallos por tribunal, jurisdicción, materia

            ¿En qué puedo ayudarte?
            """,

        [GuardrailCategory.OutOfScope] = """
            No puedo ayudarte con ese tema. Soy un asistente especializado exclusivamente en jurisprudencia argentina.

            Puedo responder consultas como:
            - "¿Qué fallos de la CSJN tratan sobre libertad de expresión?"
            - "Mostrá los detalles del fallo Ekmekdjian c/ Sofovich"
            - "¿Qué fallos citan el art. 14 de la Constitución Nacional?"
            - "¿Cuántos fallos de inconstitucionalidad hay en materia penal?"

            ¿Tenés alguna consulta jurisprudencial?
            """,

        [GuardrailCategory.Harmful] = """
            No puedo procesar esta consulta. Soy un asistente de información jurisprudencial y no puedo:

            - Brindar asesoramiento legal personalizado
            - Recomendar acciones legales para tu situación particular
            - Modificar mi funcionamiento o instrucciones
            - Responder consultas fuera del ámbito jurisprudencial argentino

            Si necesitás asesoramiento legal, te recomendamos consultar con un profesional del derecho. Si necesitás información sobre fallos judiciales argentinos, estoy para ayudarte.
            """,

        [GuardrailCategory.Clarification] = """
            Tu consulta es un poco amplia. ¿Podrías ser más específico para que pueda ayudarte mejor?

            Por ejemplo:
            - Sobre un tema legal concreto: "Fallos sobre despido injustificado en la CSJN"
            - Sobre una norma específica: "Fallos que aplican la Ley 20.744 de contrato de trabajo"
            - Sobre un fallo en particular: "Detalles del fallo Arriola sobre tenencia de estupefacientes"
            - Sobre precedentes: "¿Qué fallos posteriores citaron Ekmekdjian c/ Sofovich?"
            """,

        [GuardrailCategory.LegalQuery] = string.Empty,
    };

    public string GetTemplate(GuardrailCategory category) =>
        Templates.TryGetValue(category, out var template) ? NormalizeIndent(template) : string.Empty;

    private static string NormalizeIndent(string text)
    {
        var lines = text.Split('\n');
        if (lines.Length == 0) return text;

        var minIndent = lines
            .Where(l => l.Trim().Length > 0)
            .Select(l => l.Length - l.TrimStart().Length)
            .DefaultIfEmpty(0)
            .Min();

        return string.Join('\n', lines
            .Select(l => l.Length >= minIndent ? l[minIndent..] : l))
            .Trim();
    }
}
