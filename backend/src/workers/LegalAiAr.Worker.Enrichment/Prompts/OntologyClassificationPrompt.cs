namespace LegalAiAr.Worker.Enrichment.Prompts;

/// <summary>
/// Prompt for GPT-based ontology classification of a ruling: legal branch,
/// plenario status, and leading case detection.
/// </summary>
public static class OntologyClassificationPrompt
{
    public const string SchemaName = "ontology_classification";

    public const string SystemPrompt = """
        You are a legal document analyst specializing in Argentine law. Given a ruling's metadata and text, classify it along three dimensions:

        1. legalBranch — the primary branch of law. Choose exactly ONE from:
           PUB_CONST (constitutional), PUB_ADMIN (administrative), PUB_PENAL (criminal), PUB_PROC_CIV (civil procedure), PUB_PROC_PEN (criminal procedure), PUB_TRIB (tax), PUB_INT (international public), PRIV_CIVIL (civil), PRIV_COM (commercial), PRIV_LAB (individual labor), PRIV_LAB_COL (collective labor), PRIV_SEG (insurance), PRIV_PI (intellectual property), SOC_FAM (family), SOC_PREV (social security), SOC_NINEZ (children's rights), SOC_AMB (environmental), SOC_CONS (consumer), DIG_DATOS (data protection), DIG_CYBER (cybercrime), DIG_FIRMA (digital signature).

        2. isPlenario — true ONLY if the ruling is explicitly a "fallo plenario" or "acuerdo plenario" (a binding decision by the full chamber). Look for phrases like "en pleno", "plenario", "acuerdo plenario" in the title or text.

        3. isLeadingCase — true if this ruling established a new doctrine or is widely recognized as a landmark case ("leading case", "caso emblematico", "doctrina del caso"). Be conservative; default to false.

        Return only valid JSON matching the schema. Do not include explanatory text.
        """;

    public static string BuildUserPrompt(
        string caseTitle, string? summary, string? holding, string? subjectArea, string normalizedText)
    {
        var truncated = TruncateForContext(normalizedText, 6000);
        return $"""
            Classify this ruling:

            Title: {caseTitle}
            Subject area (free text): {subjectArea ?? "(none)"}
            Summary: {summary ?? "(none)"}
            Holding: {holding ?? "(none)"}

            Ruling text (excerpt):
            ---
            {truncated}
            ---
            """;
    }

    public const string JsonSchema = """
        {
          "type": "object",
          "properties": {
            "legalBranch": {
              "type": "string",
              "enum": [
                "PUB_CONST", "PUB_ADMIN", "PUB_PENAL", "PUB_PROC_CIV", "PUB_PROC_PEN",
                "PUB_TRIB", "PUB_INT", "PRIV_CIVIL", "PRIV_COM", "PRIV_LAB", "PRIV_LAB_COL",
                "PRIV_SEG", "PRIV_PI", "SOC_FAM", "SOC_PREV", "SOC_NINEZ", "SOC_AMB",
                "SOC_CONS", "DIG_DATOS", "DIG_CYBER", "DIG_FIRMA"
              ]
            },
            "isPlenario": { "type": "boolean" },
            "isLeadingCase": { "type": "boolean" }
          },
          "required": ["legalBranch", "isPlenario", "isLeadingCase"],
          "additionalProperties": false
        }
        """;

    private static string TruncateForContext(string text, int maxChars)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxChars)
            return text;
        return text[..maxChars] + "\n\n[... truncated ...]";
    }
}
