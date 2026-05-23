using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetJobDocumentsSummary;

public class GetJobDocumentsSummaryHandler : IRequestHandler<GetJobDocumentsSummaryQuery, JobDocumentsSummaryDto>
{
    private readonly IDocumentRepository _documentRepository;

    public GetJobDocumentsSummaryHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<JobDocumentsSummaryDto> Handle(
        GetJobDocumentsSummaryQuery request, CancellationToken cancellationToken)
    {
        var stageSummaries = await _documentRepository.GetStageSummariesAsync(request.JobId, cancellationToken);

        var stages = stageSummaries.ToDictionary(
            kv => kv.Key.ToString(),
            kv => new StageSummaryDto(
                kv.Value.Pending,
                kv.Value.Processing,
                kv.Value.Completed,
                kv.Value.Failed,
                kv.Value.Discarded,
                kv.Value.Cancelled));

        var totalDocuments = stages.Values.Sum(s => s.Total);
        var totalFailed = stages.Values.Sum(s => s.Failed);
        var totalCompleted = stages.Values
            .Where(s => stages.Last().Key == s.ToString())
            .Sum(s => s.Completed);

        var lastStageKey = stages.Keys.LastOrDefault();
        totalCompleted = lastStageKey is not null && stages.TryGetValue(lastStageKey, out var lastStage)
            ? lastStage.Completed
            : 0;

        return new JobDocumentsSummaryDto(
            request.JobId,
            stages,
            totalDocuments,
            totalFailed,
            totalCompleted);
    }
}
