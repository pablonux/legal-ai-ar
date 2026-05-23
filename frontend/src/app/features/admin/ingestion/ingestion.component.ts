import { Component, signal, computed, inject, OnInit, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SlicePipe } from '@angular/common';
import { catchError, EMPTY, finalize, forkJoin, interval, of, Subscription, switchMap, tap } from 'rxjs';
import {
  CrawlerService,
  type CrawlerConfig,
  type Job,
  type JobDocument,
  type JobAudit,
  type JobDocumentsSummary,
  type StageSummary,
  type WorkerPauseState,
  type ReconcileJobCountersResult,
  type StorageProbeResult,
} from '../../../services/admin/crawler.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

interface SourceSection {
  config: CrawlerConfig;
  jobs: Job[];
  expanded: boolean;
  showAllJobs: boolean;
}

interface StageCounter {
  label: string;
  value: number;
  workerType: string | null;
  colorClass: string;
}

/** SAIJ TemaTres thesaurus ingest (matches backend StartThesaurusIngestJobHandler). */
const THESAURUS_SOURCE_ID = 6;

const SOURCE_ENTITY_MAP: Record<number, string> = {
  1: 'rulings',
  2: 'statutes',
  3: 'rulings',
  4: 'rulings',
  5: 'rulings',
  6: 'thesaurus',
};

const WORKER_TYPES = ['Discoverer', 'Fetcher', 'Parser', 'Enricher', 'Persister', 'Indexer'];

/** Order for failed-doc filter chips (matches API PipelineStage names). */
const FAILED_STAGE_FILTER_ORDER = ['Discoverer', 'Fetcher', 'Parser', 'Enricher', 'Persister', 'Indexer'] as const;

interface CrawlTypeOption {
  value: string;
  label: string;
}

const SOURCE_CRAWL_TYPES: Record<number, CrawlTypeOption[]> = {
  1: [
    { value: 'incremental', label: 'Incremental' },
    { value: 'by-range', label: 'Por rango' },
    { value: 'fallos-destacados', label: 'Fallos destacados' },
  ],
  2: [
    { value: 'incremental', label: 'Incremental' },
    { value: 'by-range', label: 'Por rango' },
  ],
  3: [
    { value: 'incremental', label: 'Incremental' },
    { value: 'by-range', label: 'Por rango' },
  ],
  4: [
    { value: 'incremental', label: 'Incremental' },
    { value: 'by-range', label: 'Por rango' },
  ],
  5: [
    { value: 'incremental', label: 'Incremental' },
    { value: 'by-range', label: 'Por rango' },
  ],
  6: [{ value: 'thesaurus', label: 'Actualizar tesauro (API SAIJ)' }],
};

@Component({
  selector: 'app-ingestion',
  standalone: true,
  imports: [FormsModule, LoadingSpinnerComponent, SlicePipe],
  template: `
    @if (state() === 'loading') {
      <app-loading-spinner message="Cargando panel de ingesta..." />
    }

    @if (state() === 'error') {
      <div class="state-message error">
        <p>{{ error() }}</p>
        <button type="button" class="btn-retry" (click)="load()">Reintentar</button>
      </div>
    }

    @if (state() === 'loaded') {
      <div class="ingestion-page">
        @if (infraBanner(); as ib) {
          <section class="infra-banner" role="alert">
            <div class="infra-banner-main">
              <span class="infra-banner-title">{{ ib.title }}</span>
              <p class="infra-banner-body">{{ ib.body }}</p>
            </div>
            <div class="infra-banner-actions">
              @if (ib.showActions) {
                <button type="button" class="btn-infra"
                        [disabled]="infraOpsBusy()"
                        (click)="recoverFromInfra(false)">
                  {{ infraOpsBusy() ? '…' : 'Recuperar job' }}
                </button>
                <button type="button" class="btn-infra"
                        [disabled]="infraOpsBusy()"
                        (click)="recoverFromInfra(true)">
                  {{ infraOpsBusy() ? '…' : 'Recuperar + reencolar todo' }}
                </button>
                <button type="button" class="btn-infra secondary"
                        [disabled]="infraOpsBusy()"
                        (click)="reconcileProcessingStale()">
                  Processing stale → Pending
                </button>
                <button type="button" class="btn-infra secondary"
                        [disabled]="infraAuxBusy()"
                        (click)="probeStorageQueues()">
                  {{ infraAuxBusy() ? '…' : 'Probar colas' }}
                </button>
              }
            </div>
          </section>
        }
        @if (pendingJobMessage(); as msg) {
          <section class="pending-panel">
            <span class="pending-spinner"></span>
            <span>{{ msg }}</span>
          </section>
        } @else if (activeJob()) {
          @let aj = activeJob()!;
          <section class="active-panel">
            <div class="active-header">
              <div class="active-title">
                <span class="active-source">{{ aj.sourceName }}</span>
                <span class="active-job-name">{{ jobTitle(aj) }}</span>
                <span class="job-badge" [class]="jobStatusClass(aj.status)">{{ aj.status }}</span>
                @if (aj.infrastructureDegraded) {
                  <span class="job-badge infra-degraded" title="{{ aj.degradedReason || 'Infra degradada' }}">infra</span>
                }
                @if ((aj.outstandingDocuments ?? 0) > 0) {
                  <span class="job-badge outstanding-queue"
                        title="Documentos en Pending o Processing; requieren workers para avanzar">
                    {{ aj.outstandingDocuments }} en cola pipeline
                  </span>
                }
              </div>
              <div class="active-actions">
                <button type="button" class="btn-bulk audit"
                        [disabled]="workerBusy() || auditLoading()"
                        (click)="openJobAudit(aj)">
                  {{ auditLoading() ? '…' : 'Auditar' }}
                </button>
                <button type="button" class="btn-bulk pause"
                        [disabled]="workerBusy()"
                        (click)="bulkWorkers(true)">Pausar todo</button>
                <button type="button" class="btn-bulk resume"
                        [disabled]="workerBusy()"
                        (click)="bulkWorkers(false)">Reanudar todo</button>
              </div>
            </div>

            @if (aj.infrastructureDegraded) {
              <p class="active-degraded-hint" role="status">
                Pipeline en espera: infra degradada. Abrí <strong>Auditar</strong>, verificá colas y pulsá recuperación cuando ambas estén OK.
              </p>
            }

            <div class="active-bar">
              @let segs = pipelineSegments(aj);
              <div class="seg completed" [style.width.%]="segs.completed"></div>
              <div class="seg enriched" [style.width.%]="segs.enriched"></div>
              <div class="seg parsed" [style.width.%]="segs.parsed"></div>
              <div class="seg crawled" [style.width.%]="segs.crawled"></div>
              <div class="seg skipped" [style.width.%]="segs.skipped"></div>
              <div class="seg failed" [style.width.%]="segs.failed"></div>
            </div>

            <div class="stage-counters">
              @for (sc of stageCounters(); track sc.label) {
                <div class="stage-counter" [class]="sc.colorClass">
                  <span class="sc-value">{{ sc.value }}</span>
                  <span class="sc-label">{{ sc.label }}</span>
                  @if (sc.workerType) {
                    @let side = stageSideMetrics(sc.workerType);
                    <div class="sc-worker-controls">
                      <span
                        class="sc-side sc-side-queue"
                        [title]="stageQueueTooltip(sc.workerType)">
                        <svg viewBox="0 0 20 20" class="sc-side-icon" aria-hidden="true">
                          <path d="M4 5h12v2H4V5zm0 4h12v2H4V9zm0 4h8v2H4v-2z" fill="currentColor"/>
                        </svg>
                        <span class="sc-side-num">{{ side.queue }}</span>
                      </span>
                      <button type="button" class="sc-toggle"
                              [class.paused]="isWorkerPaused(sc.workerType)"
                              [disabled]="workerBusy()"
                              (click)="toggleWorkerByType(sc.workerType)"
                              [title]="isWorkerPaused(sc.workerType) ? 'Reanudar ' + sc.workerType : 'Pausar ' + sc.workerType">
                        @if (isWorkerPaused(sc.workerType)) {
                          <svg viewBox="0 0 20 20" class="sc-icon"><polygon points="6,3 17,10 6,17"/></svg>
                        } @else {
                          <svg viewBox="0 0 20 20" class="sc-icon"><rect x="4" y="3" width="4" height="14" rx="1"/><rect x="12" y="3" width="4" height="14" rx="1"/></svg>
                        }
                      </button>
                      <span
                        class="sc-side sc-side-failed"
                        [class.sc-side-has-failed]="side.failed > 0"
                        [title]="stageFailedTooltip(sc.workerType)">
                        <svg viewBox="0 0 24 24" class="sc-side-icon sc-side-icon-alert" aria-hidden="true">
                          <path fill="currentColor" d="M12 2L2 20h20L12 2zm0 3.8L18.4 18H5.6L12 5.8zm-1 6.2v2h2v-2h-2zm0 4v2h2v-2h-2z"/>
                        </svg>
                        <span class="sc-side-num">{{ side.failed }}</span>
                      </span>
                    </div>
                  }
                </div>
              }
            </div>
          </section>
        } @else {
          <section class="idle-panel">
            <h2>Nueva ingesta</h2>
            <div class="idle-form">
              <div class="idle-field">
                <label for="idle-source">Fuente</label>
                <select id="idle-source" [ngModel]="runSourceId()" (ngModelChange)="onSourceChange($event)">
                  <option [ngValue]="null" disabled>Seleccionar...</option>
                  @for (src of enabledSources(); track src.sourceId) {
                    <option [ngValue]="src.sourceId">{{ src.sourceName }} — {{ src.entityLabel }}</option>
                  }
                </select>
              </div>
              @if (runSourceId()) {
                @if (runSourceId() !== thesaurusSourceId) {
                <div class="idle-field">
                  <label for="idle-type">Tipo</label>
                  <select id="idle-type" [(ngModel)]="runType">
                    @for (ct of availableCrawlTypes(); track ct.value) {
                      <option [value]="ct.value">{{ ct.label }}</option>
                    }
                  </select>
                </div>
                @if (runType === 'incremental') {
                  <div class="idle-field">
                    <label for="idle-since">Desde</label>
                    <input id="idle-since" type="date" [(ngModel)]="runSince" />
                  </div>
                }
                @if (runType === 'by-range') {
                  <div class="idle-field">
                    <label for="idle-from">Desde</label>
                    <input id="idle-from" type="date" [(ngModel)]="runDateFrom" />
                  </div>
                  <div class="idle-field">
                    <label for="idle-to">Hasta</label>
                    <input id="idle-to" type="date" [(ngModel)]="runDateTo" />
                  </div>
                }
                <div class="idle-field narrow">
                  <label for="idle-skip">Saltear</label>
                  <input id="idle-skip" type="number" min="0" [(ngModel)]="runSkipDocs" placeholder="0" />
                </div>
                <div class="idle-field narrow">
                  <label for="idle-max">Maximo</label>
                  <input id="idle-max" type="number" min="1" [(ngModel)]="runMaxDocs" placeholder="Todos" />
                </div>
                <div class="idle-checks">
                  <label class="checkbox-label"><input type="checkbox" [(ngModel)]="runUseCache" /><span>Cache</span></label>
                  <label class="checkbox-label"><input type="checkbox" [(ngModel)]="runReprocess" /><span>Reprocesar</span></label>
                </div>
                } @else {
                <p class="idle-thesaurus-hint">Descarga el vocabulario desde la API TemaTres del SAIJ (puede tardar varios minutos). Si el servidor devuelve error de base de datos, aplicá la migración <code>AddThesaurusIngestSource</code> en el API.</p>
                <div class="idle-checks">
                  <label class="checkbox-label"><input type="checkbox" [(ngModel)]="runThesaurusNormalize" /><span>Normalizar keywords sin vínculo al tesauro</span></label>
                </div>
                }
                <button type="button" class="btn-run-inline"
                        [disabled]="running()"
                        (click)="confirmRunInline()">
                  {{ running() ? 'Iniciando...' : 'Ejecutar' }}
                </button>
              }
            </div>
          </section>
        }

        <section class="sources-section">
          <h2>Fuentes</h2>
          @for (src of sources(); track src.config.sourceId) {
            <div class="source-accordion" [class.expanded]="src.expanded">
              <div class="accordion-header" (click)="toggleSection(src)">
                <button type="button" class="expand-btn" [attr.aria-expanded]="src.expanded">
                  <span class="chevron">›</span>
                </button>
                <span class="source-name">{{ src.config.sourceName }}</span>
                <span class="source-entity-tag">{{ entityLabel(src.config.sourceId) }}</span>
                <div class="header-meta">
                  @if (src.config.lastCrawledAt) {
                    <span class="status-dot" [class]="statusDotClass(src.config.lastCrawledStatus)"></span>
                    <span class="last-crawl">{{ formatDate(src.config.lastCrawledAt) }}</span>
                  }
                  <span class="doc-count">{{ latestJobDocs(src) }} docs</span>
                </div>
                <div class="header-actions" (click)="$event.stopPropagation()">
                  @if (src.config.synthetic) {
                    <span class="toggle-pill synthetic-pill" title="Aplicá la migración AddThesaurusIngestSource en la API para persistir esta fuente. La ingesta del tesauro funciona igual.">Solo UI</span>
                  } @else {
                  <button type="button" class="toggle-pill" [class.active]="src.config.isEnabled"
                          [disabled]="updatingId() === src.config.sourceId"
                          (click)="toggleEnabled(src.config)">
                    {{ src.config.isEnabled ? 'Activo' : 'Inactivo' }}
                  </button>
                  }
                </div>
              </div>

              @if (src.expanded) {
                <div class="accordion-body">
                  @if (src.jobs.length === 0) {
                    <p class="empty-jobs">Sin jobs registrados.</p>
                  } @else {
                    <div class="jobs-list">
                      @for (job of (src.showAllJobs ? src.jobs : src.jobs | slice:0:5); track job.id) {
                        <div class="job-item">
                          <div
                            class="job-row"
                            [class.expanded]="expandedJobId() === job.id"
                            role="button"
                            tabindex="0"
                            (click)="toggleJobExpand(job)"
                            (keydown.enter)="toggleJobExpand(job)"
                            (keydown.space)="$event.preventDefault(); toggleJobExpand(job)">
                            <div class="job-info">
                              <span class="job-date">{{ formatDate(job.startedAt) }}</span>
                              <span class="job-title">{{ jobTitle(job) }}</span>
                            </div>
                            <span class="job-badge" [class]="jobStatusClass(job.status)">{{ job.status }}</span>
                            @if (job.infrastructureDegraded) {
                              <span class="job-badge infra-degraded" title="{{ job.degradedReason || 'Infra degradada' }}">infra</span>
                            }
                            <div class="job-pipeline">
                              <div class="pipeline-bar">
                                @if (pipelineTotal(job) > 0) {
                                  @let segs = pipelineSegments(job);
                                  <div class="seg completed" [style.width.%]="segs.completed"></div>
                                  <div class="seg enriched" [style.width.%]="segs.enriched"></div>
                                  <div class="seg parsed" [style.width.%]="segs.parsed"></div>
                                  <div class="seg crawled" [style.width.%]="segs.crawled"></div>
                                  <div class="seg skipped" [style.width.%]="segs.skipped"></div>
                                  <div class="seg failed" [style.width.%]="segs.failed"></div>
                                } @else {
                                  <div class="seg empty-bar" style="width:100%"></div>
                                }
                              </div>
                            </div>
                            <span class="job-stats">
                              {{ job.documentsIndexed }}/{{ job.documentsDiscovered }}
                              @if (job.documentsSkipped > 0) {
                                <span class="skipped-count">{{ job.documentsSkipped }} skip</span>
                              }
                              @if (job.documentsFailed > 0) {
                                <span class="failed-count">{{ job.documentsFailed }} err</span>
                              }
                            </span>
                            <span class="row-chevron">›</span>
                          </div>
                          @if (expandedJobId() === job.id) {
                            <div class="job-expand-panel">
                              <div class="stage-counters job-history-counters">
                                @for (sc of jobStageCounters(job); track sc.label) {
                                  @if (sc.label === 'Fallidos') {
                                    <button
                                      type="button"
                                      class="stage-counter sc-failed job-failed-trigger"
                                      [disabled]="sc.value === 0"
                                      (click)="openFailedModal(job); $event.stopPropagation()">
                                      <span class="sc-value">{{ sc.value }}</span>
                                      <span class="sc-label">{{ sc.label }}</span>
                                    </button>
                                  } @else {
                                    <div class="stage-counter" [class]="sc.colorClass">
                                      <span class="sc-value">{{ sc.value }}</span>
                                      <span class="sc-label">{{ sc.label }}</span>
                                    </div>
                                  }
                                }
                              </div>
                            </div>
                          }
                        </div>
                      }
                    </div>
                    @if (!src.showAllJobs && src.jobs.length > 5) {
                      <button type="button" class="btn-show-more" (click)="src.showAllJobs = true">
                        Ver mas ({{ src.jobs.length - 5 }} restantes)
                      </button>
                    }
                  }
                </div>
              }
            </div>
          }
        </section>

        @if (auditModalOpen()) {
          <div class="audit-modal-backdrop" (click)="closeJobAuditModal()">
            <div class="audit-modal" (click)="$event.stopPropagation()">
              <div class="audit-modal-header">
                <h3>Auditoría de job</h3>
                <button type="button" class="audit-modal-close" (click)="closeJobAuditModal()" aria-label="Cerrar">×</button>
              </div>
              @if (auditLoading()) {
                <p class="audit-modal-loading">Consultando servidor…</p>
              } @else if (auditError()) {
                <p class="audit-modal-error">{{ auditError() }}</p>
              } @else {
                @if (auditResult(); as ar) {
                <p class="audit-modal-sub">{{ ar.sourceName }} · {{ ar.jobStatus }} · {{ formatDate(ar.startedAt) }}</p>
                <p class="audit-meta">Auditado (UTC): {{ formatDate(ar.auditedAtUtc) }}</p>
                @if (ar.infrastructureDegraded) {
                  <div class="audit-degraded-banner" role="status">
                    <strong>Infra degradada (persistido en BD)</strong>
                    @if (ar.degradedReason) {
                      <span class="audit-degraded-reason">{{ ar.degradedReason }}</span>
                    }
                    @if (ar.degradedSinceUtc) {
                      <span class="audit-degraded-since">Desde {{ formatDate(ar.degradedSinceUtc) }}</span>
                    }
                  </div>
                }
                <section class="audit-section audit-infra-section">
                  <h4>Infra (colas Azure)</h4>
                  @if (auditProbeResult(); as pr) {
                    <ul class="audit-kv audit-infra-kv">
                      <li><span>Discoverer</span><code class="audit-probe-code" [class.audit-probe-ok]="pr.discovererOk" [class.audit-probe-fail]="!pr.discovererOk">{{ pr.discovererOk ? 'OK' : (pr.discovererError || 'fallo') }}</code></li>
                      <li><span>Fetcher</span><code class="audit-probe-code" [class.audit-probe-ok]="pr.fetcherOk" [class.audit-probe-fail]="!pr.fetcherOk">{{ pr.fetcherOk ? 'OK' : (pr.fetcherError || 'fallo') }}</code></li>
                    </ul>
                    <div class="audit-actions audit-actions-row">
                      <button type="button" class="btn-audit-recover"
                              [disabled]="auditRecoverBusy() || !auditInfraProbeOk()"
                              (click)="recoverPipelineFromAudit(ar)">
                        {{ auditRecoverBusy() ? 'Aplicando…' : 'Recuperar pipeline (infra OK)' }}
                      </button>
                    </div>
                    <p class="audit-risk-hint">
                      Ejecuta recuperación en servidor: probe, limpia degradado, broadcast, reencola Fetcher pendientes, resume discovery y reanuda workers. Solo si Discoverer y Fetcher muestran OK.
                    </p>
                  }
                </section>
                <div class="audit-flag-row">
                  <div class="audit-flag" [class.ok]="ar.satisfiesCompletionFormula" [class.warn]="!ar.satisfiesCompletionFormula">
                    Fórmula cierre API: Indexed+Failed ≥ Discovered−Skipped →
                    <strong>{{ ar.satisfiesCompletionFormula ? 'sí' : 'no' }}</strong>
                  </div>
                  @if (ar.formulaSatisfiedButStillOutstanding) {
                    <div class="audit-flag warn audit-flag-secondary">
                      Aun hay filas Pending/Processing: el job puede seguir en «processing» aunque la fórmula dé «sí».
                    </div>
                  }
                </div>
                <section class="audit-section">
                  <h4>Contadores del job (IngestionJobs)</h4>
                  <ul class="audit-kv">
                    <li><span>Descubiertos</span><code>{{ ar.jobDocumentsDiscovered }}</code></li>
                    <li><span>Omitidos (job)</span><code>{{ ar.jobDocumentsSkipped }}</code></li>
                    <li><span>Indexados</span><code>{{ ar.jobDocumentsIndexed }}</code></li>
                    <li><span>Fallidos (job)</span><code>{{ ar.jobDocumentsFailed }}</code></li>
                    <li><span>Persistidos</span><code>{{ ar.jobDocumentsPersisted }}</code></li>
                    <li><span>Enriquecidos</span><code>{{ ar.jobDocumentsEnriched }}</code></li>
                    <li><span>Parseados</span><code>{{ ar.jobDocumentsParsed }}</code></li>
                    <li><span>Descargados</span><code>{{ ar.jobDocumentsCrawled }}</code></li>
                  </ul>
                </section>
                <section class="audit-section">
                  <h4>Documents (filas reales)</h4>
                  <ul class="audit-kv">
                    <li><span>Total filas</span><code>{{ ar.documentsTotalRows }}</code></li>
                    <li><span>Completed</span><code>{{ ar.dbCountsByStatus.completed }}</code></li>
                    <li><span>Pending+Processing</span><code>{{ ar.dbCountsByStatus.outstandingPendingOrProcessing }}</code></li>
                    <li><span>Failed</span><code>{{ ar.dbCountsByStatus.failed }}</code></li>
                    <li><span>Δ Indexados vs Completed</span><code>{{ ar.indexedMinusCompletedDelta }}</code></li>
                    <li><span>Δ Fallidos job vs Failed filas</span><code>{{ ar.failedMinusFailedRowsDelta }}</code></li>
                  </ul>
                </section>
                <section class="audit-section">
                  <h4>Rearmar contadores</h4>
                  <p class="audit-risk-hint">
                    Recalcula Descargados…Indexados y Fallidos del job según las filas en Documents. No cambia Descubiertos ni Omitidos del job. Requiere cero Pending/Processing en Documents.
                  </p>
                  <div class="audit-actions audit-actions-row">
                    <button type="button" class="btn-audit-reconcile"
                            [disabled]="auditReconcileBusy() || !canReconcileJobCounters(ar)"
                            (click)="reconcileJobCountersFromAudit(ar)">
                      {{ auditReconcileBusy() ? 'Aplicando…' : 'Rearmar desde Documents' }}
                    </button>
                  </div>
                </section>
                <section class="audit-section">
                  <h4>Workers (pausa en BD + SignalR)</h4>
                  <ul class="audit-workers">
                    @for (w of ar.workers; track w.workerType) {
                      <li [class.paused]="w.isPaused"
                          [class.offline]="!w.isPaused && w.signalRConnectedInstances === 0">
                        <span class="aw-type">{{ w.workerType }}</span>
                        <span class="aw-state">{{ w.isPaused ? 'Pausa BD' : 'Sin pausa BD' }}</span>
                        <span class="aw-sr" [class.warn]="w.signalRConnectedInstances === 0">
                          SignalR: {{ w.signalRConnectedInstances }}
                        </span>
                      </li>
                    }
                  </ul>
                </section>
                <section class="audit-section">
                  <h4>Ventana administrativa (heurística)</h4>
                  <p class="audit-risk-hint">
                    Indica si es razonable aplicar correcciones en base de datos con menor riesgo. No prueba que no queden mensajes solo de este job ni trabajo en vuelo.
                  </p>
                  <div class="audit-flag" [class.ok]="ar.structuralRepairSafety.suggestedAdministrativeSafeWindow"
                       [class.warn]="!ar.structuralRepairSafety.suggestedAdministrativeSafeWindow">
                    Heurística «colas principales ~0» + «sin Processing para este job» →
                    <strong>{{ ar.structuralRepairSafety.suggestedAdministrativeSafeWindow ? 'alineada' : 'no alineada' }}</strong>
                  </div>
                  <ul class="audit-kv">
                    <li><span>Sin filas Processing (este job)</span><code>{{ ar.structuralRepairSafety.noProcessingRowsForJob ? 'sí' : 'no' }}</code></li>
                    <li><span>Suma aprox. colas pipeline (main)</span><code>{{ ar.structuralRepairSafety.approxMainPipelineQueueMessageCount }}</code></li>
                    <li><span>Algún worker conectado (SignalR)</span><code>{{ ar.structuralRepairSafety.anyWorkerConnectedSignalR ? 'sí' : 'no' }}</code></li>
                  </ul>
                  <p class="audit-risk-hint">Advertencias</p>
                  <ul class="audit-notes">
                    @for (c of ar.structuralRepairSafety.caveats; track $index) {
                      <li>{{ c }}</li>
                    }
                  </ul>
                </section>
                <section class="audit-section">
                  <h4>Documentos en riesgo</h4>
                  <p class="audit-risk-hint">
                    «Processing» puede ser normal si el job avanza y hay workers activos. Tras caídas, algunos atascos recién se ven cuando el resto terminó y las colas están vacías.
                  </p>
                  @if (ar.riskDocuments.length === 0) {
                    <p class="audit-empty-risk">Ninguno detectado en esta pasada.</p>
                  } @else {
                    <ul class="audit-risk-list">
                      @for (d of ar.riskDocuments; track d.id) {
                        <li>
                          <code>{{ d.externalId }}</code>
                          <span class="audit-risk-meta">{{ d.currentStage }} · {{ d.status }}</span>
                        </li>
                      }
                    </ul>
                  }
                  <div class="audit-actions">
                    <button type="button" class="btn-audit-repair"
                            [disabled]="auditRepairBusy() || !canRepairPendingTail(ar)"
                            (click)="repairPendingTailFromAudit(ar)">
                      {{ auditRepairBusy() ? 'Aplicando…' : 'Marcar Pending Persister/Indexer como fallo' }}
                    </button>
                  </div>
                </section>
                <section class="audit-section">
                  <h4>Notas</h4>
                  <ul class="audit-notes">
                    @for (n of ar.notes; track $index) {
                      <li>{{ n }}</li>
                    }
                  </ul>
                </section>
                }
              }
            </div>
          </div>
        }

        @if (failedModalJob(); as fj) {
          <div class="failed-modal-backdrop" (click)="closeFailedModal()">
            <div class="failed-modal" (click)="$event.stopPropagation()">
              <div class="failed-modal-header">
                <h3>Fallidos · {{ jobTitle(fj) }}</h3>
                <button type="button" class="failed-modal-close" (click)="closeFailedModal()" aria-label="Cerrar">×</button>
              </div>
              <p class="failed-modal-sub">{{ formatDate(fj.startedAt) }}</p>
              <div class="failed-modal-toolbar">
                <div class="failed-modal-toolbar-actions">
                  <button
                    type="button"
                    class="btn-modal-action"
                    [disabled]="failedBulkBusy() || failedLoading() || failedDocsVisible().length === 0 || !!failedRowBusyId()"
                    (click)="bulkRequeueFailed(fj)">
                    {{ failedBulkBusy() ? 'Reencolando…' : 'Reencolar todos' }}
                  </button>
                  <button
                    type="button"
                    class="btn-modal-action discard"
                    [disabled]="failedBulkBusy() || failedLoading() || failedDocsVisible().length === 0 || !!failedRowBusyId()"
                    (click)="bulkDiscardFailed(fj)">
                    {{ failedBulkBusy() ? 'Procesando…' : 'Descartar todos' }}
                  </button>
                </div>
                @if (failedWorkerTagEntries().length > 0) {
                  <div class="failed-modal-worker-tags" role="group" aria-label="Filtrar por etapa del pipeline">
                    @for (t of failedWorkerTagEntries(); track t.key) {
                      <button
                        type="button"
                        class="worker-tag"
                        [class.on]="isFailedWorkerTagOn(t.key)"
                        [class.off]="!isFailedWorkerTagOn(t.key)"
                        (click)="toggleFailedWorkerTag(t.key)">
                        <span class="worker-tag-label" [title]="t.key">{{ failedStageChipLabel(t.key) }}</span>
                        <span class="worker-tag-count">{{ t.count }}</span>
                      </button>
                    }
                  </div>
                }
              </div>
              @if (failedFeedback(); as fb) {
                <div class="failed-modal-feedback" [class]="'failed-modal-feedback-' + fb.type" role="status">
                  {{ fb.text }}
                </div>
              }
              @if (failedBulkBusy() || failedRowBusyId()) {
                <div class="failed-modal-progress" role="status" aria-live="polite">
                  <span class="failed-modal-spinner" aria-hidden="true"></span>
                  <span>
                    @if (failedBulkBusy()) {
                      Ejecutando acción masiva en el servidor…
                    } @else if (failedRowAction() === 'saveTimeout') {
                      Guardando timeout de descarga PDF…
                    } @else if (failedRowAction() === 'clearTimeout') {
                      Quitando timeout de descarga PDF…
                    } @else {
                      Enviando acción al servidor…
                    }
                  </span>
                </div>
              }
              @if (failedLoading()) {
                <p class="failed-modal-loading">Cargando fallidos...</p>
              } @else if (failedDocsTotalCount() === 0) {
                <p class="failed-modal-empty">No hay documentos fallidos (o ya fueron procesados).</p>
              } @else {
                @if (failedDocsTotalCount() > failedDocs().length) {
                  <p class="failed-modal-truncation">
                    Mostrando {{ failedDocs().length }} de {{ failedDocsTotalCount() }} fallidos.
                  </p>
                }
                @if (failedDocs().length > 0 && failedDocsVisible().length === 0) {
                  <p class="failed-modal-filter-empty">Ningún documento coincide con los workers seleccionados.</p>
                }
                @if (failedDocsVisible().length > 0) {
                <ul class="failed-doc-list">
                  @for (d of failedDocsVisible(); track d.id) {
                    <li class="failed-doc-row" [class.failed-doc-row-busy]="failedRowBusyId() === d.id">
                      <div class="failed-doc-head">
                        <span class="failed-doc-id">{{ d.externalId }}</span>
                        <span class="failed-doc-stage">{{ d.currentStage }}</span>
                      </div>
                      @if (d.errorType) {
                        <span class="failed-doc-type">{{ d.errorType }}</span>
                      }
                      <pre class="failed-doc-msg">{{ d.errorMessage || '(Sin mensaje)' }}</pre>
                      <div class="failed-doc-actions">
                        @if (d.currentStage === 'Fetcher') {
                          <div class="failed-fetch-timeout">
                            <label class="failed-fetch-timeout-label" [attr.for]="'ft-' + d.id">Timeout PDF (s)</label>
                            <input
                              type="number"
                              class="failed-fetch-timeout-input"
                              [id]="'ft-' + d.id"
                              min="60"
                              max="900"
                              step="30"
                              [value]="fetchPdfTimeoutInputValue(d)"
                              (input)="onFetchPdfTimeoutInput(d.id, $event)"
                              [disabled]="failedRowBusyId() === d.id || failedBulkBusy()"
                              title="Solo para esta descarga en Fetcher (60–900). Luego usá Reencolar." />
                            <button
                              type="button"
                              class="btn-doc-action btn-doc-action-secondary"
                              [disabled]="failedRowBusyId() === d.id || failedBulkBusy()"
                              (click)="saveFetchPdfTimeout(fj, d)">
                              {{ failedRowBusyId() === d.id && failedRowAction() === 'saveTimeout' ? 'Guardando…' : 'Guardar timeout' }}
                            </button>
                            @if (d.fetchPdfTimeoutSeconds != null) {
                              <button
                                type="button"
                                class="btn-doc-action discard"
                                [disabled]="failedRowBusyId() === d.id || failedBulkBusy()"
                                (click)="clearFetchPdfTimeout(fj, d)">
                                {{ failedRowBusyId() === d.id && failedRowAction() === 'clearTimeout' ? 'Quitando…' : 'Quitar timeout' }}
                              </button>
                            }
                          </div>
                        }
                        <div class="failed-doc-actions-row">
                          <button
                            type="button"
                            class="btn-doc-action"
                            [disabled]="failedRowBusyId() === d.id || failedBulkBusy()"
                            (click)="requeueFailedDoc(fj, d)">
                            {{ failedRowBusyId() === d.id && failedRowAction() === 'reprocess' ? 'Reencolando…' : 'Reencolar' }}
                          </button>
                          <button
                            type="button"
                            class="btn-doc-action discard"
                            [disabled]="failedRowBusyId() === d.id || failedBulkBusy()"
                            (click)="discardFailedDoc(fj, d)">
                            {{ failedRowBusyId() === d.id && failedRowAction() === 'discard' ? 'Descartando…' : 'Descartar' }}
                          </button>
                        </div>
                      </div>
                    </li>
                  }
                </ul>
                }
              }
            </div>
          </div>
        }
      </div>
    }

  `,
  styles: [`
    :host { display: block; }
    .ingestion-page { max-width: 1100px; }

    .infra-banner {
      display: flex; flex-wrap: wrap; align-items: flex-start; gap: 1rem;
      margin-bottom: 1rem; padding: 1rem 1.25rem;
      border-radius: var(--radius-md);
      border: 1px solid #ffcc80;
      background: #fff8e1;
      color: #5d4037;
    }
    .infra-banner-main { flex: 1; min-width: 200px; }
    .infra-banner-title { display: block; font-weight: 700; font-size: 0.9rem; margin-bottom: 0.35rem; }
    .infra-banner-body { margin: 0; font-size: 0.8125rem; line-height: 1.45; opacity: 0.95; }
    .infra-banner-actions { display: flex; flex-wrap: wrap; gap: 0.5rem; align-items: center; }
    .btn-infra {
      padding: 0.35rem 0.75rem; font-size: 0.75rem; font-weight: 600;
      border-radius: var(--radius-pill); cursor: pointer;
      border: 1px solid #e65100; background: #fff3e0; color: #bf360c;
    }
    .btn-infra:hover:not(:disabled) { background: #ffe0b2; }
    .btn-infra.secondary { border-color: var(--color-border-input); background: var(--color-bg-surface); color: var(--color-text-secondary); }
    .btn-infra.secondary:hover:not(:disabled) { border-color: var(--color-primary); color: var(--color-primary); }
    .btn-infra.ghost { border-color: transparent; background: transparent; color: var(--color-text-secondary); text-decoration: underline; }
    .btn-infra:disabled { opacity: 0.5; cursor: not-allowed; }

    .job-badge.infra-degraded {
      background: #fff3e0; color: #e65100; border: 1px solid #ffcc80;
      font-size: 0.65rem; text-transform: uppercase;
    }

    .state-message { text-align: center; padding: 3rem 2rem; }
    .state-message.error { color: var(--color-primary); }
    .btn-retry {
      margin-top: 1rem; padding: 0.5rem 1rem;
      background: none; border: 1px solid var(--color-primary);
      color: var(--color-primary); border-radius: var(--radius-sm); cursor: pointer;
    }

    /* --- Worker Control --- */
    .worker-control {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: 1.25rem 1.5rem;
      margin-bottom: 1.5rem;
      box-shadow: var(--shadow-sm);
    }
    .worker-header {
      display: flex; justify-content: space-between; align-items: center;
      margin-bottom: 0.75rem;
    }
    .worker-header h2 { margin: 0; font-size: 1rem; font-weight: 600; }
    .worker-bulk { display: flex; gap: 0.5rem; }
    .btn-bulk {
      padding: 0.3rem 0.75rem; font-size: 0.75rem; font-weight: 600;
      border-radius: var(--radius-pill); cursor: pointer;
      border: 1px solid var(--color-border-input);
      transition: all var(--transition-base);
    }
    .btn-bulk.pause { background: #fff3e0; color: #e65100; border-color: #ffcc80; }
    .btn-bulk.pause:hover:not(:disabled) { background: #ffe0b2; }
    .btn-bulk.resume { background: var(--color-success-bg); color: var(--color-success); border-color: var(--color-success); }
    .btn-bulk.resume:hover:not(:disabled) { background: #c8e6c9; }
    .btn-bulk:disabled { opacity: 0.5; cursor: not-allowed; }
    .btn-bulk.audit {
      background: var(--color-bg-surface);
      color: var(--color-text-secondary);
      border-color: var(--color-border-input);
    }
    .btn-bulk.audit:hover:not(:disabled) {
      border-color: var(--color-primary);
      color: var(--color-primary);
    }
    .worker-pills { display: flex; flex-wrap: wrap; gap: 0.5rem; }
    .worker-pill {
      display: flex; align-items: center; gap: 0.4rem;
      padding: 0.4rem 0.85rem; font-size: 0.8125rem; font-weight: 500;
      border-radius: var(--radius-pill);
      border: 1px solid var(--color-border-input);
      background: var(--color-bg-subtle);
      cursor: pointer;
      transition: all var(--transition-base);
    }
    .worker-pill:hover:not(:disabled) { border-color: var(--color-primary); }
    .worker-pill.paused { background: #fff3e0; border-color: #ffcc80; }
    .worker-pill:disabled { opacity: 0.6; cursor: not-allowed; }
    .pill-dot {
      width: 8px; height: 8px; border-radius: 50%;
      background: var(--color-success);
    }
    .pill-dot.paused { background: #e65100; }

    /* --- Active Job Panel --- */
    .active-panel {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: 1.25rem 1.5rem;
      margin-bottom: 1.5rem;
      box-shadow: var(--shadow-sm);
    }
    .active-header {
      display: flex; justify-content: space-between; align-items: center;
      margin-bottom: 0.875rem;
    }
    .active-title {
      display: flex; align-items: center; gap: 0.5rem;
    }
    .active-source {
      font-weight: 700; font-size: 0.9375rem;
    }
    .active-job-name {
      font-size: 0.8125rem; color: var(--color-text-secondary);
    }
    .active-actions { display: flex; gap: 0.5rem; }
    .active-degraded-hint {
      margin: 0 0 0.85rem 0;
      padding: 0.45rem 0.65rem;
      font-size: 0.75rem;
      line-height: 1.35;
      border-radius: var(--radius-sm);
      background: #fff8e1;
      border: 1px solid #ffcc80;
      color: #5d4037;
    }

    .active-bar {
      display: flex; height: 8px; border-radius: 4px;
      overflow: hidden; background: #eee;
      margin-bottom: 1rem;
    }

    .stage-counters {
      display: flex; gap: 0.25rem; flex-wrap: wrap;
    }
    .stage-counter {
      display: flex; flex-direction: column; align-items: center;
      gap: 2px; min-width: 96px; flex: 1;
      padding: 0.5rem 0.25rem;
      border-radius: var(--radius-sm);
      background: var(--color-bg-subtle);
    }
    .sc-value {
      font-family: var(--font-mono); font-size: 1.125rem; font-weight: 700;
      line-height: 1;
    }
    .sc-label {
      font-size: 0.5625rem; font-weight: 600; text-transform: uppercase;
      letter-spacing: 0.04em; color: var(--color-text-secondary);
    }
    .stage-counter.sc-completed .sc-value { color: var(--color-success); }
    .stage-counter.sc-persisted .sc-value { color: #66bb6a; }
    .stage-counter.sc-enriched .sc-value { color: #4caf50; }
    .stage-counter.sc-parsed .sc-value { color: #42a5f5; }
    .stage-counter.sc-crawled .sc-value { color: #90caf9; }
    .stage-counter.sc-discovered .sc-value { color: var(--color-text); }
    .stage-counter.sc-skipped .sc-value { color: #757575; }
    .stage-counter.sc-failed .sc-value { color: var(--color-primary); }
    .stage-counter.sc-queued .sc-value { color: #e65100; }

    .sc-toggle {
      display: flex; align-items: center; justify-content: center;
      width: 22px; height: 22px;
      border: 1px solid var(--color-border-input);
      border-radius: 50%; background: white;
      cursor: pointer; padding: 0;
      transition: all var(--transition-base);
      margin-top: 0;
      flex-shrink: 0;
    }
    .sc-toggle:hover:not(:disabled) {
      border-color: var(--color-primary);
      background: var(--color-primary-light);
    }
    .sc-toggle.paused {
      background: #fff3e0; border-color: #ffcc80;
    }
    .sc-toggle:disabled { opacity: 0.5; cursor: not-allowed; }
    .sc-icon {
      width: 12px; height: 12px;
      fill: var(--color-text-secondary);
    }
    .sc-toggle.paused .sc-icon { fill: #e65100; }

    .sc-worker-controls {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0.15rem;
      margin-top: 2px;
      width: 100%;
    }
    .sc-side {
      display: inline-flex;
      align-items: center;
      gap: 1px;
      min-width: 1.35rem;
      justify-content: center;
      color: var(--color-text-secondary);
    }
    .sc-side-queue { cursor: default; }
    .sc-side-failed { cursor: default; }
    .sc-side-failed.sc-side-has-failed {
      color: var(--color-primary);
    }
    .sc-side-failed.sc-side-has-failed .sc-side-icon {
      opacity: 1;
    }
    .sc-side-num {
      font-family: var(--font-mono);
      font-size: 0.625rem;
      font-weight: 700;
      line-height: 1;
      min-width: 0.65rem;
      text-align: center;
    }
    .sc-side-icon {
      width: 11px;
      height: 11px;
      flex-shrink: 0;
      opacity: 0.85;
    }
    .sc-side-icon-alert {
      width: 12px;
      height: 12px;
    }

    /* --- Idle Panel (new job form) --- */
    .pending-panel {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: 1.25rem 1.5rem;
      margin-bottom: 1.5rem;
      box-shadow: var(--shadow-sm);
      display: flex; align-items: center; gap: 0.75rem;
      font-size: 0.875rem; color: var(--color-text-secondary);
    }
    .pending-spinner {
      width: 18px; height: 18px;
      border: 2px solid var(--color-border);
      border-top-color: var(--color-primary);
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }
    @keyframes spin { to { transform: rotate(360deg); } }

    .idle-panel {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: 1.25rem 1.5rem;
      margin-bottom: 1.5rem;
      box-shadow: var(--shadow-sm);
    }
    .idle-panel h2 { margin: 0 0 0.75rem 0; font-size: 1rem; font-weight: 600; }
    .idle-form {
      display: flex; align-items: flex-end; gap: 0.625rem; flex-wrap: wrap;
    }
    .idle-field { display: flex; flex-direction: column; gap: 3px; }
    .idle-field label {
      font-size: 0.625rem; font-weight: 600; text-transform: uppercase;
      letter-spacing: 0.04em; color: var(--color-text-secondary);
    }
    .idle-field select,
    .idle-field input[type="date"],
    .idle-field input[type="number"] {
      height: 2rem; padding: 0 0.5rem;
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      font-size: 0.8125rem;
    }
    .idle-field select { min-width: 140px; }
    .idle-field input[type="date"] { width: 130px; }
    .idle-field.narrow input { width: 70px; }
    .idle-checks {
      display: flex; flex-direction: column; gap: 2px; padding-bottom: 2px;
    }
    .idle-checks .checkbox-label {
      display: flex; align-items: center; gap: 0.35rem;
      font-size: 0.6875rem; cursor: pointer;
    }
    .idle-checks .checkbox-label input { width: auto; height: auto; margin: 0; }
    .idle-thesaurus-hint {
      margin: 0 0 0.35rem 0;
      max-width: 28rem;
      font-size: 0.75rem;
      line-height: 1.35;
      color: var(--color-text-secondary);
    }
    .btn-run-inline {
      height: 2rem; padding: 0 1rem;
      font-size: 0.8125rem; font-weight: 600;
      background: var(--color-primary); color: white; border: none;
      border-radius: var(--radius-sm); cursor: pointer;
      transition: background var(--transition-base);
      white-space: nowrap;
    }
    .btn-run-inline:hover:not(:disabled) { background: var(--color-primary-hover); }
    .btn-run-inline:disabled { opacity: 0.5; cursor: not-allowed; }

    /* --- Sources Section --- */
    .sources-section h2 { font-size: 1rem; font-weight: 600; margin: 0 0 1rem 0; }
    .source-accordion {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      margin-bottom: 0.5rem;
      box-shadow: var(--shadow-sm);
      transition: box-shadow var(--transition-base);
    }
    .source-accordion.expanded { box-shadow: var(--shadow-md); }
    .accordion-header {
      display: flex; align-items: center; gap: 0.75rem;
      padding: 0.875rem 1.25rem;
      cursor: pointer;
      user-select: none;
    }
    .accordion-header:hover { background: var(--color-bg-subtle); border-radius: var(--radius-md); }
    .expand-btn {
      background: none; border: none; padding: 0;
      font-size: 1.25rem; color: var(--color-text-secondary);
      cursor: pointer; line-height: 1;
      transition: transform var(--transition-base);
    }
    .source-accordion.expanded .expand-btn .chevron { display: inline-block; transform: rotate(90deg); }
    .chevron { display: inline-block; transition: transform var(--transition-base); }
    .source-name { font-weight: 600; font-size: 0.9375rem; white-space: nowrap; }
    .source-entity-tag {
      font-size: 0.625rem; font-weight: 600; text-transform: uppercase;
      padding: 2px 6px; border-radius: var(--radius-pill);
      background: var(--color-bg-subtle); color: var(--color-text-secondary);
    }
    .header-meta {
      display: flex; align-items: center; gap: 0.5rem;
      font-size: 0.75rem; color: var(--color-text-secondary);
      margin-left: auto;
    }
    .status-dot { width: 7px; height: 7px; border-radius: 50%; flex-shrink: 0; }
    .status-dot.success { background: var(--color-success); }
    .status-dot.partial { background: #e6a817; }
    .status-dot.failed { background: var(--color-primary); }
    .status-dot.unknown { background: #ccc; }
    .last-crawl { white-space: nowrap; }
    .doc-count { font-family: var(--font-mono); font-size: 0.6875rem; }
    .header-actions { display: flex; gap: 0.5rem; margin-left: 0.75rem; flex-shrink: 0; }
    .toggle-pill {
      padding: 0.2rem 0.6rem; border-radius: var(--radius-pill);
      font-size: 0.6875rem; font-weight: 600;
      border: 1px solid var(--color-border-input);
      background: #f0f0f0; color: var(--color-text-secondary);
      cursor: pointer; transition: all var(--transition-base);
    }
    .toggle-pill.active {
      background: var(--color-success-bg);
      border-color: var(--color-success);
      color: var(--color-success);
    }
    .toggle-pill.synthetic-pill {
      cursor: default;
      opacity: 0.85;
      font-size: 0.625rem;
      padding: 0.2rem 0.5rem;
      background: var(--color-bg-subtle);
      color: var(--color-text-secondary);
      border: 1px dashed var(--color-border-input);
    }
    .toggle-pill:disabled { opacity: 0.6; cursor: not-allowed; }

    /* --- Accordion Body / Jobs --- */
    .accordion-body { padding: 0 1.25rem 1rem 2.75rem; }
    .empty-jobs { font-size: 0.8125rem; color: var(--color-text-secondary); margin: 0; }
    .jobs-list { display: flex; flex-direction: column; gap: 0.375rem; }
    .job-item { display: flex; flex-direction: column; gap: 2px; }
    .job-row {
      display: grid;
      grid-template-columns: minmax(200px, 1.5fr) 80px minmax(100px, 1.5fr) auto 20px;
      align-items: center; gap: 0.75rem;
      padding: 0.5rem 0.75rem;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      color: inherit;
      font-size: 0.8125rem;
      transition: all var(--transition-base);
      cursor: pointer;
      outline: none;
    }
    .job-row:focus-visible {
      outline: 2px solid var(--color-primary);
      outline-offset: 2px;
    }
    .job-row.expanded {
      border-color: var(--color-primary);
      background: var(--color-primary-light);
    }
    .job-row:hover { border-color: var(--color-primary); background: var(--color-primary-light); }
    .job-info { display: flex; flex-direction: column; gap: 1px; min-width: 0; }
    .job-date { font-size: 0.625rem; color: var(--color-text-secondary); font-family: var(--font-mono); }
    .job-title { font-weight: 500; font-size: 0.75rem; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .job-badge {
      font-size: 0.5625rem; font-weight: 700; text-transform: uppercase;
      padding: 2px 6px; border-radius: var(--radius-pill); text-align: center;
    }
    .job-badge.running, .job-badge.processing { background: #e3f2fd; color: #1565c0; }
    .job-badge.pending, .job-badge.discovered { background: #e8e8e8; color: #555; }
    .job-badge.completed, .job-badge.success { background: var(--color-success-bg); color: var(--color-success); }
    .job-badge.failed { background: var(--color-primary-light); color: var(--color-primary); }
    .job-badge.partial { background: #fff8d6; color: #7a5a00; }
    .job-badge.outstanding-queue {
      background: #fff3e0;
      color: #e65100;
      font-weight: 600;
    }
    .job-pipeline { min-width: 0; }
    .pipeline-bar {
      display: flex; height: 5px; border-radius: 3px;
      overflow: hidden; background: #eee;
    }
    .seg { min-width: 0; transition: width 0.3s; }
    .seg.completed { background: var(--color-success); }
    .seg.enriched { background: #4caf50; }
    .seg.parsed { background: #42a5f5; }
    .seg.crawled { background: #90caf9; }
    .seg.skipped { background: #bdbdbd; }
    .seg.failed { background: var(--color-primary); }
    .seg.empty-bar { background: #e0e0e0; }
    .job-stats {
      font-size: 0.6875rem; color: var(--color-text-secondary);
      font-family: var(--font-mono); white-space: nowrap;
    }
    .skipped-count { color: #757575; margin-left: 0.25rem; }
    .failed-count { color: var(--color-primary); margin-left: 0.25rem; }
    .row-chevron { font-size: 1.125rem; color: var(--color-text-secondary); }
    .job-row:hover .row-chevron { color: var(--color-primary); }
    .btn-show-more {
      margin-top: 0.5rem; padding: 0.3rem 0;
      background: none; border: none;
      font-size: 0.75rem; color: var(--color-primary);
      cursor: pointer; text-decoration: underline;
    }
    .btn-show-more:hover { color: var(--color-primary-hover); }

    .job-expand-panel {
      padding: 0.75rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
    }
    .job-history-counters { margin: 0; }
    .job-failed-trigger {
      border: none;
      font: inherit;
      width: 100%;
      cursor: pointer;
      text-align: center;
    }
    .job-failed-trigger:disabled {
      opacity: 0.45;
      cursor: not-allowed;
    }

    .failed-modal-backdrop {
      position: fixed;
      inset: 0;
      z-index: 1000;
      background: rgba(0, 0, 0, 0.45);
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 1rem;
    }
    .failed-modal {
      width: 100%;
      max-width: 640px;
      max-height: min(85vh, 720px);
      overflow: hidden;
      display: flex;
      flex-direction: column;
      background: var(--color-bg-surface);
      border-radius: var(--radius-md);
      box-shadow: var(--shadow-md);
      border: 1px solid var(--color-border);
    }
    .failed-modal-header {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      gap: 0.75rem;
      padding: 1rem 1rem 0 1rem;
    }
    .failed-modal-header h3 {
      margin: 0;
      font-size: 1rem;
      font-weight: 600;
    }
    .failed-modal-close {
      background: none;
      border: none;
      font-size: 1.5rem;
      line-height: 1;
      cursor: pointer;
      color: var(--color-text-secondary);
      padding: 0 0.25rem;
    }
    .failed-modal-close:hover { color: var(--color-primary); }
    .failed-modal-sub {
      margin: 0.25rem 1rem 0 1rem;
      font-size: 0.75rem;
      color: var(--color-text-secondary);
    }
    .failed-modal-toolbar {
      display: flex;
      flex-wrap: wrap;
      align-items: center;
      gap: 0.5rem 0.75rem;
      padding: 0.75rem 1rem;
      border-bottom: 1px solid var(--color-border);
    }
    .failed-modal-toolbar-actions {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
      flex-shrink: 0;
    }
    .failed-modal-worker-tags {
      display: flex;
      flex-wrap: wrap;
      gap: 0.35rem;
      margin-left: auto;
      align-items: center;
      max-width: 100%;
    }
    .worker-tag {
      display: inline-flex;
      align-items: center;
      gap: 0.35rem;
      max-width: 220px;
      padding: 0.2rem 0.5rem;
      font-size: 0.6875rem;
      line-height: 1.2;
      border-radius: var(--radius-pill);
      border: 1px solid var(--color-border-input);
      background: var(--color-bg-subtle);
      color: var(--color-text-body);
      cursor: pointer;
      transition: background 0.15s, border-color 0.15s, opacity 0.15s;
    }
    .worker-tag.on {
      border-color: var(--color-primary);
      background: var(--color-primary-light);
      color: var(--color-text);
    }
    .worker-tag.off {
      opacity: 0.5;
      text-decoration: line-through;
    }
    .worker-tag-label {
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
      min-width: 0;
    }
    .worker-tag-count {
      font-weight: 700;
      flex-shrink: 0;
      opacity: 0.9;
    }
    .failed-modal-filter-empty {
      padding: 0.75rem 1rem;
      margin: 0;
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
    }
    .failed-modal-feedback {
      margin: 0 1rem;
      padding: 0.5rem 0.65rem;
      font-size: 0.8125rem;
      border-radius: var(--radius-sm);
      line-height: 1.35;
    }
    .failed-modal-feedback-success {
      background: var(--color-success-bg, #e8f5e9);
      color: var(--color-success, #2e7d32);
      border: 1px solid rgba(46, 125, 50, 0.35);
    }
    .failed-modal-feedback-error {
      background: #fef2f2;
      color: #991b1b;
      border: 1px solid rgba(220, 38, 38, 0.35);
    }
    .failed-modal-feedback-info {
      background: #eff6ff;
      color: #1e40af;
      border: 1px solid rgba(59, 130, 246, 0.35);
    }
    .failed-modal-progress {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin: 0 1rem 0.5rem 1rem;
      padding: 0.45rem 0.65rem;
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
      background: var(--color-bg-subtle);
      border-radius: var(--radius-sm);
    }
    .failed-modal-spinner {
      width: 14px;
      height: 14px;
      border: 2px solid var(--color-border);
      border-top-color: var(--color-primary);
      border-radius: 50%;
      animation: failed-spin 0.75s linear infinite;
      flex-shrink: 0;
    }
    @keyframes failed-spin {
      to { transform: rotate(360deg); }
    }
    .btn-modal-action {
      padding: 0.35rem 0.85rem;
      font-size: 0.75rem;
      font-weight: 600;
      border-radius: var(--radius-sm);
      border: 1px solid var(--color-primary);
      background: var(--color-primary);
      color: white;
      cursor: pointer;
    }
    .btn-modal-action:hover:not(:disabled) {
      background: var(--color-primary-hover);
    }
    .btn-modal-action.discard {
      background: transparent;
      color: var(--color-primary);
    }
    .btn-modal-action.discard:hover:not(:disabled) {
      background: var(--color-primary-light);
    }
    .btn-modal-action:disabled {
      opacity: 0.45;
      cursor: not-allowed;
    }
    .failed-modal-loading,
    .failed-modal-empty {
      padding: 1rem;
      margin: 0;
      font-size: 0.875rem;
      color: var(--color-text-secondary);
    }
    .failed-modal-truncation {
      padding: 0 1rem;
      margin: 0 0 0.35rem 0;
      font-size: 0.75rem;
      color: var(--color-text-secondary);
    }
    .failed-doc-list {
      list-style: none;
      margin: 0;
      padding: 0.5rem;
      overflow-y: auto;
      flex: 1;
    }
    .failed-doc-row {
      padding: 0.65rem 0.75rem;
      margin-bottom: 0.5rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      background: var(--color-bg-subtle);
      transition: border-color var(--transition-base), box-shadow var(--transition-base);
    }
    .failed-doc-row-busy {
      border-color: var(--color-primary);
      box-shadow: 0 0 0 1px rgba(230, 81, 0, 0.25);
    }
    .failed-doc-head {
      display: flex;
      flex-wrap: wrap;
      gap: 0.35rem 0.75rem;
      align-items: baseline;
      margin-bottom: 0.35rem;
      font-size: 0.75rem;
    }
    .failed-doc-id {
      font-family: var(--font-mono);
      font-weight: 600;
    }
    .failed-doc-stage {
      font-size: 0.625rem;
      font-weight: 600;
      text-transform: uppercase;
      color: var(--color-text-secondary);
    }
    .failed-doc-type {
      display: inline-block;
      font-size: 0.625rem;
      color: var(--color-primary);
      margin-bottom: 0.25rem;
    }
    .failed-doc-msg {
      margin: 0 0 0.5rem 0;
      padding: 0.35rem 0.5rem;
      font-size: 0.6875rem;
      white-space: pre-wrap;
      word-break: break-word;
      background: var(--color-bg-surface);
      border-radius: var(--radius-sm);
      max-height: 120px;
      overflow-y: auto;
    }
    .failed-doc-actions {
      display: flex;
      flex-direction: column;
      align-items: flex-start;
      gap: 0.5rem;
      margin-top: 0.35rem;
    }
    .failed-doc-actions-row {
      display: flex;
      gap: 0.35rem;
      flex-wrap: wrap;
    }
    .failed-fetch-timeout {
      display: flex;
      flex-wrap: wrap;
      align-items: center;
      gap: 0.4rem;
      width: 100%;
      padding-bottom: 0.45rem;
      border-bottom: 1px solid var(--color-border);
    }
    .failed-fetch-timeout-label {
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-text-muted);
    }
    .failed-fetch-timeout-input {
      width: 5.5rem;
      padding: 0.2rem 0.35rem;
      font-size: 0.75rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      background: var(--color-bg);
    }
    .btn-doc-action-secondary {
      background: var(--color-bg-surface);
      color: var(--color-primary);
    }
    .btn-doc-action {
      padding: 0.25rem 0.55rem;
      font-size: 0.6875rem;
      font-weight: 600;
      border-radius: var(--radius-sm);
      border: 1px solid var(--color-primary);
      background: var(--color-primary);
      color: white;
      cursor: pointer;
    }
    .btn-doc-action.discard {
      background: transparent;
      color: var(--color-primary);
    }
    .btn-doc-action:disabled {
      opacity: 0.45;
      cursor: not-allowed;
    }

    /* --- Job audit modal --- */
    .audit-modal-backdrop {
      position: fixed;
      inset: 0;
      z-index: 1000;
      background: rgba(0, 0, 0, 0.45);
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 1rem;
    }
    .audit-modal {
      width: 100%;
      max-width: 560px;
      max-height: min(88vh, 720px);
      overflow-y: auto;
      background: var(--color-bg-surface);
      border-radius: var(--radius-md);
      box-shadow: var(--shadow-md);
      border: 1px solid var(--color-border);
      padding: 0 0 1rem 0;
    }
    .audit-modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0.875rem 1rem;
      border-bottom: 1px solid var(--color-border);
    }
    .audit-modal-header h3 {
      margin: 0;
      font-size: 1rem;
      font-weight: 600;
    }
    .audit-modal-close {
      background: none;
      border: none;
      font-size: 1.5rem;
      line-height: 1;
      cursor: pointer;
      color: var(--color-text-secondary);
    }
    .audit-modal-close:hover { color: var(--color-primary); }
    .audit-modal-loading,
    .audit-modal-error {
      margin: 1rem;
      font-size: 0.875rem;
    }
    .audit-modal-error { color: var(--color-primary); }
    .audit-modal-sub {
      margin: 0.5rem 1rem 0 1rem;
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
    }
    .audit-meta {
      margin: 0.25rem 1rem 0.5rem 1rem;
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      font-family: var(--font-mono);
    }
    .audit-degraded-banner {
      margin: 0 1rem 0.65rem 1rem;
      padding: 0.5rem 0.65rem;
      border-radius: var(--radius-sm);
      border: 1px solid #ffcc80;
      background: #fff8e1;
      font-size: 0.8125rem;
      color: #5d4037;
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }
    .audit-degraded-reason { font-size: 0.75rem; opacity: 0.95; word-break: break-word; }
    .audit-degraded-since { font-size: 0.6875rem; font-family: var(--font-mono); opacity: 0.85; }
    .audit-infra-section .audit-probe-code.audit-probe-ok { color: var(--color-success); font-weight: 600; }
    .audit-infra-section .audit-probe-code.audit-probe-fail { color: #c62828; font-weight: 600; }
    .btn-audit-recover {
      padding: 0.4rem 0.75rem;
      font-size: 0.75rem;
      font-weight: 600;
      border-radius: var(--radius-sm);
      border: 1px solid var(--color-success);
      background: var(--color-success-bg);
      color: var(--color-success);
      cursor: pointer;
    }
    .btn-audit-recover:hover:not(:disabled) {
      filter: brightness(0.97);
    }
    .btn-audit-recover:disabled {
      opacity: 0.45;
      cursor: not-allowed;
    }
    .audit-flag {
      margin: 0 1rem 0.75rem 1rem;
      padding: 0.45rem 0.65rem;
      font-size: 0.75rem;
      border-radius: var(--radius-sm);
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
    }
    .audit-flag.ok { border-color: var(--color-success); color: var(--color-success); }
    .audit-flag.warn { border-color: #e65100; color: #b71c1c; }
    .audit-flag-row {
      display: flex;
      flex-direction: column;
      gap: 0.35rem;
      margin: 0 1rem 0.75rem 1rem;
    }
    .audit-flag-row .audit-flag { margin: 0; }
    .audit-flag-secondary { font-size: 0.72rem; }
    .audit-section {
      margin: 0 1rem 0.75rem 1rem;
    }
    .audit-section h4 {
      margin: 0 0 0.35rem 0;
      font-size: 0.6875rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      color: var(--color-text-secondary);
    }
    .audit-kv {
      list-style: none;
      margin: 0;
      padding: 0;
      display: grid;
      grid-template-columns: 1fr auto;
      gap: 4px 12px;
      font-size: 0.8125rem;
    }
    .audit-kv li {
      display: contents;
    }
    .audit-kv span { color: var(--color-text-secondary); }
    .audit-kv code {
      font-family: var(--font-mono);
      font-size: 0.8125rem;
      text-align: right;
    }
    .audit-workers {
      list-style: none;
      margin: 0;
      padding: 0;
      display: flex;
      flex-wrap: wrap;
      gap: 0.35rem;
    }
    .audit-workers li {
      display: inline-flex;
      align-items: center;
      gap: 0.35rem;
      padding: 0.2rem 0.5rem;
      border-radius: var(--radius-pill);
      font-size: 0.6875rem;
      border: 1px solid var(--color-border-input);
      background: var(--color-bg-subtle);
    }
    .audit-workers li.paused {
      background: #fff3e0;
      border-color: #ffcc80;
    }
    .audit-workers li.offline {
      border-style: dashed;
      border-color: #c62828;
      background: #ffebee;
    }
    .aw-type { font-weight: 600; }
    .aw-state { color: var(--color-text-secondary); }
    .aw-sr {
      font-size: 0.625rem;
      font-family: var(--font-mono);
      color: var(--color-success);
    }
    .aw-sr.warn { color: #c62828; font-weight: 600; }
    .audit-notes {
      margin: 0;
      padding-left: 1.1rem;
      font-size: 0.8125rem;
      line-height: 1.45;
      color: var(--color-text-body);
    }
    .audit-notes li + li { margin-top: 0.35rem; }

    .audit-empty-risk {
      margin: 0;
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
    }
    .audit-risk-hint {
      margin: 0 0 0.5rem 0;
      font-size: 0.6875rem;
      line-height: 1.35;
      color: var(--color-text-secondary);
    }
    .audit-risk-list {
      list-style: none;
      margin: 0 0 0.65rem 0;
      padding: 0;
      max-height: 180px;
      overflow-y: auto;
    }
    .audit-risk-list li {
      display: flex;
      flex-wrap: wrap;
      align-items: baseline;
      gap: 0.35rem 0.65rem;
      padding: 0.25rem 0;
      border-bottom: 1px solid var(--color-border);
      font-size: 0.8125rem;
    }
    .audit-risk-meta {
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
    }
    .audit-actions {
      margin-top: 0.35rem;
    }
    .audit-actions-row {
      display: flex;
      flex-wrap: wrap;
      gap: 0.4rem;
    }
    .btn-audit-reconcile {
      padding: 0.4rem 0.75rem;
      font-size: 0.75rem;
      font-weight: 600;
      border-radius: var(--radius-sm);
      border: 1px solid var(--color-border-input);
      background: var(--color-bg-subtle);
      color: var(--color-text-body);
      cursor: pointer;
    }
    .btn-audit-reconcile:hover:not(:disabled) {
      border-color: var(--color-primary);
      color: var(--color-primary);
    }
    .btn-audit-reconcile:disabled {
      opacity: 0.45;
      cursor: not-allowed;
    }
    .btn-audit-repair {
      padding: 0.4rem 0.75rem;
      font-size: 0.75rem;
      font-weight: 600;
      border-radius: var(--radius-sm);
      border: 1px solid var(--color-primary);
      background: var(--color-primary);
      color: white;
      cursor: pointer;
    }
    .btn-audit-repair:hover:not(:disabled) {
      background: var(--color-primary-hover);
    }
    .btn-audit-repair:disabled {
      opacity: 0.45;
      cursor: not-allowed;
    }

  `]
})
export class IngestionComponent implements OnInit, OnDestroy {
  /** Same as backend StartThesaurusIngestJobHandler.ThesaurusSourceId — for template and merge logic. */
  readonly thesaurusSourceId = THESAURUS_SOURCE_ID;

  private crawlerService = inject(CrawlerService);
  private notify = inject(NotificationService);
  private pollSub: Subscription | null = null;
  private failedFeedbackTimer: ReturnType<typeof setTimeout> | null = null;

  state = signal<'loading' | 'loaded' | 'error'>('loading');
  error = signal('');
  sources = signal<SourceSection[]>([]);
  workers = signal<WorkerPauseState[]>([]);
  workerBusy = signal(false);
  updatingId = signal<number | null>(null);

  expandedJobId = signal<string | null>(null);

  /** Per-stage document counts for the computed active job (from GET .../documents/summary). */
  activeJobSummary = signal<JobDocumentsSummary | null>(null);

  failedModalJob = signal<Job | null>(null);
  failedDocs = signal<JobDocument[]>([]);
  failedDocsTotalCount = signal(0);
  failedLoading = signal(false);
  failedBulkBusy = signal(false);
  failedRowBusyId = signal<string | null>(null);
  failedRowAction = signal<'reprocess' | 'discard' | 'saveTimeout' | 'clearTimeout' | null>(null);
  failedFeedback = signal<{ type: 'success' | 'error' | 'info'; text: string } | null>(null);
  /** Per-document raw input for optional PDF fetch timeout (Fetcher only). */
  fetchPdfTimeoutDraft = signal<Record<string, string>>({});
  /** Worker tag key → include in list (true). Missing key defaults to true. */
  failedWorkerFilterInclude = signal<Map<string, boolean>>(new Map());

  auditModalOpen = signal(false);
  auditLoading = signal(false);
  auditResult = signal<JobAudit | null>(null);
  auditError = signal('');
  auditRepairBusy = signal(false);
  auditReconcileBusy = signal(false);

  auditProbeResult = signal<StorageProbeResult | null>(null);
  auditRecoverBusy = signal(false);

  infraOpsBusy = signal(false);
  infraAuxBusy = signal(false);

  failedWorkerTagEntries = computed(() => {
    const docs = this.failedDocs();
    const counts = new Map<string, number>();
    for (const d of docs) {
      const k = this.workerTagKey(d);
      counts.set(k, (counts.get(k) ?? 0) + 1);
    }
    const orderIndex = (key: string): number => {
      const i = (FAILED_STAGE_FILTER_ORDER as readonly string[]).indexOf(key);
      return i >= 0 ? i : FAILED_STAGE_FILTER_ORDER.length;
    };
    return [...counts.entries()]
      .map(([key, count]) => ({ key, count }))
      .sort((a, b) => {
        const oa = orderIndex(a.key);
        const ob = orderIndex(b.key);
        if (oa !== ob) return oa - ob;
        return b.count - a.count || a.key.localeCompare(b.key);
      });
  });

  failedDocsVisible = computed(() => {
    const docs = this.failedDocs();
    const inc = this.failedWorkerFilterInclude();
    return docs.filter(d => this.failedWorkerFilterOnForKey(inc, d));
  });

  activeJob = computed<Job | null>(() => {
    const allJobs = this.sources().flatMap(s => s.jobs);
    const candidates = allJobs.filter(
      j =>
        j.status === 'running' ||
        j.status === 'discovered' ||
        j.status === 'processing' ||
        j.status === 'partial' ||
        j.status === 'pending' ||
        (j.outstandingDocuments ?? 0) > 0
    );
    candidates.sort((a, b) => {
      const oa = a.outstandingDocuments ?? 0;
      const ob = b.outstandingDocuments ?? 0;
      if (ob !== oa) return ob - oa;
      const ta = a.startedAt ? new Date(a.startedAt).getTime() : 0;
      const tb = b.startedAt ? new Date(b.startedAt).getTime() : 0;
      return tb - ta;
    });
    return candidates[0] ?? null;
  });

  infraBanner = computed(() => {
    const aj = this.activeJob();
    if (!aj?.infrastructureDegraded) return null;
    const since = aj.degradedSinceUtc ? ` Desde: ${aj.degradedSinceUtc}.` : '';
    return {
      title: 'Infra degradada',
      body: (aj.degradedReason?.trim() || 'Sin motivo almacenado.') + since,
      showActions: true,
    };
  });

  /** Discoverer + Fetcher queue probes both succeeded (audit modal). */
  auditInfraProbeOk = computed(() => {
    const p = this.auditProbeResult();
    return !!p && p.discovererOk && p.fetcherOk;
  });

  stageCounters = computed<StageCounter[]>(() => {
    const aj = this.activeJob();
    if (!aj) return [];
    return [
      { label: 'Descubiertos', value: aj.documentsDiscovered, workerType: 'Discoverer', colorClass: 'sc-discovered' },
      { label: 'Descargados', value: aj.documentsCrawled, workerType: 'Fetcher', colorClass: 'sc-crawled' },
      { label: 'Parseados', value: aj.documentsParsed, workerType: 'Parser', colorClass: 'sc-parsed' },
      { label: 'Enriquecidos', value: aj.documentsEnriched, workerType: 'Enricher', colorClass: 'sc-enriched' },
      { label: 'Persistidos', value: aj.documentsPersisted, workerType: 'Persister', colorClass: 'sc-persisted' },
      { label: 'Indexados', value: aj.documentsIndexed, workerType: 'Indexer', colorClass: 'sc-completed' },
      { label: 'En cola', value: aj.outstandingDocuments ?? 0, workerType: null, colorClass: 'sc-queued' },
      { label: 'Omitidos', value: aj.documentsSkipped ?? 0, workerType: null, colorClass: 'sc-skipped' },
      { label: 'Fallidos', value: aj.documentsFailed, workerType: null, colorClass: 'sc-failed' },
    ];
  });

  enabledSources = computed(() =>
    this.sources()
      .filter(s => s.config.isEnabled)
      .map(s => {
        const map = SOURCE_ENTITY_MAP[s.config.sourceId] ?? 'rulings';
        const entityLabel =
          map === 'statutes' ? 'Legislacion' : map === 'thesaurus' ? 'Tesauro' : 'Jurisprudencia';
        return {
          sourceId: s.config.sourceId,
          sourceName: s.config.sourceName,
          entityLabel,
        };
      })
  );

  availableCrawlTypes = computed<CrawlTypeOption[]>(() => {
    const sid = this.runSourceId();
    if (!sid) return [];
    return SOURCE_CRAWL_TYPES[sid] ?? [{ value: 'incremental', label: 'Incremental' }];
  });

  runSourceId = signal<number | null>(null);
  runType = 'incremental';
  runSince = '';
  runDateFrom = '';
  runDateTo = '';
  runMaxDocs: number | null = null;
  runSkipDocs: number | null = null;
  runUseCache = false;
  runReprocess = false;
  runThesaurusNormalize = true;
  running = signal(false);

  ngOnInit() {
    this.load();
    this.pollSub = interval(10_000).pipe(
      switchMap(() =>
        forkJoin({
          crawlers: this.crawlerService.getCrawlers(),
          jobs: this.crawlerService.getJobs(),
          workers: this.crawlerService.getWorkerStatus(),
        }).pipe(
          catchError(() => EMPTY),
          tap(({ crawlers, jobs, workers }) => this.refreshData(crawlers, jobs, workers)),
          switchMap(() => this.afterJobsRefreshed())
        )
      )
    ).subscribe();
  }

  ngOnDestroy() {
    this.pollSub?.unsubscribe();
    this.clearFailedFeedbackTimer();
  }

  load() {
    this.state.set('loading');
    forkJoin({
      crawlers: this.crawlerService.getCrawlers(),
      jobs: this.crawlerService.getJobs(),
      workers: this.crawlerService.getWorkerStatus(),
    })
      .pipe(
        tap(({ crawlers, jobs, workers }) => {
          this.refreshData(crawlers, jobs, workers);
          this.state.set('loaded');
        }),
        switchMap(() => this.afterJobsRefreshed())
      )
      .subscribe({
        error: err => {
          this.error.set(err?.error?.detail ?? err?.message ?? 'Error al cargar.');
          this.state.set('error');
        },
      });
  }

  recoverFromInfra(allPipelineStages: boolean): void {
    const aj = this.activeJob();
    if (!aj) return;
    this.infraOpsBusy.set(true);
    this.crawlerService
      .recoverFromInfra(aj.id, {
        requeueAllPipelineStages: allPipelineStages,
        requeueFetcherPending: true,
        resumeDiscovery: false,
        resumeAllWorkers: true,
      })
      .pipe(finalize(() => this.infraOpsBusy.set(false)))
      .subscribe({
        next: r => {
          const probe = r.storageProbeOk ? 'Colas OK.' : `Colas: ${r.storageProbeError ?? 'fallo'}.`;
          this.notify.success(`${probe} Mensajes reencolados: ${r.requeueMessagesPublished}.`);
          this.load();
        },
        error: err => {
          this.notify.error(err?.error?.detail ?? err?.message ?? 'Error al recuperar');
        },
      });
  }

  reconcileProcessingStale(): void {
    const aj = this.activeJob();
    if (!aj) return;
    this.infraOpsBusy.set(true);
    this.crawlerService
      .reconcileProcessingDocuments(aj.id, { minAgeMinutes: 15 })
      .pipe(finalize(() => this.infraOpsBusy.set(false)))
      .subscribe({
        next: r => {
          this.notify.success(`Filas actualizadas (Processing → Pending): ${r.rowsReset}.`);
          this.load();
        },
        error: err => {
          this.notify.error(err?.error?.detail ?? err?.message ?? 'Error al reconciliar');
        },
      });
  }

  probeStorageQueues(): void {
    this.infraAuxBusy.set(true);
    this.crawlerService
      .storageProbe()
      .pipe(finalize(() => this.infraAuxBusy.set(false)))
      .subscribe({
        next: p => {
          const ok = p.discovererOk && p.fetcherOk;
          this.notify.info(
            ok
              ? 'Discoverer y Fetcher responden.'
              : `Discoverer: ${p.discovererOk ? 'OK' : p.discovererError ?? 'fallo'} · Fetcher: ${p.fetcherOk ? 'OK' : p.fetcherError ?? 'fallo'}`
          );
        },
        error: err => {
          this.notify.error(err?.error?.detail ?? err?.message ?? 'Error al probar colas');
        },
      });
  }

  /** Ensures SAIJ (tesauro) appears in the UI even if the API DB has no CrawlerConfig row yet (migration not applied). */
  private mergeThesaurusCrawlerConfig(crawlers: CrawlerConfig[]): CrawlerConfig[] {
    if (crawlers.some(c => c.sourceId === THESAURUS_SOURCE_ID)) {
      return [...crawlers].sort((a, b) => a.sourceId - b.sourceId);
    }
    return [
      ...crawlers,
      {
        sourceId: THESAURUS_SOURCE_ID,
        sourceName: 'SAIJ',
        isEnabled: true,
        lastCrawledAt: null,
        lastCrawledStatus: null,
        lastDocumentCount: null,
        synthetic: true,
      },
    ].sort((a, b) => a.sourceId - b.sourceId);
  }

  private refreshData(crawlers: CrawlerConfig[], jobs: Job[], workers: WorkerPauseState[]) {
    const merged = this.mergeThesaurusCrawlerConfig(crawlers);
    const existingExpanded = new Set(
      this.sources().filter(s => s.expanded).map(s => s.config.sourceId)
    );
    const existingShowAll = new Set(
      this.sources().filter(s => s.showAllJobs).map(s => s.config.sourceId)
    );

    const sections: SourceSection[] = merged.map(config => {
      const sourceJobs = jobs
        .filter(j => j.sourceId === config.sourceId)
        .sort((a, b) => {
          const da = a.startedAt ?? a.completedAt ?? '';
          const db = b.startedAt ?? b.completedAt ?? '';
          return db.localeCompare(da);
        });
      return {
        config,
        jobs: sourceJobs,
        expanded: existingExpanded.has(config.sourceId),
        showAllJobs: existingShowAll.has(config.sourceId),
      };
    });

    this.sources.set(sections);

    const ej = this.expandedJobId();
    if (ej) {
      const jobStillPresent = sections.some(s => s.jobs.some(j => j.id === ej));
      if (!jobStillPresent) this.expandedJobId.set(null);
    }

    const ordered = WORKER_TYPES.map(wt => workers.find(w => w.workerType === wt) ?? { workerType: wt, isPaused: false, updatedAt: '' });
    this.workers.set(ordered);
  }

  /** Loads per-stage document counts for the current active job (queue / failed by CurrentStage). */
  private afterJobsRefreshed() {
    const aj = this.activeJob();
    if (!aj) {
      this.activeJobSummary.set(null);
      return of(null);
    }
    return this.crawlerService.getJobDocumentsSummary(aj.id).pipe(
      tap(s => this.activeJobSummary.set(s)),
      catchError(() => of(null))
    );
  }

  private pickStageSummary(
    stages: Record<string, StageSummary> | undefined,
    stageKey: string
  ): StageSummary | null {
    if (!stages) return null;
    const direct = stages[stageKey];
    if (direct) return direct;
    const found = Object.keys(stages).find(k => k.toLowerCase() === stageKey.toLowerCase());
    return found ? stages[found] ?? null : null;
  }

  /** Pending + processing, and failed, for documents whose CurrentStage matches this worker. */
  stageSideMetrics(workerType: string): { queue: number; failed: number } {
    const s = this.pickStageSummary(this.activeJobSummary()?.stages, workerType);
    if (!s) return { queue: 0, failed: 0 };
    return {
      queue: (s.pending ?? 0) + (s.processing ?? 0),
      failed: s.failed ?? 0,
    };
  }

  stageQueueTooltip(workerType: string): string {
    return `Pendientes y en proceso en ${workerType}`;
  }

  stageFailedTooltip(workerType: string): string {
    return `Fallidos en ${workerType}`;
  }

  toggleJobExpand(job: Job) {
    this.expandedJobId.update(id => (id === job.id ? null : job.id));
  }

  jobStageCounters(job: Job): StageCounter[] {
    return [
      { label: 'Descubiertos', value: job.documentsDiscovered, workerType: null, colorClass: 'sc-discovered' },
      { label: 'Descargados', value: job.documentsCrawled, workerType: null, colorClass: 'sc-crawled' },
      { label: 'Parseados', value: job.documentsParsed, workerType: null, colorClass: 'sc-parsed' },
      { label: 'Enriquecidos', value: job.documentsEnriched, workerType: null, colorClass: 'sc-enriched' },
      { label: 'Persistidos', value: job.documentsPersisted, workerType: null, colorClass: 'sc-persisted' },
      { label: 'Indexados', value: job.documentsIndexed, workerType: null, colorClass: 'sc-completed' },
      { label: 'En cola', value: job.outstandingDocuments ?? 0, workerType: null, colorClass: 'sc-queued' },
      { label: 'Omitidos', value: job.documentsSkipped ?? 0, workerType: null, colorClass: 'sc-skipped' },
      { label: 'Fallidos', value: job.documentsFailed, workerType: null, colorClass: 'sc-failed' },
    ];
  }

  openFailedModal(job: Job) {
    this.clearFailedFeedbackTimer();
    this.failedFeedback.set(null);
    this.failedModalJob.set(job);
    this.failedDocs.set([]);
    this.failedDocsTotalCount.set(0);
    this.failedWorkerFilterInclude.set(new Map());
    this.failedRowBusyId.set(null);
    this.failedRowAction.set(null);
    this.fetchPdfTimeoutDraft.set({});
    this.loadFailedDocs(job.id);
  }

  closeFailedModal() {
    this.clearFailedFeedbackTimer();
    this.failedFeedback.set(null);
    this.failedModalJob.set(null);
    this.failedDocs.set([]);
    this.failedDocsTotalCount.set(0);
    this.failedWorkerFilterInclude.set(new Map());
    this.failedLoading.set(false);
    this.failedBulkBusy.set(false);
    this.failedRowBusyId.set(null);
    this.failedRowAction.set(null);
    this.fetchPdfTimeoutDraft.set({});
  }

  private clearFailedFeedbackTimer(): void {
    if (this.failedFeedbackTimer != null) {
      clearTimeout(this.failedFeedbackTimer);
      this.failedFeedbackTimer = null;
    }
  }

  private showFailedFeedback(entry: { type: 'success' | 'error' | 'info'; text: string }, durationMs = 12000): void {
    this.clearFailedFeedbackTimer();
    this.failedFeedback.set(entry);
    this.failedFeedbackTimer = setTimeout(() => {
      this.failedFeedback.set(null);
      this.failedFeedbackTimer = null;
    }, durationMs);
  }

  private loadFailedDocs(jobId: string) {
    const showFullSpinner = this.failedDocs().length === 0;
    if (showFullSpinner) this.failedLoading.set(true);
    this.crawlerService.getJobDocuments(jobId, { status: 'Failed', take: 500 }).subscribe({
      next: res => {
        if (showFullSpinner) this.failedLoading.set(false);
        this.failedDocs.set(res.documents);
        const m = new Map<string, boolean>();
        for (const d of res.documents) {
          m.set(this.workerTagKey(d), true);
        }
        this.failedWorkerFilterInclude.set(m);
        this.failedDocsTotalCount.set(res.totalCount);
      },
      error: err => {
        if (showFullSpinner) this.failedLoading.set(false);
        this.notify.error(err?.error?.detail ?? err?.message ?? 'No se pudieron cargar los fallidos.');
      },
    });
  }

  private removeFailedDoc(id: string) {
    this.failedDocs.update(list => list.filter(d => d.id !== id));
    this.failedDocsTotalCount.update(c => Math.max(0, c - 1));
    const keys = new Set(this.failedDocs().map(d => this.workerTagKey(d)));
    this.failedWorkerFilterInclude.update(map => {
      const n = new Map(map);
      for (const k of [...n.keys()]) {
        if (!keys.has(k)) n.delete(k);
      }
      return n;
    });
  }

  workerTagKey(d: JobDocument): string {
    const s = d.currentStage?.trim();
    return s && s.length > 0 ? s : '(sin etapa)';
  }

  /** Chip label: same as pipeline stage, friendly fallback for grouped unknowns. */
  failedStageChipLabel(key: string): string {
    return key === '(sin etapa)' ? 'Sin etapa' : key;
  }

  private failedWorkerFilterOnForKey(inc: Map<string, boolean>, d: JobDocument): boolean {
    return inc.get(this.workerTagKey(d)) !== false;
  }

  isFailedWorkerTagOn(key: string): boolean {
    return this.failedWorkerFilterInclude().get(key) !== false;
  }

  toggleFailedWorkerTag(key: string): void {
    this.failedWorkerFilterInclude.update(m => {
      const n = new Map(m);
      const on = n.get(key) !== false;
      n.set(key, !on);
      return n;
    });
  }

  requeueFailedDoc(job: Job, doc: JobDocument) {
    this.failedRowBusyId.set(doc.id);
    this.failedRowAction.set('reprocess');
    this.crawlerService.singleFailedDocumentAction(job.id, doc.id, 'Reprocess').subscribe({
      next: () => {
        this.failedRowBusyId.set(null);
        this.failedRowAction.set(null);
        this.notify.success(
          'Reencolado: quedó Pendiente y se publicó en la cola del pipeline.',
          6500
        );
        this.showFailedFeedback({
          type: 'success',
          text:
            `El documento ${doc.externalId} ya no está en Fallidos: pasó a Pendiente y los workers lo procesarán. Los números del job en la lista pueden tardar unos segundos en actualizarse (polling cada 10 s).`,
        });
        this.removeFailedDoc(doc.id);
        if (this.failedDocs().length === 0 && this.failedDocsTotalCount() > 0) {
          this.loadFailedDocs(job.id);
        }
        this.refreshJobsLight();
      },
      error: err => {
        this.failedRowBusyId.set(null);
        this.failedRowAction.set(null);
        const msg = err?.error?.detail ?? err?.message ?? 'Error al reencolar.';
        this.notify.error(msg);
        this.showFailedFeedback({ type: 'error', text: msg }, 14000);
      },
    });
  }

  discardFailedDoc(job: Job, doc: JobDocument) {
    this.failedRowBusyId.set(doc.id);
    this.failedRowAction.set('discard');
    this.crawlerService.singleFailedDocumentAction(job.id, doc.id, 'Discard').subscribe({
      next: () => {
        this.failedRowBusyId.set(null);
        this.failedRowAction.set(null);
        this.notify.success('Fallo descartado.', 5000);
        this.showFailedFeedback({
          type: 'success',
          text: `El documento ${doc.externalId} fue marcado como Descartado y no volverá a la cola.`,
        });
        this.removeFailedDoc(doc.id);
        if (this.failedDocs().length === 0 && this.failedDocsTotalCount() > 0) {
          this.loadFailedDocs(job.id);
        }
        this.refreshJobsLight();
      },
      error: err => {
        this.failedRowBusyId.set(null);
        this.failedRowAction.set(null);
        const msg = err?.error?.detail ?? err?.message ?? 'Error al descartar.';
        this.notify.error(msg);
        this.showFailedFeedback({ type: 'error', text: msg }, 14000);
      },
    });
  }

  fetchPdfTimeoutInputValue(d: JobDocument): string {
    const draft = this.fetchPdfTimeoutDraft()[d.id];
    if (draft !== undefined) return draft;
    return d.fetchPdfTimeoutSeconds != null ? String(d.fetchPdfTimeoutSeconds) : '';
  }

  onFetchPdfTimeoutInput(docId: string, ev: Event): void {
    const v = (ev.target as HTMLInputElement).value;
    this.fetchPdfTimeoutDraft.update(m => ({ ...m, [docId]: v }));
  }

  private parseFetchPdfTimeoutSeconds(doc: JobDocument): number | null {
    const raw = (this.fetchPdfTimeoutDraft()[doc.id] ?? '').trim();
    if (raw === '') {
      return doc.fetchPdfTimeoutSeconds ?? null;
    }
    const n = Number(raw);
    if (!Number.isFinite(n)) return null;
    return Math.trunc(n);
  }

  saveFetchPdfTimeout(job: Job, doc: JobDocument): void {
    const seconds = this.parseFetchPdfTimeoutSeconds(doc);
    if (seconds == null || seconds < 60 || seconds > 900) {
      this.notify.error('Indicá un timeout entre 60 y 900 segundos (solo para este documento en Fetcher).');
      return;
    }
    this.failedRowBusyId.set(doc.id);
    this.failedRowAction.set('saveTimeout');
    this.crawlerService.setDocumentFetchPdfTimeout(job.id, doc.id, seconds).subscribe({
      next: res => {
        this.failedRowBusyId.set(null);
        this.failedRowAction.set(null);
        this.fetchPdfTimeoutDraft.update(m => {
          const n = { ...m };
          delete n[doc.id];
          return n;
        });
        this.notify.success(res.message ?? 'Timeout guardado.', 6000);
        this.showFailedFeedback({
          type: 'info',
          text: `Timeout de PDF guardado (${seconds} s) para este documento. Reencolá para que el Fetcher lo aplique en la próxima descarga.`,
        });
        this.loadFailedDocs(job.id);
        this.refreshJobsLight();
      },
      error: err => {
        this.failedRowBusyId.set(null);
        this.failedRowAction.set(null);
        const msg = err?.error?.detail ?? err?.message ?? 'Error al guardar timeout.';
        this.notify.error(msg);
        this.showFailedFeedback({ type: 'error', text: msg }, 14000);
      },
    });
  }

  clearFetchPdfTimeout(job: Job, doc: JobDocument): void {
    this.failedRowBusyId.set(doc.id);
    this.failedRowAction.set('clearTimeout');
    this.crawlerService.setDocumentFetchPdfTimeout(job.id, doc.id, null).subscribe({
      next: res => {
        this.failedRowBusyId.set(null);
        this.failedRowAction.set(null);
        this.fetchPdfTimeoutDraft.update(m => {
          const n = { ...m };
          delete n[doc.id];
          return n;
        });
        this.notify.success(res.message ?? 'Timeout quitado.', 5000);
        this.loadFailedDocs(job.id);
        this.refreshJobsLight();
      },
      error: err => {
        this.failedRowBusyId.set(null);
        this.failedRowAction.set(null);
        const msg = err?.error?.detail ?? err?.message ?? 'Error al quitar timeout.';
        this.notify.error(msg);
        this.showFailedFeedback({ type: 'error', text: msg }, 14000);
      },
    });
  }

  bulkRequeueFailed(job: Job) {
    const ids = this.failedDocsVisible().map(d => d.id);
    if (ids.length === 0) return;
    this.failedBulkBusy.set(true);
    this.crawlerService.bulkFailedDocumentsByIds(job.id, ids, 'Reprocess').subscribe({
      next: res => {
        this.failedBulkBusy.set(false);
        this.notify.success(res.message ?? 'Listo.', 9000);
        if (res.affectedCount >= ids.length) {
          this.closeFailedModal();
        } else {
          this.loadFailedDocs(job.id);
        }
        this.refreshJobsLight();
      },
      error: err => {
        this.failedBulkBusy.set(false);
        const msg = err?.error?.detail ?? err?.message ?? 'Error al reencolar todos.';
        this.notify.error(msg);
        this.showFailedFeedback({ type: 'error', text: msg }, 14000);
      },
    });
  }

  bulkDiscardFailed(job: Job) {
    const ids = this.failedDocsVisible().map(d => d.id);
    if (ids.length === 0) return;
    this.failedBulkBusy.set(true);
    this.crawlerService.bulkFailedDocumentsByIds(job.id, ids, 'Discard').subscribe({
      next: res => {
        this.failedBulkBusy.set(false);
        this.notify.success(res.message ?? 'Listo.', 9000);
        if (res.affectedCount >= ids.length) {
          this.closeFailedModal();
        } else {
          this.loadFailedDocs(job.id);
        }
        this.refreshJobsLight();
      },
      error: err => {
        this.failedBulkBusy.set(false);
        const msg = err?.error?.detail ?? err?.message ?? 'Error al descartar todos.';
        this.notify.error(msg);
        this.showFailedFeedback({ type: 'error', text: msg }, 14000);
      },
    });
  }

  private refreshJobsLight() {
    forkJoin({
      crawlers: this.crawlerService.getCrawlers(),
      jobs: this.crawlerService.getJobs(),
      workers: this.crawlerService.getWorkerStatus(),
    })
      .pipe(
        tap(({ crawlers, jobs, workers }) => this.refreshData(crawlers, jobs, workers)),
        switchMap(() => this.afterJobsRefreshed())
      )
      .subscribe({ error: () => {} });
  }

  toggleSection(src: SourceSection) {
    src.expanded = !src.expanded;
  }

  toggleEnabled(c: CrawlerConfig) {
    if (c.synthetic) {
      this.notify.info(
        'Cuando apliques la migración de base de datos en el API, esta fila se sincroniza con CrawlerConfigs. La ingesta del tesauro con Ejecutar ya funciona.'
      );
      return;
    }
    this.updatingId.set(c.sourceId);
    this.crawlerService.updateCrawler(c.sourceId, !c.isEnabled).subscribe({
      next: () => {
        this.updatingId.set(null);
        this.load();
        this.notify.success(c.isEnabled ? 'Fuente deshabilitada.' : 'Fuente habilitada.');
      },
      error: err => {
        this.updatingId.set(null);
        this.notify.error(err?.error?.detail ?? 'Error al actualizar.');
      }
    });
  }

  toggleWorker(w: WorkerPauseState) {
    this.workerBusy.set(true);
    const action$ = w.isPaused
      ? this.crawlerService.resumeWorker(w.workerType)
      : this.crawlerService.pauseWorker(w.workerType);
    action$.subscribe({
      next: () => {
        this.workerBusy.set(false);
        this.load();
      },
      error: err => {
        this.workerBusy.set(false);
        this.notify.error(err?.error?.detail ?? 'Error al cambiar estado del worker.');
      }
    });
  }

  isWorkerPaused(workerType: string): boolean {
    return this.workers().find(w => w.workerType === workerType)?.isPaused ?? false;
  }

  toggleWorkerByType(workerType: string) {
    const w = this.workers().find(x => x.workerType === workerType);
    if (w) this.toggleWorker(w);
  }

  bulkWorkers(pause: boolean) {
    this.workerBusy.set(true);
    const actions = WORKER_TYPES.map(wt =>
      pause ? this.crawlerService.pauseWorker(wt) : this.crawlerService.resumeWorker(wt)
    );
    forkJoin(actions).subscribe({
      next: () => {
        this.workerBusy.set(false);
        this.load();
        this.notify.success(pause ? 'Todos los workers pausados.' : 'Todos los workers reanudados.');
      },
      error: err => {
        this.workerBusy.set(false);
        this.notify.error(err?.error?.detail ?? 'Error en operacion masiva.');
      }
    });
  }

  onSourceChange(sourceId: number | null) {
    this.runSourceId.set(sourceId ? +sourceId : null);
    const types = this.availableCrawlTypes();
    this.runType = types.length > 0 ? types[0].value : 'incremental';
    this.runSince = '';
    this.runDateFrom = '';
    this.runDateTo = '';
    this.runSkipDocs = null;
    this.runMaxDocs = null;
    this.runUseCache = false;
    this.runReprocess = false;
    this.runThesaurusNormalize = true;
  }

  pendingJobMessage = signal<string | null>(null);

  confirmRunInline() {
    const sid = this.runSourceId();
    if (!sid) return;

    if (sid === this.thesaurusSourceId) {
      this.running.set(true);
      this.crawlerService
        .startThesaurusIngestJob({ normalizeKeywords: this.runThesaurusNormalize })
        .pipe(finalize(() => this.running.set(false)))
        .subscribe({
          next: res => {
            this.pendingJobMessage.set('Esperando inicio del job...');
            this.notify.success(res.message ?? 'Ingesta de tesauro iniciada.');
            this.pollUntilJobAppears();
          },
          error: err => {
            this.notify.error(err?.error?.detail ?? 'Error al ejecutar.');
          },
        });
      return;
    }

    const type = this.runType as 'incremental' | 'by-range' | 'fallos-destacados';
    const opts: Record<string, string | boolean | number> = {};
    if (type === 'incremental' && this.runSince) opts['since'] = this.runSince;
    if (type === 'by-range') { opts['dateFrom'] = this.runDateFrom; opts['dateTo'] = this.runDateTo; }
    if (this.runMaxDocs) opts['maxDocuments'] = this.runMaxDocs;
    if (this.runSkipDocs) opts['skipDocuments'] = this.runSkipDocs;
    if (this.runUseCache) opts['useCache'] = true;
    if (this.runReprocess) opts['reprocess'] = true;

    this.running.set(true);
    this.crawlerService.runCrawler(sid, type, opts as any).subscribe({
      next: res => {
        this.running.set(false);
        this.pendingJobMessage.set('Esperando inicio del job...');
        this.notify.success(res.message ?? 'Crawl iniciado.');
        this.pollUntilJobAppears();
      },
      error: err => {
        this.running.set(false);
        this.notify.error(err?.error?.detail ?? 'Error al ejecutar.');
      }
    });
  }

  private pollUntilJobAppears(attempt = 0) {
    const maxAttempts = 10;
    const delayMs = attempt < 3 ? 2000 : 5000;
    if (attempt >= maxAttempts) {
      this.pendingJobMessage.set(null);
      return;
    }
    setTimeout(() => {
      forkJoin({
        crawlers: this.crawlerService.getCrawlers(),
        jobs: this.crawlerService.getJobs(),
        workers: this.crawlerService.getWorkerStatus(),
      })
        .pipe(
          tap(({ crawlers, jobs, workers }) => this.refreshData(crawlers, jobs, workers)),
          switchMap(() => this.afterJobsRefreshed()),
          catchError(() => of(null))
        )
        .subscribe(() => {
          if (this.activeJob()) {
            this.pendingJobMessage.set(null);
          } else {
            this.pollUntilJobAppears(attempt + 1);
          }
        });
    }, delayMs);
  }

  latestJobDocs(src: SourceSection): number {
    if (src.jobs.length === 0) return src.config.lastDocumentCount ?? 0;
    return src.jobs[0].documentsDiscovered;
  }

  entityLabel(sourceId: number): string {
    const key = SOURCE_ENTITY_MAP[sourceId] ?? 'rulings';
    if (key === 'statutes') return 'Legislacion';
    if (key === 'thesaurus') return 'Tesauro';
    return 'Jurisprudencia';
  }

  statusDotClass(status: string | null): string {
    if (!status) return 'unknown';
    const s = status.toLowerCase();
    if (s === 'success') return 'success';
    if (s === 'partial') return 'partial';
    if (s === 'failed') return 'failed';
    return 'unknown';
  }

  jobStatusClass(status: string): string {
    return (status ?? '').toLowerCase();
  }

  jobTitle(job: Job): string {
    const fmt = (v: string | null | undefined) => {
      if (!v) return '';
      const parts = v.split('T')[0].split('-');
      if (parts.length !== 3) return '';
      return `${parts[2]}/${parts[1]}/${parts[0]}`;
    };
    const t = (job.type ?? '').toLowerCase();
    if (t === 'thesaurus') return 'Tesauro (API)';
    if (t === 'fallos-destacados') return 'Solo destacados';
    if (t === 'reprocess') return 'Reproceso';
    if (t === 'by-range' && job.dateFrom) {
      return `Fecha desde: ${fmt(job.dateFrom)} hasta ${fmt(job.dateTo)}`;
    }
    if (t === 'incremental' && job.dateFrom) {
      return `Incremental desde ${fmt(job.dateFrom)}`;
    }
    if (job.dateFrom || job.dateTo) {
      return `${fmt(job.dateFrom)} — ${fmt(job.dateTo)}`;
    }
    return job.type ?? job.id.slice(0, 8);
  }

  pipelineTotal(job: Job): number {
    return Math.max(job.documentsDiscovered, 1);
  }

  pipelineSegments(job: Job): { completed: number; enriched: number; parsed: number; crawled: number; skipped: number; failed: number } {
    const total = this.pipelineTotal(job);
    const indexed = Math.min(job.documentsIndexed, total);
    const enriched = Math.max(0, Math.min(job.documentsEnriched, total) - indexed);
    const parsed = Math.max(0, Math.min(job.documentsParsed, total) - indexed - enriched);
    const crawled = Math.max(0, Math.min(job.documentsCrawled, total) - indexed - enriched - parsed);
    const skipped = Math.min(job.documentsSkipped ?? 0, total);
    const failed = Math.min(job.documentsFailed, total);
    const pct = (v: number) => total > 0 ? (v / total) * 100 : 0;
    return {
      completed: pct(indexed),
      enriched: pct(enriched),
      parsed: pct(parsed),
      crawled: pct(crawled),
      skipped: pct(skipped),
      failed: pct(failed),
    };
  }

  pct(value: number, total: number): number {
    if (total <= 0) return 0;
    return Math.max(0, (value / total) * 100);
  }

  formatDate(value: string | null): string {
    if (!value) return '';
    const d = new Date(value);
    if (isNaN(d.getTime())) return '';
    return d.toLocaleString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
  }

  formatDateShort(value: string | null | undefined): string {
    if (!value) return '';
    const d = new Date(value);
    if (isNaN(d.getTime())) return '';
    return d.toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }

  openJobAudit(job: Job): void {
    this.auditModalOpen.set(true);
    this.auditRepairBusy.set(false);
    this.auditReconcileBusy.set(false);
    this.auditRecoverBusy.set(false);
    this.fetchJobAudit(job.id);
  }

  private fetchJobAudit(jobId: string): void {
    this.auditLoading.set(true);
    this.auditResult.set(null);
    this.auditError.set('');
    this.auditProbeResult.set(null);
    const probeSafe$ = this.crawlerService.storageProbe().pipe(
      catchError(err =>
        of({
          discovererOk: false,
          fetcherOk: false,
          discovererError: err?.error?.detail ?? err?.message ?? 'Error al sondear',
          fetcherError: err?.error?.detail ?? err?.message ?? 'Error al sondear',
        } satisfies StorageProbeResult)
      )
    );
    forkJoin({
      audit: this.crawlerService.getJobAudit(jobId),
      probe: probeSafe$,
    }).subscribe({
      next: ({ audit, probe }) => {
        this.auditResult.set(audit);
        this.auditProbeResult.set(probe);
        this.auditLoading.set(false);
      },
      error: err => {
        this.auditLoading.set(false);
        this.auditError.set(err?.error?.detail ?? err?.message ?? 'Error al auditar el job.');
        this.notify.error(this.auditError());
      },
    });
  }

  recoverPipelineFromAudit(ar: JobAudit): void {
    if (!this.auditInfraProbeOk()) return;
    this.auditRecoverBusy.set(true);
    this.crawlerService
      .recoverFromInfra(ar.jobId, {
        requireStorageProbe: true,
        clearInfrastructureDegraded: true,
        broadcastRecovered: true,
        resumeDiscovery: true,
        requeueFetcherPending: true,
        requeueAllPipelineStages: false,
        resumeAllWorkers: true,
      })
      .pipe(finalize(() => this.auditRecoverBusy.set(false)))
      .subscribe({
        next: r => {
          const probeOk = r.storageProbeOk ? 'Probe OK.' : `Probe: ${r.storageProbeError ?? 'fallo'}.`;
          this.notify.success(
            `${probeOk} Reencolados: ${r.requeueMessagesPublished}. Discovery reanudado: ${r.discoveryQueued ? 'sí' : 'no'}.`,
            12000
          );
          this.fetchJobAudit(ar.jobId);
          this.refreshJobsLight();
        },
        error: err => {
          this.notify.error(err?.error?.detail ?? err?.message ?? 'Error al recuperar el pipeline.');
        },
      });
  }

  canRepairPendingTail(ar: JobAudit): boolean {
    return ar.riskDocuments.some(
      d =>
        d.status === 'Pending' &&
        (d.currentStage === 'Persister' || d.currentStage === 'Indexer')
    );
  }

  repairPendingTailFromAudit(ar: JobAudit): void {
    this.auditRepairBusy.set(true);
    this.crawlerService.repairJobAuditPendingTail(ar.jobId).subscribe({
      next: res => {
        this.auditRepairBusy.set(false);
        this.notify.success(res.message, 9000);
        this.fetchJobAudit(ar.jobId);
        this.refreshJobsLight();
      },
      error: err => {
        this.auditRepairBusy.set(false);
        this.notify.error(err?.error?.detail ?? err?.message ?? 'Error al aplicar reparación.');
      },
    });
  }

  canReconcileJobCounters(ar: JobAudit): boolean {
    return ar.dbCountsByStatus.outstandingPendingOrProcessing === 0;
  }

  reconcileJobCountersFromAudit(ar: JobAudit): void {
    this.auditReconcileBusy.set(true);
    this.crawlerService.reconcileJobCounters(ar.jobId).subscribe({
      next: (res: ReconcileJobCountersResult) => {
        this.auditReconcileBusy.set(false);
        const p = res.previous;
        const u = res.updated;
        const parts = [
          `Fallidos ${p.documentsFailed}→${u.documentsFailed}`,
          `Indexados ${p.documentsIndexed}→${u.documentsIndexed}`,
        ];
        if (res.jobCompletionApplied) {
          parts.push('Cierre del job evaluado (success si aplica).');
        }
        this.notify.success(parts.join(' · '), 12000);
        this.fetchJobAudit(ar.jobId);
        this.refreshJobsLight();
      },
      error: err => {
        this.auditReconcileBusy.set(false);
        this.notify.error(err?.error?.detail ?? err?.message ?? 'Error al rearmar contadores.');
      },
    });
  }

  closeJobAuditModal(): void {
    this.auditModalOpen.set(false);
    this.auditLoading.set(false);
    this.auditResult.set(null);
    this.auditError.set('');
    this.auditProbeResult.set(null);
    this.auditRepairBusy.set(false);
    this.auditReconcileBusy.set(false);
    this.auditRecoverBusy.set(false);
  }
}
