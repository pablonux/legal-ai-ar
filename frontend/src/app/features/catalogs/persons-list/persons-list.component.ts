import { Component, inject, signal, DestroyRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, combineLatest } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, startWith } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PersonService } from '../../../services/person.service';
import { SkeletonTableRowComponent } from '../../../shared/components/skeletons/skeleton-table-row.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import type { PersonListItem } from '../../../models/catalog.models';

export type PersonVista = 'todos' | 'magistrados' | 'partes';

function vistaFromQuery(v: string | null): PersonVista {
  if (v === 'magistrados' || v === 'partes') return v;
  return 'todos';
}

@Component({
  selector: 'app-persons-list',
  standalone: true,
  imports: [RouterLink, FormsModule, SkeletonTableRowComponent, EmptyStateComponent],
  template: `
    <div class="catalog-list">
      <div class="view-tabs" role="tablist" aria-label="Vista de personas">
        <button
          type="button"
          role="tab"
          class="tab"
          [class.active]="vista() === 'todos'"
          [attr.aria-selected]="vista() === 'todos'"
          (click)="goVista('todos')"
        >
          Todos
        </button>
        <button
          type="button"
          role="tab"
          class="tab"
          [class.active]="vista() === 'magistrados'"
          [attr.aria-selected]="vista() === 'magistrados'"
          (click)="goVista('magistrados')"
        >
          Magistrados
        </button>
        <button
          type="button"
          role="tab"
          class="tab"
          [class.active]="vista() === 'partes'"
          [attr.aria-selected]="vista() === 'partes'"
          (click)="goVista('partes')"
        >
          Partes
        </button>
      </div>

      <p class="view-hint">{{ vistaHint() }}</p>

      <div class="search-row">
        <input
          type="text"
          [(ngModel)]="query"
          (ngModelChange)="onSearch($event)"
          placeholder="Buscar por nombre..."
          class="search-input"
        />
      </div>

      @if (loading()) {
        <app-skeleton-table-row [count]="8" />
      } @else if (persons().length === 0) {
        <app-empty-state
          [title]="emptyTitle()"
          [subtitle]="emptySubtitle()"
          [tips]="['Probá ampliar la búsqueda', 'Los datos se actualizan al indexar fallos y expedientes']"
          [actions]="[{ label: 'Ir a jurisprudencia', route: '/jurisprudencia', primary: true }]">
          <svg esIcon xmlns="http://www.w3.org/2000/svg" width="56" height="56" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>
        </app-empty-state>
      } @else {
        <div class="catalog-table">
          <div class="table-header">
            <span class="col-name">Persona</span>
            <span class="col-meta">Tribunal</span>
            <span class="col-count">{{ countColumnLabel() }}</span>
          </div>
          @for (p of persons(); track p.id) {
            <a [routerLink]="['/sujetos', p.id]" [queryParams]="detailQueryParams()" class="table-row">
              <span class="col-name row-name">{{ p.displayName }}</span>
              <span class="col-meta">{{ p.courtName ?? '—' }}</span>
              <span class="col-count">
                <span class="count-badge">{{ p.rulingCount }}</span>
              </span>
            </a>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .view-tabs {
      display: flex;
      flex-wrap: wrap;
      gap: 0.375rem;
      margin-bottom: 0.5rem;
    }
    .tab {
      padding: 0.5rem 0.875rem;
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      background: var(--color-bg-surface);
      font-size: 0.8125rem;
      font-weight: 500;
      color: var(--color-text-body);
      cursor: pointer;
      transition: border-color 0.15s, color 0.15s, background 0.15s;
    }
    .tab:hover {
      border-color: var(--color-primary);
      color: var(--color-primary);
    }
    .tab.active {
      border-color: var(--color-primary);
      background: var(--color-nav-active-bg);
      color: var(--color-primary);
      font-weight: 600;
    }
    .view-hint {
      font-size: 0.75rem;
      color: var(--color-text-secondary);
      margin: 0 0 1rem 0;
      line-height: 1.4;
    }
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
    .catalog-table { display: flex; flex-direction: column; gap: 0; }
    .table-header, .table-row {
      display: grid;
      grid-template-columns: 1fr 1fr 80px;
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
    .col-meta { color: var(--color-text-secondary); font-size: 0.75rem; }
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
export class PersonsListComponent {
  private personService = inject(PersonService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);
  private searchSubject = new Subject<string>();

  persons = signal<PersonListItem[]>([]);
  loading = signal(true);
  vista = signal<PersonVista>('todos');
  query = '';

  constructor() {
    combineLatest([
      this.route.queryParamMap.pipe(
        map(m => vistaFromQuery(m.get('vista'))),
        distinctUntilChanged()
      ),
      this.searchSubject.pipe(debounceTime(300), distinctUntilChanged(), startWith(''))
    ])
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(([vista, q]) => {
        this.vista.set(vista);
        this.loadList(q, vista);
      });
  }

  goVista(v: PersonVista) {
    void this.router.navigate(['/sujetos'], {
      queryParams: v === 'todos' ? {} : { vista: v },
      replaceUrl: true
    });
  }

  detailQueryParams(): Record<string, string> {
    const v = this.vista();
    return v === 'todos' ? {} : { vista: v };
  }

  vistaHint(): string {
    switch (this.vista()) {
      case 'magistrados':
        return 'Personas que integran el tribunal en fallos (firmas, disidencias, concurrencias).';
      case 'partes':
        return 'Personas registradas como partes en expedientes indexados.';
      default:
        return 'Incluye quienes participaron en fallos (magistrados, fiscales, defensores) y, cuando hay datos, partes procesales.';
    }
  }

  countColumnLabel(): string {
    return this.vista() === 'partes' ? 'Causas' : 'Fallos';
  }

  emptyTitle(): string {
    switch (this.vista()) {
      case 'magistrados':
        return 'Sin magistrados en esta vista';
      case 'partes':
        return 'Sin partes en esta vista';
      default:
        return 'Sin personas encontradas';
    }
  }

  emptySubtitle(): string {
    switch (this.vista()) {
      case 'magistrados':
        return 'Probá otra búsqueda o volvé a «Todos» para ver todo el índice de intervinientes.';
      case 'partes':
        return 'Las partes aparecen cuando el expediente fue vinculado con personas en la base.';
      default:
        return 'Las personas se indexan desde los fallos y los expedientes.';
    }
  }

  onSearch(value: string) {
    this.searchSubject.next(value.trim());
  }

  private loadList(q: string, vista: PersonVista) {
    this.loading.set(true);
    const apiVista = vista === 'todos' ? undefined : vista;
    this.personService.search(q || undefined, undefined, 50, apiVista).subscribe({
      next: list => {
        this.persons.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.persons.set([]);
        this.loading.set(false);
      }
    });
  }
}
