namespace LegalAiAr.Application.Admin.Users.DTOs;

/// <summary>
/// DTO for user in admin API responses.
/// </summary>
/// <param name="Id">User GUID.</param>
/// <param name="Email">Login identifier.</param>
/// <param name="DisplayName">Optional display name.</param>
/// <param name="Role">admin, lawyer, or viewer.</param>
/// <param name="IsActive">Whether the user is active.</param>
public record UserDto(
    Guid Id,
    string Email,
    string? DisplayName,
    string Role,
    bool IsActive);
