namespace LegalAiAr.Api.Models;

/// <summary>
/// Request body for PUT /api/admin/users/{id}.
/// </summary>
/// <param name="DisplayName">Optional display name.</param>
/// <param name="Role">admin, lawyer, or viewer. Required.</param>
public record UpdateUserRequest(string? DisplayName, string? Role);
