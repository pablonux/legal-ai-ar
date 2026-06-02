using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Users.Commands.DeleteUser;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Users;

public sealed class DeleteAdminUser : IEndpoint
{
    public string GroupName => AdminUsersEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete("/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            await mediator.Send(new DeleteUserCommand(id), cancellationToken);
            return Results.NoContent();
        })
        .WithName("DeleteAdminUser")
        .WithTags("AdminUsers")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
}
