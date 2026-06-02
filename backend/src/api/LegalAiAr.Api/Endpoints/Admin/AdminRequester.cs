using System.Security.Claims;

namespace LegalAiAr.Api.Endpoints.Admin;

internal static class AdminRequester
{
    public static string GetEmail(ClaimsPrincipal user) =>
        user.Identity?.Name
        ?? user.FindFirst("preferred_username")?.Value
        ?? user.FindFirst("email")?.Value
        ?? user.FindFirst(ClaimTypes.Email)?.Value
        ?? "admin";
}
