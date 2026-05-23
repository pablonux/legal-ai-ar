using LegalAiAr.Application.Admin.Users.Commands.CreateUser;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Users.Commands.CreateUser;

public class CreateUserHandlerTests
{
    [Fact]
    public async Task Handle_WhenEmailNotExists_CreatesUser()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        userRepo.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(c => c.Arg<User>());

        var sut = new CreateUserHandler(userRepo);
        var command = new CreateUserCommand("user@test.com", "Test User", "admin");

        var result = await sut.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("user@test.com", result.Email);
        Assert.Equal("Test User", result.DisplayName);
        Assert.Equal("admin", result.Role);
        Assert.True(result.IsActive);

        await userRepo.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenEmailExists_ThrowsDomainException()
    {
        var userRepo = Substitute.For<IUserRepository>();
        var existing = new User { Email = "user@test.com" };
        userRepo.GetByEmailAsync("user@test.com", Arg.Any<CancellationToken>()).Returns(existing);

        var sut = new CreateUserHandler(userRepo);
        var command = new CreateUserCommand("user@test.com", "Test", "admin");

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            sut.Handle(command, CancellationToken.None));

        Assert.Contains("already exists", ex.Message);
        await userRepo.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
