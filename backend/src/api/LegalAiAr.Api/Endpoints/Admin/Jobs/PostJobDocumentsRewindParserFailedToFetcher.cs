using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Jobs.Commands.RewindParserFailedToFetcher;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobDocumentsRewindParserFailedToFetcher : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{jobId:guid}/documents/rewind-parser-failed-to-fetcher", async (
            Guid jobId,
            RewindParserFailedToFetcherRequest? request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var body = request ?? new RewindParserFailedToFetcherRequest();
            var result = await mediator.Send(
                new RewindParserFailedToFetcherCommand(
                    jobId,
                    body.OnlyCsjnCacheMiss,
                    body.ErrorMessageContains,
                    body.SourceId,
                    body.MaxDocuments),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobDocumentsRewindParserFailedToFetcher")
        .WithTags("AdminJobs");
}
