using LegalAiAr.Core.Interfaces.Pipeline;
using UglyToad.PdfPig;

namespace LegalAiAr.Worker.Parser.Parsers;

/// <summary>
/// Extracts text from PDF using PdfPig and normalizes it via ITextNormalizer.
/// </summary>
public class PdfTextExtractor
{
    private readonly ITextNormalizer _normalizer;

    public PdfTextExtractor(ITextNormalizer normalizer)
    {
        _normalizer = normalizer;
    }

    /// <summary>
    /// Extracts text from the PDF stream and returns normalized content.
    /// </summary>
    /// <param name="pdfStream">PDF content stream (must be seekable). Caller owns disposal.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Normalized full text of the document.</returns>
    public async Task<string> ExtractAsync(Stream pdfStream, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask; // PdfPig is synchronous; allow async signature for future streaming

        Stream? tempStream = null;
        var stream = pdfStream.CanSeek ? pdfStream : (tempStream = await CopyToMemoryStreamAsync(pdfStream, cancellationToken));

        try
        {
            using var document = PdfDocument.Open(stream);
            var pages = new List<string>();

            foreach (var page in document.GetPages())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var pageText = page.Text ?? string.Empty;
                pages.Add(pageText);
            }

            var rawText = string.Join("\n\n", pages);
            return _normalizer.Normalize(rawText);
        }
        finally
        {
            tempStream?.Dispose();
        }
    }

    private static async Task<MemoryStream> CopyToMemoryStreamAsync(Stream source, CancellationToken ct)
    {
        var ms = new MemoryStream();
        await source.CopyToAsync(ms, ct);
        ms.Position = 0;
        return ms;
    }
}
