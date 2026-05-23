import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { OntologyService } from '../../../services/ontology.service';
import type { TaxonomyResponse, TaxonomyValue } from '../../../models/ontology.models';

interface TaxonomySection {
  id: string;
  name: string;
  description: string;
  expanded: boolean;
  loading: boolean;
  values: TaxonomyValue[];
}

const TAXONOMY_IDS = [
  'LegalBranch', 'NormType', 'NormativeLevel', 'CourtType',
  'PrecedentWeight', 'Fuero', 'GovernmentLevel',
];

@Component({
  selector: 'app-taxonomy-browser',
  standalone: true,
  template: `
    <div class="taxonomy-browser">
      @for (section of sections(); track section.id) {
        <div class="taxonomy-section" [class.expanded]="section.expanded">
          <button type="button" class="section-header" (click)="toggle(section)"
                  [attr.aria-expanded]="section.expanded">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24"
                 fill="none" stroke="currentColor" stroke-width="2" class="chevron"
                 [class.rotated]="section.expanded">
              <polyline points="9 18 15 12 9 6"/>
            </svg>
            <span class="section-name">{{ section.name }}</span>
            @if (section.values.length > 0) {
              <span class="section-count">{{ section.values.length }}</span>
            }
          </button>

          @if (section.expanded) {
            @if (section.loading) {
              <div class="section-body loading">Cargando...</div>
            } @else {
              <div class="section-body">
                <p class="section-desc">{{ section.description }}</p>
                @for (group of groups(section.values); track group.name) {
                  @if (group.name) {
                    <div class="group-label">{{ group.name }}</div>
                  }
                  @for (val of group.items; track val.code) {
                    <div class="value-row" (click)="navigateToSearch(section.id, val.code)">
                      <div class="value-info">
                        <code class="value-code">{{ val.code }}</code>
                        <span class="value-label">{{ val.label }}</span>
                      </div>
                      <div class="value-bar-area">
                        <div class="value-bar-track">
                          <div class="value-bar-fill"
                               [style.width.%]="barPct(val.count, maxCount(section.values))"></div>
                        </div>
                        <span class="value-count">{{ val.count }}</span>
                      </div>
                    </div>
                  }
                }
              </div>
            }
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .taxonomy-browser { display: flex; flex-direction: column; gap: 2px; }
    .taxonomy-section {
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      overflow: hidden;
    }
    .section-header {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      width: 100%;
      padding: 0.625rem 0.75rem;
      background: var(--color-bg-surface);
      border: none;
      cursor: pointer;
      text-align: left;
      font-size: 0.8125rem;
      font-weight: 600;
      color: var(--color-text);
      transition: background var(--transition-fast);
    }
    .section-header:hover { background: var(--color-bg-subtle); }
    .chevron { transition: transform 0.2s; flex-shrink: 0; color: var(--color-text-secondary); }
    .chevron.rotated { transform: rotate(90deg); }
    .section-name { flex: 1; }
    .section-count {
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      background: var(--color-bg-subtle);
      padding: 0.0625rem 0.375rem;
      border-radius: 9999px;
    }
    .section-body { padding: 0.5rem 0.75rem 0.75rem; }
    .section-body.loading {
      font-size: 0.75rem;
      color: var(--color-text-secondary);
      padding: 1rem 0.75rem;
    }
    .section-desc {
      font-size: 0.75rem;
      color: var(--color-text-secondary);
      margin: 0 0 0.5rem;
      line-height: 1.4;
    }
    .group-label {
      font-size: 0.625rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: var(--color-text-secondary);
      margin-top: 0.5rem;
      margin-bottom: 0.25rem;
    }
    .value-row {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.25rem 0;
      cursor: pointer;
      border-radius: var(--radius-xs, 3px);
      transition: background var(--transition-fast);
    }
    .value-row:hover { background: var(--color-bg-subtle); }
    .value-info { flex: 1; min-width: 0; }
    .value-code {
      font-size: 0.625rem;
      color: var(--color-text-secondary);
      background: var(--color-bg-subtle);
      padding: 0.0625rem 0.25rem;
      border-radius: 2px;
      margin-right: 0.375rem;
    }
    .value-label {
      font-size: 0.75rem;
      color: var(--color-text-body);
    }
    .value-bar-area {
      display: flex;
      align-items: center;
      gap: 0.375rem;
      width: 120px;
      flex-shrink: 0;
    }
    .value-bar-track {
      flex: 1;
      height: 5px;
      background: var(--color-border-input);
      border-radius: 3px;
      overflow: hidden;
    }
    .value-bar-fill {
      height: 100%;
      background: var(--color-primary);
      border-radius: 3px;
      min-width: 1px;
      transition: width 0.3s;
    }
    .value-count {
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      font-variant-numeric: tabular-nums;
      min-width: 2rem;
      text-align: right;
    }
  `]
})
export class TaxonomyBrowserComponent {
  private svc = inject(OntologyService);
  private router = inject(Router);

  sections = signal<TaxonomySection[]>(
    TAXONOMY_IDS.map(id => ({
      id,
      name: id,
      description: '',
      expanded: false,
      loading: false,
      values: [],
    }))
  );

  toggle(section: TaxonomySection) {
    const updated = this.sections().map(s => {
      if (s.id !== section.id) return s;
      const next = { ...s, expanded: !s.expanded };
      if (next.expanded && next.values.length === 0 && !next.loading) {
        next.loading = true;
        this.loadTaxonomy(section.id);
      }
      return next;
    });
    this.sections.set(updated);
  }

  groups(values: TaxonomyValue[]): { name: string | null; items: TaxonomyValue[] }[] {
    const grouped = new Map<string | null, TaxonomyValue[]>();
    for (const v of values) {
      const key = v.group;
      if (!grouped.has(key)) grouped.set(key, []);
      grouped.get(key)!.push(v);
    }
    return Array.from(grouped.entries()).map(([name, items]) => ({ name, items }));
  }

  maxCount(values: TaxonomyValue[]): number {
    return values.reduce((max, v) => Math.max(max, v.count), 1);
  }

  barPct(count: number, max: number): number {
    if (max === 0) return 0;
    return Math.max(1, (count / max) * 100);
  }

  navigateToSearch(taxonomyId: string, code: string) {
    if (taxonomyId === 'LegalBranch') {
      this.router.navigate(['/jurisprudencia/resultados'], { queryParams: { legalBranch: code } });
    }
  }

  private loadTaxonomy(id: string) {
    this.svc.getTaxonomy(id).subscribe({
      next: (resp) => {
        this.sections.set(this.sections().map(s =>
          s.id === id
            ? { ...s, loading: false, name: resp.name, description: resp.description, values: resp.values }
            : s
        ));
      },
      error: () => {
        this.sections.set(this.sections().map(s =>
          s.id === id ? { ...s, loading: false } : s
        ));
      },
    });
  }
}
