using System.Text.RegularExpressions;

namespace LegalAiAr.Application.Chat.Pipeline;

/// <summary>
/// Provides ontology-based context to inject into the chat pipeline when the user's
/// query matches a legal reasoning pattern (action available, applicable norm,
/// deadline, liability, challenge, validity).
/// </summary>
public sealed class OntologyContextProvider
{
    private static readonly RegexOptions Opts =
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

    private static readonly (Regex Pattern, string FlowId, string Label)[] ReasoningPatterns =
    [
        (new(@"qu[ée]\s+(?:acci[oó]n|recurso|v[ií]a|reclamo|demanda)\b.*(?:tengo|puedo|disponible|corresponde|procede)", Opts),
            "action_available", "¿Qué acción tengo disponible?"),

        (new(@"qu[ée]\s+(?:norma|ley|art[ií]culo|c[oó]digo|decreto)\b.*(?:aplic|rige|regul|gobierna)", Opts),
            "applicable_norm", "¿Qué norma aplica?"),

        (new(@"(?:qu[ée]\s+)?plazo\b|prescri[bp]ci[oó]n\b|caducidad\b|cu[aá]nto\s+tiempo\b|venc(?:e|imiento)\b|d[ií]as?\s+(?:h[aá]bil|corrido)", Opts),
            "deadline", "¿Qué plazo tengo?"),

        (new(@"qui[eé]n\s+responde\b|responsab(?:le|ilidad)\b|(?:da[ñn]os?\s+y\s+)?perjuicios\b|indemni[zs]aci[oó]n\b|culp[ao]\b|dolo\b|riesgo\s+creado\b", Opts),
            "liability", "¿Quién responde?"),

        (new(@"(?:c[oó]mo\s+)?impugn[ao]\b|recurr[io]\b|apel[ao]\b|nul(?:o|a|idad)\b|rev(?:oc|is)[ao]\b|recurso\s+extraordinario\b|queja\b|casaci[oó]n\b", Opts),
            "challenge", "¿Cómo impugno?"),

        (new(@"(?:es\s+)?v[aá]lido\b|validez\b|nul(?:o|a|idad)\b|anulable\b|vicio\w*\b|simula\w+\b|lesi[oó]n\b|objeto\s+il[ií]cito\b|causa\s+il[ií]cita\b", Opts),
            "validity", "¿Es válido?"),
    ];

    private static readonly Dictionary<string, string> FlowInstructions = new()
    {
        ["action_available"] = """
            The user wants to know what legal action is available. Follow this reasoning:
            1. Identify the FACTS described.
            2. Determine the BRANCH OF LAW (civil, penal, laboral, administrativo, familia).
            3. Determine COMPETENCE: material (fuero) + territorial.
            4. Identify the TYPE OF PROCESS (ordinario, sumarísimo, ejecutivo, amparo, etc.).
            5. State the AVAILABLE CLAIM with specific legal basis (article + statute).
            6. Calculate the APPLICABLE DEADLINE (prescripción/caducidad).
            """,
        ["applicable_norm"] = """
            The user wants to know which norm applies. Follow this reasoning:
            1. Understand the FACTUAL SITUATION described.
            2. Traverse the NORMATIVE HIERARCHY: Constitution → Treaties → Laws → Decrees → Resolutions.
            3. Identify the SPECIFIC NORM (statute + articles) that governs the situation.
            4. Verify CURRENT VALIDITY: is it still in force?
            5. Search for APPLICABLE JURISPRUDENCE: CSJN rulings first, then Cámara decisions.
            6. State the CURRENT INTERPRETATION based on the most recent binding precedent.
            """,
        ["deadline"] = """
            The user is asking about deadlines or statutes of limitation. Follow this reasoning:
            1. Identify the TYPE OF ACT OR CLAIM.
            2. Determine the APPLICABLE PROCEDURAL CODE (CPCCN, CPPF, LCT, provincial code).
            3. Look up the SPECIFIC DEADLINE.
            4. Determine COMPUTATION: business days (días hábiles) vs calendar days (días corridos).
            5. Check for SUSPENSION or INTERRUPTION events.
            6. Present the deadline with a clear calculation.
            """,
        ["liability"] = """
            The user is asking about liability or who is responsible. Follow this reasoning:
            1. Identify the HARMFUL EVENT (hecho dañoso).
            2. Determine the ATTRIBUTION FACTOR: subjective (dolo/culpa, art. 1724 CCyCN) or objective (risk/guarantee, arts. 1722-1723 CCyCN).
            3. Identify the LIABLE SUBJECT(S): direct, indirect, joint, subsidiary.
            4. Classify the TYPE OF LIABILITY: civil, criminal, administrative, state (Ley 26.944).
            5. Describe the AVAILABLE REMEDY: compensation, criminal penalty, administrative sanction.
            """,
        ["challenge"] = """
            The user wants to know how to challenge or appeal an act or decision. Follow this reasoning:
            1. Determine the NATURE OF THE ACT: administrative act, judicial resolution, or private act.
            2. For ADMINISTRATIVE ACTS: check if administrative remedies must be exhausted first (Ley 19.549, art. 30).
            3. For JUDICIAL RESOLUTIONS: identify the type and available remedy (Reposición 3 days, Apelación 5 days, REF 10 days).
            4. Identify the COMPETENT COURT for the appeal (ad quem).
            5. State the DEADLINE AND REQUIREMENTS for each remedy.
            """,
        ["validity"] = """
            The user is asking whether a contract or legal act is valid. Follow this reasoning:
            1. Identify the TYPE OF LEGAL ACT (contrato, donación, testamento, acto administrativo).
            2. Check ESSENTIAL ELEMENTS (art. 259 CCyCN): voluntary manifestation, lawful object, lawful cause.
            3. Check FORMAL REQUIREMENTS: is it solemn? Does it require escritura pública?
            4. Analyze potential DEFECTS (vicios): error, dolo, violence, lesión, simulation, fraud.
            5. Conclude: VALID, NULL (absolute nullity, arts. 386-387 CCyCN), or VOIDABLE (relative nullity, art. 388 CCyCN).
            """,
    };

    /// <summary>
    /// Detects if the query matches a legal reasoning pattern and returns
    /// the structured reasoning instructions to inject into the chat context.
    /// Returns null if no pattern matches.
    /// </summary>
    public (string FlowId, string Label, string Instructions)? DetectReasoningFlow(string query)
    {
        foreach (var (pattern, flowId, label) in ReasoningPatterns)
        {
            if (pattern.IsMatch(query) && FlowInstructions.TryGetValue(flowId, out var instructions))
                return (flowId, label, instructions);
        }

        return null;
    }
}
