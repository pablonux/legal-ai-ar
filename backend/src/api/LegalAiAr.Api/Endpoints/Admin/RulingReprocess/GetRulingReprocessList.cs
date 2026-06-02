using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.RulingReprocess.DTOs;
using LegalAiAr.Application.Admin.RulingReprocess.Queries.ListRulingReprocessRequests;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Api.Endpoints.Admin.RulingReprocess;

public sealed class GetRulingReprocessList : IEndpoint
{
    public string GroupName => AdminRulingReprocessEndpointGroup.Name;

    public string? AuthorizationPolicy => "AdminOnly";

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(string.Empty, async (
            string? status,
            int page,
            int pageSize,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            page = page == 0 ? 1 : page;
            pageSize = pageSize == 0 ? 50 : pageSize;

            RulingReprocessRequestStatus? statusFilter = null;
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<RulingReprocessRequestStatus>(status, true, out var parsed))
                statusFilter = parsed;

            var result = await mediator.Send(
                new ListRulingReprocessRequestsQuery(statusFilter, page, pageSize),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetRulingReprocessList")
        .WithTags("AdminRulingReprocess")
        .Produces<RulingReprocessListResult>(StatusCodes.Status200OK);
}

internal static class AdminRulingReprocessEndpointGroup
{
    public const string Name = "admin/ruling-reprocess";
}
