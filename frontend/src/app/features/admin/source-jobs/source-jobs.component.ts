import { Component, signal, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CrawlerService, type CrawlerConfig, type Job } from '../../../services/admin/crawler.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { forkJoin } from 'rxjs';

const ENTITY_LABELS: Record<string, string> = {
  rulings: 'Jurisprudencia',
  statutes: 'Legislación'
};

@Component({
  selector: 'app-source-jobs',
  standalone: true,
  imports: [RouterLink, LoadingSpinnerComponent],
  template: `
    <div class="source-jobs-page">
      <nav class="breadcrumb">
        <a routerLink="/admin">Ingesta</a>
        <span class="sep">›</span>
        <a [routerLink]="['/admin', entityType]">{{ entityLabel }}</a>
        <span class="sep">›</span>
        <span>{{ sourceName() }}</span>
      </nav>

      @if (source()) {
        <div class="source-header">
          <div class="source-info">
            <h2>{{ sourceName() }}</h2>
            <div class="header-meta">
              <span>Último crawl: {{ formatDate(source()!.lastCrawledAt) }}</span>
              <span>Documentos: {{ source()!.lastDocumentCount ?? 0 }}</span>
              <span class="status-pill" [class]="statusClass(source()!.lastCrawledStatus)">
                {{ statusLabel(source()!.lastCrawledStatus) }}
              </span>
            </div>
          </div>
        </div>
      }

      @if (state() === 'loading') {
        <app-loading-spinner message="Cargando jobs..." />
      }

      @if (state() === 'error') {
        <div class="state-message error">
          <p>{{ error() }}</p>
          <button type="button" class="btn-retry" (click)="load()">Reintentar</button>
        </div>
      }

      @if (state() === 'loaded') {
        @if (jobs().length > 0) {
          <div class="jobs-list">
            @for (job of jobs(); track job.id) {
              <a class="job-row"
                 [routerLink]="['/admin', entityType, 'sources', sourceId, 'jobs', job.id]">
                <div class="job-main">
                  <div class="job-range">
                    @if (job.dateFrom || job.dateTo) {
                      {{ formatDateShort(job.dateFrom) }} — {{ formatDateShort(job.dateTo) }}
                    } @else {
                      Job {{ job.id.slice(0, 8) }}
                    }
                  </div>
                  <span class="job-status-badge" [class]="jobStatusClass(job.status)">{{ job.status }}</span>
                  <span class="job-type">{{ job.type }}</span>
                </div>

                <div class="job-pipeline">
                  <div class="pipeline-bar">
                    @if (pipelineTotal(job) > 0) {
                      <div class="bar-segment completed"
                           [style.width.%]="pct(job.documentsIndexed, pipelineTotal(job))"
                           title="Indexados: {{ job.documentsIndexed }}"></div>
                      <div class="bar-segment persisted"
                           [style.width.%]="pct(job.documentsPersisted - job.documentsIndexed, pipelineTotal(job))"
                           title="Persistidos: {{ job.documentsPersisted - job.documentsIndexed }}"></div>
                      <div class="bar-segment enriched"
                           [style.width.%]="pct(job.documentsEnriched - job.documentsPersisted, pipelineTotal(job))"
                           title="Enriquecidos: {{ job.documentsEnriched - job.documentsPersisted }}"></div>
                      <div class="bar-segment parsed"
                           [style.width.%]="pct(job.documentsParsed - job.documentsEnriched, pipelineTotal(job))"
                           title="Parseados: {{ job.documentsParsed - job.documentsEnriched }}"></div>
                      <div class="bar-segment crawled"
                           [style.width.%]="pct(job.documentsCrawled - job.documentsParsed, pipelineTotal(job))"
                           title="Descargados: {{ job.documentsCrawled - job.documentsParsed }}"></div>
                      <div class="bar-segment failed"
                           [style.width.%]="pct(job.documentsFailed, pipelineTotal(job))"
                           title="Fallidos: {{ job.documentsFailed }}"></div>
                    } @else {
                      <div class="bar-segment empty" style="width:100%"></div>
                    }
                  </div>
                  <span class="pipeline-label">
                    {{ job.documentsIndexed }}/{{ job.documentsDiscovered }} indexados
                    @if (job.documentsFailed > 0) {
                      · {{ job.documentsFailed }} fallidos
                    }
                  </span>
                </div>

                <div class="job-footer">
                  <span class="job-date">{{ formatDate(job.startedAt ?? job.completedAt) }}</span>
                  <span class="job-trigger">{{ job.triggeredBy }}</span>
                </div>

                <span class="row-chevron">›</span>
              </a>
            }
          </div>
        } @else {
          <p class="empty">No hay jobs para esta fuente.</p>
        }
      }
    </div>
  `,
  styles: [`
    .source-jobs-page { max-width: 900px; }
    .breadcrumb {
      display: flex; align-items: center; gap: 0.5rem;
      font-size: 0.875rem; margin-bottom: 1.5rem; color: var(--color-text-secondary);
    }
    .breadcrumb a { color: var(--color-primary); text-decoration: none; }
    .breadcrumb a:hover { text-decoration: underline; }
    .breadcrumb .sep { color: var(--color-text-secondary); }

    .source-header {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: 1.25rem;
      margin-bottom: 1.5rem;
      box-shadow: var(--shadow-sm);
    }
    .source-info h2 { font-size: 1.125rem; font-weight: 600; margin: 0 0 0.5rem 0; }
    .header-meta {
      display: flex; gap: 1.5rem; flex-wrap: wrap;
      font-size: 0.8125rem; color: var(--color-text-secondary);
    }
    .status-pill {
      font-size: 0.6875rem; font-weight: 600;
      padding: 2px 8px; border-radius: var(--radius-pill);
    }
    .status-pill.success { background: var(--color-success-bg); color: var(--color-success); }
    .status-pill.partial { background: #fff8d6; color: #7a5a00; }
    .status-pill.failed { background: var(--color-primary-light); color: var(--color-primary); }
    .status-pill.unknown { background: #f0f0f0; color: var(--color-text-secondary); }

    .state-message { text-align: center; padding: 3rem 2rem; }
    .state-message.error { color: var(--color-primary); }
    .btn-retry {
      margin-top: 1rem; padding: 0.5rem 1rem;
      background: none; border: 1px solid var(--color-primary);
      color: var(--color-primary); border-radius: var(--radius-sm); cursor: pointer;
    }

    .jobs-list {
      display: flex; flex-direction: column; gap: 0.5rem;
    }
    .job-row {
      display: grid;
      grid-template-columns: 1fr 1fr auto auto;
      align-items: center;
      gap: 1rem;
      padding: 1rem 1.25rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      text-decoration: none;
      color: inherit;
      transition: all var(--transition-base);
      cursor: pointer;
    }
    .job-row:hover {
      border-color: var(--color-primary);
      box-shadow: var(--shadow-sm);
    }
    .job-main { display: flex; align-items: center; gap: 0.5rem; flex-wrap: wrap; }
    .job-range { font-weight: 600; font-size: 0.9375rem; }
    .job-status-badge {
      font-size: 0.625rem; font-weight: 600;
      padding: 2px 8px; border-radius: var(--radius-pill);
      text-transform: uppercase;
    }
    .job-status-badge.running { background: #e3f2fd; color: #1565c0; }
    .job-status-badge.pending { background: #e8e8e8; color: #555; }
    .job-status-badge.completed, .job-status-badge.success { background: var(--color-success-bg); color: var(--color-success); }
    .job-status-badge.failed { background: var(--color-primary-light); color: var(--color-primary); }
    .job-type { font-size: 0.75rem; color: var(--color-text-secondary); }

    .job-pipeline { min-width: 0; }
    .pipeline-bar {
      display: flex;
      height: 6px;
      border-radius: 3px;
      overflow: hidden;
      background: #eee;
      margin-bottom: 0.25rem;
    }
    .bar-segment { min-width: 0; transition: width 0.3s; }
    .bar-segment.completed { background: var(--color-success); }
    .bar-segment.persisted { background: #66bb6a; }
    .bar-segment.enriched { background: #4caf50; }
    .bar-segment.parsed { background: #42a5f5; }
    .bar-segment.crawled { background: #90caf9; }
    .bar-segment.failed { background: var(--color-primary); }
    .bar-segment.empty { background: #e0e0e0; }
    .pipeline-label { font-size: 0.6875rem; color: var(--color-text-secondary); }

    .job-footer {
      display: flex; flex-direction: column; gap: 0.125rem;
      font-size: 0.75rem; color: var(--color-text-secondary); text-align: right;
    }
    .row-chevron {
      font-size: 1.5rem; color: var(--color-text-secondary); flex-shrink: 0;
    }
    .job-row:hover .row-chevron { color: var(--color-primary); }
    .empty { text-align: center; padding: 3rem; color: var(--color-text-secondary); }
  `]
})
export class SourceJobsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private crawlerService = inject(CrawlerService);

  entityType = '';
  entityLabel = '';
  sourceId = 0;
  source = signal<CrawlerConfig | null>(null);
  sourceName = signal('');

  state = signal<'loading' | 'loaded' | 'error'>('loading');
  jobs = signal<Job[]>([]);
  error = signal('');

  ngOnInit() {
    this.entityType = this.route.snapshot.paramMap.get('entityType') ?? '';
    this.sourceId = Number(this.route.snapshot.paramMap.get('sourceId') ?? 0);
    this.entityLabel = ENTITY_LABELS[this.entityType] ?? this.entityType;
    this.load();
  }

  load() {
    this.state.set('loading');
    forkJoin({
      crawler: this.crawlerService.getCrawlerById(this.sourceId),
      jobs: this.crawlerService.getJobs()
    }).subscribe({
      next: ({ crawler, jobs }) => {
        this.source.set(crawler);
        this.sourceName.set(crawler.sourceName);
        this.jobs.set(
          jobs
            .filter(j => j.sourceId === this.sourceId)
            .sort((a, b) => {
              const da = a.startedAt ?? a.completedAt ?? '';
              const db = b.startedAt ?? b.completedAt ?? '';
              return db.localeCompare(da);
            })
        );
        this.state.set('loaded');
      },
      error: err => {
        this.error.set(err?.error?.detail ?? err?.message ?? 'Error al cargar.');
        this.state.set('error');
      }
    });
  }

  pipelineTotal(job: Job): number {
    return Math.max(job.documentsDiscovered, 1);
  }

  pct(value: number, total: number): number {
    if (total <= 0) return 0;
    return Math.max(0, (value / total) * 100);
  }

  jobStatusClass(status: string): string {
    return (status ?? '').toLowerCase();
  }

  statusClass(status: string | null): string {
    if (!status) return 'unknown';
    const s = status.toLowerCase();
    if (s === 'success') return 'success';
    if (s === 'partial') return 'partial';
    if (s === 'failed') return 'failed';
    return 'unknown';
  }

  statusLabel(status: string | null): string {
    if (!status) return 'Sin datos';
    const labels: Record<string, string> = { success: 'OK', partial: 'Parcial', failed: 'Error' };
    return labels[status.toLowerCase()] ?? status;
  }

  formatDate(value: string | null): string {
    if (!value) return '—';
    const d = new Date(value);
    if (isNaN(d.getTime())) return '—';
    return d.toLocaleString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
  }

  formatDateShort(value: string | null | undefined): string {
    if (!value) return '—';
    const d = new Date(value);
    if (isNaN(d.getTime())) return '—';
    return d.toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }
}
