using LegalAiAr.Application.Admin.Users.DTOs;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Users.Commands.UpdateUser;

/// <summary>
/// Handles UpdateUserCommand: updates user display name and role.
/// </summary>
public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"User with id {request.Id} not found.");

        user.DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? null : request.DisplayName.Trim();
        user.Role = request.Role.Trim().ToLowerInvariant();
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);

        return new UserDto(
            Id: user.Id,
            Email: user.Email,
            DisplayName: user.DisplayName,
            Role: user.Role,
            IsActive: user.IsActive);
    }
}
