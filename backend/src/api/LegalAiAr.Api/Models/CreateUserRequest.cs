namespace LegalAiAr.Api.Models;

/// <summary>
/// Request body for POST /api/admin/users.
/// </summary>
/// <param name="Email">User email. Must be unique.</param>
/// <param name="DisplayName">Optional display name.</param>
/// <param name="Role">admin, lawyer, or viewer.</param>
public record CreateUserRequest(string? Email, string? DisplayName, string? Role);
