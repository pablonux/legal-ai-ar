using LegalAiAr.Application.Proceedings.Queries;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Proceedings.Queries;

public class GetProceedingByRulingHandlerTests
{
    private readonly IJudicialProceedingRepository _repo = Substitute.For<IJudicialProceedingRepository>();
    private readonly GetProceedingByRulingHandler _sut;

    public GetProceedingByRulingHandlerTests()
    {
        _sut = new GetProceedingByRulingHandler(_repo);
    }

    [Fact]
    public async Task Handle_WhenNoProceedingFound_ReturnsNull()
    {
        var rulingId = Guid.NewGuid();
        _repo.GetByRulingIdAsync(rulingId, Arg.Any<CancellationToken>()).Returns((JudicialProceeding?)null);

        var result = await _sut.Handle(new GetProceedingByRulingQuery(rulingId), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenProceedingExists_ReturnsMappedResponse()
    {
        var rulingId = Guid.NewGuid();
        var court = new Court { Id = 1, Name = "CSJN", JurisdictionArea = "Federal", Territory = "Nacional", Instance = "CSJN", InstanceLevel = 3 };
        var proceeding = new JudicialProceeding
        {
            Id = 42,
            CaseNumber = "CSJ 123/2024",
            DisplayName = "Test v. Test",
            JurisdictionArea = "Federal",
            Rulings = new List<Ruling>
            {
                new() { Id = rulingId, CaseTitle = "Current Ruling", RulingDate = new DateOnly(2024, 6, 1), Court = court, RulingDirection = "Confirma" },
                new() { Id = Guid.NewGuid(), CaseTitle = "Earlier Ruling", RulingDate = new DateOnly(2023, 3, 15), Court = new Court { Id = 2, Name = "Cámara", JurisdictionArea = "Federal", Territory = "Nacional", Instance = "Cámara", InstanceLevel = 2 } },
            }
        };
        _repo.GetByRulingIdAsync(rulingId, Arg.Any<CancellationToken>()).Returns(proceeding);

        var result = await _sut.Handle(new GetProceedingByRulingQuery(rulingId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(42, result!.Id);
        Assert.Equal("CSJ 123/2024", result.CaseNumber);
        Assert.Equal(2, result.Rulings.Count);
        Assert.Contains(result.Rulings, r => r.IsCurrent && r.RulingId == rulingId);
    }

    [Fact]
    public async Task Handle_RulingsOrderedByInstanceLevelThenDate()
    {
        var rulingId = Guid.NewGuid();
        var court1 = new Court { Id = 1, Name = "Juzgado", JurisdictionArea = "Federal", Territory = "Nacional", Instance = "1ra", InstanceLevel = 1 };
        var court2 = new Court { Id = 2, Name = "Cámara", JurisdictionArea = "Federal", Territory = "Nacional", Instance = "2da", InstanceLevel = 2 };
        var court3 = new Court { Id = 3, Name = "CSJN", JurisdictionArea = "Federal", Territory = "Nacional", Instance = "CSJN", InstanceLevel = 3 };

        var proceeding = new JudicialProceeding
        {
            Id = 1,
            CaseNumber = "EXP-1",
            Rulings = new List<Ruling>
            {
                new() { Id = Guid.NewGuid(), CaseTitle = "Third", RulingDate = new DateOnly(2025, 1, 1), Court = court3 },
                new() { Id = rulingId, CaseTitle = "First", RulingDate = new DateOnly(2023, 1, 1), Court = court1 },
                new() { Id = Guid.NewGuid(), CaseTitle = "Second", RulingDate = new DateOnly(2024, 1, 1), Court = court2 },
            }
        };
        _repo.GetByRulingIdAsync(rulingId, Arg.Any<CancellationToken>()).Returns(proceeding);

        var result = await _sut.Handle(new GetProceedingByRulingQuery(rulingId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("First", result!.Rulings[0].CaseTitle);
        Assert.Equal("Second", result.Rulings[1].CaseTitle);
        Assert.Equal("Third", result.Rulings[2].CaseTitle);
    }
}
