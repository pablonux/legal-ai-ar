using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Jobs.Commands.StartThesaurusIngestJob;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobsThesaurus : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/thesaurus", async (
            StartThesaurusIngestJobRequest? request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var normalize = request?.NormalizeKeywords ?? true;
            var result = await mediator.Send(new StartThesaurusIngestJobCommand(normalize), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobsThesaurus")
        .WithTags("AdminJobs");
}
