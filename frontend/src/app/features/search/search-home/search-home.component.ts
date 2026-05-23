import { Component, signal, computed, ElementRef, ViewChild, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { FiltersPanelComponent } from '../filters-panel/filters-panel.component';
import { RecentSearchesService } from '../../../services/recent-searches.service';
import type { SearchFiltersRequest } from '../../../models/ruling.models';

interface SuggestedSearch {
  icon: string;
  title: string;
  query: string;
}

@Component({
  selector: 'app-search-home',
  standalone: true,
  imports: [FormsModule, FiltersPanelComponent],
  template: `
    <div class="search-page">
      <div class="search-header">
        <h1 class="page-title">
          <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
          </svg>
          Buscar Jurisprudencia
        </h1>
        <p class="page-desc">Busque fallos por texto libre, filtros o ambos. Puede aplicar filtros sin ingresar texto de búsqueda.</p>
      </div>

      <div class="search-bar-row">
        <div class="search-box" [class.focused]="inputFocused()">
          <svg class="search-icon" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
          </svg>
          <input
            #inputEl
            type="text"
            class="search-input"
            placeholder="Buscar por texto libre, nombre de partes, concepto legal..."
            [ngModel]="query()"
            (ngModelChange)="query.set($event)"
            (keydown.enter)="onSearch()"
            (focus)="inputFocused.set(true)"
            (blur)="inputFocused.set(false)"
            aria-label="Buscar jurisprudencia"
            autocomplete="off"
          />
        </div>
        <button
          type="button"
          class="search-btn"
          [class.active]="canSearch()"
          [disabled]="!canSearch()"
          (click)="onSearch()"
        >
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
          </svg>
          Buscar
        </button>
      </div>

      <div class="filters-toggle-row">
        <button type="button" class="filters-toggle" (click)="filtersExpanded.set(!filtersExpanded())" [attr.aria-expanded]="filtersExpanded()">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"/>
          </svg>
          Filtros avanzados
          @if (activeFilterCount() > 0) {
            <span class="filter-badge">{{ activeFilterCount() }}</span>
          }
          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="chevron" [class.open]="filtersExpanded()" aria-hidden="true">
            <polyline points="6 9 12 15 18 9"/>
          </svg>
        </button>
      </div>

      @if (filtersExpanded()) {
        <app-filters-panel
          [inline]="true"
          [isExpanded]="true"
          [filters]="filters()"
          (filtersChange)="filters.set($event)"
        />
      }

      @if (recentSearches.searches().length > 0) {
        <div class="suggestions-section">
          <div class="recents-header">
            <h3 class="suggestions-title">Búsquedas recientes</h3>
            <button type="button" class="clear-recents" (click)="recentSearches.clear()">Limpiar</button>
          </div>
          <div class="suggestions">
            @for (r of recentSearches.searches(); track r) {
              <button class="suggestion-chip recent-chip" (click)="useSuggestion(r)">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true" class="recent-icon"><circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/></svg>
                {{ r }}
              </button>
            }
          </div>
        </div>
      }

      <div class="suggestions-section">
        <h3 class="suggestions-title">Búsquedas sugeridas</h3>
        <div class="suggestions">
          @for (s of suggestions; track s.title) {
            <button class="suggestion-chip" (click)="useSuggestion(s.query)">
              <span class="suggestion-icon" aria-hidden="true">{{ s.icon }}</span>
              {{ s.title }}
            </button>
          }
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }

    .search-page {
      max-width: 860px;
      margin: 0 auto;
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
    }

    .search-header {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .page-title {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-family: var(--font-heading);
      font-size: 1.375rem;
      font-weight: 600;
      color: var(--color-text);
      margin: 0;
    }

    .page-title svg {
      color: var(--color-primary);
      flex-shrink: 0;
    }

    .page-desc {
      font-size: 0.875rem;
      color: var(--color-text-secondary);
      margin: 0;
      line-height: 1.5;
    }

    /* ── Search bar row ── */

    .search-bar-row {
      display: flex;
      gap: 0.625rem;
      align-items: stretch;
    }

    .search-box {
      flex: 1;
      display: flex;
      align-items: center;
      gap: 0.625rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border-input);
      border-radius: 10px;
      padding: 0 1rem;
      transition: border-color 0.15s, box-shadow 0.15s;
    }

    .search-box.focused {
      border-color: var(--color-primary);
      box-shadow: 0 0 0 2px rgba(208, 74, 2, 0.08);
    }

    .search-icon {
      color: var(--color-text-secondary);
      flex-shrink: 0;
    }

    .search-box.focused .search-icon {
      color: var(--color-primary);
    }

    .search-input {
      flex: 1;
      border: none;
      outline: none;
      background: transparent;
      font-family: inherit;
      font-size: 0.9375rem;
      line-height: 1.5;
      color: var(--color-text);
      padding: 0.625rem 0;
    }

    .search-input::placeholder {
      color: var(--color-text-secondary);
    }

    .search-btn {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0 1.25rem;
      border: none;
      border-radius: 10px;
      font-size: 0.875rem;
      font-weight: 600;
      font-family: inherit;
      cursor: default;
      white-space: nowrap;
      transition: background 0.15s, opacity 0.15s;
      background: var(--color-text-secondary);
      color: #fff;
      opacity: 0.4;
    }

    .search-btn.active {
      background: var(--color-primary);
      opacity: 1;
      cursor: pointer;
    }

    .search-btn.active:hover {
      background: var(--color-primary-hover);
    }

    /* ── Filters toggle ── */

    .filters-toggle-row {
      display: flex;
    }

    .filters-toggle {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 0.375rem 0.75rem;
      background: none;
      border: 1px solid var(--color-border);
      border-radius: 100px;
      font-size: 0.8125rem;
      font-weight: 500;
      color: var(--color-text-body);
      cursor: pointer;
      transition: border-color 0.15s, color 0.15s;
    }

    .filters-toggle:hover {
      border-color: var(--color-primary);
      color: var(--color-primary);
    }

    .filter-badge {
      font-size: 0.625rem;
      font-weight: 700;
      background: var(--color-primary);
      color: #fff;
      border-radius: 50%;
      min-width: 16px;
      height: 16px;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      padding: 0 3px;
    }

    .chevron {
      transition: transform 0.2s;
    }

    .chevron.open {
      transform: rotate(180deg);
    }

    /* ── Suggestions ── */

    .suggestions-section {
      margin-top: 0.5rem;
    }

    .suggestions-title {
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      color: var(--color-text-secondary);
      margin: 0 0 0.5rem;
    }

    .suggestions {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
    }

    .suggestion-chip {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 0.375rem 0.875rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: 100px;
      font-size: 0.8125rem;
      color: var(--color-text-body);
      cursor: pointer;
      transition: border-color 0.15s, box-shadow 0.15s;
    }

    .suggestion-chip:hover {
      border-color: var(--color-primary);
      box-shadow: 0 0 0 1px rgba(208, 74, 2, 0.08);
      color: var(--color-primary);
    }

    .suggestion-icon {
      font-size: 1rem;
      flex-shrink: 0;
    }

    .recents-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 0.5rem;
    }

    .recents-header .suggestions-title {
      margin: 0;
    }

    .clear-recents {
      background: none;
      border: none;
      font-size: 0.75rem;
      font-weight: 500;
      color: var(--color-text-secondary);
      cursor: pointer;
      padding: 0.125rem 0.375rem;
      border-radius: var(--radius-xs);
      transition: color 0.15s;
    }

    .clear-recents:hover {
      color: var(--color-primary);
    }

    .recent-chip {
      font-size: 0.8125rem;
    }

    .recent-icon {
      flex-shrink: 0;
      color: var(--color-text-secondary);
    }
  `]
})
export class SearchHomeComponent {
  @ViewChild('inputEl') inputEl!: ElementRef<HTMLInputElement>;
  recentSearches = inject(RecentSearchesService);

  query = signal('');
  filters = signal<SearchFiltersRequest>({});
  inputFocused = signal(false);
  filtersExpanded = signal(false);

  private hasFilters = computed(() => {
    const f = this.filters();
    return !!(f.jurisdictionArea || f.instance || f.court || f.courtId != null
      || f.dateFrom || f.dateTo || f.keywords?.length || f.subjectArea
      || f.courtType || f.fuero || f.legalBranch || f.precedentWeight
      || f.resourceType || f.isUnconstitutional);
  });

  activeFilterCount = computed(() => {
    const f = this.filters();
    let count = 0;
    if (f.jurisdictionArea) count++;
    if (f.instance) count++;
    if (f.court) count++;
    if (f.courtType) count++;
    if (f.fuero) count++;
    if (f.legalBranch) count++;
    if (f.precedentWeight) count++;
    if (f.dateFrom) count++;
    if (f.dateTo) count++;
    if (f.keywords?.length) count++;
    if (f.subjectArea) count++;
    if (f.resourceType) count++;
    if (f.isUnconstitutional) count++;
    return count;
  });

  canSearch = computed(() =>
    this.query().trim().length > 0 || this.hasFilters()
  );

  suggestions: SuggestedSearch[] = [
    { icon: '\u2696\uFE0F', title: 'Despido sin causa \u2014 indemnizaci\u00f3n laboral', query: 'despido sin causa justificada indemnizaci\u00f3n' },
    { icon: '\uD83C\uDFE5', title: 'Responsabilidad civil por mala praxis', query: 'responsabilidad civil mala praxis m\u00e9dica' },
    { icon: '\uD83D\uDE97', title: 'Da\u00f1os y perjuicios accidente de tr\u00e1nsito', query: 'da\u00f1os y perjuicios accidente de tr\u00e1nsito' },
    { icon: '\uD83D\uDCDC', title: 'CSJN \u2014 libertad de expresi\u00f3n', query: 'CSJN libertad de expresi\u00f3n y prensa' },
  ];

  constructor(private router: Router) {}

  useSuggestion(query: string) {
    this.query.set(query);
    setTimeout(() => this.onSearch());
  }

  onSearch() {
    if (!this.canSearch()) return;

    const f = this.filters();
    if (f.dateFrom && f.dateTo && f.dateFrom > f.dateTo) return;

    const params: Record<string, string> = { page: '1' };

    const q = this.query().trim();
    if (q) {
      params['query'] = q;
      this.recentSearches.add(q);
    }

    if (f.jurisdictionArea) params['jurisdictionArea'] = f.jurisdictionArea;
    if (f.instance) params['instance'] = f.instance;
    if (f.courtId != null) params['courtId'] = String(f.courtId);
    if (f.court) params['court'] = f.court;
    if (f.dateFrom) params['dateFrom'] = f.dateFrom;
    if (f.dateTo) params['dateTo'] = f.dateTo;
    if (f.keywords?.length) params['keywords'] = f.keywords.join(',');
    if (f.subjectArea) params['subjectArea'] = f.subjectArea;
    if (f.courtType) params['courtType'] = f.courtType;
    if (f.fuero) params['fuero'] = f.fuero;
    if (f.legalBranch) params['legalBranch'] = f.legalBranch;
    if (f.precedentWeight) params['precedentWeight'] = f.precedentWeight;
    if (f.resourceType) params['resourceType'] = f.resourceType;
    if (f.isUnconstitutional) params['isUnconstitutional'] = 'true';

    this.router.navigate(['/jurisprudencia/resultados'], { queryParams: params });
  }
}
