import { Injectable, signal, inject, effect } from '@angular/core';
import { Router } from '@angular/router';
import { RecentSearchesService } from './recent-searches.service';

export interface PaletteCommand {
  id: string;
  label: string;
  section: 'recent' | 'navigation' | 'action' | 'search';
  icon?: string;
  subtitle?: string;
  shortcut?: string;
  action: () => void;
}

@Injectable({ providedIn: 'root' })
export class CommandPaletteService {
  readonly isOpen = signal(false);

  private commands = signal<PaletteCommand[]>([]);
  private customCommands: PaletteCommand[] = [];
  private recentSearches = inject(RecentSearchesService);

  constructor(private router: Router) {
    effect(() => {
      this.recentSearches.searches();
      this.rebuildCommands();
    });
  }

  open() { this.isOpen.set(true); }
  close() { this.isOpen.set(false); }
  toggle() { this.isOpen.update(v => !v); }

  registerCommands(commands: PaletteCommand[]) {
    this.customCommands = [...this.customCommands, ...commands];
    this.rebuildCommands();
  }

  unregisterCommands(ids: string[]) {
    const idSet = new Set(ids);
    this.customCommands = this.customCommands.filter(c => !idSet.has(c.id));
    this.rebuildCommands();
  }

  getCommands(): PaletteCommand[] {
    return this.commands();
  }

  private rebuildCommands() {
    const recent: PaletteCommand[] = this.recentSearches.searches().map((q, i) => ({
      id: `recent:${i}`,
      label: q,
      section: 'recent' as const,
      icon: 'clock',
      action: () => {
        this.close();
        this.router.navigate(['/jurisprudencia/resultados'], { queryParams: { query: q, page: '1' } });
      },
    }));

    const nav: PaletteCommand[] = [
      { id: 'nav:home', label: 'Inicio', section: 'navigation', icon: 'home', shortcut: 'G H', action: () => this.nav('/bienvenida') },
      { id: 'nav:jurisprudencia', label: 'Jurisprudencia', section: 'navigation', icon: 'search', shortcut: 'G J', action: () => this.nav('/jurisprudencia') },
      { id: 'nav:organismos', label: 'Tribunales', section: 'navigation', icon: 'building', shortcut: 'G O', action: () => this.nav('/organismos') },
      { id: 'nav:sujetos', label: 'Intervinientes', section: 'navigation', icon: 'users', shortcut: 'G S', action: () => this.nav('/sujetos') },
      { id: 'nav:vocabulario', label: 'Descriptores', section: 'navigation', icon: 'book', shortcut: 'G V', action: () => this.nav('/vocabulario') },
      { id: 'nav:ordenamiento', label: 'Ordenamiento', section: 'navigation', icon: 'scroll', shortcut: 'G R', action: () => this.nav('/ordenamiento') },
      { id: 'nav:procesos', label: 'Causas', section: 'navigation', icon: 'clipboard', shortcut: 'G P', action: () => this.nav('/procesos') },
      { id: 'nav:asistente', label: 'Asistente', section: 'navigation', icon: 'chat', shortcut: 'G A', action: () => this.nav('/asistente') },
      { id: 'nav:explorador', label: 'Explorador de Relaciones', section: 'navigation', icon: 'graph', shortcut: 'G E', action: () => this.nav('/explorador') },
      { id: 'nav:estadisticas', label: 'Estadísticas', section: 'navigation', icon: 'chart', shortcut: 'G D', action: () => this.nav('/estadisticas') },
      { id: 'nav:ontologia', label: 'Ontología', section: 'navigation', icon: 'layers', shortcut: 'G N', action: () => this.nav('/ontologia') },
      { id: 'nav:admin', label: 'Administración', section: 'navigation', icon: 'settings', action: () => this.nav('/admin') },
    ];

    this.commands.set([...recent, ...nav, ...this.customCommands]);
  }

  private nav(path: string) {
    this.close();
    this.router.navigate([path]);
  }
}
