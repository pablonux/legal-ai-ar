import { Component, signal, inject, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SlicePipe } from '@angular/common';
import {
  CrawlerService,
  type Job,
  type JobDocumentsSummary,
  type StageSummary,
  type JobDocument
} from '../../../../services/admin/crawler.service';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';
import { catchError, EMPTY, interval, Subscription, switchMap } from 'rxjs';

const STAGE_ORDER = ['Discoverer', 'Fetcher', 'Parser', 'Enricher', 'Persister', 'Indexer'];
const STAGE_LABELS: Record<string, string> = {
  Discoverer: 'Descubrimiento',
  Fetcher: 'Descarga',
  Parser: 'Parseo',
  Enricher: 'Enriquecimiento',
  Persister: 'Persistencia',
  Indexer: 'Indexación'
};

const ENTITY_LABELS: Record<string, string> = {
  rulings: 'Jurisprudencia',
  statutes: 'Legislación'
};

@Component({
  selector: 'app-job-detail',
  standalone: true,
  imports: [LoadingSpinnerComponent, FormsModule, RouterLink, SlicePipe],
  template: `
    <div class="job-detail">
      <nav class="breadcrumb">
        <a routerLink="/admin">Ingesta</a>
        <span class="sep">›</span>
        @if (sourceName) {
          <span>{{ sourceName }}</span>
          <span class="sep">›</span>
        }
        <span>Job {{ jobId.slice(0, 8) }}</span>
      </nav>

      @if (loadingState() === 'loading' && !summary()) {
        <app-loading-spinner message="Cargando detalles del job..." />
      }

      @if (loadingState() === 'error') {
        <div class="state-message error">
          <p>{{ errorMsg() }}</p>
          <button type="button" class="btn-retry" (click)="loadSummary()">Reintentar</button>
        </div>
      }

      @if (summary()) {
        <section class="summary-section">
          <div class="pipeline-visual">
            <div class="pipeline-flow">
              @for (stage of orderedStages(); track stage.name; let idx = $index) {
                <div class="pipeline-node" [class.active]="stageFilter() === stage.name"
                     (click)="selectStage(stage.name)">
                  <span class="node-label">{{ stageLabel(stage.name) }}</span>
                  <div class="node-circle" [class.has-activity]="stage.data.processing > 0">
                    <span class="node-count">{{ passedThrough(idx) }}</span>
                  </div>
                  <div class="node-indicators">
                    <div class="indicator queue" [class.has-items]="(stage.data.pending + stage.data.processing) > 0"
                         [title]="'Cola: ' + stage.data.pending + ' pendientes, ' + stage.data.processing + ' procesando'">
                      <svg viewBox="0 0 20 20" class="indicator-icon">
                        <rect x="2" y="4" width="5" height="12" rx="1" />
                        <rect x="8" y="4" width="5" height="12" rx="1" />
                        <rect x="14" y="4" width="5" height="12" rx="1" />
                      </svg>
                      <span class="indicator-count">{{ stage.data.pending + stage.data.processing }}</span>
                    </div>
                    <div class="indicator error" [class.has-items]="stage.data.failed > 0"
                         [title]="'Fallidos: ' + stage.data.failed">
                      <svg viewBox="0 0 20 20" class="indicator-icon">
                        <circle cx="10" cy="10" r="8" />
                        <text x="10" y="14" text-anchor="middle" font-size="12" font-weight="bold" fill="white">!</text>
                      </svg>
                      <span class="indicator-count">{{ stage.data.failed }}</span>
                    </div>
                  </div>
                </div>
                <div class="pipeline-arrow">
                  <div class="arrow-line"></div>
                  <div class="arrow-head"></div>
                </div>
              }
              <div class="pipeline-node final-node">
                <span class="node-label">Finalizados</span>
                <div class="node-circle finished">
                  <span class="node-count">{{ finishedCount() }}</span>
                </div>
              </div>
            </div>

            <div class="pipeline-totals">
              <div class="total-item docs">
                <svg viewBox="0 0 24 24" class="total-icon">
                  <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" fill="none" stroke="currentColor" stroke-width="2"/>
                  <polyline points="14 2 14 8 20 8" fill="none" stroke="currentColor" stroke-width="2"/>
                  <line x1="16" y1="13" x2="8" y2="13" stroke="currentColor" stroke-width="2"/>
                  <line x1="16" y1="17" x2="8" y2="17" stroke="currentColor" stroke-width="2"/>
                </svg>
                <span class="total-count">{{ summary()!.totalDocuments }}</span>
                <span class="total-label">documentos</span>
              </div>
              @if (skippedCount() > 0) {
                <div class="total-item skipped">
                  <svg viewBox="0 0 24 24" class="total-icon">
                    <path d="M13 7h-2v2h2V7zm0 4h-2v6h2v-6zm-1-9C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z" fill="none" stroke="currentColor" stroke-width="2"/>
                    <line x1="5" y1="5" x2="19" y2="19" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
                  </svg>
                  <span class="total-count">{{ skippedCount() }}</span>
                  <span class="total-label">omitidos</span>
                </div>
              }
              <div class="total-item errors" [class.has-errors]="summary()!.totalFailed > 0">
                <svg viewBox="0 0 24 24" class="total-icon">
                  <circle cx="12" cy="12" r="10" fill="none" stroke="currentColor" stroke-width="2"/>
                  <line x1="12" y1="8" x2="12" y2="13" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
                  <circle cx="12" cy="16.5" r="1" fill="currentColor"/>
                </svg>
                <span class="total-count">{{ summary()!.totalFailed }}</span>
                <span class="total-label">fallidos</span>
              </div>
              <div class="total-item completed">
                <svg viewBox="0 0 24 24" class="total-icon">
                  <circle cx="12" cy="12" r="10" fill="none" stroke="currentColor" stroke-width="2"/>
                  <polyline points="8 12 11 15 16 9" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                <span class="total-count">{{ summary()!.totalCompleted }}</span>
                <span class="total-label">completados</span>
              </div>
            </div>
          </div>
        </section>

        <section class="documents-section">
          <div class="docs-header">
            <h2>Documentos</h2>
            <div class="filters">
              <select [(ngModel)]="stageFilter" (ngModelChange)="applyFilters()">
                <option [ngValue]="null">Todas las etapas</option>
                @for (s of orderedStages(); track s.name) {
                  <option [value]="s.name">{{ stageLabel(s.name) }}</option>
                }
              </select>
              <select [(ngModel)]="statusFilter" (ngModelChange)="applyFilters()">
                <option [ngValue]="null">Todos los estados</option>
                <option value="Pending">Pendiente</option>
                <option value="Processing">Procesando</option>
                <option value="Completed">Completado</option>
                <option value="Failed">Fallido</option>
                <option value="Discarded">Descartado</option>
              </select>
            </div>
            <div class="actions">
              @if (stageFilter() && hasFailedInStage()) {
                <button type="button" class="btn-action batch"
                        (click)="reprocessNextBatch(10)" [disabled]="actionRunning()">
                  {{ actionRunning() ? 'Procesando...' : 'Reprocesar próx. 10' }}
                </button>
                <button type="button" class="btn-action batch-outline"
                        (click)="reprocessNextBatch(5)" [disabled]="actionRunning()">
                  Próx. 5
                </button>
                <button type="button" class="btn-action reprocess"
                        (click)="bulkAction('Reprocess')" [disabled]="actionRunning()">
                  {{ actionRunning() ? 'Procesando...' : 'Reprocesar fallidos' }}
                </button>
                <button type="button" class="btn-action discard"
                        (click)="bulkAction('Discard')" [disabled]="actionRunning()">
                  Descartar fallidos
                </button>
              }
            </div>
          </div>

          @if (actionMessage()) {
            <div class="action-feedback">{{ actionMessage() }}</div>
          }

          @if (docsLoading()) {
            <app-loading-spinner message="Cargando documentos..." />
          }

          @if (documents().length > 0) {
            <div class="table-wrap">
              <table class="docs-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Tipo</th>
                    <th>External ID</th>
                    <th>Etapa</th>
                    <th>Estado</th>
                    <th>Error</th>
                    <th>Reintentos</th>
                    <th>Actualizado</th>
                  </tr>
                </thead>
                <tbody>
                  @for (doc of documents(); track doc.id) {
                    <tr>
                      <td class="mono">{{ doc.id | slice:0:8 }}…</td>
                      <td>{{ doc.entityType }}</td>
                      <td class="mono">{{ doc.externalId | slice:0:20 }}</td>
                      <td>{{ stageLabel(doc.currentStage) }}</td>
                      <td>
                        <span class="status-badge" [class]="statusClass(doc.status)">
                          {{ statusLabel(doc.status) }}
                        </span>
                      </td>
                      <td class="error-cell" [title]="doc.errorMessage ?? ''">
                        {{ doc.errorMessage ? (doc.errorMessage | slice:0:60) + '…' : '—' }}
                      </td>
                      <td>{{ doc.retryCount }}</td>
                      <td class="nowrap">{{ formatDate(doc.lastUpdatedAt) }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
            <div class="pagination">
              <button type="button" [disabled]="skip() === 0" (click)="prevPage()">← Anterior</button>
              <span>{{ skip() + 1 }}–{{ skip() + documents().length }} de {{ totalDocs() }}</span>
              <button type="button" [disabled]="skip() + pageSize >= totalDocs()" (click)="nextPage()">Siguiente →</button>
            </div>
          }

          @if (!docsLoading() && documents().length === 0) {
            <p class="empty-docs">No hay documentos con los filtros seleccionados.</p>
          }
        </section>
      }
    </div>
  `,
  styles: [`
    .job-detail { max-width: 1200px; }
    .breadcrumb {
      display: flex; align-items: center; gap: 0.5rem;
      font-size: 0.875rem; margin-bottom: 1.5rem; color: var(--color-text-secondary);
    }
    .breadcrumb a { color: var(--color-primary); text-decoration: none; }
    .breadcrumb a:hover { text-decoration: underline; }
    .breadcrumb .sep { color: var(--color-text-secondary); }
    .state-message { text-align: center; padding: 2rem; }
    .state-message.error { color: var(--color-primary); }
    .btn-retry {
      margin-top: 0.5rem; padding: 0.4rem 0.75rem;
      border: 1px solid var(--color-primary); color: var(--color-primary);
      background: none; border-radius: var(--radius-sm); cursor: pointer;
    }

    .summary-section { margin-bottom: 2rem; }

    .pipeline-visual {
      display: flex; align-items: flex-start; gap: 2.5rem;
      background: var(--color-bg-surface); border-radius: var(--radius-md);
      padding: 2rem 2rem 1.5rem; box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      overflow-x: auto;
    }
    .pipeline-flow {
      display: flex; align-items: flex-start; gap: 0; flex: 1;
    }

    .pipeline-node {
      display: flex; flex-direction: column; align-items: center;
      cursor: pointer; position: relative; min-width: 100px;
    }
    .pipeline-node:hover .node-circle { transform: scale(1.08); }
    .pipeline-node.active .node-circle {
      box-shadow: 0 0 0 3px var(--color-primary), 0 4px 16px rgba(208,74,2,0.25);
    }

    .node-label {
      font-family: Arial, sans-serif; font-size: 0.6875rem; font-weight: 600;
      text-transform: uppercase; letter-spacing: 0.04em;
      color: var(--color-text-secondary); margin-bottom: 0.5rem;
      white-space: nowrap;
    }
    .node-circle {
      width: 72px; height: 72px; border-radius: 50%;
      background: #2d4a5c; color: white;
      display: flex; align-items: center; justify-content: center;
      transition: all 0.2s ease; position: relative;
    }
    .node-circle.has-activity {
      background: #1a6b4a;
      animation: pulse-ring 2s ease-out infinite;
    }
    @keyframes pulse-ring {
      0% { box-shadow: 0 0 0 0 rgba(26, 107, 74, 0.4); }
      70% { box-shadow: 0 0 0 8px rgba(26, 107, 74, 0); }
      100% { box-shadow: 0 0 0 0 rgba(26, 107, 74, 0); }
    }
    .node-count {
      font-family: Georgia, serif; font-size: 1.375rem; font-weight: 700;
      line-height: 1;
    }

    .node-indicators {
      display: flex; gap: 0.75rem; margin-top: 0.625rem;
    }
    .indicator {
      display: flex; flex-direction: column; align-items: center; gap: 2px;
      opacity: 0.35;
    }
    .indicator.has-items { opacity: 1; }
    .indicator-icon {
      width: 18px; height: 18px;
    }
    .indicator.queue .indicator-icon rect { fill: #7b68ae; }
    .indicator.error .indicator-icon circle { fill: #8b1a1a; }
    .indicator-count {
      font-size: 0.6875rem; font-weight: 600; color: var(--color-text-secondary);
    }
    .indicator.error.has-items .indicator-count { color: #8b1a1a; font-weight: 700; }

    .node-circle.finished {
      background: #1a6b4a;
    }
    .final-node { cursor: default; }

    .pipeline-arrow {
      display: flex; align-items: center; padding-top: 2.25rem;
      min-width: 28px;
    }
    .arrow-line {
      height: 2px; flex: 1; min-width: 14px;
      background: #b0bec5;
    }
    .arrow-head {
      width: 0; height: 0;
      border-top: 5px solid transparent;
      border-bottom: 5px solid transparent;
      border-left: 8px solid #b0bec5;
    }

    .pipeline-totals {
      display: flex; flex-direction: column; gap: 1.25rem;
      padding: 0.25rem 0; border-left: 1px solid var(--color-border);
      padding-left: 2rem; min-width: 120px;
    }
    .total-item {
      display: flex; flex-direction: column; align-items: center; gap: 0.125rem;
    }
    .total-icon { width: 28px; height: 28px; color: #2d4a5c; }
    .total-item.skipped .total-icon { color: #757575; }
    .total-item.skipped .total-count { color: #757575; }
    .total-item.errors .total-icon { color: #999; }
    .total-item.errors.has-errors .total-icon { color: #8b1a1a; }
    .total-item.completed .total-icon { color: #1a6b4a; }
    .total-count {
      font-family: Georgia, serif; font-size: 1.25rem; font-weight: 700;
      color: var(--color-text);
    }
    .total-item.errors.has-errors .total-count { color: #8b1a1a; }
    .total-item.completed .total-count { color: #1a6b4a; }
    .total-label {
      font-size: 0.625rem; text-transform: uppercase; letter-spacing: 0.04em;
      color: var(--color-text-secondary); font-weight: 600;
    }

    .documents-section h2 { font-size: 1.125rem; margin-bottom: 0.5rem; }
    .docs-header { display: flex; align-items: center; gap: 1rem; flex-wrap: wrap; margin-bottom: 1rem; }
    .filters { display: flex; gap: 0.5rem; }
    .filters select {
      padding: 0.35rem 0.5rem; font-size: 0.8125rem;
      border: 1px solid var(--color-border-input); border-radius: var(--radius-sm);
    }
    .actions { display: flex; gap: 0.5rem; margin-left: auto; }
    .btn-action {
      padding: 0.35rem 0.75rem; font-size: 0.8125rem; border: none;
      border-radius: var(--radius-sm); cursor: pointer; transition: all var(--transition-base);
    }
    .btn-action.reprocess { background: var(--color-primary); color: white; }
    .btn-action.discard { background: #888; color: white; }
    .btn-action.batch { background: #2d4a5c; color: white; }
    .btn-action.batch-outline {
      background: transparent;
      color: var(--color-primary);
      border: 1px solid var(--color-primary);
    }
    .btn-action:disabled { opacity: 0.6; cursor: not-allowed; }
    .action-feedback {
      padding: 0.5rem 0.75rem; background: var(--color-success-bg); color: var(--color-success);
      border-radius: var(--radius-sm); font-size: 0.8125rem; margin-bottom: 1rem;
    }

    .table-wrap { overflow-x: auto; }
    .docs-table { width: 100%; border-collapse: collapse; font-size: 0.8125rem; }
    .docs-table th, .docs-table td {
      padding: 0.625rem 0.75rem; text-align: left; border-bottom: 1px solid var(--color-border);
    }
    .docs-table th {
      font-size: 0.6875rem; font-weight: 600; text-transform: uppercase;
      letter-spacing: 0.04em; color: var(--color-text-secondary); background: var(--color-bg-subtle);
    }
    .mono { font-family: var(--font-mono, monospace); font-size: 0.75rem; }
    .error-cell {
      max-width: 250px; overflow: hidden; text-overflow: ellipsis;
      white-space: nowrap; color: var(--color-primary); font-size: 0.75rem;
    }
    .nowrap { white-space: nowrap; }
    .status-badge { font-size: 0.625rem; font-weight: 600; padding: 2px 6px; border-radius: var(--radius-pill); }
    .status-badge.completed { background: var(--color-success-bg); color: var(--color-success); }
    .status-badge.processing { background: #e3f2fd; color: #1565c0; }
    .status-badge.pending { background: #e8e8e8; color: #555; }
    .status-badge.failed { background: var(--color-primary-light); color: var(--color-primary); }
    .status-badge.discarded { background: #f5f5f5; color: #888; }

    .pagination {
      display: flex; align-items: center; justify-content: center; gap: 1rem;
      margin-top: 1rem; font-size: 0.8125rem;
    }
    .pagination button {
      padding: 0.35rem 0.75rem; font-size: 0.8125rem;
      border: 1px solid var(--color-border-input); border-radius: var(--radius-sm);
      background: var(--color-bg-surface); cursor: pointer;
    }
    .pagination button:disabled { opacity: 0.5; cursor: not-allowed; }
    .empty-docs { text-align: center; padding: 2rem; color: var(--color-text-secondary); font-size: 0.875rem; }
  `]
})
export class JobDetailComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private crawlerService = inject(CrawlerService);
  private pollSub?: Subscription;

  jobId = '';
  entityType = '';
  entityLabel = '';
  sourceId = '';
  sourceName = '';

  loadingState = signal<'loading' | 'loaded' | 'error'>('loading');
  errorMsg = signal('');
  summary = signal<JobDocumentsSummary | null>(null);

  stageFilter = signal<string | null>(null);
  statusFilter = signal<string | null>(null);

  documents = signal<JobDocument[]>([]);
  totalDocs = signal(0);
  docsLoading = signal(false);
  pageSize = 50;
  skip = signal(0);

  skippedCount = signal(0);

  actionRunning = signal(false);
  actionMessage = signal<string | null>(null);

  ngOnInit() {
    this.jobId = this.route.snapshot.paramMap.get('jobId') ?? this.route.snapshot.paramMap.get('id') ?? '';
    this.entityType = this.route.snapshot.paramMap.get('entityType') ?? '';
    this.sourceId = this.route.snapshot.paramMap.get('sourceId') ?? '';

    this.entityLabel = ENTITY_LABELS[this.entityType] ?? this.entityType;

    if (this.sourceId) {
      this.crawlerService.getCrawlerById(Number(this.sourceId)).subscribe({
        next: c => this.sourceName = c.sourceName,
        error: () => this.sourceName = `Fuente ${this.sourceId}`
      });
    } else {
      this.crawlerService.getJobs().subscribe({
        next: jobs => {
          const match = jobs.find(j => j.id === this.jobId);
          if (match) this.sourceName = match.sourceName;
        }
      });
    }

    this.loadSummary();
    this.loadDocuments();
    this.loadJobSkipped();

    this.pollSub = interval(10_000)
      .pipe(
        switchMap(() =>
          this.crawlerService.getJobDocumentsSummary(this.jobId).pipe(catchError(() => EMPTY))
        )
      )
      .subscribe({
        next: s => {
          this.summary.set(s);
          this.loadingState.set('loaded');
          this.loadDocuments();
        }
      });
  }

  ngOnDestroy() {
    this.pollSub?.unsubscribe();
  }

  loadSummary() {
    this.loadingState.set('loading');
    this.crawlerService.getJobDocumentsSummary(this.jobId).subscribe({
      next: s => {
        this.summary.set(s);
        this.loadingState.set('loaded');
      },
      error: err => {
        this.errorMsg.set(err?.error?.detail ?? err?.message ?? 'Error al cargar.');
        this.loadingState.set('error');
      }
    });
  }

  loadJobSkipped() {
    this.crawlerService.getJobs().subscribe({
      next: jobs => {
        const job = jobs.find(j => j.id === this.jobId);
        if (job) this.skippedCount.set(job.documentsSkipped ?? 0);
      }
    });
  }

  loadDocuments() {
    this.docsLoading.set(true);
    const params: Record<string, string | number> = {
      skip: this.skip(),
      take: this.pageSize
    };
    const stage = this.stageFilter();
    const status = this.statusFilter();
    if (stage) params['stage'] = stage;
    if (status) params['status'] = status;

    this.crawlerService.getJobDocuments(this.jobId, params).subscribe({
      next: res => {
        this.documents.set(res.documents);
        this.totalDocs.set(res.totalCount);
        this.docsLoading.set(false);
      },
      error: () => this.docsLoading.set(false)
    });
  }

  private _stagesCache: { name: string; data: StageSummary }[] = [];

  orderedStages(): { name: string; data: StageSummary }[] {
    const s = this.summary();
    const empty: StageSummary = { completed: 0, processing: 0, pending: 0, failed: 0, discarded: 0, cancelled: 0, total: 0 };
    this._stagesCache = STAGE_ORDER.map(name => ({
      name,
      data: s?.stages?.[name] ?? { ...empty }
    }));
    return this._stagesCache;
  }

  passedThrough(stageIndex: number): number {
    const stages = this._stagesCache;
    let sum = 0;
    for (let i = stageIndex + 1; i < stages.length; i++) {
      sum += stages[i].data.total;
    }
    const last = stages[stages.length - 1];
    if (stageIndex < stages.length - 1) {
      return sum;
    }
    return last?.data.completed ?? 0;
  }

  finishedCount(): number {
    const stages = this._stagesCache;
    if (stages.length === 0) return 0;
    return stages[stages.length - 1].data.completed;
  }

  selectStage(name: string) {
    this.stageFilter.set(this.stageFilter() === name ? null : name);
    this.skip.set(0);
    this.loadDocuments();
  }

  applyFilters() {
    this.skip.set(0);
    this.loadDocuments();
  }

  prevPage() {
    this.skip.set(Math.max(0, this.skip() - this.pageSize));
    this.loadDocuments();
  }

  nextPage() {
    this.skip.set(this.skip() + this.pageSize);
    this.loadDocuments();
  }

  hasFailedInStage(): boolean {
    const s = this.summary();
    const stage = this.stageFilter();
    if (!s || !stage || !(stage in s.stages)) return false;
    return s.stages[stage].failed > 0;
  }

  bulkAction(action: 'Reprocess' | 'Discard') {
    const stage = this.stageFilter();
    if (!stage) return;
    this.actionRunning.set(true);
    this.actionMessage.set(null);
    this.crawlerService.bulkDocumentAction(this.jobId, stage, action).subscribe({
      next: res => {
        this.actionMessage.set(res.message);
        this.actionRunning.set(false);
        this.loadSummary();
        this.loadDocuments();
      },
      error: () => {
        this.actionRunning.set(false);
        this.actionMessage.set('Error al ejecutar la acción.');
      }
    });
  }

  reprocessNextBatch(take: number) {
    const stage = this.stageFilter();
    if (!stage) return;
    this.actionRunning.set(true);
    this.actionMessage.set(null);
    this.crawlerService.reprocessNextFailed(this.jobId, stage, take).subscribe({
      next: res => {
        this.actionMessage.set(`${res.message} (documentos encolados: ${res.affectedCount})`);
        this.actionRunning.set(false);
        this.loadSummary();
        this.loadDocuments();
      },
      error: err => {
        this.actionRunning.set(false);
        const msg = err?.error?.detail ?? err?.error?.title ?? err?.message ?? 'Error al reprocesar el lote.';
        this.actionMessage.set(msg);
      }
    });
  }

  stageLabel(name: string): string {
    return STAGE_LABELS[name] ?? name;
  }

  statusClass(status: string): string {
    return (status ?? '').toLowerCase();
  }

  statusLabel(status: string): string {
    const labels: Record<string, string> = {
      pending: 'Pendiente',
      processing: 'Procesando',
      completed: 'Completado',
      failed: 'Fallido',
      discarded: 'Descartado',
      cancelled: 'Cancelado'
    };
    return labels[(status ?? '').toLowerCase()] ?? status;
  }

  formatDate(value: string | null): string {
    if (!value) return '—';
    const d = new Date(value);
    if (isNaN(d.getTime())) return '—';
    return d.toLocaleString('es-AR', {
      day: '2-digit', month: '2-digit', year: 'numeric',
      hour: '2-digit', minute: '2-digit'
    });
  }
}
