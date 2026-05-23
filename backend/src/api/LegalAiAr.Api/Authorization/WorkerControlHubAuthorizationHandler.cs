using System.Security.Cryptography;
using System.Text;
using LegalAiAr.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Api.Authorization;

/// <summary>
/// Authorizes SignalR worker-control connections when either the user is authenticated
/// or a valid <see cref="WorkerControlOptions.HubAccessKey"/> is supplied via <c>X-Worker-Hub-Key</c>.
/// </summary>
public sealed class WorkerControlHubAuthorizationHandler : AuthorizationHandler<WorkerControlHubRequirement>
{
    public const string HubKeyHeaderName = "X-Worker-Hub-Key";

    private readonly IOptionsMonitor<WorkerControlOptions> _workerOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WorkerControlHubAuthorizationHandler(
        IOptionsMonitor<WorkerControlOptions> workerOptions,
        IHttpContextAccessor httpContextAccessor)
    {
        _workerOptions = workerOptions;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        WorkerControlHubRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var http = context.Resource as HttpContext ?? _httpContextAccessor.HttpContext;
        if (http is null)
            return Task.CompletedTask;

        var expected = _workerOptions.CurrentValue.HubAccessKey;
        if (string.IsNullOrEmpty(expected))
            return Task.CompletedTask;

        if (!http.Request.Headers.TryGetValue(HubKeyHeaderName, out var suppliedValues))
            return Task.CompletedTask;

        var supplied = suppliedValues.FirstOrDefault();
        if (!FixedTimeEquals(supplied, expected))
            return Task.CompletedTask;

        context.Succeed(requirement);
        return Task.CompletedTask;
    }

    private static bool FixedTimeEquals(string? a, string b)
    {
        var ab = Encoding.UTF8.GetBytes(a ?? string.Empty);
        var bb = Encoding.UTF8.GetBytes(b);
        if (ab.Length != bb.Length)
            return false;
        return CryptographicOperations.FixedTimeEquals(ab, bb);
    }
}
