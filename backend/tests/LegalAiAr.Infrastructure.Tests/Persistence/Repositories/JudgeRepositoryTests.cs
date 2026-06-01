using LegalAiAr.Core.Entities;
using LegalAiAr.Infrastructure.Caching;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace LegalAiAr.Infrastructure.Tests.Persistence.Repositories;

public class PersonRepositoryTests
{
    private static readonly EntityCacheService Cache = new(NullLogger<EntityCacheService>.Instance);

    [Fact]
    public async Task GetByIdAsync_WhenPersonExists_ReturnsPerson()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var person = await CreateAndSavePersonAsync(context);
        var repository = new PersonRepository(context, Cache);

        var result = await repository.GetByIdAsync(person.Id);

        Assert.NotNull(result);
        Assert.Equal(person.Id, result.Id);
        Assert.Equal(person.DisplayName, result.DisplayName);
        Assert.Equal(person.FirstName, result.FirstName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonDoesNotExist_ReturnsNull()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new PersonRepository(context, Cache);

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    private static async Task<Person> CreateAndSavePersonAsync(AppDbContext context)
    {
        var person = new Person
        {
            DisplayName = "Juan Pérez",
            FirstName = "Juan",
            LastName = "Pérez"
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();
        return person;
    }
}
