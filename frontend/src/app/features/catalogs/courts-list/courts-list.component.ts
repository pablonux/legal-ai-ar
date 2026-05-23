import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { CourtService } from '../../../services/court.service';
import { SkeletonTableRowComponent } from '../../../shared/components/skeletons/skeleton-table-row.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import type { CourtListItem } from '../../../models/catalog.models';

@Component({
  selector: 'app-courts-list',
  standalone: true,
  imports: [RouterLink, FormsModule, SkeletonTableRowComponent, EmptyStateComponent],
  template: `
    <div class="catalog-list">
      <div class="search-row">
        <input type="text" [(ngModel)]="query" (ngModelChange)="onSearch($event)" placeholder="Buscar tribunal..." class="search-input" />
      </div>

      @if (loading()) {
        <app-skeleton-table-row [count]="8" />
      } @else if (courts().length === 0) {
        <app-empty-state
          title="Sin organismos encontrados"
          subtitle="Los tribunales y organismos judiciales se indexan a medida que se procesan fallos."
          [tips]="['Probá ampliar los criterios de búsqueda', 'Los organismos se crean automáticamente al indexar jurisprudencia']"
          [actions]="[{ label: 'Explorar jurisprudencia', route: '/jurisprudencia', primary: true }]">
          <svg esIcon xmlns="http://www.w3.org/2000/svg" width="56" height="56" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1"><path d="M3 21h18"/><path d="M5 21V7l8-4v18"/><path d="M19 21V11l-6-4"/></svg>
        </app-empty-state>
      } @else {
        <div class="catalog-table">
          <div class="table-header">
            <span class="col-name">Tribunal</span>
            <span class="col-meta">Jurisdicción</span>
            <span class="col-meta">Instancia</span>
            <span class="col-meta">Categoría</span>
            <span class="col-meta">Fuero</span>
            <span class="col-meta col-narrow">Nivel</span>
            <span class="col-meta">Gobierno</span>
            <span class="col-count">Fallos</span>
          </div>
          @for (c of courts(); track c.id) {
            <a [routerLink]="['/organismos', c.id]" class="table-row">
              <span class="col-name row-name">{{ c.name }}</span>
              <span class="col-meta">{{ c.jurisdictionArea }}</span>
              <span class="col-meta">{{ c.instance }}</span>
              <span class="col-meta">{{ c.courtCategory ?? '—' }}</span>
              <span class="col-meta">{{ c.fuero ?? '—' }}</span>
              <span class="col-meta col-narrow">{{ c.instanceLevel != null ? c.instanceLevel : '—' }}</span>
              <span class="col-meta">{{ c.governmentLevel ?? '—' }}</span>
              <span class="col-count">
                <span class="count-badge">{{ c.rulingCount }}</span>
              </span>
            </a>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .search-row { margin-bottom: 1rem; }
    .search-input {
      width: 100%;
      max-width: 400px;
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
    .empty-state {
      text-align: center;
      padding: 3rem;
      color: var(--color-text-secondary);
    }
    .catalog-table { display: flex; flex-direction: column; gap: 0; }
    .table-header, .table-row {
      display: grid;
      grid-template-columns: 1fr 120px 100px 100px 90px 52px 100px 76px;
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
    .col-meta { color: var(--color-text-secondary); font-size: 0.75rem; min-width: 0; }
    .col-narrow { text-align: center; }
    .col-count { text-align: right; }
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
  `]
})
export class CourtsListComponent implements OnInit {
  private courtService = inject(CourtService);
  private searchSubject = new Subject<string>();

  courts = signal<CourtListItem[]>([]);
  loading = signal(true);
  query = '';

  constructor() {
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(q => this.load(q));
  }

  ngOnInit() { this.load(''); }

  onSearch(value: string) { this.searchSubject.next(value.trim()); }

  private load(query: string) {
    this.loading.set(true);
    this.courtService.search(query || undefined).subscribe({
      next: list => { this.courts.set(list); this.loading.set(false); },
      error: () => { this.courts.set([]); this.loading.set(false); }
    });
  }
}
