namespace LegalAiAr.Worker.Enrichment.Prompts;

/// <summary>
/// System and user prompt templates for statute extraction (E049).
/// </summary>
public static class StatutesExtractionPrompt
{
    public const string SchemaName = "statutes_extraction";

    public const string SystemPrompt = """
        You are a legal document analyst for Argentine court rulings. Extract all laws, decrees, and regulations cited in the ruling. For each statute, provide: number (e.g. "24.767", "11.683"), name (official or common name), and articles (specific articles cited, e.g. "art. 80, art. 64" or null if not specified). Use Argentine legal citation format. Return only valid JSON matching the schema. Do not include explanatory text.
        """;

    public static string BuildUserPrompt(string normalizedText)
    {
        var truncated = TruncateForContext(normalizedText, 8000);
        return $"""
            Extract the laws and regulations cited in the following Argentine court ruling. Return a JSON object with a "statutes" array. Each statute must have number, name, and articles (or null).

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
            "statutes": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "number": { "type": "string" },
                  "name": { "type": "string" },
                  "articles": { "type": ["string", "null"] }
                },
                "required": ["number", "name", "articles"],
                "additionalProperties": false
              }
            }
          },
          "required": ["statutes"],
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
