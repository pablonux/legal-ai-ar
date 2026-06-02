using LegalAiAr.Api.Interfaces;

namespace LegalAiAr.Api.Endpoints;

/// <summary>
/// Optional base for endpoints that share a group name and authorization policy.
/// </summary>
public abstract class EndpointGroupBase : IEndpoint
{
    public abstract string GroupName { get; }

    public abstract void MapEndpoint(IEndpointRouteBuilder app);

    protected static RouteGroupBuilder ConfigureAuthorizedGroup(
        IEndpointRouteBuilder app,
        string groupName,
        string? authorizationPolicy = null)
    {
        var group = app.MapGroup($"api/{groupName}");
        if (authorizationPolicy is null)
            group.RequireAuthorization();
        else
            group.RequireAuthorization(authorizationPolicy);

        group.AddEndpointFilter<Filters.ValidationEndpointFilter>();
        return group;
    }
}
