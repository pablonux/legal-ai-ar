using LegalAiAr.Application.Admin.Users.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Users.Commands.CreateUser;

/// <summary>
/// Command to create a new user.
/// </summary>
/// <param name="Email">User email. Must be unique.</param>
/// <param name="DisplayName">Optional display name.</param>
/// <param name="Role">admin, lawyer, or viewer.</param>
public record CreateUserCommand(string Email, string? DisplayName, string Role) : IRequest<UserDto>;
