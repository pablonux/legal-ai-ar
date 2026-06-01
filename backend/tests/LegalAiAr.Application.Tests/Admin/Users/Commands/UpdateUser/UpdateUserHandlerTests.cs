using LegalAiAr.Application.Admin.Users.Commands.UpdateUser;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Users.Commands.UpdateUser;

public class UpdateUserHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserExists_UpdatesAndReturnsDto()
    {
        var userRepo = Substitute.For<IUserRepository>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            DisplayName = "Old Name",
            Role = "admin",
            IsActive = true
        };
        userRepo.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        var sut = new UpdateUserHandler(userRepo);
        var command = new UpdateUserCommand(user.Id, "New Name", "lawyer");

        var result = await sut.Handle(command, CancellationToken.None);

        Assert.Equal("New Name", result.DisplayName);
        Assert.Equal("lawyer", result.Role);
        await userRepo.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsNotFoundException()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var sut = new UpdateUserHandler(userRepo);
        var command = new UpdateUserCommand(Guid.NewGuid(), "Name", "admin");

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            sut.Handle(command, CancellationToken.None));

        Assert.Contains("not found", ex.Message);
        await userRepo.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
