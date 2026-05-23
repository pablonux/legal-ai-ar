using LegalAiAr.Application.Admin.Users.Commands.DeleteUser;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Admin.Users.Commands.DeleteUser;

public class DeleteUserHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserExists_DeactivatesUser()
    {
        var userRepo = Substitute.For<IUserRepository>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            IsActive = true
        };
        userRepo.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        var sut = new DeleteUserHandler(userRepo);
        var command = new DeleteUserCommand(user.Id);

        await sut.Handle(command, CancellationToken.None);

        Assert.False(user.IsActive);
        await userRepo.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsNotFoundException()
    {
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var sut = new DeleteUserHandler(userRepo);
        var command = new DeleteUserCommand(Guid.NewGuid());

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            sut.Handle(command, CancellationToken.None));

        Assert.Contains("not found", ex.Message);
        await userRepo.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
