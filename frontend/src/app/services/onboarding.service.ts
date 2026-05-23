import { Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'legal-ai-ar:onboarding';

export type OnboardingStep =
  | 'welcome-palette'
  | 'search-filters'
  | 'chat-citations'
  | 'explorer-controls';

export interface OnboardingTip {
  step: OnboardingStep;
  title: string;
  body: string;
  shortcut?: string;
}

const TIPS: Record<OnboardingStep, OnboardingTip> = {
  'welcome-palette': {
    step: 'welcome-palette',
    title: 'Command Palette',
    body: 'Presioná Ctrl+K en cualquier momento para buscar, navegar y ejecutar acciones rápidamente.',
    shortcut: 'Ctrl+K',
  },
  'search-filters': {
    step: 'search-filters',
    title: 'Filtros avanzados',
    body: 'Usá los filtros laterales para refinar por tribunal, fecha, rama del derecho y más. Presioná / para enfocar la búsqueda.',
    shortcut: '/',
  },
  'chat-citations': {
    step: 'chat-citations',
    title: 'Citas verificables',
    body: 'Las respuestas del asistente incluyen citas numeradas [1][2] que enlazan directamente a los fallos de la KB. Cada fuente es verificable.',
  },
  'explorer-controls': {
    step: 'explorer-controls',
    title: 'Explorador de Grafo',
    body: 'Usá scroll para zoom, arrastrá para mover el grafo. Podés filtrar las capas de entidades desde el panel lateral.',
  },
};

@Injectable({ providedIn: 'root' })
export class OnboardingService {
  private completed = new Set<OnboardingStep>();
  activeTip = signal<OnboardingTip | null>(null);

  constructor() {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (raw) {
        const arr = JSON.parse(raw) as OnboardingStep[];
        arr.forEach(s => this.completed.add(s));
      }
    } catch { /* ignore corrupt data */ }
  }

  tryShow(step: OnboardingStep): boolean {
    if (this.completed.has(step)) return false;
    this.activeTip.set(TIPS[step]);
    return true;
  }

  dismiss() {
    const tip = this.activeTip();
    if (tip) {
      this.completed.add(tip.step);
      this.persist();
    }
    this.activeTip.set(null);
  }

  isCompleted(step: OnboardingStep): boolean {
    return this.completed.has(step);
  }

  resetAll() {
    this.completed.clear();
    this.persist();
    this.activeTip.set(null);
  }

  private persist() {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify([...this.completed]));
    } catch { /* quota exceeded */ }
  }
}
