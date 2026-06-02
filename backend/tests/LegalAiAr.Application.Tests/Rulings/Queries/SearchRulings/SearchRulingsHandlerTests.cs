using LegalAiAr.Contracts.Responses.Rulings;
using LegalAiAr.Application.Rulings.Queries.SearchRulings;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Rulings.Queries.SearchRulings;

public class SearchRulingsHandlerTests
{
    private static ISearchQueryPreprocessor NoOpPreprocessor()
    {
        var pp = Substitute.For<ISearchQueryPreprocessor>();
        pp.PreprocessAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
          .Returns((PreprocessedQuery?)null);
        return pp;
    }

    private static IThesaurusContextProvider NoOpThesaurus()
    {
        var tc = Substitute.For<IThesaurusContextProvider>();
        tc.GetContextAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
          .Returns((string?)null);
        return tc;
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsSearchRulingsResult()
    {
        var search = Substitute.For<ISearchService>();
        var embeddings = Substitute.For<IEmbeddingService>();
        var courtRepo = Substitute.For<ICourtRepository>();
        var logger = Substitute.For<ILogger<SearchRulingsHandler>>();

        var embedding = new float[] { 0.1f };
        embeddings.GenerateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(embedding);

        var rulingId = Guid.NewGuid();
        var items = new List<SearchResultItem>
        {
            new(
                rulingId,
                "Test Case",
                "Summary",
                "Holding",
                "Highlight",
                DateOnly.FromDateTime(DateTime.Today),
                "CONTENCIOSO",
                "CSJN",
                "Court",
                Array.Empty<string>(),
                "CITES",
                0.95)
        };
        search.SearchAsync(Arg.Any<float[]?>(), Arg.Any<string?>(), Arg.Any<SearchFilters?>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new PagedSearchResult(items, 1));

        var sut = new SearchRulingsHandler(search, embeddings, NoOpPreprocessor(), NoOpThesaurus(), courtRepo, logger);
        var query = new SearchRulingsQuery("test query", null, 1, 10);

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Single(result.Results);
        Assert.Equal("Test Case", result.Results[0].CaseTitle);
        Assert.Equal(0.95, result.Results[0].RelevanceScore);
    }

    [Fact]
    public async Task Handle_WithCourtIdFilter_ResolvesCourtNameBeforeSearch()
    {
        var search = Substitute.For<ISearchService>();
        var embeddings = Substitute.For<IEmbeddingService>();
        var courtRepo = Substitute.For<ICourtRepository>();
        var logger = Substitute.For<ILogger<SearchRulingsHandler>>();

        embeddings.GenerateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new float[] { 0.1f });

        var court = new LegalAiAr.Core.Entities.Court
        {
            Id = 1,
            Name = "Corte Suprema",
            JurisdictionArea = "Nacional",
            Territory = "Nacional",
            Instance = "CSJN"
        };
        courtRepo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(court);

        search.SearchAsync(Arg.Any<float[]?>(), Arg.Any<string?>(), Arg.Any<SearchFilters?>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new PagedSearchResult(Array.Empty<SearchResultItem>(), 0));

        var sut = new SearchRulingsHandler(search, embeddings, NoOpPreprocessor(), NoOpThesaurus(), courtRepo, logger);
        var filters = new SearchFilters(CourtId: 1);
        var query = new SearchRulingsQuery("query", filters, 1, 10);

        await sut.Handle(query, CancellationToken.None);

        await search.Received(1).SearchAsync(
            Arg.Any<float[]?>(),
            Arg.Any<string?>(),
            Arg.Is<SearchFilters>(f => f.CourtName == "Corte Suprema"),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CourtNotFound_SkipsCourtFilterAndSearches()
    {
        var search = Substitute.For<ISearchService>();
        var embeddings = Substitute.For<IEmbeddingService>();
        var courtRepo = Substitute.For<ICourtRepository>();
        var logger = Substitute.For<ILogger<SearchRulingsHandler>>();

        embeddings.GenerateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new float[] { 0.1f });

        courtRepo.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((LegalAiAr.Core.Entities.Court?)null);

        search.SearchAsync(Arg.Any<float[]?>(), Arg.Any<string?>(), Arg.Any<SearchFilters?>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new PagedSearchResult(Array.Empty<SearchResultItem>(), 0));

        var sut = new SearchRulingsHandler(search, embeddings, NoOpPreprocessor(), NoOpThesaurus(), courtRepo, logger);
        var filters = new SearchFilters(CourtId: 999);
        var query = new SearchRulingsQuery("query", filters, 1, 10);

        var result = await sut.Handle(query, CancellationToken.None);

        await search.Received(1).SearchAsync(
            Arg.Any<float[]?>(),
            Arg.Any<string?>(),
            Arg.Is<SearchFilters>(f => f.CourtId == null),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>());
        Assert.Equal(0, result.TotalCount);
    }
}
