namespace LegalAiAr.Worker.Enrichment.Prompts;

/// <summary>
/// System and user prompt templates for judge extraction (E049).
/// </summary>
public static class JudgesExtractionPrompt
{
    public const string SchemaName = "judges_extraction";

    public const string SystemPrompt = """
        You are a legal document analyst for Argentine court rulings. Extract the list of judges who participated in the ruling. For each judge, provide firstName, lastName, and participationType. participationType must be one of: SIGNATORY (firmante), DISSENT (disidente), MAJORITY (mayoría). Use SIGNATORY for judges who signed the majority opinion. Use DISSENT for judges who wrote or joined a dissenting opinion. Use MAJORITY when the judge voted with the majority but did not sign. If uncertain, use SIGNATORY. Return only valid JSON matching the schema. Do not include explanatory text.
        """;

    public static string BuildUserPrompt(string normalizedText)
    {
        var truncated = TruncateForContext(normalizedText, 8000);
        return $"""
            Extract the judges from the following Argentine court ruling text. Return a JSON object with a "judges" array. Each judge must have firstName, lastName, and participationType.

            Ruling text:
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
            "judges": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "firstName": { "type": "string" },
                  "lastName": { "type": "string" },
                  "participationType": { "type": "string", "enum": ["SIGNATORY", "DISSENT", "MAJORITY"] }
                },
                "required": ["firstName", "lastName", "participationType"],
                "additionalProperties": false
              }
            }
          },
          "required": ["judges"],
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
