using LegalAiAr.Api.Interfaces;
using LegalAiAr.Contracts.Responses.Catalogs;
using LegalAiAr.Application.Catalogs.Mapping;
using LegalAiAr.Application.Catalogs.Queries.GetPersons;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Persons;

public sealed class GetPersonsList : IEndpoint
{
    public string GroupName => PersonsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(string.Empty, async (
            string? q,
            string? court,
            int limit,
            string? vista,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            limit = limit == 0 ? 50 : limit;
            var listView = PersonListViewParser.Parse(vista);
            var result = await mediator.Send(
                new GetPersonsQuery(q, court, limit, listView),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetPersonsList")
        .WithTags("Persons")
        .Produces<IReadOnlyList<PersonListItemDto>>(StatusCodes.Status200OK);
}

internal static class PersonsEndpointGroup
{
    public const string Name = "persons";
}
