namespace LegalAiAr.Worker.Indexer.Prompts;

/// <summary>
/// Prompt for LLM-based chunk contextualization (Contextual Retrieval).
/// Generates a brief description that situates a chunk within its parent ruling,
/// improving semantic search recall vs a simple metadata prefix.
/// </summary>
public static class ChunkContextualizationPrompt
{
    public const string SchemaName = "chunk_context";

    public const string SystemPrompt = """
        Sos un analista juridico argentino. Dada una sentencia judicial y un fragmento especifico de esa sentencia, genera una descripcion contextual breve (2-3 oraciones) que situe el fragmento dentro del documento. Incluye: de que trata la sentencia, que tema/parte/cuestion aborda este fragmento, y que conceptos juridicos se discuten. Responde en castellano.
        """;

    public static string BuildUserPrompt(
        string? caseTitle,
        string? summary,
        string? holding,
        string? court,
        DateOnly rulingDate,
        string chunkText)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(court))
            parts.Add($"Tribunal: {court}");
        parts.Add($"Fecha: {rulingDate:yyyy-MM-dd}");
        if (!string.IsNullOrWhiteSpace(caseTitle))
            parts.Add($"Caratula: {Truncate(caseTitle, 200)}");
        if (!string.IsNullOrWhiteSpace(summary))
            parts.Add($"Resumen: {Truncate(summary, 500)}");
        if (!string.IsNullOrWhiteSpace(holding))
            parts.Add($"Holding: {Truncate(holding, 500)}");

        var ctx = string.Join("\n", parts);
        var truncatedChunk = Truncate(chunkText, 2000);

        return $"""
            Sentencia:
            {ctx}

            Fragmento a contextualizar:
            ---
            {truncatedChunk}
            ---

            Genera la descripcion contextual breve para este fragmento.
            """;
    }

    public const string JsonSchema = """
        {
          "type": "object",
          "properties": {
            "context": { "type": "string" }
          },
          "required": ["context"],
          "additionalProperties": false
        }
        """;

    private static string Truncate(string text, int maxChars) =>
        text.Length <= maxChars ? text : text[..maxChars] + "...";
}
