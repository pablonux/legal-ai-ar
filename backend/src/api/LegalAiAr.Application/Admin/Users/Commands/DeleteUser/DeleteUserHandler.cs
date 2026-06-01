using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Admin.Users.Commands.DeleteUser;

/// <summary>
/// Handles DeleteUserCommand: soft-deletes user by setting IsActive = false.
/// </summary>
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"User with id {request.Id} not found.");

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        return Unit.Value;
    }
}
