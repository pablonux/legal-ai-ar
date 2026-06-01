namespace LegalAiAr.Api.Services;

/// <summary>
/// Maps GCaaS / Entra platform role headers or claim values to application roles.
/// Phase 2 auth-only: defaults to <see cref="DefaultRole"/> when no recognized role is present.
/// </summary>
public static class PlatformRoleResolver
{
    public const string DefaultRole = "admin";

    private static readonly HashSet<string> KnownRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "admin",
        "lawyer",
        "viewer",
    };

    /// <summary>
    /// Resolves role from <c>X-User-Roles</c> (comma-separated or single value).
    /// </summary>
    public static string FromRolesHeader(string? rolesHeader)
    {
        if (string.IsNullOrWhiteSpace(rolesHeader))
            return DefaultRole;

        foreach (var part in rolesHeader.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var mapped = MapRole(part);
            if (mapped is not null)
                return mapped;
        }

        return DefaultRole;
    }

    public static string? MapRole(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim().ToLowerInvariant();
        return KnownRoles.Contains(normalized) ? normalized : null;
    }
}
