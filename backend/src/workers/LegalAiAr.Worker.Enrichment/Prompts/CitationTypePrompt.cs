namespace LegalAiAr.Worker.Enrichment.Prompts;

/// <summary>
/// System and user prompt templates for citation type classification (E049).
/// </summary>
public static class CitationTypePrompt
{
    public const string SchemaName = "citation_types_extraction";

    public const string SystemPrompt = """
        You are a legal document analyst for Argentine court rulings. Given a citation to another ruling and the surrounding text, classify the citation type. Types: UPHOLDS (the ruling confirms or follows the cited precedent), OVERRULES (the ruling overrules or rejects the cited precedent), DISTINGUISHES (the ruling distinguishes the case from the cited precedent), CITES (the ruling merely cites the precedent without affirming, overruling, or distinguishing). Default to CITES when uncertain. Return only valid JSON matching the schema. Do not include explanatory text.
        """;

    public static string BuildUserPrompt(string citationsJson, string normalizedText)
    {
        var truncated = TruncateForContext(normalizedText, 8000);
        return $"""
            Classify the citation type for each of the following citations based on the ruling text. Return a JSON object with a "citationTypes" array. Each element must have alias (exactly as given) and citationType (UPHOLDS, OVERRULES, DISTINGUISHES, or CITES).

            Citations to classify:
            {citationsJson}

            Ruling text (excerpt around citations):
            ---
            {truncated}
            ---
            """;
    }

    /// <summary>
    /// JSON schema for response_format (strict mode).
    /// </summary>
    public const string JsonSchema = """
        {
          "type": "object",
          "properties": {
            "citationTypes": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "alias": { "type": "string" },
                  "citationType": { "type": "string", "enum": ["UPHOLDS", "OVERRULES", "DISTINGUISHES", "CITES"] }
                },
                "required": ["alias", "citationType"],
                "additionalProperties": false
              }
            }
          },
          "required": ["citationTypes"],
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
