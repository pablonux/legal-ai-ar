namespace LegalAiAr.Worker.Enrichment.Prompts;

/// <summary>
/// System and user prompt templates for extracting Procurador General opinions from CSJN rulings.
/// </summary>
public static class ProsecutorOpinionPrompt
{
    public const string SchemaName = "prosecutor_opinion";

    public const string SystemPrompt = """
        Sos un analista de documentos jurídicos argentinos. Dado el texto de un fallo de la CSJN, determiná si contiene un dictamen del Procurador General / Fiscal de la Nación. Si existe, extraé la información solicitada. Si el texto no contiene dictamen del Procurador, retorná hasDictamen: false y dejá los demás campos vacíos.

        Guías:
        - El dictamen suele aparecer al inicio del texto, antes del "Y VISTOS" o "AUTOS Y VISTOS".
        - Buscá frases como "El Procurador General", "El señor Procurador Fiscal", "Dictamen del Procurador", "A fs. ... obra el dictamen".
        - prosecutorName: nombre completo del procurador/fiscal.
        - summary: resumen de 2-3 oraciones de la opinión del procurador.
        - recommendedDirection: qué recomendó (ej. "hacer lugar al recurso", "rechazar la queja", "declarar procedente").
        - agreedWithCourt: true si la recomendación del procurador coincide con la decisión final de la Corte (misma dirección: ambos hacen lugar, ambos rechazan, etc.), false si difieren o no se puede determinar.
        - Si no podés determinar algún campo con certeza, dejalo como string vacío (o false para agreedWithCourt).
        """;

    public static string BuildUserPrompt(string normalizedText)
    {
        var truncated = TruncateForContext(normalizedText, 12000);
        return $"""
            Analizá el siguiente texto de un fallo de la CSJN y determiná si contiene un dictamen del Procurador General / Fiscal. Retorná un objeto JSON según el schema.

            Texto del fallo:
            ---
            {truncated}
            ---
            """;
    }

    public const string JsonSchema = """
        {
          "type": "object",
          "properties": {
            "hasDictamen": { "type": "boolean" },
            "prosecutorName": { "type": "string" },
            "summary": { "type": "string" },
            "recommendedDirection": { "type": "string" },
            "agreedWithCourt": { "type": "boolean" }
          },
          "required": ["hasDictamen", "prosecutorName", "summary", "recommendedDirection", "agreedWithCourt"],
          "additionalProperties": false
        }
        """;

    private static string TruncateForContext(string text, int maxChars)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxChars)
            return text;
        return text[..maxChars] + "\n\n[... truncado ...]";
    }
}
