using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Users.DTOs;
using LegalAiAr.Application.Admin.Users.Queries.GetUsers;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Users;

public sealed class GetAdminUsers : IEndpoint
{
    public string GroupName => AdminUsersEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(string.Empty, async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetUsersQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetAdminUsers")
        .WithTags("AdminUsers")
        .Produces<IReadOnlyList<UserDto>>(StatusCodes.Status200OK);
}

internal static class AdminUsersEndpointGroup
{
    public const string Name = "admin/users";
}
