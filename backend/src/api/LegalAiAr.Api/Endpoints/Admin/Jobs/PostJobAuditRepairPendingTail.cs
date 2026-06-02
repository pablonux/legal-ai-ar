using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Commands.RepairJobAuditTail;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobAuditRepairPendingTail : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{id:guid}/audit/repair-pending-tail", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RepairJobAuditTailCommand(id), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobAuditRepairPendingTail")
        .WithTags("AdminJobs");
}
