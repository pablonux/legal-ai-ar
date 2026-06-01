using LegalAiAr.Core.Entities;
using LegalAiAr.Infrastructure.Persistence.Repositories;
using LegalAiAr.Infrastructure.Tests.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Tests.Graph;

public class GraphCommunityRepositoryTests
{
    [Fact]
    public async Task AddCommunityAsync_PersistsCommunityWithMemberships()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        var sut = new GraphCommunityRepository(context);

        var community = new GraphCommunity
        {
            Level = 0,
            Title = "Derecho ambiental - contaminación",
            Summary = "Cluster sobre derecho ambiental",
            EntityCount = 5
        };

        await sut.AddCommunityAsync(community);

        var memberships = new List<CommunityMembership>
        {
            new() { CommunityId = community.Id, EntityType = "Ruling", EntityId = Guid.NewGuid().ToString(), Relevance = 1.0f },
            new() { CommunityId = community.Id, EntityType = "Person", EntityId = "42", Relevance = 0.8f }
        };
        await sut.AddMembershipsAsync(memberships);

        var loaded = await sut.GetByIdAsync(community.Id);
        Assert.NotNull(loaded);
        Assert.Equal("Derecho ambiental - contaminación", loaded.Title);
        Assert.Equal(2, loaded.Memberships.Count);
    }

    [Fact]
    public async Task SearchByKeywordAsync_FindsMatchingCommunities()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        var sut = new GraphCommunityRepository(context);

        context.GraphCommunities.Add(new GraphCommunity
        {
            Level = 0,
            Title = "Derecho laboral - despido",
            Summary = "Cluster laboral",
            EntityCount = 10
        });
        context.GraphCommunities.Add(new GraphCommunity
        {
            Level = 1,
            Title = "Derecho tributario",
            Summary = "Ganancias e IVA",
            EntityCount = 20
        });
        await context.SaveChangesAsync();

        var results = await sut.SearchByKeywordAsync("laboral");

        Assert.Single(results);
        Assert.Equal("Derecho laboral - despido", results[0].Title);
    }

    [Fact]
    public async Task ClearAllAsync_RemovesAllCommunitiesAndMemberships()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        var sut = new GraphCommunityRepository(context);

        var community = new GraphCommunity { Level = 0, Title = "Test", Summary = "S", EntityCount = 1 };
        await sut.AddCommunityAsync(community);
        await sut.AddMembershipsAsync(new[]
        {
            new CommunityMembership { CommunityId = community.Id, EntityType = "Ruling", EntityId = "1" }
        });

        await sut.ClearAllAsync();

        Assert.Equal(0, await sut.GetCommunityCountAsync());
        Assert.Empty(await context.CommunityMemberships.ToListAsync());
    }

    [Fact]
    public async Task GetCommunitiesForEntityAsync_FindsCommunityContainingEntity()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        var sut = new GraphCommunityRepository(context);

        var community = new GraphCommunity { Level = 0, Title = "Test", Summary = "S", EntityCount = 3 };
        await sut.AddCommunityAsync(community);
        await sut.AddMembershipsAsync(new[]
        {
            new CommunityMembership { CommunityId = community.Id, EntityType = "Person", EntityId = "42", Relevance = 0.9f }
        });

        var results = await sut.GetCommunitiesForEntityAsync("Person", "42");

        Assert.Single(results);
        Assert.Equal(community.Id, results[0].Id);
    }
}
