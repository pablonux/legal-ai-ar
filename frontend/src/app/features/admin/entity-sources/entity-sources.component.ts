import { Component, signal, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { A11yModule } from '@angular/cdk/a11y';
import { CrawlerService, type CrawlerConfig, type Job } from '../../../services/admin/crawler.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { forkJoin } from 'rxjs';

interface SourceCard {
  config: CrawlerConfig;
  activeJobs: number;
  completedJobs: number;
  failedJobs: number;
  totalJobs: number;
}

const ENTITY_LABELS: Record<string, string> = {
  rulings: 'Jurisprudencia',
  statutes: 'Legislación'
};

const SOURCE_ENTITY_MAP: Record<number, string> = {
  1: 'rulings', 2: 'statutes', 3: 'rulings', 4: 'rulings', 5: 'rulings',
};

@Component({
  selector: 'app-entity-sources',
  standalone: true,
  imports: [RouterLink, FormsModule, LoadingSpinnerComponent, A11yModule],
  template: `
    <div class="sources-page">
      <nav class="breadcrumb">
        <a routerLink="/admin">Ingesta</a>
        <span class="sep">›</span>
        <span>{{ entityLabel }}</span>
      </nav>

      @if (state() === 'loading') {
        <app-loading-spinner message="Cargando fuentes..." />
      }

      @if (state() === 'error') {
        <div class="state-message error">
          <p>{{ error() }}</p>
          <button type="button" class="btn-retry" (click)="load()">Reintentar</button>
        </div>
      }

      @if (state() === 'loaded') {
        <div class="source-grid">
          @for (src of sources(); track src.config.sourceId) {
            <div class="source-card">
              <div class="card-top">
                <a class="source-name"
                   [routerLink]="['/admin', entityType, 'sources', src.config.sourceId, 'jobs']">
                  {{ src.config.sourceName }}
                </a>
                <button type="button" class="toggle-pill" [class.active]="src.config.isEnabled"
                        [disabled]="updatingId() === src.config.sourceId"
                        (click)="toggleEnabled(src.config)">
                  {{ src.config.isEnabled ? 'Activo' : 'Inactivo' }}
                </button>
              </div>

              <div class="card-meta">
                <div class="meta-row">
                  <span class="meta-label">Último crawl</span>
                  <span class="meta-value">
                    {{ formatDate(src.config.lastCrawledAt) }}
                    @if (src.config.lastCrawledStatus) {
                      <span class="status-dot" [class]="statusDotClass(src.config.lastCrawledStatus)"></span>
                    }
                  </span>
                </div>
                <div class="meta-row">
                  <span class="meta-label">Documentos</span>
                  <span class="meta-value">{{ src.config.lastDocumentCount ?? 0 }}</span>
                </div>
              </div>

              <div class="card-jobs">
                <div class="job-stat" title="Jobs activos">
                  <span class="job-num active">{{ src.activeJobs }}</span>
                  <span class="job-label">activos</span>
                </div>
                <div class="job-stat" title="Jobs completados">
                  <span class="job-num completed">{{ src.completedJobs }}</span>
                  <span class="job-label">completados</span>
                </div>
                <div class="job-stat" title="Jobs fallidos">
                  <span class="job-num failed">{{ src.failedJobs }}</span>
                  <span class="job-label">fallidos</span>
                </div>
              </div>

              <div class="card-actions">
                <a class="btn-view"
                   [routerLink]="['/admin', entityType, 'sources', src.config.sourceId, 'jobs']">
                  Ver jobs
                </a>
                <button type="button" class="btn-run"
                        [disabled]="!src.config.isEnabled"
                        (click)="openRunModal(src.config)">
                  Ejecutar crawl
                </button>
              </div>
            </div>
          }
        </div>

        @if (sources().length === 0) {
          <p class="empty">No hay fuentes configuradas para esta entidad.</p>
        }
      }
    </div>

    @if (showRunModal()) {
      <div class="modal-overlay" (click)="closeModal()" (keydown.escape)="closeModal()">
        <div class="modal" role="dialog" aria-modal="true" cdkTrapFocus cdkTrapFocusAutoCapture
             (click)="$event.stopPropagation()">
          <h2>Ejecutar crawl — {{ runCrawler()?.sourceName }}</h2>
          <div class="modal-body">
            <div class="form-group">
              <label for="crawl-type">Tipo</label>
              <select id="crawl-type" [(ngModel)]="runType">
                <option value="incremental">Incremental</option>
                <option value="by-range">Por rango de fechas</option>
                <option value="fallos-destacados">Fallos destacados</option>
              </select>
            </div>
            @if (runType === 'incremental') {
              <div class="form-group">
                <label for="crawl-since">Desde</label>
                <input id="crawl-since" type="date" [(ngModel)]="runSince" />
                <span class="hint">Opcional. Por defecto: última ejecución.</span>
              </div>
            }
            @if (runType === 'by-range') {
              <div class="form-group">
                <label for="crawl-from">Desde</label>
                <input id="crawl-from" type="date" [(ngModel)]="runDateFrom" required />
              </div>
              <div class="form-group">
                <label for="crawl-to">Hasta</label>
                <input id="crawl-to" type="date" [(ngModel)]="runDateTo" required />
              </div>
            }
            <div class="form-row">
              <div class="form-group half">
                <label for="crawl-skip">Saltear primeros</label>
                <input id="crawl-skip" type="number" min="0" [(ngModel)]="runSkipDocs" placeholder="0" />
              </div>
              <div class="form-group half">
                <label for="crawl-max">Máximo a procesar</label>
                <input id="crawl-max" type="number" min="1" [(ngModel)]="runMaxDocs" placeholder="Todos" />
              </div>
            </div>
            <div class="form-group checkbox-group">
              <label class="checkbox-label">
                <input type="checkbox" [(ngModel)]="runUseCache" />
                <span>Usar cache</span>
              </label>
            </div>
            <div class="form-group checkbox-group">
              <label class="checkbox-label">
                <input type="checkbox" [(ngModel)]="runReprocess" />
                <span>Reprocesar existentes</span>
              </label>
            </div>
          </div>
          <div class="modal-actions">
            <button type="button" class="btn-cancel" (click)="closeModal()">Cancelar</button>
            <button type="button" class="btn-confirm" [disabled]="running()" (click)="confirmRun()">
              {{ running() ? 'Ejecutando...' : 'Confirmar' }}
            </button>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .sources-page { max-width: 900px; }
    .breadcrumb {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.875rem;
      margin-bottom: 1.5rem;
      color: var(--color-text-secondary);
    }
    .breadcrumb a { color: var(--color-primary); text-decoration: none; }
    .breadcrumb a:hover { text-decoration: underline; }
    .breadcrumb .sep { color: var(--color-text-secondary); }
    .state-message { text-align: center; padding: 3rem 2rem; }
    .state-message.error { color: var(--color-primary); }
    .btn-retry {
      margin-top: 1rem;
      padding: 0.5rem 1rem;
      background: none;
      border: 1px solid var(--color-primary);
      color: var(--color-primary);
      border-radius: var(--radius-sm);
      cursor: pointer;
    }
    .source-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
      gap: 1rem;
    }
    .source-card {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: 1.25rem;
      box-shadow: var(--shadow-sm);
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }
    .card-top {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .source-name {
      font-size: 1rem;
      font-weight: 600;
      color: var(--color-text);
      text-decoration: none;
    }
    .source-name:hover { color: var(--color-primary); }
    .toggle-pill {
      padding: 0.2rem 0.6rem;
      border-radius: var(--radius-pill);
      font-size: 0.6875rem;
      font-weight: 600;
      border: 1px solid var(--color-border-input);
      background: #f0f0f0;
      color: var(--color-text-secondary);
      cursor: pointer;
      transition: all var(--transition-base);
    }
    .toggle-pill.active {
      background: var(--color-success-bg);
      border-color: var(--color-success);
      color: var(--color-success);
    }
    .toggle-pill:disabled { opacity: 0.6; cursor: not-allowed; }
    .card-meta {
      display: flex;
      flex-direction: column;
      gap: 0.375rem;
    }
    .meta-row {
      display: flex;
      justify-content: space-between;
      font-size: 0.8125rem;
    }
    .meta-label { color: var(--color-text-secondary); }
    .meta-value {
      display: flex;
      align-items: center;
      gap: 0.375rem;
      font-weight: 500;
    }
    .status-dot {
      width: 7px;
      height: 7px;
      border-radius: 50%;
    }
    .status-dot.success { background: var(--color-success); }
    .status-dot.partial { background: #e6a817; }
    .status-dot.failed { background: var(--color-primary); }
    .status-dot.unknown { background: #ccc; }
    .card-jobs {
      display: flex;
      gap: 1rem;
      padding: 0.625rem 0;
      border-top: 1px solid var(--color-border);
      border-bottom: 1px solid var(--color-border);
    }
    .job-stat { display: flex; flex-direction: column; align-items: center; flex: 1; }
    .job-num { font-size: 1.125rem; font-weight: 700; line-height: 1; }
    .job-num.active { color: #1565c0; }
    .job-num.completed { color: var(--color-success); }
    .job-num.failed { color: var(--color-primary); }
    .job-label { font-size: 0.6875rem; color: var(--color-text-secondary); margin-top: 0.125rem; }
    .card-actions {
      display: flex;
      gap: 0.5rem;
    }
    .btn-view {
      flex: 1;
      text-align: center;
      padding: 0.4rem 0;
      font-size: 0.8125rem;
      color: var(--color-primary);
      border: 1px solid var(--color-primary);
      border-radius: var(--radius-sm);
      text-decoration: none;
      transition: all var(--transition-base);
    }
    .btn-view:hover { background: var(--color-primary-light); }
    .btn-run {
      flex: 1;
      padding: 0.4rem 0;
      font-size: 0.8125rem;
      background: var(--color-primary);
      color: white;
      border: none;
      border-radius: var(--radius-sm);
      cursor: pointer;
      transition: all var(--transition-base);
    }
    .btn-run:hover:not(:disabled) { background: var(--color-primary-hover); }
    .btn-run:disabled { opacity: 0.6; cursor: not-allowed; }
    .empty { text-align: center; padding: 3rem; color: var(--color-text-secondary); }

    .modal-overlay {
      position: fixed; inset: 0;
      background: rgba(0,0,0,0.4);
      display: flex; align-items: center; justify-content: center;
      z-index: 200;
    }
    .modal {
      background: var(--color-bg-surface);
      border-radius: var(--radius-md);
      padding: 1.5rem;
      max-width: 400px;
      width: 90%;
      box-shadow: var(--shadow-lg);
    }
    .modal h2 { font-size: 1.1rem; margin: 0 0 1rem 0; }
    .modal-body { margin-bottom: 1rem; }
    .form-group { margin-bottom: 0.75rem; }
    .form-group label { display: block; font-size: 0.8125rem; margin-bottom: 0.25rem; }
    .form-group select,
    .form-group input[type="date"],
    .form-group input[type="number"] {
      width: 100%; height: 2.25rem;
      padding: 0 0.75rem;
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      font-size: 0.875rem;
    }
    .hint { font-size: 0.75rem; color: var(--color-text-secondary); }
    .form-row { display: flex; gap: 0.75rem; }
    .form-group.half { flex: 1; }
    .checkbox-group { margin-top: 0.5rem; }
    .checkbox-label { display: flex; align-items: center; gap: 0.5rem; font-size: 0.8125rem; cursor: pointer; }
    .checkbox-label input { width: auto; height: auto; margin: 0; }
    .modal-actions { display: flex; gap: 0.5rem; justify-content: flex-end; }
    .btn-cancel {
      padding: 0.4rem 0.75rem; font-size: 0.8125rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      cursor: pointer;
    }
    .btn-confirm {
      padding: 0.4rem 0.75rem; font-size: 0.8125rem;
      background: var(--color-primary); color: white; border: none;
      border-radius: var(--radius-sm); cursor: pointer;
    }
    .btn-confirm:disabled { opacity: 0.6; cursor: not-allowed; }
  `]
})
export class EntitySourcesComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private crawlerService = inject(CrawlerService);
  private notify = inject(NotificationService);

  entityType = '';
  entityLabel = '';

  state = signal<'loading' | 'loaded' | 'error'>('loading');
  sources = signal<SourceCard[]>([]);
  error = signal('');
  updatingId = signal<number | null>(null);

  showRunModal = signal(false);
  runCrawler = signal<CrawlerConfig | null>(null);
  runType = 'incremental';
  runSince = '';
  runDateFrom = '';
  runDateTo = '';
  runMaxDocs: number | null = null;
  runSkipDocs: number | null = null;
  runUseCache = false;
  runReprocess = false;
  running = signal(false);

  ngOnInit() {
    this.entityType = this.route.snapshot.paramMap.get('entityType') ?? this.route.snapshot.url[0]?.path ?? 'rulings';
    this.entityLabel = ENTITY_LABELS[this.entityType] ?? this.entityType;
    this.load();
  }

  load() {
    this.state.set('loading');
    forkJoin({
      crawlers: this.crawlerService.getCrawlers(),
      jobs: this.crawlerService.getJobs()
    }).subscribe({
      next: ({ crawlers, jobs }) => {
        const filtered = crawlers.filter(c => (SOURCE_ENTITY_MAP[c.sourceId] ?? 'rulings') === this.entityType);
        this.sources.set(filtered.map(config => {
          const sourceJobs = jobs.filter(j => j.sourceId === config.sourceId);
          return {
            config,
            activeJobs: sourceJobs.filter(j => ['running', 'pending'].includes((j.status ?? '').toLowerCase())).length,
            completedJobs: sourceJobs.filter(j => ['completed', 'success'].includes((j.status ?? '').toLowerCase())).length,
            failedJobs: sourceJobs.filter(j => (j.status ?? '').toLowerCase() === 'failed').length,
            totalJobs: sourceJobs.length,
          };
        }));
        this.state.set('loaded');
      },
      error: err => {
        this.error.set(err?.error?.detail ?? err?.message ?? 'Error al cargar.');
        this.state.set('error');
      }
    });
  }

  toggleEnabled(c: CrawlerConfig) {
    this.updatingId.set(c.sourceId);
    this.crawlerService.updateCrawler(c.sourceId, !c.isEnabled).subscribe({
      next: () => { this.updatingId.set(null); this.load(); this.notify.success(c.isEnabled ? 'Fuente deshabilitada.' : 'Fuente habilitada.'); },
      error: err => { this.updatingId.set(null); this.notify.error(err?.error?.detail ?? 'Error al actualizar.'); }
    });
  }

  openRunModal(c: CrawlerConfig) {
    this.runCrawler.set(c);
    this.runType = 'incremental';
    this.runSince = c.lastCrawledAt ? new Date(c.lastCrawledAt).toISOString().slice(0, 10) : '';
    this.runDateFrom = '';
    this.runDateTo = '';
    this.runMaxDocs = null;
    this.runSkipDocs = null;
    this.runUseCache = false;
    this.runReprocess = false;
    this.showRunModal.set(true);
  }

  closeModal() { this.showRunModal.set(false); this.runCrawler.set(null); }

  confirmRun() {
    const c = this.runCrawler();
    if (!c) return;
    const type = this.runType as 'incremental' | 'by-range' | 'fallos-destacados';
    const opts: Record<string, string | boolean | number> = {};
    if (type === 'incremental' && this.runSince) opts['since'] = this.runSince;
    if (type === 'by-range') { opts['dateFrom'] = this.runDateFrom; opts['dateTo'] = this.runDateTo; }
    if (this.runMaxDocs) opts['maxDocuments'] = this.runMaxDocs;
    if (this.runSkipDocs) opts['skipDocuments'] = this.runSkipDocs;
    if (this.runUseCache) opts['useCache'] = true;
    if (this.runReprocess) opts['reprocess'] = true;

    this.running.set(true);
    this.crawlerService.runCrawler(c.sourceId, type, opts as any).subscribe({
      next: res => { this.running.set(false); this.closeModal(); this.load(); this.notify.success(res.message ?? 'Crawl iniciado.'); },
      error: err => { this.running.set(false); this.notify.error(err?.error?.detail ?? 'Error al ejecutar.'); }
    });
  }

  statusDotClass(status: string | null): string {
    if (!status) return 'unknown';
    const s = status.toLowerCase();
    if (s === 'success') return 'success';
    if (s === 'partial') return 'partial';
    if (s === 'failed') return 'failed';
    return 'unknown';
  }

  formatDate(value: string | null): string {
    if (!value) return '—';
    const d = new Date(value);
    if (isNaN(d.getTime())) return '—';
    return d.toLocaleString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
  }
}
