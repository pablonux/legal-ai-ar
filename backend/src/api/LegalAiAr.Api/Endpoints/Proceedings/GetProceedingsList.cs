using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.DTOs;
using LegalAiAr.Application.Proceedings.Queries;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Api.Endpoints.Proceedings;

public sealed class GetProceedingsList : IEndpoint
{
    public string GroupName => ProceedingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(string.Empty, async (
            string? q,
            ProcessType? processType,
            LegalBranch? legalBranch,
            int? courtId,
            ProcessStatus? status,
            int page,
            int pageSize,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            page = page == 0 ? 1 : page;
            pageSize = pageSize == 0 ? 25 : pageSize;
            var result = await mediator.Send(
                new GetProceedingsQuery(q, processType, legalBranch, courtId, status, page, pageSize),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetProceedingsList")
        .WithTags("Proceedings")
        .Produces<ProceedingPageDto>(StatusCodes.Status200OK);
}

internal static class ProceedingsEndpointGroup
{
    public const string Name = "proceedings";
}
