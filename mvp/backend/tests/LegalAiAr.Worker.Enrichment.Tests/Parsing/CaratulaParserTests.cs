using LegalAiAr.Worker.Enrichment.Parsing;

namespace LegalAiAr.Worker.Enrichment.Tests.Parsing;

public class CaratulaParserTests
{
    [Theory]
    [InlineData("Fernández, Juan c/ Estado Nacional s/ daños y perjuicios",
        "Fernández, Juan", "PLAINTIFF", "Estado Nacional", "DEFENDANT")]
    [InlineData("YPF S.A. c/ Provincia de Neuquén s/ acción declarativa",
        "YPF S.A", "PLAINTIFF", "Provincia de Neuquén", "DEFENDANT")]
    [InlineData("Rodríguez c/ AFIP s/ recurso directo",
        "Rodríguez", "PLAINTIFF", "AFIP", "DEFENDANT")]
    [InlineData("García, María c. Municipalidad de Córdoba s/ amparo",
        "García, María", "PLAINTIFF", "Municipalidad de Córdoba", "DEFENDANT")]
    public void ExtractParties_StandardCvPattern_ExtractsPlaintiffAndDefendant(
        string caseTitle, string expectedPlaintiff, string expectedPlaintiffRole,
        string expectedDefendant, string expectedDefendantRole)
    {
        var parties = CaratulaParser.ExtractParties(caseTitle);

        Assert.Equal(2, parties.Count);
        Assert.Equal(expectedPlaintiff, parties[0].Name);
        Assert.Equal(expectedPlaintiffRole, parties[0].PartyRole);
        Assert.Equal(expectedDefendant, parties[1].Name);
        Assert.Equal(expectedDefendantRole, parties[1].PartyRole);
    }

    [Fact]
    public void ExtractParties_LabeledPattern_ExtractsParties()
    {
        var title = "ACTOR: González, Pedro - DEMANDADO: Banco de la Nación Argentina";
        var parties = CaratulaParser.ExtractParties(title);

        Assert.Equal(2, parties.Count);
        Assert.Equal("González, Pedro", parties[0].Name);
        Assert.Equal("PLAINTIFF", parties[0].PartyRole);
        Assert.Equal("Banco de la Nación Argentina", parties[1].Name);
        Assert.Equal("DEFENDANT", parties[1].PartyRole);
    }

    [Fact]
    public void ExtractParties_RecurrenteLabel_ExtractsAsPlaintiff()
    {
        var title = "RECURRENTE: Martínez, Ana - RECURRIDA: ANSES";
        var parties = CaratulaParser.ExtractParties(title);

        Assert.Equal(2, parties.Count);
        Assert.Equal("Martínez, Ana", parties[0].Name);
        Assert.Equal("PLAINTIFF", parties[0].PartyRole);
        Assert.Equal("ANSES", parties[1].Name);
        Assert.Equal("DEFENDANT", parties[1].PartyRole);
    }

    [Fact]
    public void ExtractParties_NullOrEmpty_ReturnsEmptyList()
    {
        Assert.Empty(CaratulaParser.ExtractParties(null));
        Assert.Empty(CaratulaParser.ExtractParties(""));
        Assert.Empty(CaratulaParser.ExtractParties("  "));
    }

    [Fact]
    public void ExtractParties_NoPattern_ReturnsEmptyList()
    {
        Assert.Empty(CaratulaParser.ExtractParties("Acordada 5/2024"));
    }

    [Theory]
    [InlineData("YPF S.A.", "Legal")]
    [InlineData("Estado Nacional", "Legal")]
    [InlineData("AFIP", "Legal")]
    [InlineData("Municipalidad de Buenos Aires", "Legal")]
    [InlineData("García, Juan Carlos", "Physical")]
    [InlineData("Fernández", "Physical")]
    public void ClassifyPersonType_CorrectlyClassifiesEntities(string name, string expected)
    {
        Assert.Equal(expected, CaratulaParser.ClassifyPersonType(name));
    }

    [Fact]
    public void ExtractParties_ContraVariant_Works()
    {
        var title = "López contra Banco Central de la República Argentina s/ amparo";
        var parties = CaratulaParser.ExtractParties(title);

        Assert.Equal(2, parties.Count);
        Assert.Equal("López", parties[0].Name);
        Assert.Equal("PLAINTIFF", parties[0].PartyRole);
        Assert.Contains("Banco Central", parties[1].Name);
        Assert.Equal("DEFENDANT", parties[1].PartyRole);
    }
}
