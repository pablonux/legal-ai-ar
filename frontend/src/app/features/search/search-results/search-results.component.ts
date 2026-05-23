import { Component, signal, computed, inject, DestroyRef } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { RulingService } from '../../../services/ruling.service';
import { RulingCardComponent } from '../ruling-card/ruling-card.component';
import { SkeletonCardComponent } from '../../../shared/components/skeletons/skeleton-card.component';
import { BreadcrumbComponent } from '../../../shared/components/breadcrumb/breadcrumb.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { OnboardingService } from '../../../services/onboarding.service';
import type { SearchFiltersRequest, SearchRulingsResult } from '../../../models/ruling.models';

type SearchState = 'idle' | 'loading' | 'results' | 'empty' | 'error';

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [RouterLink, RulingCardComponent, SkeletonCardComponent, FormsModule, BreadcrumbComponent, EmptyStateComponent],
  template: `
    <div class="search-results">
      <app-breadcrumb [items]="[{ label: 'Jurisprudencia', route: '/jurisprudencia' }, { label: 'Resultados' }]" />

      @if (activeFilters().length > 0 && state() !== 'idle') {
        <div class="active-filters-bar">
          <div class="active-filters-label">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
              <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"/>
            </svg>
            <span>Filtros activos:</span>
          </div>
          <div class="active-filters-chips">
            @for (f of activeFilters(); track f.key) {
              <span class="filter-chip">
                <span class="filter-chip-label">{{ f.label }}</span>
                <span class="filter-chip-value">{{ f.display }}</span>
                <button type="button" class="filter-chip-remove" (click)="removeFilter(f.key)" [attr.aria-label]="'Quitar filtro ' + f.label">
                  <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
                </button>
              </span>
            }
          </div>
          <button type="button" class="clear-filters-btn" (click)="retryWithoutFilters()">Limpiar todos</button>
        </div>
      }

      @if (state() === 'idle') {
        <div class="state-message">
          <p>Ingrese un término de búsqueda o aplique filtros para ver resultados.</p>
          <a routerLink="/jurisprudencia" class="link-button">Ir a buscar</a>
        </div>
      }

      @if (state() === 'loading') {
        <div aria-live="polite">
          <app-skeleton-card [count]="5" />
        </div>
      }

      @if (state() === 'empty') {
        <app-empty-state
          [title]="'Sin resultados' + (query() ? ' para &quot;' + query() + '&quot;' : '')"
          subtitle="No encontramos jurisprudencia que coincida con tu búsqueda."
          variant="search"
          [tips]="searchEmptyTips()"
          [actions]="searchEmptyActions()">
          <svg esIcon xmlns="http://www.w3.org/2000/svg" width="56" height="56" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
        </app-empty-state>
      }

      @if (state() === 'error') {
        <div class="state-message error">
          <p>Error al buscar. Intenta de nuevo.</p>
          <button type="button" class="link-button" (click)="retry()">Reintentar</button>
        </div>
      }

      @if (state() === 'results' && result(); as r) {
        <h2 class="query-summary">{{ query() ? 'Resultados para: ' + query() : 'Resultados por filtros' }}</h2>

        <p class="results-info">
          Mostrando {{ startIndex() }}–{{ endIndex() }} de {{ r.totalCount }} resultados
        </p>

        <div class="results-list">
          @for (item of r.results; track item.id) {
            <app-ruling-card [ruling]="item" [maxScore]="maxScore()" />
          }
        </div>

        <div class="pagination">
          <div class="page-size">
            <label for="pageSize">Mostrar</label>
            <select id="pageSize" [ngModel]="pageSize()" (ngModelChange)="onPageSizeChange(+$event)">
              <option [value]="10">10</option>
              <option [value]="25">25</option>
              <option [value]="50">50</option>
            </select>
            <span>por página</span>
          </div>
          <div class="page-nav">
            <button
              type="button"
              class="page-arrow"
              [disabled]="page() <= 1"
              (click)="goToPage(page() - 1)"
              aria-label="Página anterior"
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="15 18 9 12 15 6"/></svg>
            </button>
            @for (p of pageButtons(); track $index) {
              @if (p.type === 'ellipsis') {
                <span class="page-ellipsis">…</span>
              } @else {
                <button
                  type="button"
                  class="page-btn"
                  [class.active]="p.value === page()"
                  (click)="goToPage(p.value!)"
                >{{ p.label }}</button>
              }
            }
            <button
              type="button"
              class="page-arrow"
              [disabled]="page() >= totalPages()"
              (click)="goToPage(page() + 1)"
              aria-label="Página siguiente"
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="9 18 15 12 9 6"/></svg>
            </button>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .search-results {
      max-width: 860px;
      margin: 0 auto;
      padding: 0;
    }
    .breadcrumb {
      display: flex;
      align-items: center;
      flex-wrap: wrap;
      gap: 0.375rem;
      font-size: 0.8125rem;
      margin-bottom: 1rem;
      color: var(--color-text-secondary);
    }
    .breadcrumb a,
    .breadcrumb > span:not(.sep) {
      display: inline-block;
      padding: 0.25rem 0.625rem;
      background: var(--color-bg-subtle);
      border-radius: var(--radius-xs);
    }
    .breadcrumb a { color: var(--color-primary); text-decoration: none; }
    .breadcrumb a:hover { text-decoration: underline; }
    .breadcrumb .sep {
      margin: 0;
      color: var(--color-text-secondary);
      opacity: 0.35;
      font-weight: 300;
      font-size: 0.6875rem;
      user-select: none;
    }
    .active-filters-bar {
      display: flex;
      flex-wrap: wrap;
      align-items: center;
      gap: 0.5rem;
      padding: 0.625rem 0.875rem;
      margin-bottom: 1rem;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
    }
    .active-filters-label {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 0.75rem;
      font-weight: 600;
      color: var(--color-text-secondary);
      white-space: nowrap;
    }
    .active-filters-chips {
      display: flex;
      flex-wrap: wrap;
      gap: 0.375rem;
    }
    .filter-chip {
      display: inline-flex;
      align-items: center;
      gap: 2px;
      padding: 0.1875rem 0.5rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-pill);
      font-size: 0.75rem;
      line-height: 1.4;
      color: var(--color-text-body);
    }
    .filter-chip-label {
      font-weight: 600;
      color: var(--color-text-secondary);
    }
    .filter-chip-label::after { content: ':'; }
    .filter-chip-value {
      max-width: 160px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
    .filter-chip-remove {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 16px;
      height: 16px;
      margin-left: 2px;
      padding: 0;
      border: none;
      background: transparent;
      color: var(--color-text-secondary);
      cursor: pointer;
      border-radius: 50%;
      transition: background 0.15s, color 0.15s;
    }
    .filter-chip-remove:hover {
      background: rgba(208, 74, 2, 0.1);
      color: var(--color-primary);
    }
    .clear-filters-btn {
      margin-left: auto;
      padding: 0.25rem 0.625rem;
      background: transparent;
      border: none;
      color: var(--color-primary);
      font-size: 0.75rem;
      font-weight: 600;
      cursor: pointer;
      white-space: nowrap;
      transition: text-decoration 0.15s;
    }
    .clear-filters-btn:hover { text-decoration: underline; }
    .query-summary {
      font-size: 1.25rem;
      font-weight: 600;
      margin: 0 0 1rem;
      color: var(--color-text);
    }
    .results-info {
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
      margin-bottom: 1rem;
    }
    .results-list {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }
    .state-message {
      text-align: center;
      padding: 4rem 2rem;
      color: var(--color-text-body);
    }
    .state-message.error { color: var(--color-primary); }

    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      padding: 3rem 2rem;
      gap: 0.5rem;
    }
    .empty-icon {
      color: var(--color-text-secondary);
      opacity: 0.35;
      margin-bottom: 0.5rem;
    }
    .empty-title {
      font-size: 1.125rem;
      font-weight: 600;
      color: var(--color-text);
      margin: 0;
    }
    .empty-body {
      font-size: 0.875rem;
      color: var(--color-text-secondary);
      margin: 0;
      max-width: 420px;
    }
    .empty-hint {
      display: flex;
      align-items: center;
      gap: 6px;
      margin-top: 0.75rem;
      padding: 0.5rem 0.875rem;
      background: rgba(208, 74, 2, 0.06);
      border: 1px solid rgba(208, 74, 2, 0.15);
      border-radius: 8px;
      font-size: 0.8125rem;
      color: var(--color-primary);
    }
    .outline-btn {
      margin-top: 0.5rem;
      padding: 0.5rem 1.25rem;
      background: transparent;
      border: 1px solid var(--color-primary);
      border-radius: var(--radius-sm);
      color: var(--color-primary);
      font-size: 0.8125rem;
      font-weight: 600;
      cursor: pointer;
      transition: background 0.15s;
    }
    .outline-btn:hover {
      background: rgba(208, 74, 2, 0.06);
    }
    .empty-suggestions {
      margin-top: 1rem;
      text-align: left;
      max-width: 320px;
    }
    .empty-suggestions-title {
      font-size: 0.8125rem;
      font-weight: 600;
      color: var(--color-text-secondary);
      margin: 0 0 0.375rem;
    }
    .empty-suggestions ul {
      margin: 0;
      padding-left: 1.25rem;
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
      line-height: 1.7;
    }
    .link-button {
      display: inline-block;
      margin-top: 1rem;
      padding: 0.625rem 1.5rem;
      height: auto;
      line-height: normal;
      background: var(--color-primary);
      color: #fff;
      text-decoration: none;
      border-radius: var(--radius-sm);
      font-size: 0.875rem;
      font-weight: 600;
      border: none;
      cursor: pointer;
      transition: background-color var(--transition-fast);
    }
    .link-button:hover { background: var(--color-primary-hover); }
    .pagination {
      display: flex;
      flex-wrap: wrap;
      justify-content: space-between;
      align-items: center;
      gap: 1rem;
      margin-top: 2rem;
      padding-top: 1.5rem;
      border-top: 1px solid var(--color-border-input);
    }
    .page-size {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.875rem;
    }
    .page-size select {
      padding: 0.35rem 0.5rem;
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      transition: border-color var(--transition-fast), box-shadow var(--transition-fast);
    }
    .page-size select:hover {
      border-color: var(--color-text-secondary);
    }
    .page-nav {
      display: flex;
      align-items: center;
      gap: 1rem;
    }
    .page-arrow {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 34px;
      height: 34px;
      border: 1px solid var(--color-border-input);
      background: var(--color-bg-surface);
      border-radius: var(--radius-sm);
      cursor: pointer;
      color: var(--color-text);
      transition: all 0.15s;
    }
    .page-arrow:hover:not(:disabled) {
      border-color: var(--color-primary);
      color: var(--color-primary);
    }
    .page-arrow:disabled {
      opacity: 0.35;
      cursor: not-allowed;
    }
    .page-btn {
      min-width: 34px;
      height: 34px;
      padding: 0 0.5rem;
      border: 1px solid var(--color-border-input);
      background: var(--color-bg-surface);
      border-radius: var(--radius-sm);
      cursor: pointer;
      font-size: 0.8125rem;
      font-family: inherit;
      color: var(--color-text);
      transition: all 0.15s;
    }
    .page-btn:hover { border-color: var(--color-primary); color: var(--color-primary); }
    .page-btn.active {
      background: var(--color-primary);
      border-color: var(--color-primary);
      color: #fff;
      font-weight: 600;
      cursor: default;
    }
    .page-ellipsis {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 28px;
      font-size: 0.875rem;
      color: var(--color-text-secondary);
      user-select: none;
    }
  `]
})
export class SearchResultsComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private rulingService = inject(RulingService);
  private destroyRef = inject(DestroyRef);

  query = signal('');
  page = signal(1);
  pageSize = signal(10);
  filters = signal<SearchFiltersRequest>({});
  state = signal<SearchState>('idle');
  result = signal<SearchRulingsResult | null>(null);
  error = signal<string | null>(null);

  startIndex = computed(() => {
    const r = this.result();
    if (!r) return 0;
    return (r.page - 1) * r.pageSize + 1;
  });

  endIndex = computed(() => {
    const r = this.result();
    if (!r) return 0;
    return (r.page - 1) * r.pageSize + r.results.length;
  });

  totalPages = computed(() => {
    const r = this.result();
    if (!r || r.totalCount === 0) return 0;
    return Math.ceil(r.totalCount / r.pageSize);
  });

  activeFilters = computed<{ key: string; label: string; display: string }[]>(() => {
    const f = this.filters();
    const chips: { key: string; label: string; display: string }[] = [];
    if (f.jurisdictionArea) chips.push({ key: 'jurisdictionArea', label: 'Jurisdicción', display: f.jurisdictionArea });
    if (f.instance) chips.push({ key: 'instance', label: 'Instancia', display: f.instance });
    if (f.court) chips.push({ key: 'court', label: 'Tribunal', display: f.court });
    if (f.courtId != null) chips.push({ key: 'courtId', label: 'Tribunal ID', display: String(f.courtId) });
    if (f.dateFrom) chips.push({ key: 'dateFrom', label: 'Desde', display: f.dateFrom });
    if (f.dateTo) chips.push({ key: 'dateTo', label: 'Hasta', display: f.dateTo });
    if (f.keywords?.length) chips.push({ key: 'keywords', label: 'Palabras clave', display: f.keywords.join(', ') });
    if (f.subjectArea) chips.push({ key: 'subjectArea', label: 'Materia', display: f.subjectArea });
    if (f.courtType) chips.push({ key: 'courtType', label: 'Tipo tribunal', display: f.courtType });
    if (f.fuero) chips.push({ key: 'fuero', label: 'Fuero', display: f.fuero });
    if (f.legalBranch) chips.push({ key: 'legalBranch', label: 'Rama del derecho', display: f.legalBranch });
    if (f.precedentWeight) chips.push({ key: 'precedentWeight', label: 'Peso precedencial', display: f.precedentWeight });
    if (f.resourceType) chips.push({ key: 'resourceType', label: 'Tipo', display: f.resourceType });
    if (f.isUnconstitutional) chips.push({ key: 'isUnconstitutional', label: 'Inconstitucionalidad', display: 'Sí' });
    return chips;
  });

  activeFilterCount = computed(() => this.activeFilters().length);

  searchEmptyTips = computed(() => {
    const tips = [
      'Usá términos más generales o sinónimos',
      'Verificá la ortografía de tu búsqueda',
      'Probá con menos palabras clave',
    ];
    const fc = this.activeFilterCount();
    if (fc > 0) {
      tips.unshift(`Tenés ${fc} filtro${fc > 1 ? 's' : ''} activo${fc > 1 ? 's' : ''} que podrían limitar resultados`);
    }
    return tips;
  });

  searchEmptyActions = computed<import('../../../shared/components/empty-state/empty-state.component').EmptyStateAction[]>(() => {
    const acts: import('../../../shared/components/empty-state/empty-state.component').EmptyStateAction[] = [];
    if (this.activeFilterCount() > 0) {
      acts.push({ label: 'Buscar sin filtros', action: () => this.retryWithoutFilters(), primary: true });
    }
    acts.push({ label: 'Nueva búsqueda', route: '/jurisprudencia' });
    return acts;
  });

  maxScore = computed(() => {
    const r = this.result();
    if (!r?.results.length) return 0;
    return Math.max(...r.results.map(i => i.relevanceScore ?? 0));
  });

  pageButtons = computed<{ type: 'page' | 'ellipsis'; label: string; value?: number }[]>(() => {
    const p = this.page();
    const total = this.totalPages();
    if (total <= 7) {
      return Array.from({ length: total }, (_, i) => ({
        type: 'page' as const, label: String(i + 1), value: i + 1
      }));
    }
    const pages: { type: 'page' | 'ellipsis'; label: string; value?: number }[] = [];
    pages.push({ type: 'page', label: '1', value: 1 });
    if (p > 3) pages.push({ type: 'ellipsis', label: '…' });
    const start = Math.max(2, p - 1);
    const end = Math.min(total - 1, p + 1);
    for (let i = start; i <= end; i++) {
      pages.push({ type: 'page', label: String(i), value: i });
    }
    if (p < total - 2) pages.push({ type: 'ellipsis', label: '…' });
    pages.push({ type: 'page', label: String(total), value: total });
    return pages;
  });

  private onboarding = inject(OnboardingService);

  constructor() {
    this.route.queryParams
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => this.onQueryParamsChange(params));
    this.onboarding.tryShow('search-filters');
  }

  private onQueryParamsChange(params: Record<string, string | undefined>) {
    const q = (params['query'] ?? '').trim();
    this.query.set(q);

    const filters: SearchFiltersRequest = {};
    if (params['jurisdictionArea']) filters.jurisdictionArea = params['jurisdictionArea'];
    if (params['instance']) filters.instance = params['instance'];
    if (params['courtId']) filters.courtId = parseInt(params['courtId']!, 10);
    if (params['court']) filters.court = params['court'];
    if (params['dateFrom']) filters.dateFrom = params['dateFrom'];
    if (params['dateTo']) filters.dateTo = params['dateTo'];
    if (params['keywords']) filters.keywords = params['keywords']!.split(',').map((k: string) => k.trim()).filter(Boolean);
    if (params['subjectArea']) filters.subjectArea = params['subjectArea'];
    if (params['courtType']) filters.courtType = params['courtType'];
    if (params['fuero']) filters.fuero = params['fuero'];
    if (params['legalBranch']) filters.legalBranch = params['legalBranch'];
    if (params['precedentWeight']) filters.precedentWeight = params['precedentWeight'];
    if (params['resourceType']) filters.resourceType = params['resourceType'];
    if (params['isUnconstitutional'] === 'true') filters.isUnconstitutional = true;
    this.filters.set(filters);

    const hasFilters = Object.keys(filters).length > 0;
    if (!q && !hasFilters) {
      this.state.set('idle');
      this.result.set(null);
      return;
    }

    const page = Math.max(1, parseInt(params['page'] ?? '1', 10) || 1);
    const pageSize = Math.min(50, Math.max(1, parseInt(params['pageSize'] ?? '10', 10) || 10));
    const allowedSizes = [10, 25, 50];
    const validPageSize = allowedSizes.includes(pageSize) ? pageSize : 10;

    this.page.set(page);
    this.pageSize.set(validPageSize);

    this.runSearch(q, page, validPageSize, filters);
  }

  private runSearch(
    query: string,
    page: number,
    pageSize: number,
    filters: SearchFiltersRequest
  ) {
    this.state.set('loading');
    this.error.set(null);

    this.rulingService
      .search({
        query: query || undefined,
        filters: Object.keys(filters).length ? filters : undefined,
        page,
        pageSize
      })
      .subscribe({
        next: (res) => {
          this.result.set(res);
          this.state.set(res.results.length ? 'results' : 'empty');
        },
        error: (err) => {
          this.error.set(err.message || 'Error desconocido');
          this.state.set('error');
        }
      });
  }

  goToPage(p: number) {
    this.updateUrl({ page: p });
  }

  onPageSizeChange(size: number) {
    this.pageSize.set(size);
    this.updateUrl({ page: 1, pageSize: size });
  }

  retry() {
    this.runSearch(this.query(), this.page(), this.pageSize(), this.filters());
  }

  removeFilter(key: string) {
    const params = { ...this.route.snapshot.queryParams };
    delete params[key];
    params['page'] = '1';
    this.router.navigate([], { queryParams: params, replaceUrl: true });
  }

  retryWithoutFilters() {
    const q = this.query();
    const params: Record<string, string> = { page: '1' };
    if (q) params['query'] = q;
    this.router.navigate([], {
      queryParams: params,
      replaceUrl: true
    });
  }

  private updateUrl(updates: Record<string, number>) {
    const params = { ...this.route.snapshot.queryParams };
    for (const [k, v] of Object.entries(updates)) {
      params[k] = String(v);
    }
    this.router.navigate([], { queryParams: params, replaceUrl: true });
  }
}
