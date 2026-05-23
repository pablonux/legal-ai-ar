using LegalAiAr.Infrastructure.Thesaurus;

namespace LegalAiAr.Infrastructure.Tests.Thesaurus;

public class ThesaurusLabelNormalizerTests
{
    [Theory]
    [InlineData("  a  b  ", "A B")]
    [InlineData("Niño", "NINO")]
    [InlineData("PRISIÓN PREVENTIVA", "PRISION PREVENTIVA")]
    public void NormalizeForLookup_CollapsesWhitespaceAndStripsCombiningMarks(string input, string expected)
    {
        Assert.Equal(expected, ThesaurusLabelNormalizer.NormalizeForLookup(input));
    }

    [Fact]
    public void NormalizeForLookup_WhenNullOrWhitespace_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, ThesaurusLabelNormalizer.NormalizeForLookup(null));
        Assert.Equal(string.Empty, ThesaurusLabelNormalizer.NormalizeForLookup("   "));
    }
}
