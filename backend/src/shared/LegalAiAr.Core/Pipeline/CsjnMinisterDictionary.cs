namespace LegalAiAr.Core.Pipeline;

/// <summary>
/// Canonical CSJN minister/conjuez lookup. Shared between Parser, Enrichment and Indexer.
/// Keyed by the stable ministros[].id from votosAnalisisDocumental in abrirAnalisis.
/// Source: sjconsulta.csjn.gov.ar (search filter) + abrirAnalisis API samples (1811 fallos destacados).
/// </summary>
public static class CsjnMinisterDictionary
{
    public static readonly Dictionary<int, (string FirstName, string LastName, bool IsTitular)> Ministers = new()
    {
        // ── Ministros titulares (confirmados en filtro de búsqueda CSJN) ──
        [1]   = ("Ricardo Luis",    "Lorenzetti",         true),
        [2]   = ("Elena Ines",      "Highton de Nolasco", true),
        [3]   = ("Carlos Santiago",  "Fayt",               true),
        [4]   = ("Enrique Santiago", "Petracchi",          true),
        [5]   = ("Juan Carlos",     "Maqueda",             true),
        [6]   = ("Carmen Maria",    "Argibay",             true),
        [7]   = ("Eugenio Raul",   "Zaffaroni",           true),
        [68]  = ("Horacio Daniel",  "Rosatti",             true),
        [69]  = ("Carlos Fernando", "Rosenkrantz",         true),
        [241] = ("Manuel Jose",     "Garcia-Mansilla",     true),

        // ── Conjueces (extraídos de ministros[] en 1811 fallos destacados) ──
        [42]  = ("",  "Tazza",                  false),
        [72]  = ("",  "Sotelo de Andreau",      false),
        [73]  = ("",  "Irurzun",                false),
        [80]  = ("",  "Montesi",                false),
        [82]  = ("",  "Cossio",                 false),
        [83]  = ("",  "Moran",                  false),
        [85]  = ("",  "Gonzalez",               false),
        [87]  = ("",  "Antelo",                 false),
        [88]  = ("",  "Tyden",                  false),
        [102] = ("",  "Aranguren",              false),
        [163] = ("",  "Amabile",                false),
        [172] = ("",  "Rabbi-Baldi Cabanillas", false),
        [185] = ("",  "Boldu",                  false),
        [193] = ("",  "Alcala",                 false),
        [194] = ("",  "Bruglia",                false),
        [201] = ("",  "Castellanos",            false),
        [217] = ("",  "Borinsky",               false),
        [219] = ("",  "Bertuzzi",               false),
        [221] = ("",  "Moran",                  false),
        [227] = ("",  "Lozano",                 false),
        [229] = ("",  "Hornos",                 false),
        [230] = ("",  "Perez Tognola",          false),
        [231] = ("",  "Corcuera",               false),
        [232] = ("",  "Sanchez Torres",         false),
        [237] = ("",  "Tazza",                  false),
        [238] = ("",  "Andalaf Casiello",       false),
        [240] = ("",  "Candisano Mera",         false),
        [244] = ("",  "Moltini",                false),
        [245] = ("",  "Llorens",                false),
        [248] = ("",  "Castineira de Dios",     false),
        [250] = ("",  "Catalano",               false),
        [251] = ("",  "Bejas",                  false),
        [253] = ("",  "Perozziello Vizier",     false),
    };

    public static string ResolveFullName(int ministroId, string fallbackSurname)
    {
        if (Ministers.TryGetValue(ministroId, out var canonical))
            return $"{canonical.FirstName} {canonical.LastName}".Trim();

        return CleanConjuezSuffix(fallbackSurname);
    }

    public static (string FirstName, string LastName) ResolveSplit(int ministroId, string fallbackSurname)
    {
        if (Ministers.TryGetValue(ministroId, out var canonical))
            return (canonical.FirstName, canonical.LastName);

        return ("", CleanConjuezSuffix(fallbackSurname));
    }

    private static string CleanConjuezSuffix(string name)
    {
        var cleaned = name.Trim();
        var idx = cleaned.IndexOf('(');
        if (idx > 0)
            cleaned = cleaned[..idx].Trim();
        return cleaned;
    }
}
