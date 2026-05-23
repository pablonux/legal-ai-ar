import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { StatuteService } from '../../../services/statute.service';
import { SkeletonTableRowComponent } from '../../../shared/components/skeletons/skeleton-table-row.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import type { StatuteListItem } from '../../../models/statute.models';

const NORM_TYPE_LABELS: Record<string, string> = {
  CONSTITUTION: 'Constitución', TREATY: 'Tratado', LAW: 'Ley', DECREE: 'Decreto',
  DNU: 'DNU', RESOLUTION: 'Resolución', ACORDADA: 'Acordada', ORDINANCE: 'Ordenanza',
};

const LEVEL_LABELS: Record<string, string> = {
  CONSTITUTIONAL: 'Constitucional', SUPRALEGAL: 'Supralegal', LEGAL: 'Legal',
  REGULATORY: 'Reglamentario', INDIVIDUAL: 'Individual',
};

@Component({
  selector: 'app-statute-list',
  standalone: true,
  imports: [RouterLink, FormsModule, SkeletonTableRowComponent, EmptyStateComponent],
  template: `
    <div class="statute-list">
      <div class="list-header">
        <h1 class="page-title">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" aria-hidden="true"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/></svg>
          Ordenamiento Jurídico
        </h1>
        <a routerLink="/ordenamiento/piramide" class="pyramid-link">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true"><polygon points="12 2 22 22 2 22"/></svg>
          Ver pirámide normativa
        </a>
      </div>

      <div class="filters-row">
        <input type="text" [(ngModel)]="query" (ngModelChange)="onSearch($event)" placeholder="Buscar norma por nombre o número..." class="search-input" />
        <select [(ngModel)]="normTypeFilter" (ngModelChange)="reload()" class="filter-select">
          <option value="">Tipo</option>
          <option value="LAW">Ley</option>
          <option value="DECREE">Decreto</option>
          <option value="DNU">DNU</option>
          <option value="RESOLUTION">Resolución</option>
          <option value="TREATY">Tratado</option>
          <option value="CONSTITUTION">Constitución</option>
          <option value="ACORDADA">Acordada</option>
          <option value="ORDINANCE">Ordenanza</option>
        </select>
      </div>

      @if (loading()) {
        <app-skeleton-table-row [count]="10" />
      } @else if (statutes().length === 0) {
        <app-empty-state
          title="Sin normas encontradas"
          subtitle="El ordenamiento jurídico aún no tiene normas indexadas con estos criterios."
          variant="space"
          [tips]="['Probá ampliar la búsqueda o quitar filtros', 'Las normas se indexan a medida que se procesan fallos de la CSJN', 'Podés explorar la pirámide normativa para ver qué niveles están disponibles']"
          [actions]="[{ label: 'Ver pirámide normativa', route: '/ordenamiento/piramide', primary: true }, { label: 'Ir a jurisprudencia', route: '/jurisprudencia' }]">
          <svg esIcon xmlns="http://www.w3.org/2000/svg" width="56" height="56" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>
        </app-empty-state>
      } @else {
        <div class="statute-table">
          <div class="table-header">
            <span class="col-number">Número</span>
            <span class="col-name">Nombre</span>
            <span class="col-type">Tipo</span>
            <span class="col-level">Nivel</span>
            <span class="col-status">Estado</span>
            <span class="col-count">Fallos</span>
          </div>
          @for (s of statutes(); track s.id) {
            <a [routerLink]="['/ordenamiento', s.id]" class="table-row hover-lift">
              <span class="col-number">{{ s.number || '—' }}</span>
              <span class="col-name row-name">{{ s.name }}</span>
              <span class="col-type">
                @if (s.normType) {
                  <span class="type-badge">{{ normTypeLabel(s.normType) }}</span>
                } @else { — }
              </span>
              <span class="col-level">{{ s.normativeLevel ? levelLabel(s.normativeLevel) : '—' }}</span>
              <span class="col-status">
                <span class="vigencia-dot" [class.vigente]="s.isVigente" [class.no-vigente]="!s.isVigente"></span>
                {{ s.isVigente ? 'Vigente' : 'No vigente' }}
              </span>
              <span class="col-count">
                <span class="count-badge">{{ s.rulingCount }}</span>
              </span>
            </a>
          }
        </div>

        @if (totalCount() > statutes().length) {
          <div class="pagination-info">
            Mostrando {{ statutes().length }} de {{ totalCount() }} normas
          </div>
        }
      }
    </div>
  `,
  styles: [`
    .statute-list { max-width: 1100px; margin: 0 auto; }

    .list-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 1rem;
      flex-wrap: wrap;
      gap: 0.5rem;
    }

    .page-title {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 1.375rem;
      font-weight: 600;
      margin: 0;
      color: var(--color-text);
    }

    .page-title svg { color: var(--color-primary); }

    .pyramid-link {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      font-size: 0.8125rem;
      font-weight: 500;
      color: var(--color-primary);
      text-decoration: none;
      padding: 0.375rem 0.75rem;
      border: 1px solid var(--color-primary);
      border-radius: var(--radius-pill);
      transition: background 0.15s;
    }

    .pyramid-link:hover {
      background: rgba(208, 74, 2, 0.06);
    }

    .filters-row {
      display: flex;
      gap: 0.75rem;
      margin-bottom: 1rem;
      flex-wrap: wrap;
    }

    .search-input {
      flex: 1;
      min-width: 200px;
      padding: 0.5rem 0.75rem;
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      font-size: 0.875rem;
      font-family: inherit;
      transition: border-color 0.15s;
    }

    .search-input:focus {
      outline: none;
      border-color: var(--color-primary);
      box-shadow: 0 0 0 2px rgba(208, 74, 2, 0.08);
    }

    .filter-select {
      padding: 0.5rem 0.75rem;
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      font-size: 0.8125rem;
      font-family: inherit;
      background: var(--color-bg-surface);
      cursor: pointer;
    }

    .empty-state {
      text-align: center;
      padding: 3rem;
      color: var(--color-text-secondary);
    }

    .statute-table { display: flex; flex-direction: column; }

    .table-header, .table-row {
      display: grid;
      grid-template-columns: 100px 1fr 100px 100px 100px 70px;
      align-items: center;
      padding: 0.625rem 1rem;
      gap: 0.5rem;
    }

    .table-header {
      font-size: 0.6875rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      color: var(--color-text-secondary);
      border-bottom: 1px solid var(--color-border);
    }

    .table-row {
      text-decoration: none;
      color: var(--color-text);
      border-bottom: 1px solid var(--color-border);
      transition: background 0.1s;
      font-size: 0.8125rem;
    }

    .table-row:hover { background: var(--color-bg-subtle); }
    .row-name { font-weight: 500; }
    .col-number { font-family: var(--font-mono, monospace); font-size: 0.75rem; color: var(--color-text-secondary); }
    .col-type, .col-level, .col-status { font-size: 0.75rem; color: var(--color-text-secondary); }
    .col-count { text-align: right; }

    .type-badge {
      display: inline-block;
      padding: 1px 6px;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-xs);
      font-size: 0.6875rem;
      font-weight: 600;
    }

    .vigencia-dot {
      display: inline-block;
      width: 6px;
      height: 6px;
      border-radius: 50%;
      margin-right: 3px;
    }

    .vigencia-dot.vigente { background: #16a34a; }
    .vigencia-dot.no-vigente { background: #dc2626; }

    .count-badge {
      display: inline-block;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-pill);
      padding: 2px 8px;
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-text-secondary);
    }

    .pagination-info {
      text-align: center;
      padding: 1rem;
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
    }
  `]
})
export class StatuteListComponent implements OnInit {
  private statuteService = inject(StatuteService);
  private searchSubject = new Subject<string>();

  statutes = signal<StatuteListItem[]>([]);
  totalCount = signal(0);
  loading = signal(true);
  query = '';
  normTypeFilter = '';

  constructor() {
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => this.reload());
  }

  ngOnInit() { this.reload(); }

  onSearch(value: string) { this.searchSubject.next(value.trim()); }

  reload() {
    this.loading.set(true);
    this.statuteService.search({
      q: this.query.trim() || undefined,
      normType: this.normTypeFilter || undefined,
      page: 1,
      pageSize: 50,
    }).subscribe({
      next: res => {
        this.statutes.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.statutes.set([]);
        this.totalCount.set(0);
        this.loading.set(false);
      },
    });
  }

  normTypeLabel(type: string): string { return NORM_TYPE_LABELS[type] ?? type; }
  levelLabel(level: string): string { return LEVEL_LABELS[level] ?? level; }
}
