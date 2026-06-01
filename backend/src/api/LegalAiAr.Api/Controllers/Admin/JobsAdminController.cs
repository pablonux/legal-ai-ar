using LegalAiAr.Api.Models;
using LegalAiAr.Api.Services;
using LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction;
using LegalAiAr.Application.Admin.Jobs.Commands.BulkFailedDocumentsByIds;
using LegalAiAr.Application.Admin.Jobs.Commands.ReconcileJobCounters;
using LegalAiAr.Application.Admin.Jobs.Commands.ReconcileJobProcessingDocuments;
using LegalAiAr.Application.Admin.Jobs.Commands.RecoverJobFromInfra;
using LegalAiAr.Application.Admin.Jobs.Commands.RepairJobAuditTail;
using LegalAiAr.Application.Admin.Jobs.Commands.ReprocessNextFailedDocuments;
using LegalAiAr.Application.Admin.Jobs.Commands.RequeueFetcherPending;
using LegalAiAr.Application.Admin.Jobs.Commands.RequeueMissingPipelineMessages;
using LegalAiAr.Application.Admin.Jobs.Commands.ResumeDiscovery;
using LegalAiAr.Application.Admin.Jobs.Commands.RetryJob;
using LegalAiAr.Application.Admin.Jobs.Commands.RewindParserFailedToFetcher;
using LegalAiAr.Application.Admin.Jobs.Commands.SetDocumentFetchPdfTimeout;
using LegalAiAr.Application.Admin.Jobs.Commands.SingleFailedDocumentAction;
using LegalAiAr.Application.Admin.Jobs.Commands.StartThesaurusIngestJob;
using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Admin.Jobs.Queries.GetJobAudit;
using LegalAiAr.Application.Admin.Jobs.Queries.GetJobDocuments;
using LegalAiAr.Application.Admin.Jobs.Queries.GetJobDocumentsSummary;
using LegalAiAr.Application.Admin.Jobs.Queries.GetJobMetrics;
using LegalAiAr.Application.Admin.Jobs.Queries.GetJobs;
using LegalAiAr.Application.Admin.Jobs.Queries.GetPipelineStatus;
using LegalAiAr.Application.Admin.Pipeline.Commands.BackfillKbQuality;
using LegalAiAr.Application.Admin.Pipeline.Commands.BulkRequeue;
using LegalAiAr.Application.Admin.Pipeline.Commands.RequeueDocument;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers.Admin;

/// <summary>
/// Admin API for pipeline status, jobs and document re-execution.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize]
public class JobsAdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly JobInfraRecoveryOrchestrator _infraRecoveryOrchestrator;

    public JobsAdminController(IMediator mediator, JobInfraRecoveryOrchestrator infraRecoveryOrchestrator)
    {
        _mediator = mediator;
        _infraRecoveryOrchestrator = infraRecoveryOrchestrator;
    }

    /// <summary>
    /// Pipeline status per source (crawler configs + queue length).
    /// </summary>
    [HttpGet("pipeline/status")]
    [ProducesResponseType(typeof(GetPipelineStatusResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPipelineStatusResult>> GetPipelineStatus(CancellationToken cancellationToken)
    {
        var query = new GetPipelineStatusQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Re-queue a document to parser, enrichment, or indexer stage.
    /// Provide either message (full payload) or rulingId for reconstruction.
    /// </summary>
    [HttpPost("pipeline/requeue-document")]
    [ProducesResponseType(typeof(RequeueDocumentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequeueDocumentResult>> RequeueDocument(
        [FromBody] RequeueDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RequeueDocumentCommand(
            request.Stage,
            request.Message,
            request.RulingId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Bulk re-queue all eligible rulings to enrichment or indexer stage.
    /// Useful after schema changes to backfill ontology fields.
    /// </summary>
    [HttpPost("pipeline/bulk-requeue")]
    [ProducesResponseType(typeof(BulkRequeueResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BulkRequeueResult>> BulkRequeue(
        [FromBody] BulkRequeueRequest request,
        CancellationToken cancellationToken)
    {
        var command = new BulkRequeueCommand(
            request.Stage,
            request.OnlyMissingOntology,
            request.SourceId,
            request.BatchSize);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Backfill KB quality: classify statutes, propagate LegalBranch, update person court associations.
    /// SQL-only operations, no GPT calls.
    /// </summary>
    [HttpPost("pipeline/backfill-kb-quality")]
    [ProducesResponseType(typeof(BackfillKbQualityResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<BackfillKbQualityResult>> BackfillKbQuality(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new BackfillKbQualityCommand(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Active, completed and failed jobs. Real data from IngestionJobs when available.
    /// </summary>
    [HttpGet("jobs")]
    [ProducesResponseType(typeof(IReadOnlyList<JobDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<JobDto>>> GetJobs(CancellationToken cancellationToken)
    {
        var query = new GetJobsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Starts a background SAIJ thesaurus ingest (TemaTres API) and optional keyword normalization.
    /// Creates an IngestionJob with EntityType Thesaurus (not the document pipeline).
    /// </summary>
    [HttpPost("jobs/thesaurus")]
    [ProducesResponseType(typeof(StartThesaurusIngestJobResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StartThesaurusIngestJobResult>> StartThesaurusIngestJob(
        [FromBody] StartThesaurusIngestJobRequest? request,
        CancellationToken cancellationToken)
    {
        var normalize = request?.NormalizeKeywords ?? true;
        var result = await _mediator.Send(new StartThesaurusIngestJobCommand(normalize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Re-execute a partial or failed job. Publishes a new CrawlerMessage with the
    /// original parameters; already-indexed documents are skipped automatically.
    /// </summary>
    [HttpPost("jobs/{id:guid}/retry")]
    [ProducesResponseType(typeof(RetryJobResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RetryJobResult>> RetryJob(Guid id, CancellationToken cancellationToken)
    {
        var command = new RetryJobCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Performance metrics for a job: avg/p50/p95 duration per stage, throughput.
    /// </summary>
    [HttpGet("jobs/{id:guid}/metrics")]
    [ProducesResponseType(typeof(JobMetricsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<JobMetricsDto>> GetJobMetrics(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetJobMetricsQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Compares ingestion job counters with aggregated <c>Documents</c> rows and returns worker pause flags.
    /// Read-only diagnostic for operators after outages or counter drift.
    /// </summary>
    [HttpGet("jobs/{id:guid}/audit")]
    [ProducesResponseType(typeof(JobAuditDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobAuditDto>> GetJobAudit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetJobAuditQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Marks documents that are Pending at Persister or Indexer as Failed with an audit message,
    /// bumps the job failed counter, and tries to complete the job if nothing is left outstanding.
    /// </summary>
    [HttpPost("jobs/{id:guid}/audit/repair-pending-tail")]
    [ProducesResponseType(typeof(RepairJobAuditTailResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RepairJobAuditTailResult>> RepairJobAuditPendingTail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RepairJobAuditTailCommand(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Recomputes <c>DocumentsCrawled</c>…<c>DocumentsIndexed</c> and <c>DocumentsFailed</c> on the job from
    /// aggregated <c>Documents</c> rows (same semantics as workers). Leaves <c>DocumentsDiscovered</c> and
    /// <c>DocumentsSkipped</c> unchanged. Requires no Pending/Processing documents for the job.
    /// </summary>
    [HttpPost("jobs/{id:guid}/reconcile-counters")]
    [ProducesResponseType(typeof(ReconcileJobCountersResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReconcileJobCountersResultDto>> ReconcileJobCounters(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ReconcileJobCountersCommand(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Re-publishes all documents for this job that are Pending at the Fetcher stage (recovery after infra outage).
    /// </summary>
    [HttpPost("jobs/{id:guid}/requeue-fetcher-pending")]
    [ProducesResponseType(typeof(RequeueFetcherPendingResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequeueFetcherPendingResultDto>> RequeueFetcherPending(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RequeueFetcherPendingCommand(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Re-publishes queue messages for documents Pending at pipeline stages (Fetcher through Indexer), optionally one stage only.
    /// </summary>
    [HttpPost("jobs/{id:guid}/requeue-missing-pipeline-messages")]
    [ProducesResponseType(typeof(RequeueMissingPipelineMessagesResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequeueMissingPipelineMessagesResultDto>> RequeueMissingPipelineMessages(
        Guid id,
        [FromQuery] PipelineStage? stage,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RequeueMissingPipelineMessagesCommand(id, stage), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Resets stale <c>Processing</c> documents to <c>Pending</c> for this job (optional stage filter, min age in minutes).
    /// </summary>
    [HttpPost("jobs/{id:guid}/reconcile-processing-documents")]
    [ProducesResponseType(typeof(ReconcileJobProcessingDocumentsResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReconcileJobProcessingDocumentsResultDto>> ReconcileProcessingDocuments(
        Guid id,
        [FromBody] ReconcileJobProcessingDocumentsRequestDto? body,
        CancellationToken cancellationToken)
    {
        body ??= new ReconcileJobProcessingDocumentsRequestDto();
        var result = await _mediator.Send(
            new ReconcileJobProcessingDocumentsCommand(id, body.MinAgeMinutes, body.Stage),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Re-queues a Discoverer message to continue discovery for this job (skip head + preserve counters).
    /// </summary>
    [HttpPost("jobs/{id:guid}/resume-discovery")]
    [ProducesResponseType(typeof(ResumeDiscoveryResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResumeDiscoveryResultDto>> ResumeDiscovery(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ResumeDiscoveryCommand(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// After infra is healthy: optional storage probe, clear degraded flags, broadcast recovery,
    /// requeue Fetcher pending, optional resume discovery, and resume all pipeline workers (SignalR + DB).
    /// </summary>
    [HttpPost("jobs/{id:guid}/recover-from-infra")]
    [ProducesResponseType(typeof(RecoverJobFromInfraResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecoverJobFromInfraResultDto>> RecoverFromInfra(
        Guid id,
        [FromBody] RecoverJobFromInfraRequestDto? body,
        CancellationToken cancellationToken)
    {
        body ??= new RecoverJobFromInfraRequestDto();
        var command = new RecoverJobFromInfraCommand(
            id,
            body.RequireStorageProbe,
            body.ClearInfrastructureDegraded,
            body.BroadcastRecovered,
            body.ResumeDiscovery,
            body.RequeueFetcherPending,
            body.RequeueAllPipelineStages);
        var result = await _infraRecoveryOrchestrator.RecoverJobAsync(command, body.ResumeAllWorkers, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Stage-level summary for a job: counts per stage (pending, processing, completed, failed, discarded).
    /// </summary>
    [HttpGet("jobs/{id:guid}/documents/summary")]
    [ProducesResponseType(typeof(JobDocumentsSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<JobDocumentsSummaryDto>> GetJobDocumentsSummary(
        Guid id, CancellationToken cancellationToken)
    {
        var query = new GetJobDocumentsSummaryQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Paginated list of documents for a job, with optional stage and status filters.
    /// </summary>
    [HttpGet("jobs/{id:guid}/documents")]
    [ProducesResponseType(typeof(GetJobDocumentsResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetJobDocumentsResult>> GetJobDocuments(
        Guid id,
        [FromQuery] PipelineStage? stage,
        [FromQuery] DocumentStatus? status,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetJobDocumentsQuery(id, stage, status, skip, take);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Bulk reprocess or discard failed documents at a specific pipeline stage.
    /// </summary>
    [HttpPost("jobs/{id:guid}/documents/action")]
    [ProducesResponseType(typeof(BulkDocumentActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BulkDocumentActionResult>> BulkDocumentAction(
        Guid id,
        [FromBody] BulkDocumentActionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new BulkDocumentActionCommand(id, request.Stage, request.Action);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Reprocess the next N failed documents (default 10, max 50) at one pipeline stage only.
    /// </summary>
    [HttpPost("jobs/{id:guid}/documents/reprocess-next")]
    [ProducesResponseType(typeof(BulkDocumentActionResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkDocumentActionResult>> ReprocessNextFailed(
        Guid id,
        [FromBody] ReprocessNextFailedRequest request,
        CancellationToken cancellationToken)
    {
        var take = request.Take ?? 10;
        var command = new ReprocessNextFailedDocumentsCommand(id, request.Stage, take);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Reprocess or discard a set of failed documents by id (same rules as single-document action).
    /// </summary>
    [HttpPost("jobs/{jobId:guid}/documents/bulk-by-ids")]
    [ProducesResponseType(typeof(BulkDocumentActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BulkDocumentActionResult>> BulkFailedDocumentsByIds(
        Guid jobId,
        [FromBody] BulkFailedDocumentsByIdsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new BulkFailedDocumentsByIdsCommand(jobId, request.DocumentIds, request.Action);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Moves Parser/Failed documents back to Fetcher/Pending (DB) and publishes Fetcher queue messages so
    /// the Fetcher can re-prime CSJN sjconsulta JSON caches. By default only CSJN cache-miss Parser failures.
    /// </summary>
    [HttpPost("jobs/{jobId:guid}/documents/rewind-parser-failed-to-fetcher")]
    [ProducesResponseType(typeof(RewindParserFailedToFetcherResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RewindParserFailedToFetcherResultDto>> RewindParserFailedToFetcher(
        Guid jobId,
        [FromBody] RewindParserFailedToFetcherRequest? request,
        CancellationToken cancellationToken)
    {
        var body = request ?? new RewindParserFailedToFetcherRequest();
        var command = new RewindParserFailedToFetcherCommand(
            jobId,
            body.OnlyCsjnCacheMiss,
            body.ErrorMessageContains,
            body.SourceId,
            body.MaxDocuments);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Sets optional HTTP timeout (seconds) for PDF download on this document only (Fetcher). Null clears.
    /// Allowed range 60–900. Does not change the global worker HttpClient default.
    /// </summary>
    [HttpPatch("jobs/{jobId:guid}/documents/{documentId:guid}/fetch-pdf-timeout")]
    [ProducesResponseType(typeof(SetDocumentFetchPdfTimeoutResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SetDocumentFetchPdfTimeoutResultDto>> SetDocumentFetchPdfTimeout(
        Guid jobId,
        Guid documentId,
        [FromBody] SetDocumentFetchPdfTimeoutRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SetDocumentFetchPdfTimeoutCommand(jobId, documentId, request.TimeoutSeconds),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Reprocess or discard a single failed document for a job (Failed → Pending + queue, or Failed → Discarded).
    /// </summary>
    [HttpPost("jobs/{jobId:guid}/documents/{documentId:guid}/action")]
    [ProducesResponseType(typeof(SingleFailedDocumentActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SingleFailedDocumentActionResult>> SingleFailedDocumentAction(
        Guid jobId,
        Guid documentId,
        [FromBody] SingleDocumentActionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SingleFailedDocumentActionCommand(jobId, documentId, request.Action);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
