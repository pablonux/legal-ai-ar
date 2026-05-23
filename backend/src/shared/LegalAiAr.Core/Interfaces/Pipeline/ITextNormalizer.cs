namespace LegalAiAr.Core.Interfaces.Pipeline;

/// <summary>
/// Normalizes extracted PDF text: collapse spaces, fix spaced text, strip image headers, normalize line breaks.
/// </summary>
public interface ITextNormalizer
{
    /// <summary>
    /// Normalizes the raw extracted text.
    /// </summary>
    /// <param name="rawText">Raw text from PdfPig or other extractor.</param>
    /// <returns>Normalized text.</returns>
    string Normalize(string rawText);
}
