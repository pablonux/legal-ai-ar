using LegalAiAr.Application.Admin.Users.Queries.GetUsers;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Users.Queries.GetUsers;

public class GetUsersHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllUsers()
    {
        var userRepo = Substitute.For<IUserRepository>();
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), Email = "a@test.com", Role = "admin", IsActive = true },
            new() { Id = Guid.NewGuid(), Email = "b@test.com", Role = "viewer", IsActive = true }
        };
        userRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(users);

        var sut = new GetUsersHandler(userRepo);
        var query = new GetUsersQuery();

        var result = await sut.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("a@test.com", result[0].Email);
        Assert.Equal("admin", result[0].Role);
        Assert.Equal("viewer", result[1].Role);
    }
}
