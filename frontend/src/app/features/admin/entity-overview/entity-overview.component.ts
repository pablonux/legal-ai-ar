import { Component, signal, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CrawlerService, type PipelineSourceStatus, type Job } from '../../../services/admin/crawler.service';
import { LoadingSpinnerComponent } from '@legal-ai-ar/shared-common/components/loading-spinner/loading-spinner.component';
import { forkJoin } from 'rxjs';

interface EntityCard {
  key: string;
  label: string;
  icon: string;
  route: string;
  sources: PipelineSourceStatus[];
  totalDocs: number;
  activeJobs: number;
  lastCrawl: string | null;
  lastStatus: string | null;
}

const SOURCE_ENTITY_MAP: Record<number, string> = {
  1: 'rulings',   // CSJN
  2: 'statutes',  // SAIJ Legislation
  3: 'rulings',   // SAIJ Rulings
  4: 'rulings',   // SCBA
  5: 'rulings',   // MJN
};

@Component({
  selector: 'app-entity-overview',
  standalone: true,
  imports: [RouterLink, LoadingSpinnerComponent],
  template: `
    <div class="overview">
      @if (state() === 'loading') {
        <div class="state-message" aria-live="polite">
          <app-loading-spinner message="Cargando estado de ingesta..." />
        </div>
      }

      @if (state() === 'error') {
        <div class="state-message error">
          <p>{{ error() }}</p>
          <button type="button" class="btn-retry" (click)="load()">Reintentar</button>
        </div>
      }

      @if (state() === 'loaded') {
        <div class="entity-grid">
          @for (entity of entities(); track entity.key) {
            <a [routerLink]="entity.route" class="entity-card">
              <div class="card-icon">{{ entity.icon }}</div>
              <div class="card-body">
                <h2>{{ entity.label }}</h2>
                <div class="card-stats">
                  <div class="stat">
                    <span class="stat-value">{{ entity.sources.length }}</span>
                    <span class="stat-label">{{ entity.sources.length === 1 ? 'fuente' : 'fuentes' }}</span>
                  </div>
                  <div class="stat">
                    <span class="stat-value">{{ entity.totalDocs }}</span>
                    <span class="stat-label">documentos</span>
                  </div>
                  <div class="stat">
                    <span class="stat-value">{{ entity.activeJobs }}</span>
                    <span class="stat-label">jobs activos</span>
                  </div>
                </div>
                <div class="card-footer">
                  @if (entity.lastCrawl) {
                    <span class="last-crawl">Último crawl: {{ formatDate(entity.lastCrawl) }}</span>
                    <span class="status-dot" [class]="statusClass(entity.lastStatus)"></span>
                  } @else {
                    <span class="last-crawl muted">Sin ejecuciones</span>
                  }
                </div>
              </div>
              <span class="card-chevron">›</span>
            </a>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .overview { max-width: 800px; }
    .state-message {
      text-align: center;
      padding: 3rem 2rem;
      color: var(--color-text-body);
    }
    .state-message.error { color: var(--color-primary); }
    .btn-retry {
      margin-top: 1rem;
      padding: 0.5rem 1rem;
      background: none;
      border: 1px solid var(--color-primary);
      color: var(--color-primary);
      border-radius: var(--radius-sm);
      cursor: pointer;
      font-size: 0.875rem;
    }
    .entity-grid {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }
    .entity-card {
      display: flex;
      align-items: center;
      gap: 1.5rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: 1.5rem 2rem;
      text-decoration: none;
      color: inherit;
      box-shadow: var(--shadow-sm);
      transition: all var(--transition-base);
      cursor: pointer;
    }
    .entity-card:hover {
      box-shadow: var(--shadow-md);
      border-color: var(--color-primary);
    }
    .card-icon {
      font-size: 2.5rem;
      flex-shrink: 0;
      width: 56px;
      height: 56px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--color-bg-subtle);
      border-radius: var(--radius-md);
    }
    .card-body {
      flex: 1;
      min-width: 0;
    }
    .card-body h2 {
      font-size: 1.125rem;
      font-weight: 600;
      margin: 0 0 0.75rem 0;
      color: var(--color-text);
    }
    .card-stats {
      display: flex;
      gap: 2rem;
      margin-bottom: 0.75rem;
    }
    .stat {
      display: flex;
      flex-direction: column;
    }
    .stat-value {
      font-size: 1.5rem;
      font-weight: 700;
      color: var(--color-text);
      line-height: 1;
    }
    .stat-label {
      font-size: 0.75rem;
      color: var(--color-text-secondary);
      margin-top: 0.25rem;
    }
    .card-footer {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
    }
    .last-crawl.muted { font-style: italic; }
    .status-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      flex-shrink: 0;
    }
    .status-dot.success { background: var(--color-success); }
    .status-dot.partial { background: #e6a817; }
    .status-dot.failed { background: var(--color-primary); }
    .status-dot.unknown { background: #ccc; }
    .card-chevron {
      font-size: 1.75rem;
      color: var(--color-text-secondary);
      flex-shrink: 0;
      transition: color var(--transition-base);
    }
    .entity-card:hover .card-chevron { color: var(--color-primary); }
  `]
})
export class EntityOverviewComponent implements OnInit {
  private crawlerService = inject(CrawlerService);

  state = signal<'loading' | 'loaded' | 'error'>('loading');
  entities = signal<EntityCard[]>([]);
  error = signal('');

  ngOnInit() { this.load(); }

  load() {
    this.state.set('loading');
    this.error.set('');
    forkJoin({
      pipeline: this.crawlerService.getPipelineStatus(),
      jobs: this.crawlerService.getJobs()
    }).subscribe({
      next: ({ pipeline, jobs }) => {
        this.entities.set(this.buildEntities(pipeline.sources, jobs));
        this.state.set('loaded');
      },
      error: err => {
        this.error.set(err?.error?.detail ?? err?.message ?? 'Error al cargar.');
        this.state.set('error');
      }
    });
  }

  private buildEntities(sources: PipelineSourceStatus[], jobs: Job[]): EntityCard[] {
    const groups: Record<string, { label: string; icon: string; route: string; sources: PipelineSourceStatus[] }> = {
      rulings: { label: 'Jurisprudencia', icon: '⚖', route: '/admin/rulings', sources: [] },
      statutes: { label: 'Legislación', icon: '📜', route: '/admin/statutes', sources: [] },
    };

    for (const src of sources) {
      const entityKey = SOURCE_ENTITY_MAP[src.sourceId] ?? 'rulings';
      groups[entityKey]?.sources.push(src);
    }

    return Object.entries(groups).map(([key, g]) => {
      const sourceIds = new Set(g.sources.map(s => s.sourceId));
      const entityJobs = jobs.filter(j => sourceIds.has(j.sourceId));
      const activeJobs = entityJobs.filter(j => {
        const s = (j.status ?? '').toLowerCase();
        return s === 'running' || s === 'pending';
      }).length;

      let lastCrawl: string | null = null;
      let lastStatus: string | null = null;
      for (const src of g.sources) {
        if (src.lastCrawledAt && (!lastCrawl || src.lastCrawledAt > lastCrawl)) {
          lastCrawl = src.lastCrawledAt;
          lastStatus = src.lastCrawledStatus;
        }
      }

      const totalDocs = g.sources.reduce((sum, s) => sum + (s.lastDocumentCount ?? 0), 0);

      return { key, ...g, totalDocs, activeJobs, lastCrawl, lastStatus };
    });
  }

  statusClass(status: string | null): string {
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
    return d.toLocaleString('es-AR', {
      day: '2-digit', month: '2-digit', year: 'numeric',
      hour: '2-digit', minute: '2-digit'
    });
  }
}
