using System.Text.RegularExpressions;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Application.Chat.Pipeline;

/// <summary>
/// Layer 1 classifier: regex-based fast classification (&lt;10ms).
/// Returns high-confidence results for clear matches; null when inconclusive
/// (triggering LLM fallback if enabled).
/// </summary>
public sealed class RuleBasedGuardrailClassifier : IGuardrailClassifier
{
    private static readonly RegexOptions Opts =
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

    #region Prompt Injection Patterns (PI-01..PI-09)

    private static readonly Regex[] PromptInjectionPatterns =
    [
        new(@"ignor[aá]\s+.*(?:instrucciones|prompt|reglas|sistema)", Opts),
        new(@"(?:ahora\s+)?(?:sos|eres|serás|actua\s+como)\s+(?:un|una|el|la)", Opts),
        new(@"repet[ií]\s+.*(?:instrucciones|prompt|sistema|reglas)", Opts),
        new(@"(?:cu[áa]l(?:es)?\s+(?:son|es)\s+tu|mostr[áa].*tu)\s+(?:prompt|instrucciones|sistema|reglas)", Opts),
        new(@"(?:olvidá|olvidate|olvida)\s+.*(?:anterior|previo|reglas)", Opts),
        new(@"(?:ignore|disregard|forget)\s+.*(?:previous|prior|above|instructions)", Opts),
        new(@"(?:you\s+are\s+now|act\s+as|pretend\s+you)", Opts),
        new(@"system\s*:\s*", Opts),
        new(@"\[INST\]|\[/INST\]|<\|im_start\|>|<\|im_end\|>", Opts),
    ];

    #endregion

    #region Personal Legal Advice Patterns (PLA-01..PLA-04)

    private static readonly Regex[] PersonalAdvicePatterns =
    [
        new(@"(?:qu[ée]\s+(?:deber[ií]a|tengo\s+que|puedo)\s+hacer\s+(?:en|con)\s+mi\s+caso)", Opts),
        new(@"(?:me\s+conviene|me\s+recomend[áa]s|aconse[jg][áa]me)", Opts),
        new(@"(?:mi\s+(?:abogado|caso|situaci[oó]n|problema)\s+)", Opts),
        new(@"(?:cómo\s+(?:demando|denuncio|inicio\s+(?:un\s+)?juicio))", Opts),
    ];

    #endregion

    #region PII Patterns (PII-01..PII-03)

    private static readonly Regex CuitPattern = new(@"\b\d{2}[-.]?\d{8}[-.]?\d{1}\b", Opts);
    private static readonly Regex DniPrefixPattern = new(@"\b(?:DNI|D\.N\.I\.?)\s*:?\s*\d{7,8}\b", Opts);
    private static readonly Regex PersonalContextPattern = new(@"(?:mi\s+caso|mi\s+DNI|mi\s+documento|mi\s+CUIT)", Opts);
    private static readonly Regex StandaloneDniPattern = new(@"\b\d{7,8}\b", Opts);

    #endregion

    #region Greeting Patterns (GR-01..GR-03)

    private static readonly Regex[] GreetingPatterns =
    [
        new(@"^\W*(?:hola|buenas?\s+(?:tardes?|noches?|d[ií]as?)|hey|hi|buen[oa]s?)[\s!?.]*$", Opts),
        new(@"^\W*(?:qu[ée]\s+(?:pod[ée]s|pued[ée]s)\s+hacer|(?:para\s+)?qu[ée]\s+serv[ií]s|ayuda|help)[\s?]*$", Opts),
        new(@"^\W*(?:gracias|muchas\s+gracias|ok|dale|perfecto|genial)[\s!.]*$", Opts),
    ];

    #endregion

    #region Legal Scope Keywords

    private static readonly HashSet<string> LegalKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        // Instituciones y procedimientos
        "fallo", "fallos", "jurisprudencia", "sentencia", "resolución", "tribunal", "corte",
        "cámara", "juzgado", "juez", "jueces", "csjn", "recurso", "apelación", "casación",
        "amparo", "habeas corpus", "inconstitucionalidad", "constitución", "constitucional",
        "código", "ley", "decreto", "norma", "artículo", "art.", "demanda", "demandante",
        "demandado", "actor", "acción", "competencia", "jurisdicción", "cautelar", "embargo",
        "litispendencia", "nulidad", "debido proceso", "garantía", "derecho", "precedente",
        "doctrina", "dictamen", "queja", "extraordinario",

        // Ramas del derecho
        "penal", "civil", "laboral", "comercial", "contencioso", "administrativo",
        "tributario", "previsional", "ambiental", "constitucional",

        // Derecho penal
        "homicidio", "estupefacientes", "narcotráfico", "abuso", "robo", "hurto",
        "estafa", "contrabando", "extradición", "prisión preventiva", "libertad condicional",
        "imputado", "condena", "absolución", "pena",

        // Derecho laboral
        "despido", "indemnización", "trabajador", "empleador", "contrato de trabajo",
        "accidente de trabajo", "riesgos del trabajo",

        // Derecho civil / daños
        "daños", "perjuicios", "responsabilidad", "prescripción", "caducidad",
        "cosa juzgada", "obligación", "contrato",

        // Derechos fundamentales
        "libertad", "expresión", "igualdad", "propiedad", "intimidad", "honor",
        "discriminación", "salud", "vivienda", "educación",

        // Derecho tributario / previsional
        "impuesto", "tributo", "ganancias", "jubilación", "jubilatorio", "haberes",
        "reajuste", "movilidad", "pensión", "ANSES", "AFIP",

        // Derecho del consumidor / salud
        "consumidor", "obra social", "prepaga",

        // Términos generales de búsqueda jurídica
        "inconstitucional", "arbitrariedad", "arbitraria", "honorarios",
        "intereses", "capitalización", "ejecución",
    };

    private static readonly Regex SingleWordToken = new(@"\b[\wáéíóúñü]+\b", Opts);

    #endregion

    public Task<GuardrailClassification> ClassifyAsync(string query, CancellationToken cancellationToken = default)
    {
        var trimmed = query.Trim();

        if (IsPromptInjection(trimmed))
            return Task.FromResult(new GuardrailClassification(GuardrailCategory.Harmful, GuardrailSource.RuleBased, 0.95f));

        if (IsPersonalAdviceWithPii(trimmed))
            return Task.FromResult(new GuardrailClassification(GuardrailCategory.Harmful, GuardrailSource.RuleBased, 0.90f));

        if (IsPersonalAdvice(trimmed))
            return Task.FromResult(new GuardrailClassification(GuardrailCategory.Harmful, GuardrailSource.RuleBased, 0.85f));

        if (IsGreeting(trimmed))
            return Task.FromResult(new GuardrailClassification(GuardrailCategory.Greeting, GuardrailSource.RuleBased, 0.95f));

        var hasLegalKeywords = ContainsLegalKeywords(trimmed);
        var wordCount = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        if (hasLegalKeywords && wordCount >= 2)
            return Task.FromResult(new GuardrailClassification(GuardrailCategory.LegalQuery, GuardrailSource.RuleBased, 0.85f));

        if (hasLegalKeywords && wordCount < 2)
            return Task.FromResult(new GuardrailClassification(GuardrailCategory.Clarification, GuardrailSource.RuleBased, 0.70f));

        // Inconclusive: no legal keywords, no harmful patterns.
        // Return null confidence to signal LLM fallback needed.
        return Task.FromResult(new GuardrailClassification(GuardrailCategory.OutOfScope, GuardrailSource.RuleBased, null));
    }

    private static bool IsPromptInjection(string text)
    {
        foreach (var pattern in PromptInjectionPatterns)
            if (pattern.IsMatch(text))
                return true;
        return false;
    }

    private static bool IsPersonalAdvice(string text)
    {
        foreach (var pattern in PersonalAdvicePatterns)
            if (pattern.IsMatch(text))
                return true;
        return false;
    }

    private static bool IsPersonalAdviceWithPii(string text)
    {
        if (!IsPersonalAdvice(text)) return false;
        return CuitPattern.IsMatch(text) || DniPrefixPattern.IsMatch(text)
            || (PersonalContextPattern.IsMatch(text) && StandaloneDniPattern.IsMatch(text));
    }

    private static bool IsGreeting(string text) =>
        GreetingPatterns.Any(p => p.IsMatch(text));

    private bool ContainsLegalKeywords(string text)
    {
        foreach (var keyword in LegalKeywords)
        {
            if (!keyword.Contains(' ', StringComparison.Ordinal))
                continue;
            var parts = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var pattern = string.Join(@"\s+", parts.Select(Regex.Escape));
            if (Regex.IsMatch(text, $@"\b{pattern}\b", Opts))
                return true;
        }

        foreach (Match match in SingleWordToken.Matches(text))
            if (LegalKeywords.Contains(match.Value))
                return true;
        return false;
    }
}
