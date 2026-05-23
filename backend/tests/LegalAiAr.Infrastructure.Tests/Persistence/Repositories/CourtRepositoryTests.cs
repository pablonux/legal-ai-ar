using LegalAiAr.Infrastructure.Caching;
using LegalAiAr.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace LegalAiAr.Infrastructure.Tests.Persistence.Repositories;

public class CourtRepositoryTests
{
    private static readonly EntityCacheService Cache = new(NullLogger<EntityCacheService>.Instance);

    [Fact]
    public async Task GetByIdAsync_WhenCourtExists_ReturnsCourt()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new CourtRepository(context, Cache);

        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Corte Suprema de Justicia de la Nación", result.Name);
        Assert.Equal("FEDERAL", result.JurisdictionArea);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCourtDoesNotExist_ReturnsNull()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new CourtRepository(context, Cache);

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }
}
