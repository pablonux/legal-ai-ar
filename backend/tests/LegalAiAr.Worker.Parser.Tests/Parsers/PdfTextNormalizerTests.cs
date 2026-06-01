using LegalAiAr.Worker.Parser.Parsers;

namespace LegalAiAr.Worker.Parser.Tests.Parsers;

public class PdfTextNormalizerTests
{
    private readonly PdfTextNormalizer _sut = new();

    [Fact]
    public void Normalize_NullInput_ReturnsEmptyString()
    {
        var result = _sut.Normalize(null!);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Normalize_EmptyInput_ReturnsEmptyString()
    {
        var result = _sut.Normalize("");

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Normalize_CollapseMultipleSpaces_ReplacesWithSingleSpace()
    {
        var result = _sut.Normalize("Texto   con    espacios");

        Assert.Equal("Texto con espacios", result);
    }

    [Fact]
    public void Normalize_FixSpacedText_CollapsesLetterSpacing()
    {
        var result = _sut.Normalize("S u p r e m a   C o r t e");

        Assert.Equal("Suprema Corte", result);
    }

    [Fact]
    public void Normalize_FixSpacedText_PreservesArt80()
    {
        var result = _sut.Normalize("art. 80 y art. 64");

        Assert.Equal("art. 80 y art. 64", result);
    }

    [Fact]
    public void Normalize_NormalizeLineBreaks_UnifiesToLf()
    {
        var result = _sut.Normalize("Línea1\r\nLínea2\rLínea3");

        Assert.Equal("Línea1\nLínea2\nLínea3", result);
    }

    [Fact]
    public void Normalize_CollapseMultipleNewlines_ReducesToTwo()
    {
        var result = _sut.Normalize("Párrafo1\n\n\n\nPárrafo2");

        Assert.Equal("Párrafo1\n\nPárrafo2", result);
    }

    [Fact]
    public void Normalize_RemoveHeaderLines_StripsPáginaPattern()
    {
        var input = "CORTE SUPREMA\nPágina 1\n\nContenido real aquí.";
        var result = _sut.Normalize(input);

        Assert.DoesNotContain("Página 1", result);
        Assert.Contains("Contenido real aquí", result);
    }

    [Fact]
    public void Normalize_TrimWhitespace_TrimsLeadingAndTrailing()
    {
        var result = _sut.Normalize("  texto  \n  más  ");

        Assert.Equal("texto\nmás", result);
    }

    [Fact]
    public void Normalize_FullPipeline_AppliesAllRules()
    {
        var input = "  S u p r e m a   C o r t e  \r\n\r\n   Texto   con   espacios   ";
        var result = _sut.Normalize(input);

        Assert.Equal("Suprema Corte\n\nTexto con espacios", result);
    }
}
