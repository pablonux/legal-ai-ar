import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { interval, Subscription, startWith } from 'rxjs';
import { StatsService } from '@legal-ai-ar/app/services/stats.service';
import { CrawlerService, type Job } from '@legal-ai-ar/app/services/admin/crawler.service';

const POLL_INTERVAL_MS = 60_000;

interface StatusMetrics {
  totalRulings: number;
  totalCourts: number;
  totalPersons: number;
  lastSync: string | null;
}

@Component({
  selector: 'app-status-bar',
  standalone: true,
  template: `
    <div class="status-bar">
      <div class="status-left">
        @if (ingesting()) {
          <span class="ingest-indicator" title="Ingesta en progreso">
            <span class="pulse-dot"></span>
            <span class="ingest-label">Ingesta en progreso</span>
          </span>
        }
        <button type="button" class="metric" title="Total de fallos en la KB" (click)="goToStats()">
          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>
          <span>{{ metrics()?.totalRulings?.toLocaleString() ?? '–' }} fallos</span>
        </button>
        <span class="metric" title="Total de tribunales">
          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M3 21h18"/><path d="M5 21V7l8-4v18"/><path d="M19 21V11l-6-4"/></svg>
          <span>{{ metrics()?.totalCourts?.toLocaleString() ?? '–' }}</span>
        </span>
        <span class="metric" title="Total de personas indexadas">
          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/></svg>
          <span>{{ metrics()?.totalPersons?.toLocaleString() ?? '–' }}</span>
        </span>
        @if (metrics()?.lastSync) {
          <span class="metric sync" title="Última sincronización">
            <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/></svg>
            <span>{{ formatSync(metrics()!.lastSync!) }}</span>
          </span>
        }
      </div>
      <div class="status-right">
        <span class="hint">
          <kbd>Ctrl</kbd><span class="kbd-plus">+</span><kbd>K</kbd>
          <span class="hint-label">Buscar</span>
        </span>
      </div>
    </div>
  `,
  styles: [`
    .status-bar {
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: 28px;
      padding: 0 1rem 0 1.25rem;
      background: var(--color-bg-surface);
      border-top: 1px solid var(--color-border);
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      flex-shrink: 0;
      gap: 1rem;
      user-select: none;
    }

    .status-left, .status-right {
      display: flex;
      align-items: center;
      gap: 0.875rem;
    }

    .metric {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      background: none;
      border: none;
      padding: 0;
      font: inherit;
      color: inherit;
      cursor: default;
      white-space: nowrap;
    }

    button.metric {
      cursor: pointer;
      transition: color 0.15s;
    }

    button.metric:hover {
      color: var(--color-primary);
    }

    .ingest-indicator {
      display: inline-flex;
      align-items: center;
      gap: 5px;
      color: #16a34a;
      font-weight: 600;
    }

    .pulse-dot {
      width: 7px;
      height: 7px;
      border-radius: 50%;
      background: #16a34a;
      animation: pulse-ring 1.5s ease-in-out infinite;
    }

    @keyframes pulse-ring {
      0%, 100% { box-shadow: 0 0 0 0 rgba(22, 163, 74, 0.4); }
      50% { box-shadow: 0 0 0 4px rgba(22, 163, 74, 0); }
    }

    .ingest-label {
      font-size: 0.625rem;
      text-transform: uppercase;
      letter-spacing: 0.03em;
    }

    .hint {
      display: inline-flex;
      align-items: center;
      gap: 2px;
    }

    kbd {
      display: inline-block;
      padding: 0 4px;
      font-family: var(--font-mono, ui-monospace, monospace);
      font-size: 0.625rem;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      border-radius: 3px;
      line-height: 1.4;
    }

    .kbd-plus {
      font-size: 0.5rem;
      opacity: 0.5;
    }

    .hint-label {
      margin-left: 4px;
      opacity: 0.7;
    }

    @media (prefers-reduced-motion: reduce) {
      .pulse-dot { animation: none; }
    }

    @media (max-width: 768px) {
      .ingest-label, .sync, .hint { display: none; }
    }
  `]
})
export class StatusBarComponent implements OnInit, OnDestroy {
  private statsService = inject(StatsService);
  private crawlerService = inject(CrawlerService);
  private router = inject(Router);
  private sub?: Subscription;

  metrics = signal<StatusMetrics | null>(null);
  ingesting = signal(false);

  ngOnInit() {
    this.sub = interval(POLL_INTERVAL_MS)
      .pipe(startWith(0))
      .subscribe(() => {
        this.fetchStats();
        this.fetchJobs();
      });
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
  }

  goToStats() {
    this.router.navigate(['/estadisticas']);
  }

  formatSync(iso: string): string {
    try {
      const d = new Date(iso);
      const now = new Date();
      const diffMs = now.getTime() - d.getTime();
      const diffMin = Math.floor(diffMs / 60_000);

      if (diffMin < 1) return 'hace instantes';
      if (diffMin < 60) return `hace ${diffMin}m`;
      const diffH = Math.floor(diffMin / 60);
      if (diffH < 24) return `hace ${diffH}h`;
      const diffD = Math.floor(diffH / 24);
      return `hace ${diffD}d`;
    } catch {
      return iso;
    }
  }

  private fetchStats() {
    this.statsService.getKbStats().subscribe({
      next: (s) =>
        this.metrics.set({
          totalRulings: s.totalRulings,
          totalCourts: s.totalCourts,
          totalPersons: s.totalPersons,
          lastSync: s.latestRulingDate,
        }),
    });
  }

  private fetchJobs() {
    this.crawlerService.getJobs().subscribe({
      next: (jobs: Job[]) => {
        const hasActive = jobs.some(
          (j) => j.status === 'Running' || j.status === 'Queued'
        );
        this.ingesting.set(hasActive);
      },
    });
  }
}
