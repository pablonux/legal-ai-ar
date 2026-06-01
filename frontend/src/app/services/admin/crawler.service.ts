import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

const BASE = `${environment.apiUrl}/api/admin`;

export interface CrawlerConfig {
  sourceId: number;
  sourceName: string;
  isEnabled: boolean;
  lastCrawledAt: string | null;
  lastCrawledStatus: string | null;
  lastDocumentCount: number | null;
  /**
   * True when the UI injected this row because the API did not return source 6 yet
   * (e.g. migration AddThesaurusIngestSource not applied). Ingest still works; do not PATCH toggle.
   */
  synthetic?: boolean;
}

export interface PipelineSourceStatus {
  sourceId: number;
  sourceName: string;
  lastCrawledAt: string | null;
  lastCrawledStatus: string | null;
  lastDocumentCount: number | null;
  queueLength: number;
}

export interface PipelineStatusResult {
  sources: PipelineSourceStatus[];
}

export interface Job {
  id: string;
  sourceId: number;
  sourceName: string;
  type: string;
  triggeredBy: string;
  startedAt: string | null;
  completedAt: string | null;
  status: string;
  totalSearchResults: number | null;
  documentsDiscovered: number;
  documentsCrawled: number;
  documentsParsed: number;
  documentsEnriched: number;
  documentsPersisted: number;
  documentsIndexed: number;
  documentsSkipped: number;
  documentsFailed: number;
  /** Pending + Processing en Documents (verdad de tablas). */
  outstandingDocuments?: number;
  errorSummary?: string | null;
  creationLog?: string | null;
  dateFrom?: string | null;
  dateTo?: string | null;
  parentJobId?: string | null;
  partitionIndex?: number | null;
  partitionTotal?: number | null;
  infrastructureDegraded?: boolean;
  degradedSinceUtc?: string | null;
  degradedReason?: string | null;
  discoveryBatchPublished?: boolean;
}

export interface StorageProbeResult {
  discovererOk: boolean;
  fetcherOk: boolean;
  discovererError: string | null;
  fetcherError: string | null;
}

export interface RecoverJobFromInfraResult {
  storageProbeOk: boolean;
  storageProbeError: string | null;
  clearedDegraded: boolean;
  discoveryQueued: boolean;
  requeueMessagesPublished: number;
}

export interface RequeueMissingPipelineMessagesResult {
  totalPublished: number;
  publishedByStage: Record<string, number>;
}

export interface ReconcileJobProcessingDocumentsResult {
  rowsReset: number;
}

export interface RunCrawlerResult {
  success: boolean;
  message: string;
}

export interface StartThesaurusIngestJobResult {
  success: boolean;
  message: string;
  jobId: string;
}

export interface StageSummary {
  pending: number;
  processing: number;
  completed: number;
  failed: number;
  discarded: number;
  cancelled: number;
  total: number;
}

export interface JobDocumentsSummary {
  jobId: string;
  stages: Record<string, StageSummary>;
  totalDocuments: number;
  totalFailed: number;
  totalCompleted: number;
}

export interface JobDocument {
  id: string;
  ingestionJobId: string;
  entityType: string;
  sourceId: number;
  externalId: string;
  analysisId: string | null;
  currentStage: string;
  status: string;
  blobPath: string | null;
  contentHash: string | null;
  errorMessage: string | null;
  errorType: string | null;
  retryCount: number;
  createdAt: string;
  lastUpdatedAt: string | null;
  rulingId: string | null;
  statuteId: number | null;
  /** From the last stage log row with an error (not shown in admin UI). */
  lastErrorWorkerInstanceId?: string | null;
  /** Optional longer HTTP timeout (seconds) for PDF fetch only; 60–900 when set. */
  fetchPdfTimeoutSeconds?: number | null;
}

export interface JobDocumentsResult {
  documents: JobDocument[];
  totalCount: number;
}

export interface StageMetrics {
  count: number;
  avgDurationMs: number;
  p50DurationMs: number;
  p95DurationMs: number;
  maxDurationMs: number;
  minDurationMs: number;
  docsPerMinute: number | null;
}

export interface JobMetrics {
  jobId: string;
  stages: Record<string, StageMetrics>;
  overallDocsPerMinute: number | null;
  totalElapsedMs: number | null;
}

export interface StuckPipelineDocumentDto {
  id: string;
  externalId: string;
  currentStage: string;
  status: string;
}

export interface WorkerAuditDto {
  workerType: string;
  isPaused: boolean;
  pauseUpdatedAtUtc: string | null;
  signalRConnectedInstances: number;
}

export interface DocumentStatusCountsDto {
  pending: number;
  processing: number;
  completed: number;
  failed: number;
  discarded: number;
  cancelled: number;
  total: number;
  outstandingPendingOrProcessing: number;
}

/** Heuristic only: shared queues and no per-job dequeue telemetry. */
export interface StructuralRepairSafety {
  noProcessingRowsForJob: boolean;
  approxMainPipelineQueueMessageCount: number;
  anyWorkerConnectedSignalR: boolean;
  suggestedAdministrativeSafeWindow: boolean;
  caveats: string[];
}

export interface JobAudit {
  jobId: string;
  sourceName: string;
  jobStatus: string;
  startedAt: string;
  completedAt: string | null;
  jobDocumentsDiscovered: number;
  jobDocumentsSkipped: number;
  jobDocumentsCrawled: number;
  jobDocumentsParsed: number;
  jobDocumentsEnriched: number;
  jobDocumentsPersisted: number;
  jobDocumentsIndexed: number;
  jobDocumentsFailed: number;
  documentsTotalRows: number;
  dbCountsByStatus: DocumentStatusCountsDto;
  indexedMinusCompletedDelta: number;
  failedMinusFailedRowsDelta: number;
  satisfiesCompletionFormula: boolean;
  formulaSatisfiedButStillOutstanding: boolean;
  workers: WorkerAuditDto[];
  riskDocuments: StuckPipelineDocumentDto[];
  structuralRepairSafety: StructuralRepairSafety;
  notes: string[];
  auditedAtUtc: string;
  infrastructureDegraded?: boolean;
  degradedSinceUtc?: string | null;
  degradedReason?: string | null;
}

export interface RepairJobAuditTailResult {
  affectedCount: number;
  message: string;
}

export interface IngestionPipelineCountersDto {
  documentsCrawled: number;
  documentsParsed: number;
  documentsEnriched: number;
  documentsPersisted: number;
  documentsIndexed: number;
  documentsFailed: number;
}

export interface ReconcileJobCountersResult {
  previous: IngestionPipelineCountersDto;
  updated: IngestionPipelineCountersDto;
  jobCompletionApplied: boolean;
}

export interface WorkerPauseState {
  workerType: string;
  isPaused: boolean;
  updatedAt: string;
}

@Injectable({ providedIn: 'root' })
export class CrawlerService {
  constructor(private http: HttpClient) {}

  getCrawlers(): Observable<CrawlerConfig[]> {
    return this.http.get<CrawlerConfig[]>(`${BASE}/crawlers`);
  }

  getCrawlerById(sourceId: number): Observable<CrawlerConfig> {
    return this.http.get<CrawlerConfig>(`${BASE}/crawlers/${sourceId}`);
  }

  updateCrawler(sourceId: number, isEnabled: boolean): Observable<CrawlerConfig> {
    return this.http.patch<CrawlerConfig>(`${BASE}/crawlers/${sourceId}`, { isEnabled });
  }

  runCrawler(
    sourceId: number,
    type: 'incremental' | 'by-range' | 'fallos-destacados',
    options?: { since?: string; dateFrom?: string; dateTo?: string; useCache?: boolean; reprocess?: boolean; maxDocuments?: number; skipDocuments?: number }
  ): Observable<RunCrawlerResult> {
    const body: Record<string, string | boolean | number> = { type };
    const opts = options ?? {};
    if (opts.since != null) body['since'] = opts.since;
    if (opts.dateFrom != null) body['dateFrom'] = opts.dateFrom;
    if (opts.dateTo != null) body['dateTo'] = opts.dateTo;
    if (opts.useCache) body['useCache'] = true;
    if (opts.reprocess) body['reprocess'] = true;
    if (opts.maxDocuments != null && opts.maxDocuments > 0) body['maxDocuments'] = opts.maxDocuments;
    if (opts.skipDocuments != null && opts.skipDocuments > 0) body['skipDocuments'] = opts.skipDocuments;
    return this.http.post<RunCrawlerResult>(`${BASE}/crawlers/${sourceId}/run`, body);
  }

  startThesaurusIngestJob(body?: { normalizeKeywords?: boolean }): Observable<StartThesaurusIngestJobResult> {
    return this.http.post<StartThesaurusIngestJobResult>(`${BASE}/jobs/thesaurus`, body ?? {});
  }

  getPipelineStatus(): Observable<PipelineStatusResult> {
    return this.http.get<PipelineStatusResult>(`${BASE}/pipeline/status`);
  }

  getJobs(): Observable<Job[]> {
    return this.http.get<Job[]>(`${BASE}/jobs`);
  }

  requeueDocument(params: { stage: string; rulingId: string }): Observable<{ success: boolean; stage: string }> {
    return this.http.post<{ success: boolean; stage: string }>(`${BASE}/pipeline/requeue-document`, params);
  }

  retryJob(jobId: string): Observable<{ success: boolean; message: string }> {
    return this.http.post<{ success: boolean; message: string }>(`${BASE}/jobs/${jobId}/retry`, {});
  }

  getJobDocumentsSummary(jobId: string): Observable<JobDocumentsSummary> {
    return this.http.get<JobDocumentsSummary>(`${BASE}/jobs/${jobId}/documents/summary`);
  }

  getJobDocuments(
    jobId: string,
    params?: { stage?: string; status?: string; skip?: number; take?: number }
  ): Observable<JobDocumentsResult> {
    const query: Record<string, string | number> = {};
    if (params?.stage) query['stage'] = params.stage;
    if (params?.status) query['status'] = params.status;
    if (params?.skip != null) query['skip'] = params.skip;
    if (params?.take != null) query['take'] = params.take;
    return this.http.get<JobDocumentsResult>(`${BASE}/jobs/${jobId}/documents`, { params: query });
  }

  bulkDocumentAction(
    jobId: string,
    stage: string,
    action: 'Reprocess' | 'Discard'
  ): Observable<{ affectedCount: number; message: string }> {
    return this.http.post<{ affectedCount: number; message: string }>(
      `${BASE}/jobs/${jobId}/documents/action`,
      { stage, action }
    );
  }

  /** Reprocess the next N oldest Failed documents at one stage only (API clamps 1–50). */
  reprocessNextFailed(
    jobId: string,
    stage: string,
    take = 10
  ): Observable<{ affectedCount: number; message: string }> {
    return this.http.post<{ affectedCount: number; message: string }>(
      `${BASE}/jobs/${jobId}/documents/reprocess-next`,
      { stage, take }
    );
  }

  bulkFailedDocumentsByIds(
    jobId: string,
    documentIds: string[],
    action: 'Reprocess' | 'Discard'
  ): Observable<{ affectedCount: number; message: string }> {
    return this.http.post<{ affectedCount: number; message: string }>(
      `${BASE}/jobs/${jobId}/documents/bulk-by-ids`,
      { documentIds, action }
    );
  }

  /** Reprocess or discard all failed documents in a job, per pipeline stage (same as bulk document action). */
  bulkFailedDocumentsByStage(
    jobId: string,
    action: 'Reprocess' | 'Discard'
  ): Observable<{ affectedCount: number; message: string }[]> {
    return this.getJobDocumentsSummary(jobId).pipe(
      switchMap(summary => {
        const stages = Object.entries(summary.stages)
          .filter(([, s]) => s.failed > 0)
          .map(([stage]) => this.bulkDocumentAction(jobId, stage, action));
        return stages.length === 0 ? of([]) : forkJoin(stages);
      })
    );
  }

  singleFailedDocumentAction(
    jobId: string,
    documentId: string,
    action: 'Reprocess' | 'Discard'
  ): Observable<{ affectedCount: number; message: string }> {
    return this.http.post<{ affectedCount: number; message: string }>(
      `${BASE}/jobs/${jobId}/documents/${documentId}/action`,
      { action }
    );
  }

  setDocumentFetchPdfTimeout(
    jobId: string,
    documentId: string,
    timeoutSeconds: number | null
  ): Observable<{ message: string }> {
    return this.http.patch<{ message: string }>(
      `${BASE}/jobs/${jobId}/documents/${documentId}/fetch-pdf-timeout`,
      { timeoutSeconds }
    );
  }

  getJobMetrics(jobId: string): Observable<JobMetrics> {
    return this.http.get<JobMetrics>(`${BASE}/jobs/${jobId}/metrics`);
  }

  getJobAudit(jobId: string): Observable<JobAudit> {
    return this.http.get<JobAudit>(`${BASE}/jobs/${jobId}/audit`);
  }

  repairJobAuditPendingTail(jobId: string): Observable<RepairJobAuditTailResult> {
    return this.http.post<RepairJobAuditTailResult>(`${BASE}/jobs/${jobId}/audit/repair-pending-tail`, {});
  }

  reconcileJobCounters(jobId: string): Observable<ReconcileJobCountersResult> {
    return this.http.post<ReconcileJobCountersResult>(`${BASE}/jobs/${jobId}/reconcile-counters`, {});
  }

  pauseWorker(workerType: string): Observable<{ workerType: string; isPaused: boolean }> {
    return this.http.post<{ workerType: string; isPaused: boolean }>(`${BASE}/workers/${workerType}/pause`, {});
  }

  resumeWorker(workerType: string): Observable<{ workerType: string; isPaused: boolean }> {
    return this.http.post<{ workerType: string; isPaused: boolean }>(`${BASE}/workers/${workerType}/resume`, {});
  }

  getWorkerStatus(): Observable<WorkerPauseState[]> {
    return this.http.get<WorkerPauseState[]>(`${BASE}/workers/status`);
  }

  storageProbe(): Observable<StorageProbeResult> {
    return this.http.get<StorageProbeResult>(`${BASE}/infra/storage-probe`);
  }

  resumeDiscovery(jobId: string): Observable<{ skipDocumentsQueued: number }> {
    return this.http.post<{ skipDocumentsQueued: number }>(
      `${BASE}/jobs/${jobId}/resume-discovery`,
      {}
    );
  }

  requeueFetcherPending(jobId: string): Observable<{ messagesPublished: number }> {
    return this.http.post<{ messagesPublished: number }>(`${BASE}/jobs/${jobId}/requeue-fetcher-pending`, {});
  }

  requeueMissingPipelineMessages(
    jobId: string,
    stage?: string
  ): Observable<RequeueMissingPipelineMessagesResult> {
    const q: Record<string, string> = {};
    if (stage) q['stage'] = stage;
    return this.http.post<RequeueMissingPipelineMessagesResult>(
      `${BASE}/jobs/${jobId}/requeue-missing-pipeline-messages`,
      {},
      { params: q }
    );
  }

  reconcileProcessingDocuments(
    jobId: string,
    body?: { minAgeMinutes?: number; stage?: string }
  ): Observable<ReconcileJobProcessingDocumentsResult> {
    return this.http.post<ReconcileJobProcessingDocumentsResult>(
      `${BASE}/jobs/${jobId}/reconcile-processing-documents`,
      body ?? {}
    );
  }

  recoverFromInfra(
    jobId: string,
    body?: Partial<{
      requireStorageProbe: boolean;
      clearInfrastructureDegraded: boolean;
      broadcastRecovered: boolean;
      resumeDiscovery: boolean;
      requeueFetcherPending: boolean;
      requeueAllPipelineStages: boolean;
      resumeAllWorkers: boolean;
    }>
  ): Observable<RecoverJobFromInfraResult> {
    return this.http.post<RecoverJobFromInfraResult>(`${BASE}/jobs/${jobId}/recover-from-infra`, body ?? {});
  }
}
