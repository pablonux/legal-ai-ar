using LegalAiAr.Application.Chat.Utilities;

namespace LegalAiAr.Application.Tests.Chat.Utilities;

public class CitationParserTests
{
    private static readonly Guid Guid1 = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
    private static readonly Guid Guid2 = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");

    [global::Xunit.Fact]
    public void Parse_ValidCitation_ReturnsCitation()
    {
        var citationText = @"{caso: ""Ekmekdjian c/ Sofovich"", id: ""a1b2c3d4-e5f6-7890-abcd-ef1234567890""}";
        var results = CitationParser.Parse(citationText);

        Assert.Single(results);
        var c = results[0];
        Assert.Equal("Ekmekdjian c/ Sofovich", c.CaseTitle);
        Assert.Equal(Guid1, c.RulingId);
        Assert.Equal(0, c.Offset);
        Assert.Equal(citationText.Length, c.Length);
    }

    [global::Xunit.Fact]
    public void Parse_MultipleCitations_ReturnsAll()
    {
        var a = @"{caso: ""First"", id: ""a1b2c3d4-e5f6-7890-abcd-ef1234567890""}";
        var b = @"{caso: ""Second"", id: ""b2c3d4e5-f6a7-8901-bcde-f12345678901""}";
        var text = $"Prefix {a} mid {b} tail";
        var results = CitationParser.Parse(text);

        Assert.Equal(2, results.Count);
        Assert.Equal("First", results[0].CaseTitle);
        Assert.Equal(Guid1, results[0].RulingId);
        Assert.Equal("Second", results[1].CaseTitle);
        Assert.Equal(Guid2, results[1].RulingId);
        Assert.Equal(text.IndexOf(a, StringComparison.Ordinal), results[0].Offset);
        Assert.Equal(text.IndexOf(b, StringComparison.Ordinal), results[1].Offset);
    }

    [global::Xunit.Fact]
    public void Parse_NoCitations_ReturnsEmpty()
    {
        var results = CitationParser.Parse("Plain text without citations.");

        Assert.Empty(results);
    }

    [global::Xunit.Fact]
    public void Parse_MalformedGuid_SkipsCitation()
    {
        var results = CitationParser.Parse(@"{caso: ""Test"", id: ""not-a-guid""}");

        Assert.Empty(results);
    }

    [global::Xunit.Fact]
    public void Normalize_SingleQuotes_ConvertsToDouble()
    {
        var input = @"{caso: 'Test', id: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890'}";
        var expected = @"{caso: ""Test"", id: ""a1b2c3d4-e5f6-7890-abcd-ef1234567890""}";

        var actual = CitationParser.Normalize(input);

        Assert.Equal(expected, actual);
    }

    [global::Xunit.Fact]
    public void Normalize_AlreadyDouble_Unchanged()
    {
        var text = @"{caso: ""Test"", id: ""a1b2c3d4-e5f6-7890-abcd-ef1234567890""}";

        var actual = CitationParser.Normalize(text);

        Assert.Equal(text, actual);
    }

    [global::Xunit.Fact]
    public void Validate_IdExistsAndMatches_ReturnsNone()
    {
        var citation = new Citation("Ekmekdjian c/ Sofovich", Guid1, 0, 1);
        var known = new Dictionary<Guid, string> { [Guid1] = "Ekmekdjian c/ Sofovich" };
        var resolved = new HashSet<Guid> { Guid1 };

        var result = CitationParser.Validate(citation, known, resolved);

        Assert.Equal(CitationSeverity.None, result.Severity);
        Assert.Empty(result.Issues);
    }

    [global::Xunit.Fact]
    public void Validate_IdNotInDb_ReturnsCritical()
    {
        var citation = new Citation("Any", Guid1, 0, 1);
        var known = new Dictionary<Guid, string>();
        var resolved = new HashSet<Guid> { Guid1 };

        var result = CitationParser.Validate(citation, known, resolved);

        Assert.Equal(CitationSeverity.Critical, result.Severity);
        Assert.Single(result.Issues);
        Assert.Contains("not found", result.Issues[0], StringComparison.OrdinalIgnoreCase);
    }

    [global::Xunit.Fact]
    public void Validate_TitleMismatch_ReturnsWarning()
    {
        var citation = new Citation("ZZZZZZZZZZZZZZZZ", Guid1, 0, 1);
        var known = new Dictionary<Guid, string> { [Guid1] = "Ekmekdjian c/ Sofovich" };
        var resolved = new HashSet<Guid> { Guid1 };

        var result = CitationParser.Validate(citation, known, resolved);

        Assert.Equal(CitationSeverity.Warning, result.Severity);
        Assert.Contains(result.Issues, i => i.Contains("mismatch", StringComparison.OrdinalIgnoreCase));
    }

    [global::Xunit.Fact]
    public void Validate_NotToolGrounded_ReturnsWarning()
    {
        var citation = new Citation("Ekmekdjian c/ Sofovich", Guid1, 0, 1);
        var known = new Dictionary<Guid, string> { [Guid1] = "Ekmekdjian c/ Sofovich" };
        var resolved = new HashSet<Guid>();

        var result = CitationParser.Validate(citation, known, resolved);

        Assert.Equal(CitationSeverity.Warning, result.Severity);
        Assert.Contains(result.Issues, i => i.Contains("tool", StringComparison.OrdinalIgnoreCase));
    }

    [global::Xunit.Fact]
    public void FuzzyMatchRatio_IdenticalStrings_ReturnsOne()
    {
        var ratio = CitationParser.FuzzyMatchRatio("Same", "Same");

        Assert.Equal(1.0, ratio);
    }

    [global::Xunit.Fact]
    public void FuzzyMatchRatio_CompletelyDifferent_ReturnsLow()
    {
        var ratio = CitationParser.FuzzyMatchRatio("aaaaaa", "xxxxxx");

        Assert.True(ratio < 0.5);
    }
}
