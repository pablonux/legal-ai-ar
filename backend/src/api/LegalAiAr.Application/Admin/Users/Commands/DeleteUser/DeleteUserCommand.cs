using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Users.Commands.DeleteUser;

/// <summary>
/// Command to deactivate a user (soft delete).
/// </summary>
/// <param name="Id">User ID.</param>
public record DeleteUserCommand(Guid Id) : IRequest<Unit>;
