using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LegalAiAr.Infrastructure.Thesaurus;

/// <summary>
/// Canonical form for matching keyword descriptions to in-memory thesaurus labels and UF synonyms.
/// Collapses whitespace, strips combining marks (accents), uppercases — aligned with common payload vs DB drift.
/// </summary>
public static class ThesaurusLabelNormalizer
{
    private static readonly Regex WhitespaceRuns = new(@"\s+", RegexOptions.Compiled);

    /// <summary>
    /// Returns a stable lookup key, or <see cref="string.Empty"/> when the label has no comparable characters.
    /// </summary>
    public static string NormalizeForLookup(string? label)
    {
        if (string.IsNullOrWhiteSpace(label))
            return string.Empty;

        var collapsed = WhitespaceRuns.Replace(label.Trim(), " ");
        if (collapsed.Length == 0)
            return string.Empty;

        var formD = collapsed.Normalize(NormalizationForm.FormD);
        var stripped = new StringBuilder(formD.Length);
        foreach (var ch in formD)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                stripped.Append(ch);
        }

        var formC = stripped.ToString().Normalize(NormalizationForm.FormC);
        return formC.Length == 0 ? string.Empty : formC.ToUpperInvariant();
    }
}
