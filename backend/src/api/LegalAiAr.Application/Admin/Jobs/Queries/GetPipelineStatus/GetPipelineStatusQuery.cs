using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetPipelineStatus;

/// <summary>
/// Query for pipeline status per source (crawler configs + queue length).
/// </summary>
public record GetPipelineStatusQuery : IRequest<GetPipelineStatusResult>;

/// <summary>
/// Result with sources and their pipeline status.
/// </summary>
/// <param name="Sources">Pipeline status per source.</param>
public record GetPipelineStatusResult(IReadOnlyList<PipelineSourceStatusDto> Sources);
