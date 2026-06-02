using LegalAiAr.Api.Interfaces;
using LegalAiAr.Contracts.Responses.Catalogs;
using LegalAiAr.Application.Catalogs.Queries.GetPersonById;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Persons;

public sealed class GetPersonById : IEndpoint
{
    public string GroupName => PersonsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:int}", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetPersonByIdQuery(id), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetPersonById")
        .WithTags("Persons")
        .Produces<PersonDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
