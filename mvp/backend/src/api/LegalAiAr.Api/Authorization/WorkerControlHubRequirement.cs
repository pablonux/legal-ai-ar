using Microsoft.AspNetCore.Authorization;

namespace LegalAiAr.Api.Authorization;

/// <summary>
/// Allows browser users (authenticated) or pipeline workers (shared hub key header).
/// </summary>
public sealed class WorkerControlHubRequirement : IAuthorizationRequirement;
