using LegalAiAr.Application.Admin.Users.DTOs;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Users.Queries.GetUsers;

/// <summary>
/// Handles GetUsersQuery: returns all users.
/// </summary>
public class GetUsersHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(MapToDto).ToList();
    }

    private static UserDto MapToDto(Core.Entities.User user)
    {
        return new UserDto(
            Id: user.Id,
            Email: user.Email,
            DisplayName: user.DisplayName,
            Role: user.Role,
            IsActive: user.IsActive);
    }
}
