import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, filter } from 'rxjs';
import { ThesaurusService } from '../../../services/thesaurus.service';
import { LoadingSpinnerComponent } from '@legal-ai-ar/shared-common/components/loading-spinner/loading-spinner.component';
import type { ThesaurusTerm } from '../../../models/thesaurus.models';

interface TreeNode {
  term: ThesaurusTerm;
  expanded: boolean;
  children: TreeNode[] | null;
  loading: boolean;
}

interface FlatRow {
  node: TreeNode;
  depth: number;
  hasChildren: boolean;
}

@Component({
  selector: 'app-thesaurus-list',
  standalone: true,
  imports: [RouterLink, FormsModule, LoadingSpinnerComponent],
  template: `
    <div class="catalog-list">
      <div class="search-wrapper">
        <div class="search-row">
          <input type="text" [(ngModel)]="query" (ngModelChange)="onSearch($event)"
                 placeholder="Buscar descriptor del tesauro..." class="search-input" />
          @if (query.trim().length >= 2) {
            <button type="button" class="clear-search" (click)="clearSearch()" title="Limpiar">&times;</button>
          }
        </div>
        <span class="hint">Ingrese al menos 2 caracteres para buscar, o explore el árbol debajo</span>
      </div>

      @if (searchLoading()) {
        <app-loading-spinner message="Buscando descriptores..." />
      } @else if (searchMode && searchResults().length === 0) {
        <div class="empty-state">
          <p>No se encontraron descriptores para "{{ lastQuery }}".</p>
        </div>
      } @else if (searchMode && searchResults().length > 0) {
        <div class="catalog-table">
          <div class="table-header">
            <span class="col-name">Descriptor</span>
            <span class="col-meta">Rama</span>
            <span class="col-depth">Nivel</span>
          </div>
          @for (t of searchResults(); track t.id) {
            <a [routerLink]="['/vocabulario', t.id]" class="table-row">
              <span class="col-name row-name">{{ t.label }}</span>
              <span class="col-meta">{{ t.branch ?? '—' }}</span>
              <span class="col-depth">
                <span class="depth-badge">{{ t.depth }}</span>
              </span>
            </a>
          }
        </div>
      } @else {
        @if (treeLoading()) {
          <app-loading-spinner message="Cargando ramas del tesauro..." />
        } @else if (rows().length === 0) {
          <div class="empty-state">
            <p>No se encontraron términos raíz.</p>
          </div>
        } @else {
          <div class="tree">
            @for (row of rows(); track row.node.term.id) {
              <div class="tree-row" [style.padding-left.px]="12 + row.depth * 24">
                @if (row.hasChildren) {
                  <button type="button" class="tree-toggle"
                          [class.expanded]="row.node.expanded"
                          (click)="toggleNode(row.node)"
                          [attr.aria-label]="row.node.expanded ? 'Colapsar' : 'Expandir'">
                    <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                      <polyline points="9 18 15 12 9 6"/>
                    </svg>
                  </button>
                } @else {
                  <span class="tree-leaf-dot"></span>
                }
                <a [routerLink]="['/vocabulario', row.node.term.id]" class="tree-label">
                  {{ row.node.term.label }}
                </a>
                @if (row.depth === 0 && row.node.term.branch) {
                  <span class="tree-branch">{{ row.node.term.branch }}</span>
                }
                <span class="tree-depth">{{ row.node.term.depth }}</span>
              </div>
              @if (row.node.loading) {
                <div class="tree-loading" [style.padding-left.px]="36 + row.depth * 24">
                  Cargando...
                </div>
              }
            }
          </div>
        }
      }
    </div>
  `,
  styles: [`
    .search-wrapper { margin-bottom: 1.5rem; }
    .search-row { position: relative; display: inline-block; }
    .search-input {
      width: 400px;
      max-width: 100%;
      padding: 0.5rem 2rem 0.5rem 0.75rem;
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
    .clear-search {
      position: absolute;
      right: 8px;
      top: 50%;
      transform: translateY(-50%);
      background: none;
      border: none;
      font-size: 1.125rem;
      line-height: 1;
      color: var(--color-text-secondary);
      cursor: pointer;
      padding: 2px 4px;
    }
    .clear-search:hover { color: var(--color-primary); }
    .hint {
      display: block;
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      margin-top: 4px;
    }
    .empty-state {
      text-align: center;
      padding: 3rem;
      color: var(--color-text-secondary);
    }

    /* ── Search results table ── */
    .catalog-table { display: flex; flex-direction: column; }
    .table-header, .table-row {
      display: grid;
      grid-template-columns: 1fr 200px 60px;
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
    .col-depth { text-align: right; }
    .depth-badge {
      display: inline-block;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-pill);
      padding: 2px 8px;
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-text-secondary);
    }

    /* ── Tree ── */
    .tree {
      border: 1px solid var(--color-border);
      border-radius: 8px;
      overflow: hidden;
    }
    .tree-row {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 0.5rem 0.75rem;
      border-bottom: 1px solid var(--color-border);
      transition: background 0.1s;
    }
    .tree-row:last-child { border-bottom: none; }
    .tree-row:hover { background: var(--color-bg-subtle); }
    .tree-toggle {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 22px;
      height: 22px;
      border: none;
      background: none;
      cursor: pointer;
      color: var(--color-text-secondary);
      padding: 0;
      border-radius: 4px;
      flex-shrink: 0;
      transition: background 0.1s, color 0.1s;
    }
    .tree-toggle:hover {
      background: rgba(208, 74, 2, 0.08);
      color: var(--color-primary);
    }
    .tree-toggle svg { transition: transform 0.15s ease; }
    .tree-toggle.expanded svg { transform: rotate(90deg); }
    .tree-leaf-dot {
      width: 6px;
      height: 6px;
      border-radius: 50%;
      background: var(--color-border);
      flex-shrink: 0;
      margin: 0 8px;
    }
    .tree-label {
      flex: 1;
      font-size: 0.8125rem;
      color: var(--color-text);
      text-decoration: none;
      font-weight: 500;
      min-width: 0;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
    .tree-label:hover { color: var(--color-primary); }
    .tree-branch {
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      flex-shrink: 0;
    }
    .tree-depth {
      display: inline-block;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-pill);
      padding: 1px 6px;
      font-size: 0.625rem;
      font-weight: 600;
      color: var(--color-text-secondary);
      flex-shrink: 0;
    }
    .tree-loading {
      padding: 0.375rem 0.75rem;
      font-size: 0.75rem;
      color: var(--color-text-secondary);
      font-style: italic;
      border-bottom: 1px solid var(--color-border);
    }
  `]
})
export class ThesaurusListComponent implements OnInit {
  private thesaurusService = inject(ThesaurusService);
  private searchSubject = new Subject<string>();

  private treeVersion = signal(0);
  tree = signal<TreeNode[]>([]);
  treeLoading = signal(true);

  searchResults = signal<ThesaurusTerm[]>([]);
  searchLoading = signal(false);
  searchMode = false;
  query = '';
  lastQuery = '';

  rows = computed<FlatRow[]>(() => {
    this.treeVersion();
    return this.flatten(this.tree());
  });

  constructor() {
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged(), filter(q => q.length >= 2))
      .subscribe(q => this.runSearch(q));
  }

  ngOnInit() {
    this.loadRoots();
  }

  onSearch(value: string) {
    const trimmed = value.trim();
    if (trimmed.length < 2) {
      this.searchResults.set([]);
      this.searchMode = false;
      return;
    }
    this.searchSubject.next(trimmed);
  }

  clearSearch() {
    this.query = '';
    this.searchResults.set([]);
    this.searchMode = false;
    this.searchLoading.set(false);
  }

  toggleNode(node: TreeNode) {
    if (node.expanded) {
      node.expanded = false;
      this.bumpVersion();
      return;
    }

    if (node.children !== null) {
      node.expanded = true;
      this.bumpVersion();
      return;
    }

    node.loading = true;
    node.expanded = true;
    this.bumpVersion();

    this.thesaurusService.getChildren(node.term.id).subscribe({
      next: terms => {
        node.children = terms.map(t => ({
          term: t,
          expanded: false,
          children: null,
          loading: false
        }));
        node.loading = false;
        this.bumpVersion();
      },
      error: () => {
        node.children = [];
        node.loading = false;
        this.bumpVersion();
      }
    });
  }

  private bumpVersion() {
    this.treeVersion.update(v => v + 1);
  }

  private flatten(nodes: TreeNode[]): FlatRow[] {
    const result: FlatRow[] = [];
    const walk = (list: TreeNode[], depth: number) => {
      for (const node of list) {
        const hasChildren = node.children === null || node.children.length > 0;
        result.push({ node, depth, hasChildren });
        if (node.expanded && node.children) {
          walk(node.children, depth + 1);
        }
      }
    };
    walk(nodes, 0);
    return result;
  }

  private loadRoots() {
    this.treeLoading.set(true);
    this.thesaurusService.getRoots().subscribe({
      next: terms => {
        this.tree.set(terms.map(t => ({
          term: t,
          expanded: false,
          children: null,
          loading: false
        })));
        this.treeLoading.set(false);
      },
      error: () => {
        this.tree.set([]);
        this.treeLoading.set(false);
      }
    });
  }

  private runSearch(query: string) {
    this.searchLoading.set(true);
    this.searchMode = true;
    this.lastQuery = query;
    this.thesaurusService.search(query, 50).subscribe({
      next: list => { this.searchResults.set(list); this.searchLoading.set(false); },
      error: () => { this.searchResults.set([]); this.searchLoading.set(false); }
    });
  }
}
