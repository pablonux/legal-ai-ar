using LegalAiAr.Application.Catalogs.Queries.GetPersonById;
using LegalAiAr.Application.Catalogs.Queries.GetPersons;
using LegalAiAr.Application.Catalogs.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PersonListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PersonListItemDto>>> List(
        [FromQuery(Name = "q")] string? query = null,
        [FromQuery] string? court = null,
        [FromQuery] int limit = 50,
        [FromQuery(Name = "vista")] string? vista = null,
        CancellationToken cancellationToken = default)
    {
        var listView = ParsePersonListView(vista);
        var result = await _mediator.Send(
            new GetPersonsQuery(query, court, limit, listView), cancellationToken);
        return Ok(result);
    }

    private static PersonListView ParsePersonListView(string? vista) =>
        vista?.Trim().ToLowerInvariant() switch
        {
            "magistrados" => PersonListView.Magistrates,
            "partes" => PersonListView.Parties,
            _ => PersonListView.All
        };

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PersonDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPersonByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
