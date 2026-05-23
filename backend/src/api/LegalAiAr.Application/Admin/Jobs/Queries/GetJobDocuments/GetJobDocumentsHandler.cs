using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetJobDocuments;

public class GetJobDocumentsHandler : IRequestHandler<GetJobDocumentsQuery, GetJobDocumentsResult>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentStageLogRepository _stageLogRepository;

    public GetJobDocumentsHandler(
        IDocumentRepository documentRepository,
        IDocumentStageLogRepository stageLogRepository)
    {
        _documentRepository = documentRepository;
        _stageLogRepository = stageLogRepository;
    }

    public async Task<GetJobDocumentsResult> Handle(GetJobDocumentsQuery request, CancellationToken cancellationToken)
    {
        // Sequential awaits: same DbContext cannot run two queries concurrently (EF Core).
        var totalCount = await _documentRepository.CountByJobAsync(
            request.JobId,
            request.Stage,
            request.Status,
            cancellationToken);

        var documents = await _documentRepository.GetByJobAsync(
            request.JobId,
            request.Stage,
            request.Status,
            request.Skip,
            request.Take,
            cancellationToken);

        var workerByDoc = await _stageLogRepository.GetLastErrorWorkerInstanceByDocumentIdsAsync(
            documents.Select(d => d.Id).ToList(),
            cancellationToken);

        var dtos = documents.Select(d => new DocumentDto(
            d.Id,
            d.IngestionJobId,
            d.EntityType.ToString(),
            d.SourceId,
            d.ExternalId,
            d.AnalysisId,
            d.CurrentStage.ToString(),
            d.Status.ToString(),
            d.BlobPath,
            d.ContentHash,
            d.ErrorMessage,
            d.ErrorType,
            d.RetryCount,
            d.FetchPdfTimeoutSeconds,
            d.CreatedAt,
            d.LastUpdatedAt,
            d.RulingId,
            d.StatuteId,
            workerByDoc.TryGetValue(d.Id, out var w) ? w : null)).ToList();

        return new GetJobDocumentsResult(dtos, totalCount);
    }
}
