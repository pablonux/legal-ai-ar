using System.Text.RegularExpressions;
using LegalAiAr.Core.Interfaces.Pipeline;

namespace LegalAiAr.Worker.Parser.Parsers;

/// <summary>
/// Normalizes PDF-extracted text per E040: collapse spaces, fix spaced text,
/// strip image headers, normalize line breaks, trim whitespace.
/// </summary>
public class PdfTextNormalizer : ITextNormalizer
{
    // Unify line break types to \n without collapsing (2.4)
    private static readonly Regex LineBreakRegex = new(@"\r\n|\r|\f|\v", RegexOptions.Compiled);
    private static readonly Regex MultipleNewlinesRegex = new(@"\n{3,}", RegexOptions.Compiled);
    private static readonly Regex MultipleSpacesRegex = new(@"[ \t\u00A0]{2,}", RegexOptions.Compiled);
    // Spaced single letters (e.g. "S u p r e m a") -> collapse to word; break at lowercase+space+uppercase (word boundary)
    private static readonly Regex SpacedTextRegex = new(@"(?<![A-Za-z])([A-Za-z])(?: ([A-Za-z]))*(?![A-Za-z])", RegexOptions.Compiled);
    private static readonly Regex WordBoundaryRegex = new(@"([a-z]) ([A-Z])", RegexOptions.Compiled);

    private static readonly Regex[] HeaderPatterns =
    [
        new Regex(@"^\s*Página\s+\d+\s*$", RegexOptions.Multiline | RegexOptions.Compiled),
        new Regex(@"^\s*https?://[\w.-]+\s*$", RegexOptions.Multiline | RegexOptions.Compiled),
        new Regex(@"^\s*www\.\S+\s*$", RegexOptions.Multiline | RegexOptions.Compiled),
    ];

    /// <inheritdoc />
    public string Normalize(string rawText)
    {
        if (string.IsNullOrEmpty(rawText))
            return string.Empty;

        var text = rawText;

        // 1. Normalize line breaks (2.4)
        text = LineBreakRegex.Replace(text, "\n");

        // 2. Remove image headers (2.3)
        text = RemoveHeaderLines(text);

        // 3. Collapse multiple newlines (2.5)
        text = MultipleNewlinesRegex.Replace(text, "\n\n");

        // 4. Collapse multiple spaces (2.1)
        text = MultipleSpacesRegex.Replace(text, " ");

        // 5. Fix spaced text (2.2) — "S u p r e m a" -> "Suprema"; break at lowercase+space+uppercase (e.g. "a C")
        const char boundary = '\uFFFF';
        text = WordBoundaryRegex.Replace(text, m => m.Groups[1].Value + boundary + m.Groups[2].Value);
        text = string.Join(" ", text.Split(boundary).Select(part => SpacedTextRegex.Replace(part, m => m.Value.Replace(" ", ""))));

        // 6. Trim whitespace (2.6) — trim each line, then whole text
        text = string.Join("\n", text.Split('\n').Select(l => l.Trim()));
        text = text.Trim();

        return text;
    }

    private static string RemoveHeaderLines(string text)
    {
        var lines = text.Split('\n');
        var result = new List<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                result.Add(line);
                continue;
            }

            var isHeader = HeaderPatterns.Any(p => p.IsMatch(trimmed));
            if (!isHeader)
                result.Add(line);
        }

        return string.Join("\n", result);
    }
}
