using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Commands.RetryJob;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobRetry : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{id:guid}/retry", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RetryJobCommand(id), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobRetry")
        .WithTags("AdminJobs");
}
