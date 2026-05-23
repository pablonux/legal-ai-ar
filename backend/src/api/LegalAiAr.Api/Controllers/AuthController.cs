using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

/// <summary>
/// Session info for the SPA. Identity comes from platform headers (GCaaS/Envoy) or development injection.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Returns the current authenticated user info from platform identity headers.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(MeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var email = User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email")
            ?? "";
        var name = User.FindFirstValue(ClaimTypes.Name)
            ?? User.FindFirstValue("name")
            ?? email;
        var role = User.FindFirstValue(ClaimTypes.Role)
            ?? User.FindFirstValue("role")
            ?? "viewer";
        var groups = User.FindFirstValue("groups") ?? "";

        return Ok(new MeResponse(email, name, role, groups));
    }

    /// <summary>
    /// Logout. Clears client-side state; platform logout URL is handled by the SPA in GCaaS.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(LogoutResponse), StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        return Ok(new LogoutResponse(Success: true));
    }
}

public record MeResponse(string Email, string DisplayName, string Role, string Groups);
public record LogoutResponse(bool Success);
