using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetJobs;

/// <summary>
/// Query for jobs (active, completed, failed). Phase 1: synthetic from CrawlerConfigs.
/// </summary>
public record GetJobsQuery : IRequest<IReadOnlyList<JobDto>>;
