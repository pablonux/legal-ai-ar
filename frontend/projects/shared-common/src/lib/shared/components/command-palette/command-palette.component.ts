import { Component, ElementRef, HostListener, OnInit, OnDestroy, ViewChild, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, switchMap, of, Subscription, catchError } from 'rxjs';
import Fuse from 'fuse.js';
import { Router } from '@angular/router';
import {
  CommandPaletteService,
  type PaletteCommand
} from '@legal-ai-ar/app/services/command-palette.service';
import { GlobalSearchService } from '@legal-ai-ar/app/services/global-search.service';

@Component({
  selector: 'app-command-palette',
  standalone: true,
  imports: [FormsModule],
  template: `
    @if (paletteService.isOpen()) {
      <div class="palette-backdrop" (click)="paletteService.close()"></div>
      <div class="palette-container" role="dialog" aria-label="Command palette">
        <div class="palette-search">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
            <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
          </svg>
          <input
            #searchInput
            type="text"
            class="palette-input"
            placeholder="Buscar o navegar..."
            [ngModel]="query()"
            (ngModelChange)="onQueryChange($event)"
            (keydown)="onKeydown($event)"
            autocomplete="off"
          />
          <kbd class="palette-esc">Esc</kbd>
        </div>
        <div class="palette-results" role="listbox">
          @for (section of sections(); track section.title) {
            <div class="palette-section">
              <div class="palette-section-title">{{ section.title }}</div>
              @for (item of section.items; track item.id; let i = $index) {
                <button
                  type="button"
                  class="palette-item"
                  [class.highlighted]="flatIndex(section, i) === highlightIndex()"
                  (mouseenter)="highlightIndex.set(flatIndex(section, i))"
                  (click)="executeCommand(item)"
                  role="option"
                >
                  <span class="palette-item-icon">{{ iconFor(item.icon) }}</span>
                  <span class="palette-item-label">
                    {{ item.label }}
                    @if (item.subtitle) {
                      <span class="palette-item-subtitle">{{ item.subtitle }}</span>
                    }
                  </span>
                  @if (item.shortcut) {
                    <span class="palette-item-shortcut">{{ item.shortcut }}</span>
                  }
                </button>
              }
            </div>
          }
          @if (sections().length === 0) {
            <div class="palette-empty">Sin resultados para "{{ query() }}"</div>
          }
        </div>
      </div>
    }
  `,
  styles: [`
    .palette-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(0, 0, 0, 0.4);
      z-index: 9998;
      backdrop-filter: blur(2px);
    }

    .palette-container {
      position: fixed;
      top: 15vh;
      left: 50%;
      transform: translateX(-50%);
      width: 560px;
      max-width: calc(100vw - 2rem);
      max-height: 60vh;
      background: var(--color-bg-surface, #fff);
      border: 1px solid var(--color-border, #e5e7eb);
      border-radius: 12px;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.2), 0 0 0 1px rgba(0, 0, 0, 0.05);
      z-index: 9999;
      display: flex;
      flex-direction: column;
      overflow: hidden;
      animation: palette-in 120ms ease-out;
    }

    @keyframes palette-in {
      from { opacity: 0; transform: translateX(-50%) scale(0.97) translateY(-8px); }
      to { opacity: 1; transform: translateX(-50%) scale(1) translateY(0); }
    }

    .palette-search {
      display: flex;
      align-items: center;
      gap: 0.625rem;
      padding: 0.875rem 1rem;
      border-bottom: 1px solid var(--color-border, #e5e7eb);
    }

    .palette-search svg {
      color: var(--color-text-secondary, #6b7280);
      flex-shrink: 0;
    }

    .palette-input {
      flex: 1;
      border: none;
      outline: none;
      background: transparent;
      font-size: 0.9375rem;
      font-family: inherit;
      color: var(--color-text, #111);
    }

    .palette-input::placeholder {
      color: var(--color-text-secondary, #9ca3af);
    }

    .palette-esc {
      font-size: 0.625rem;
      font-weight: 600;
      padding: 2px 6px;
      border: 1px solid var(--color-border, #e5e7eb);
      border-radius: 4px;
      background: var(--color-bg-subtle, #f9fafb);
      color: var(--color-text-secondary, #6b7280);
      font-family: inherit;
    }

    .palette-results {
      overflow-y: auto;
      padding: 0.375rem 0;
    }

    .palette-section {
      padding: 0.25rem 0;
    }

    .palette-section-title {
      font-size: 0.625rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: var(--color-text-secondary, #6b7280);
      padding: 0.375rem 1rem 0.25rem;
    }

    .palette-item {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      width: 100%;
      padding: 0.5rem 1rem;
      border: none;
      background: transparent;
      font-size: 0.875rem;
      color: var(--color-text, #111);
      cursor: pointer;
      text-align: left;
      font-family: inherit;
      transition: background 80ms;
    }

    .palette-item:hover,
    .palette-item.highlighted {
      background: var(--color-bg-subtle, #f3f4f6);
    }

    .palette-item-icon {
      width: 24px;
      text-align: center;
      font-size: 1rem;
      flex-shrink: 0;
    }

    .palette-item-label {
      flex: 1;
      min-width: 0;
      display: flex;
      flex-direction: column;
    }
    .palette-item-subtitle {
      font-size: 0.6875rem;
      color: var(--color-text-secondary, #9ca3af);
      margin-top: 1px;
    }

    .palette-item-shortcut {
      font-size: 0.6875rem;
      font-family: var(--font-mono, ui-monospace, monospace);
      color: var(--color-text-secondary, #6b7280);
      background: var(--color-bg-subtle, #f3f4f6);
      padding: 1px 5px;
      border-radius: 3px;
      flex-shrink: 0;
    }

    .palette-empty {
      padding: 2rem 1rem;
      text-align: center;
      font-size: 0.8125rem;
      color: var(--color-text-secondary, #9ca3af);
    }
  `]
})
export class CommandPaletteComponent implements OnInit, OnDestroy {
  paletteService = inject(CommandPaletteService);
  private globalSearch = inject(GlobalSearchService);
  private router = inject(Router);

  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;

  query = signal('');
  highlightIndex = signal(0);
  searchResults = signal<PaletteCommand[]>([]);

  private fuse!: Fuse<PaletteCommand>;
  private searchSubject = new Subject<string>();
  private searchSub?: Subscription;

  private static iconMap: Record<string, string> = {
    home: '🏠', search: '🔍', building: '🏛️', users: '👥', book: '📚',
    chat: '💬', graph: '🔗', chart: '📊', layers: '◇', settings: '⚙️',
    clock: '🕐',
    scroll: '📜',
    clipboard: '📋',
    ruling: '⚖️', court: '🏛️', person: '👤', statute: '📜',
    proceeding: '📋', thesaurus: '📚',
  };

  filteredCommands = computed(() => {
    const q = this.query().trim();
    const base = q ? this.fuse.search(q).map(r => r.item) : this.paletteService.getCommands();
    return [...this.searchResults(), ...base];
  });

  sections = computed(() => {
    const cmds = this.filteredCommands();
    const groups: Record<string, { title: string; items: PaletteCommand[] }> = {};
    const order = ['search', 'recent', 'navigation', 'action'];
    const titles: Record<string, string> = {
      search: 'Resultados',
      recent: 'Recientes',
      navigation: 'Navegación',
      action: 'Acciones',
    };

    for (const cmd of cmds) {
      if (!groups[cmd.section]) {
        groups[cmd.section] = { title: titles[cmd.section] ?? cmd.section, items: [] };
      }
      groups[cmd.section].items.push(cmd);
    }

    return order.filter(s => groups[s]).map(s => groups[s]);
  });

  ngOnInit() {
    this.fuse = new Fuse(this.paletteService.getCommands(), {
      keys: ['label'],
      threshold: 0.4,
      distance: 100,
    });

    this.searchSub = this.searchSubject.pipe(
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(q => q.length >= 2
        ? this.globalSearch.search(q, 3).pipe(catchError(() => of({ items: [], totalCount: 0 })))
        : of({ items: [], totalCount: 0 })),
    ).subscribe(result => {
      const cmds: PaletteCommand[] = result.items.map(item => ({
        id: `search:${item.entityType}:${item.id}`,
        label: item.title,
        subtitle: item.subtitle ?? undefined,
        section: 'search' as const,
        icon: item.entityType,
        action: () => {
          this.paletteService.close();
          this.router.navigateByUrl(item.route);
        },
      }));
      this.searchResults.set(cmds);
    });
  }

  ngOnDestroy() {
    this.searchSub?.unsubscribe();
  }

  onQueryChange(value: string) {
    this.query.set(value);
    this.highlightIndex.set(0);
    this.fuse.setCollection(this.paletteService.getCommands());
    this.searchSubject.next(value.trim());
  }

  onKeydown(e: KeyboardEvent) {
    const items = this.filteredCommands();

    if (e.key === 'Escape') {
      e.preventDefault();
      this.paletteService.close();
      return;
    }

    if (e.key === 'ArrowDown') {
      e.preventDefault();
      this.highlightIndex.update(i => Math.min(i + 1, items.length - 1));
      return;
    }

    if (e.key === 'ArrowUp') {
      e.preventDefault();
      this.highlightIndex.update(i => Math.max(i - 1, 0));
      return;
    }

    if (e.key === 'Enter') {
      e.preventDefault();
      const idx = this.highlightIndex();
      if (items[idx]) {
        this.executeCommand(items[idx]);
      }
      return;
    }
  }

  executeCommand(cmd: PaletteCommand) {
    this.query.set('');
    this.highlightIndex.set(0);
    this.searchResults.set([]);
    cmd.action();
  }

  flatIndex(section: { items: PaletteCommand[] }, itemIndex: number): number {
    const allSections = this.sections();
    let offset = 0;
    for (const s of allSections) {
      if (s === section) return offset + itemIndex;
      offset += s.items.length;
    }
    return offset + itemIndex;
  }

  iconFor(icon?: string): string {
    return icon ? (CommandPaletteComponent.iconMap[icon] ?? '•') : '•';
  }

  @HostListener('document:keydown', ['$event'])
  onGlobalKeydown(e: KeyboardEvent) {
    if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
      e.preventDefault();
      this.paletteService.toggle();
    }
    if (e.key === 'Escape' && this.paletteService.isOpen()) {
      e.preventDefault();
      this.paletteService.close();
    }
  }
}
