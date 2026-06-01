import { Component, EventEmitter, Output, inject, signal, DestroyRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, switchMap, of, catchError } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { GraphExplorerService } from '../../../services/graph-explorer.service';
import { ENTITY_TYPE_CONFIG, type EntitySearchResult } from '../../../models/graph-explorer.models';

@Component({
  selector: 'app-entity-search-popover',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="search-popover">
      <div class="search-input-wrap">
        <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
        </svg>
        <input type="text" placeholder="Buscar entidad para agregar al grafo..."
               [(ngModel)]="searchText" (ngModelChange)="onSearchChange($event)"
               (focus)="showDropdown.set(true)" class="search-input" />
        @if (searchText) {
          <button class="clear-btn" (click)="clearSearch()">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
          </button>
        }
      </div>

      @if (showDropdown() && (results().length > 0 || searching())) {
        <div class="search-dropdown">
          @if (searching()) {
            <div class="search-status">Buscando...</div>
          }
          @for (group of groupedResults(); track group.type) {
            <div class="result-group">
              <div class="group-header">
                <span class="group-dot" [style.background]="group.color"></span>
                {{ group.label }}
              </div>
              @for (item of group.items; track item.id) {
                <button class="result-item" (click)="selectResult(item)">
                  <span class="result-label">{{ item.label }}</span>
                  @if (item.subtitle) {
                    <span class="result-sub">{{ item.subtitle }}</span>
                  }
                </button>
              }
            </div>
          }
          @if (!searching() && results().length === 0 && searchText.length >= 2) {
            <div class="search-status">Sin resultados</div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .search-popover { position: relative; }
    .search-input-wrap {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 0.5rem 0.75rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      transition: border-color 0.15s;
    }
    .search-input-wrap:focus-within {
      border-color: var(--color-primary);
      box-shadow: 0 0 0 2px rgba(208, 74, 2, 0.1);
    }
    .search-input-wrap svg { color: var(--color-text-secondary); flex-shrink: 0; }
    .search-input {
      flex: 1;
      border: none;
      outline: none;
      font-size: 0.8125rem;
      background: transparent;
      color: var(--color-text);
    }
    .search-input::placeholder { color: var(--color-text-secondary); }
    .clear-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      color: var(--color-text-secondary);
      cursor: pointer;
      padding: 2px;
    }
    .clear-btn:hover { color: var(--color-text); }
    .search-dropdown {
      position: absolute;
      top: calc(100% + 4px);
      left: 0;
      right: 0;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      box-shadow: var(--shadow-md);
      max-height: 320px;
      overflow-y: auto;
      z-index: 50;
    }
    .search-status {
      padding: 0.75rem 1rem;
      font-size: 0.75rem;
      color: var(--color-text-secondary);
      text-align: center;
    }
    .result-group { padding: 0.25rem 0; }
    .group-header {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 0.375rem 0.75rem;
      font-size: 0.6875rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      color: var(--color-text-secondary);
    }
    .group-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      flex-shrink: 0;
    }
    .result-item {
      display: flex;
      flex-direction: column;
      width: 100%;
      padding: 0.5rem 0.75rem 0.5rem 1.75rem;
      text-align: left;
      cursor: pointer;
      transition: background 0.1s;
      border: none;
      background: none;
    }
    .result-item:hover { background: var(--color-bg-subtle); }
    .result-label {
      font-size: 0.8125rem;
      color: var(--color-text);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
    .result-sub {
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  `],
  host: {
    '(document:click)': 'onDocumentClick($event)',
  }
})
export class EntitySearchPopoverComponent {
  @Output() entityChosen = new EventEmitter<EntitySearchResult>();

  private graphService = inject(GraphExplorerService);
  private destroyRef = inject(DestroyRef);
  private searchSubject = new Subject<string>();

  searchText = '';
  results = signal<EntitySearchResult[]>([]);
  searching = signal(false);
  showDropdown = signal(false);

  groupedResults = signal<{ type: string; label: string; color: string; items: EntitySearchResult[] }[]>([]);

  constructor() {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(q => {
        if (q.length < 2) {
          this.searching.set(false);
          return of({ results: [] as EntitySearchResult[] });
        }
        this.searching.set(true);
        return this.graphService.searchEntities(q).pipe(
          catchError(() => of({ results: [] as EntitySearchResult[] }))
        );
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(resp => {
      this.results.set(resp.results);
      this.searching.set(false);
      this.buildGroups(resp.results);
    });
  }

  onSearchChange(value: string) {
    this.showDropdown.set(true);
    this.searchSubject.next(value);
  }

  selectResult(item: EntitySearchResult) {
    this.entityChosen.emit(item);
    this.clearSearch();
  }

  clearSearch() {
    this.searchText = '';
    this.results.set([]);
    this.groupedResults.set([]);
    this.showDropdown.set(false);
  }

  onDocumentClick(event: Event) {
    const el = event.target as HTMLElement;
    if (!el.closest('app-entity-search-popover')) {
      this.showDropdown.set(false);
    }
  }

  private buildGroups(items: EntitySearchResult[]) {
    const map = new Map<string, EntitySearchResult[]>();
    for (const item of items) {
      const list = map.get(item.entityType) ?? [];
      list.push(item);
      map.set(item.entityType, list);
    }

    this.groupedResults.set(
      [...map.entries()].map(([type, groupItems]) => {
        const cfg = ENTITY_TYPE_CONFIG[type as keyof typeof ENTITY_TYPE_CONFIG];
        return {
          type,
          label: cfg?.label ?? type,
          color: cfg?.color ?? '#6b7280',
          items: groupItems,
        };
      })
    );
  }
}
