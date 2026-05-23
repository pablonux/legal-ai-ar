using LegalAiAr.Application.Admin.Users.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Users.Commands.UpdateUser;

/// <summary>
/// Command to update a user's display name and role.
/// </summary>
/// <param name="Id">User ID.</param>
/// <param name="DisplayName">Optional display name.</param>
/// <param name="Role">admin, lawyer, or viewer. Required.</param>
public record UpdateUserCommand(Guid Id, string? DisplayName, string Role) : IRequest<UserDto>;
