import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { StatsService } from '../../services/stats.service';
import { SkeletonStatComponent } from '@legal-ai-ar/shared-common/components/skeletons/skeleton-stat.component';
import type { KbStats, NameCount } from '../../models/stats.models';

@Component({
  selector: 'app-kb-dashboard',
  standalone: true,
  imports: [SkeletonStatComponent],
  template: `
    <div class="dash-layout">

      @if (state() === 'loading') {
        <div class="center-state">
          <app-skeleton-stat [count]="6" />
        </div>
      }

      @if (state() === 'error') {
        <div class="center-state">
          <div class="error-block">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>
            <span>{{ error() }}</span>
            <button type="button" class="retry-btn" (click)="load()">Reintentar</button>
          </div>
        </div>
      }

      @if (state() === 'loaded' && stats()) {
        <div class="dash-scroll">

          <!-- Header -->
          <div class="dash-header">
            <div class="header-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M21 12V7H5a2 2 0 0 1 0-4h14v4"/>
                <path d="M3 5v14a2 2 0 0 0 2 2h16v-5"/>
                <path d="M18 12a2 2 0 0 0 0 4h4v-4Z"/>
              </svg>
            </div>
            <h1 class="header-title">Base de Conocimiento</h1>
            <p class="header-sub">Composición, cobertura y calidad de la jurisprudencia indexada</p>
          </div>

          <!-- KPI cards -->
          <div class="kpi-grid">
            <div class="kpi-card">
              <span class="kpi-value">{{ formatNumber(stats()!.totalRulings) }}</span>
              <span class="kpi-label">Fallos indexados</span>
            </div>
            <div class="kpi-card">
              <span class="kpi-value">{{ formatNumber(stats()!.totalCourts) }}</span>
              <span class="kpi-label">Tribunales</span>
            </div>
            <div class="kpi-card">
              <span class="kpi-value">{{ formatNumber(stats()!.totalPersons) }}</span>
              <span class="kpi-label">Personas</span>
            </div>
            <div class="kpi-card">
              <span class="kpi-value">{{ dateRange() }}</span>
              <span class="kpi-label">Cobertura temporal</span>
            </div>
          </div>

          <!-- Sections row 1: sources + timeline -->
          <div class="section-row">
            <section class="section-card">
              <div class="section-header">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 12h-4l-3 9L9 3l-3 9H2"/></svg>
                <h2>Por fuente</h2>
              </div>
              <div class="section-body">
                @for (s of stats()!.bySource; track s.sourceId) {
                  <div class="bar-row">
                    <span class="bar-label">{{ s.name }}</span>
                    <div class="bar-track">
                      <div class="bar-fill" [style.width.%]="pct(s.count, maxSource())"></div>
                    </div>
                    <span class="bar-value">{{ formatNumber(s.count) }}</span>
                  </div>
                }
                @if (stats()!.bySource.length === 0) {
                  <p class="empty-text">Sin datos de fuentes</p>
                }
              </div>
            </section>

            <section class="section-card section-card--wide">
              <div class="section-header">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
                <h2>Cobertura temporal</h2>
              </div>
              <div class="section-body">
                @for (y of stats()!.byYear; track y.year) {
                  <div class="bar-row bar-row--compact">
                    <span class="bar-label bar-label--mono">{{ y.year }}</span>
                    <div class="bar-track">
                      <div class="bar-fill" [style.width.%]="pct(y.count, maxYear())"></div>
                    </div>
                    <span class="bar-value">{{ formatNumber(y.count) }}</span>
                  </div>
                }
                @if (stats()!.byYear.length === 0) {
                  <p class="empty-text">Sin datos temporales</p>
                }
              </div>
            </section>
          </div>

          <!-- Sections row 2: courts + jurisdiction -->
          <div class="section-row">
            <section class="section-card section-card--wide">
              <div class="section-header">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="m16 16 3-8 3 8c-.87.65-1.92 1-3 1s-2.13-.35-3-1Z"/><path d="m2 16 3-8 3 8c-.87.65-1.92 1-3 1s-2.13-.35-3-1Z"/><path d="M7 21h10"/><path d="M12 3v18"/><path d="M3 7h2c2 0 5-1 7-2 2 1 5 2 7 2h2"/></svg>
                <h2>Top tribunales</h2>
              </div>
              <div class="section-body">
                @for (c of stats()!.topCourts; track c.courtId) {
                  <div class="bar-row bar-row--compact">
                    <span class="bar-label" [title]="c.name">{{ c.name }}</span>
                    <div class="bar-track">
                      <div class="bar-fill" [style.width.%]="pct(c.count, maxCourt())"></div>
                    </div>
                    <span class="bar-value">{{ formatNumber(c.count) }}</span>
                  </div>
                }
              </div>
            </section>

            <section class="section-card">
              <div class="section-header">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0Z"/><circle cx="12" cy="10" r="3"/></svg>
                <h2>Por jurisdicción</h2>
              </div>
              <div class="section-body">
                @for (j of stats()!.byJurisdiction; track j.name) {
                  <div class="bar-row">
                    <span class="bar-label">{{ j.name }}</span>
                    <div class="bar-track">
                      <div class="bar-fill" [style.width.%]="pct(j.count, maxOf(stats()!.byJurisdiction))"></div>
                    </div>
                    <span class="bar-value">{{ formatNumber(j.count) }}</span>
                  </div>
                }
                @if (stats()!.byJurisdiction.length === 0) {
                  <p class="empty-text">Sin datos de jurisdicción</p>
                }
              </div>
            </section>
          </div>

          <!-- Quality section -->
          <section class="section-card section-card--full">
            <div class="section-header">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>
              <h2>Calidad de la información</h2>
            </div>
            <div class="section-body quality-grid">
              @for (q of qualityItems(); track q.label) {
                <div class="quality-row">
                  <span class="quality-label">{{ q.label }}</span>
                  <div class="quality-track">
                    <div class="quality-fill" [style.width.%]="q.pct" [class.high]="q.pct >= 75" [class.mid]="q.pct >= 40 && q.pct < 75" [class.low]="q.pct < 40"></div>
                  </div>
                  <span class="quality-value">{{ q.pct }}%</span>
                  <span class="quality-count">{{ formatNumber(q.value) }} / {{ formatNumber(stats()!.totalRulings) }}</span>
                </div>
              }
            </div>
          </section>

          <!-- Sections row 3: keywords + instance + subject -->
          <div class="section-row section-row--triple">
            <section class="section-card">
              <div class="section-header">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M20.59 13.41l-7.17 7.17a2 2 0 0 1-2.83 0L2 12V2h10l8.59 8.59a2 2 0 0 1 0 2.82z"/><line x1="7" y1="7" x2="7.01" y2="7"/></svg>
                <h2>Top keywords</h2>
              </div>
              <div class="section-body">
                @for (k of stats()!.topKeywords; track k.name) {
                  <div class="bar-row bar-row--compact">
                    <span class="bar-label" [title]="k.name">{{ k.name }}</span>
                    <span class="bar-value">{{ formatNumber(k.count) }}</span>
                  </div>
                }
              </div>
            </section>

            <section class="section-card">
              <div class="section-header">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/></svg>
                <h2>Por instancia</h2>
              </div>
              <div class="section-body">
                @for (i of stats()!.byInstance; track i.name) {
                  <div class="bar-row">
                    <span class="bar-label">{{ i.name }}</span>
                    <div class="bar-track">
                      <div class="bar-fill" [style.width.%]="pct(i.count, maxOf(stats()!.byInstance))"></div>
                    </div>
                    <span class="bar-value">{{ formatNumber(i.count) }}</span>
                  </div>
                }
                @if (stats()!.byInstance.length === 0) {
                  <p class="empty-text">Sin datos de instancia</p>
                }
              </div>
            </section>

            <section class="section-card">
              <div class="section-header">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"/></svg>
                <h2>Por materia</h2>
              </div>
              <div class="section-body">
                @for (s of stats()!.bySubjectArea; track s.name) {
                  <div class="bar-row bar-row--compact">
                    <span class="bar-label" [title]="s.name">{{ s.name }}</span>
                    <span class="bar-value">{{ formatNumber(s.count) }}</span>
                  </div>
                }
                @if (stats()!.bySubjectArea.length === 0) {
                  <p class="empty-text">Sin datos de materia</p>
                }
              </div>
            </section>
          </div>

          <!-- Secondary KPIs -->
          <div class="kpi-grid kpi-grid--secondary">
            <div class="kpi-card kpi-card--small">
              <span class="kpi-value">{{ formatNumber(stats()!.totalKeywords) }}</span>
              <span class="kpi-label">Palabras clave</span>
            </div>
            <div class="kpi-card kpi-card--small">
              <span class="kpi-value">{{ formatNumber(stats()!.totalStatutes) }}</span>
              <span class="kpi-label">Normas citadas</span>
            </div>
            <div class="kpi-card kpi-card--small">
              <span class="kpi-value">{{ formatNumber(stats()!.totalCitations) }}</span>
              <span class="kpi-label">Citas cruzadas</span>
            </div>
          </div>

        </div>
      }
    </div>
  `,
  styles: [`
    :host {
      display: block;
      height: 100%;
      margin: -2rem -2.5rem;
    }

    .dash-layout {
      height: 100%;
      display: flex;
      flex-direction: column;
    }

    .center-state {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 2rem;
    }

    .dash-scroll {
      flex: 1;
      overflow-y: auto;
      padding: 2rem 2.5rem 3rem;
    }

    .dash-scroll::-webkit-scrollbar { width: 6px; }
    .dash-scroll::-webkit-scrollbar-track { background: transparent; }
    .dash-scroll::-webkit-scrollbar-thumb { background: var(--color-border); border-radius: 3px; }

    /* ── Header ── */

    .dash-header {
      text-align: center;
      margin-bottom: 1.75rem;
    }

    .header-icon {
      color: var(--color-primary);
      opacity: 0.85;
      margin-bottom: 0.5rem;
    }

    .header-title {
      font-family: var(--font-heading);
      font-size: 1.625rem;
      font-weight: 600;
      color: var(--color-text);
      margin: 0;
    }

    .header-sub {
      font-size: 0.9375rem;
      color: var(--color-text-secondary);
      margin: 0.5rem 0 0;
    }

    /* ── KPI cards ── */

    .kpi-grid {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: 12px;
      margin-bottom: 1.25rem;
    }

    .kpi-grid--secondary {
      grid-template-columns: repeat(3, 1fr);
      margin-top: 1.25rem;
      margin-bottom: 0;
    }

    .kpi-card {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: 12px;
      padding: 1.25rem 1rem;
      text-align: center;
      transition: border-color 0.15s;
    }

    .kpi-card:hover {
      border-color: rgba(208, 74, 2, 0.3);
    }

    .kpi-card--small {
      padding: 1rem 0.75rem;
    }

    .kpi-value {
      display: block;
      font-family: var(--font-heading);
      font-size: 1.5rem;
      font-weight: 700;
      color: var(--color-primary);
      line-height: 1.2;
    }

    .kpi-card--small .kpi-value {
      font-size: 1.25rem;
    }

    .kpi-label {
      display: block;
      font-size: 0.75rem;
      font-weight: 500;
      color: var(--color-text-secondary);
      text-transform: uppercase;
      letter-spacing: 0.5px;
      margin-top: 4px;
    }

    /* ── Section cards ── */

    .section-row {
      display: grid;
      grid-template-columns: 1fr 1.5fr;
      gap: 12px;
      margin-bottom: 12px;
    }

    .section-row--triple {
      grid-template-columns: repeat(3, 1fr);
    }

    .section-card {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: 12px;
      display: flex;
      flex-direction: column;
      overflow: hidden;
    }

    .section-card--wide {
      /* handled by grid */
    }

    .section-card--full {
      margin-bottom: 12px;
    }

    .section-header {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 0.5rem 0.875rem;
      border-bottom: 1px solid var(--color-border);
    }

    .section-header > svg:first-child {
      color: var(--color-primary);
      flex-shrink: 0;
    }

    .section-header h2 {
      flex: 1;
      font-size: 0.625rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.6px;
      color: var(--color-primary);
      margin: 0;
    }

    .section-body {
      flex: 1;
      padding: 0.75rem 0.875rem;
      overflow-y: auto;
      max-height: 280px;
    }

    .section-body::-webkit-scrollbar { width: 4px; }
    .section-body::-webkit-scrollbar-thumb { background: var(--color-border); border-radius: 2px; }

    .empty-text {
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
      margin: 0;
      text-align: center;
      padding: 1rem 0;
    }

    /* ── Bar rows ── */

    .bar-row {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 5px 0;
    }

    .bar-row + .bar-row {
      border-top: 1px solid var(--color-border);
    }

    .bar-row--compact {
      padding: 3px 0;
    }

    .bar-label {
      flex-shrink: 0;
      width: 120px;
      font-size: 0.75rem;
      color: var(--color-text);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .bar-label--mono {
      font-family: 'Consolas', 'Monaco', monospace;
      width: 40px;
    }

    .bar-track {
      flex: 1;
      height: 6px;
      background: var(--color-bg-subtle, #f5f5f4);
      border-radius: 3px;
      overflow: hidden;
    }

    .bar-fill {
      height: 100%;
      background: var(--color-primary);
      border-radius: 3px;
      transition: width 0.4s ease;
      min-width: 2px;
      opacity: 0.7;
    }

    .bar-value {
      flex-shrink: 0;
      width: 50px;
      text-align: right;
      font-size: 0.75rem;
      font-weight: 600;
      color: var(--color-text);
      font-variant-numeric: tabular-nums;
    }

    /* ── Quality ── */

    .quality-grid {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }

    .quality-row {
      display: flex;
      align-items: center;
      gap: 10px;
    }

    .quality-label {
      flex-shrink: 0;
      width: 100px;
      font-size: 0.8125rem;
      font-weight: 500;
      color: var(--color-text);
    }

    .quality-track {
      flex: 1;
      height: 8px;
      background: var(--color-bg-subtle, #f5f5f4);
      border-radius: 4px;
      overflow: hidden;
    }

    .quality-fill {
      height: 100%;
      border-radius: 4px;
      transition: width 0.4s ease;
    }

    .quality-fill.high { background: #16a34a; }
    .quality-fill.mid { background: #d97706; }
    .quality-fill.low { background: var(--color-primary); }

    .quality-value {
      flex-shrink: 0;
      width: 38px;
      text-align: right;
      font-size: 0.8125rem;
      font-weight: 700;
      color: var(--color-text);
      font-variant-numeric: tabular-nums;
    }

    .quality-count {
      flex-shrink: 0;
      width: 110px;
      text-align: right;
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      font-variant-numeric: tabular-nums;
    }

    /* ── Error ── */

    .error-block {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 10px 14px;
      background: var(--color-error-bg);
      border: 1px solid rgba(208, 74, 2, 0.2);
      border-radius: 8px;
      font-size: 0.8125rem;
      color: var(--color-error);
    }

    .retry-btn {
      display: inline-flex;
      align-items: center;
      gap: 5px;
      margin-left: auto;
      padding: 5px 12px;
      background: var(--color-bg-surface);
      color: var(--color-primary);
      border: 1px solid var(--color-primary);
      border-radius: 6px;
      font-size: 0.8125rem;
      font-weight: 500;
      cursor: pointer;
      transition: background 0.15s;
    }

    .retry-btn:hover {
      background: var(--color-nav-active-bg);
    }

    @media (max-width: 800px) {
      .section-row { grid-template-columns: 1fr; }
      .section-row--triple { grid-template-columns: 1fr; }
      .kpi-grid { grid-template-columns: repeat(2, 1fr); }
      .kpi-grid--secondary { grid-template-columns: repeat(3, 1fr); }
    }
  `]
})
export class KbDashboardComponent implements OnInit {
  private statsService = inject(StatsService);

  state = signal<'loading' | 'loaded' | 'error'>('loading');
  stats = signal<KbStats | null>(null);
  error = signal('');

  maxSource = computed(() => Math.max(1, ...this.stats()?.bySource.map(s => s.count) ?? [1]));
  maxYear = computed(() => Math.max(1, ...this.stats()?.byYear.map(y => y.count) ?? [1]));
  maxCourt = computed(() => Math.max(1, ...this.stats()?.topCourts.map(c => c.count) ?? [1]));

  dateRange = computed(() => {
    const s = this.stats();
    if (!s?.earliestRulingDate || !s?.latestRulingDate) return '—';
    const from = s.earliestRulingDate.substring(0, 4);
    const to = s.latestRulingDate.substring(0, 4);
    return from === to ? from : `${from} – ${to}`;
  });

  qualityItems = computed(() => {
    const s = this.stats();
    if (!s) return [];
    const total = s.totalRulings || 1;
    return [
      { label: 'Con resumen', value: s.quality.withSummary, pct: Math.round(s.quality.withSummary / total * 100) },
      { label: 'Con holding', value: s.quality.withHolding, pct: Math.round(s.quality.withHolding / total * 100) },
      { label: 'Con texto', value: s.quality.withFullText, pct: Math.round(s.quality.withFullText / total * 100) },
      { label: 'Con keywords', value: s.quality.withKeywords, pct: Math.round(s.quality.withKeywords / total * 100) },
      { label: 'Con personas', value: s.quality.withPersons, pct: Math.round(s.quality.withPersons / total * 100) },
      { label: 'Con normas', value: s.quality.withStatutes, pct: Math.round(s.quality.withStatutes / total * 100) },
      { label: 'Con citas', value: s.quality.withCitations, pct: Math.round(s.quality.withCitations / total * 100) },
    ];
  });

  ngOnInit() { this.load(); }

  load() {
    this.state.set('loading');
    this.error.set('');
    this.statsService.getKbStats().subscribe({
      next: (data) => { this.stats.set(data); this.state.set('loaded'); },
      error: (err) => {
        this.error.set(err?.error?.detail ?? err?.message ?? 'Error al cargar estadísticas.');
        this.state.set('error');
      }
    });
  }

  pct(value: number, max: number): number {
    return max > 0 ? (value / max) * 100 : 0;
  }

  maxOf(items: NameCount[]): number {
    return Math.max(1, ...items.map(i => i.count));
  }

  formatNumber(n: number): string {
    return n.toLocaleString('es-AR');
  }
}
