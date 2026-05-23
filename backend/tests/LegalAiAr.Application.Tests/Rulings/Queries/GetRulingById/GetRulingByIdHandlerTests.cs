using LegalAiAr.Application.Rulings.Queries.GetRulingById;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Rulings.Queries.GetRulingById;

public class GetRulingByIdHandlerTests
{
    [Fact]
    public async Task Handle_RulingExists_ReturnsRulingDto()
    {
        var rulingRepo = Substitute.For<IRulingRepository>();

        var rulingId = Guid.NewGuid();
        var ruling = new Ruling
        {
            Id = rulingId,
            SourceId = 1,
            ExternalId = "ext-1",
            CaseTitle = "Test Case Title",
            CaseNumber = "CAF 123/2024",
            RulingDate = DateOnly.FromDateTime(DateTime.Today),
            Court = new Court
            {
                Id = 1,
                Name = "Corte Suprema",
                JurisdictionArea = "Nacional",
                Territory = "Nacional",
                Instance = "CSJN"
            },
            IndexedAt = DateTime.UtcNow,
            Status = RulingStatus.Indexed,
            RulingParticipations = [],
            RulingKeywords = [],
            RulingStatutes = [],
            OutboundCitations = []
        };

        rulingRepo.GetByIdAsync(rulingId, Arg.Any<CancellationToken>()).Returns(ruling);

        var sut = new GetRulingByIdHandler(rulingRepo);
        var query = new GetRulingByIdQuery(rulingId);

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.Equal(rulingId, result.Id);
        Assert.Equal("Test Case Title", result.CaseTitle);
        Assert.Equal("Corte Suprema", result.Court.Name);
    }

    [Fact]
    public async Task Handle_RulingNotFound_ThrowsNotFoundException()
    {
        var rulingRepo = Substitute.For<IRulingRepository>();

        rulingRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Ruling?)null);

        var sut = new GetRulingByIdHandler(rulingRepo);
        var query = new GetRulingByIdQuery(Guid.NewGuid());

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => sut.Handle(query, CancellationToken.None));

        Assert.Equal("Ruling not found.", ex.Message);
    }
}
