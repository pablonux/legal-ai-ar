using LegalAiAr.Contracts.Responses.Rulings;
using LegalAiAr.Application.Rulings.Queries.GetRelatedRulings;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Rulings.Queries.GetRelatedRulings;

public class GetRelatedRulingsHandlerTests
{
    [Fact]
    public async Task Handle_RulingExists_ReturnsRelatedRulings()
    {
        var search = Substitute.For<ISearchService>();
        var rulingRepo = Substitute.For<IRulingRepository>();

        var rulingId = Guid.NewGuid();
        var relatedId = Guid.NewGuid();
        var ruling = new Ruling { Id = rulingId };
        rulingRepo.GetByIdAsync(rulingId, Arg.Any<CancellationToken>()).Returns(ruling);

        var relatedItems = new List<SearchResultItem>
        {
            new(
                relatedId,
                "Related Case",
                null,
                null,
                null,
                DateOnly.FromDateTime(DateTime.Today),
                "CONTENCIOSO",
                "CSJN",
                "Court",
                Array.Empty<string>(),
                null,
                0.92)
        };
        search.SearchRelatedAsync(rulingId, 10, Arg.Any<CancellationToken>()).Returns(relatedItems);

        var sut = new GetRelatedRulingsHandler(search, rulingRepo);
        var query = new GetRelatedRulingsQuery(rulingId, 10);

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Related Case", result[0].CaseTitle);
        Assert.Equal(0.92, result[0].SimilarityScore);
    }

    [Fact]
    public async Task Handle_RulingNotFound_ThrowsNotFoundException()
    {
        var search = Substitute.For<ISearchService>();
        var rulingRepo = Substitute.For<IRulingRepository>();

        rulingRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Ruling?)null);

        var sut = new GetRelatedRulingsHandler(search, rulingRepo);
        var query = new GetRelatedRulingsQuery(Guid.NewGuid(), 10);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => sut.Handle(query, CancellationToken.None));

        Assert.Equal("Ruling not found.", ex.Message);
        await search.DidNotReceive().SearchRelatedAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LimitExceedsMax_ClampsToMaxLimit()
    {
        var search = Substitute.For<ISearchService>();
        var rulingRepo = Substitute.For<IRulingRepository>();

        var rulingId = Guid.NewGuid();
        rulingRepo.GetByIdAsync(rulingId, Arg.Any<CancellationToken>()).Returns(new Ruling { Id = rulingId });
        search.SearchRelatedAsync(rulingId, 20, Arg.Any<CancellationToken>()).Returns(Array.Empty<SearchResultItem>());

        var sut = new GetRelatedRulingsHandler(search, rulingRepo);
        var query = new GetRelatedRulingsQuery(rulingId, 100);

        await sut.Handle(query, CancellationToken.None);

        await search.Received(1).SearchRelatedAsync(rulingId, 20, Arg.Any<CancellationToken>());
    }
}
