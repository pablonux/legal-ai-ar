using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetJobDocuments;

public record GetJobDocumentsQuery(
    Guid JobId,
    PipelineStage? Stage = null,
    DocumentStatus? Status = null,
    int Skip = 0,
    int Take = 50) : IRequest<GetJobDocumentsResult>;

public record GetJobDocumentsResult(
    IReadOnlyList<DocumentDto> Documents,
    int TotalCount);
