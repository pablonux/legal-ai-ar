using System.Security.Claims;

namespace LegalAiAr.Api.Services;

/// <summary>
/// Normalizes a validated Entra / gateway JWT principal for ASP.NET authorization.
/// </summary>
public static class PlatformJwtPrincipalNormalizer
{
    public static ClaimsPrincipal Normalize(
        ClaimsPrincipal principal,
        PlatformAuthOptions options,
        string headerEmail,
        string? headerDisplayName,
        string? headerGroups,
        string? headerRoles)
    {
        var claims = principal.Claims.ToList();

        EnsureEmail(claims, headerEmail);

        if (!string.IsNullOrWhiteSpace(headerDisplayName)
            && claims.All(c => c.Type != ClaimTypes.Name && c.Type != "name"))
        {
            claims.Add(new Claim(ClaimTypes.Name, headerDisplayName));
        }

        if (!string.IsNullOrWhiteSpace(headerGroups)
            && claims.All(c => c.Type != "groups"))
        {
            claims.Add(new Claim("groups", headerGroups));
        }

        ApplyRoles(claims, principal, options, headerRoles);

        var identity = new ClaimsIdentity(claims, PlatformAuthenticationHandler.SchemeName);
        return new ClaimsPrincipal(identity);
    }

    private static void EnsureEmail(List<Claim> claims, string headerEmail)
    {
        if (claims.Any(c => c.Type == ClaimTypes.Email || c.Type == "email"))
            return;

        if (!string.IsNullOrWhiteSpace(headerEmail))
            claims.Add(new Claim(ClaimTypes.Email, headerEmail));
    }

    private static void ApplyRoles(
        List<Claim> claims,
        ClaimsPrincipal principal,
        PlatformAuthOptions options,
        string? headerRoles)
    {
        var rolesClaimType = options.RolesClaimType;

        foreach (var claim in principal.Claims.Where(c =>
                     string.Equals(c.Type, rolesClaimType, StringComparison.OrdinalIgnoreCase)
                     || string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase)
                     || string.Equals(c.Type, "role", StringComparison.OrdinalIgnoreCase)))
        {
            var mapped = PlatformRoleResolver.MapRole(claim.Value);
            if (mapped is not null
                && claims.All(c => !(c.Type == ClaimTypes.Role && string.Equals(c.Value, mapped, StringComparison.OrdinalIgnoreCase))))
            {
                claims.Add(new Claim(ClaimTypes.Role, mapped));
            }
        }

        if (claims.Any(c => c.Type == ClaimTypes.Role))
            return;

        var fromHeader = PlatformRoleResolver.FromRolesHeader(headerRoles);
        claims.Add(new Claim(ClaimTypes.Role, fromHeader));

        if (!string.IsNullOrWhiteSpace(headerRoles)
            && claims.All(c => c.Type != "roles"))
        {
            claims.Add(new Claim("roles", headerRoles));
        }
    }
}
