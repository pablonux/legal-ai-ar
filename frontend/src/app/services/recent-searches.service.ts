import { Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'legalai:recent-searches';
const MAX_ITEMS = 10;

@Injectable({ providedIn: 'root' })
export class RecentSearchesService {
  readonly searches = signal<string[]>(this.load());

  add(query: string) {
    const q = query.trim();
    if (!q) return;
    const current = this.searches().filter(s => s !== q);
    const updated = [q, ...current].slice(0, MAX_ITEMS);
    this.searches.set(updated);
    this.persist(updated);
  }

  getAll(): string[] {
    return this.searches();
  }

  clear() {
    this.searches.set([]);
    this.persist([]);
  }

  private load(): string[] {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) return [];
      const parsed = JSON.parse(raw);
      return Array.isArray(parsed) ? parsed.slice(0, MAX_ITEMS) : [];
    } catch {
      return [];
    }
  }

  private persist(items: string[]) {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
    } catch { /* quota exceeded — silently ignore */ }
  }
}
