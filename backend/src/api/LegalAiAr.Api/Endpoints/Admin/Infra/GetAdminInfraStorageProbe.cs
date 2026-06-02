using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Infra.Queries.ProbeStorageHealth;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Infra;

public sealed class GetAdminInfraStorageProbe : IEndpoint
{
    public string GroupName => AdminInfraEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/storage-probe", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new ProbeStorageHealthQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetAdminInfraStorageProbe")
        .WithTags("AdminInfra")
        .Produces<ProbeStorageHealthResultDto>(StatusCodes.Status200OK);
}

internal static class AdminInfraEndpointGroup
{
    public const string Name = "admin/infra";
}
