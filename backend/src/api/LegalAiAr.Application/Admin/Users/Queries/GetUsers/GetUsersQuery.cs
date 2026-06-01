using LegalAiAr.Application.Admin.Users.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Users.Queries.GetUsers;

/// <summary>
/// Query to list all users.
/// </summary>
public record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;
