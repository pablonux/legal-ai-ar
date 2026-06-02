import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { StatuteService } from '../../../services/statute.service';
import { BreadcrumbComponent } from '@legal-ai-ar/shared-common/components/breadcrumb/breadcrumb.component';
import { SkeletonStatComponent } from '@legal-ai-ar/shared-common/components/skeletons/skeleton-stat.component';
import type { PyramidLevel } from '../../../models/statute.models';

@Component({
  selector: 'app-normative-pyramid',
  standalone: true,
  imports: [RouterLink, BreadcrumbComponent, SkeletonStatComponent],
  template: `
    <div class="pyramid-page">
      <app-breadcrumb [items]="[{ label: 'Ordenamiento', route: '/ordenamiento' }, { label: 'Pirámide Normativa' }]" />

      <h1 class="page-title">
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" aria-hidden="true"><polygon points="12 2 22 22 2 22"/></svg>
        Pirámide Normativa (Kelsen)
      </h1>
      <p class="page-desc">Jerarquía de normas jurídicas en el ordenamiento argentino. Click en un nivel para ver las normas de esa categoría.</p>

      @if (loading()) {
        <app-skeleton-stat [count]="5" />
      } @else {
        <div class="pyramid-summary">
          <span class="summary-total">{{ totalCount() }} normas</span>
          <span class="summary-sep">·</span>
          <span class="summary-vigente">{{ totalVigente() }} vigentes</span>
        </div>
        <div class="pyramid">
          @for (level of levels(); track level.level; let i = $index) {
            <button class="pyramid-level" [style.--level-width]="pyramidWidth(i)" (click)="filterByLevel(level.level)">
              <span class="level-label">{{ level.label }}</span>
              <span class="level-meta">
                <span class="level-count">{{ level.count }}</span>
                @if (level.vigenteCount < level.count) {
                  <span class="level-vigente">({{ level.vigenteCount }} vigentes)</span>
                }
              </span>
            </button>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .pyramid-page { max-width: 800px; margin: 0 auto; }

    .page-title {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 1.375rem;
      font-weight: 600;
      margin: 0 0 0.25rem;
      color: var(--color-text);
    }

    .page-title svg { color: var(--color-primary); }

    .page-desc {
      font-size: 0.875rem;
      color: var(--color-text-secondary);
      margin: 0 0 2rem;
    }

    .pyramid {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 6px;
    }

    .pyramid-level {
      display: flex;
      align-items: center;
      justify-content: space-between;
      width: var(--level-width);
      min-height: 56px;
      padding: 0.75rem 1.25rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      background: var(--color-bg-surface);
      cursor: pointer;
      transition: all 0.2s;
      font-family: inherit;
    }

    .pyramid-level:hover {
      border-color: var(--color-primary);
      background: rgba(208, 74, 2, 0.04);
      transform: scale(1.01);
    }

    .pyramid-level:first-child {
      background: rgba(208, 74, 2, 0.06);
      border-color: rgba(208, 74, 2, 0.2);
    }

    .level-label {
      font-size: 0.9375rem;
      font-weight: 600;
      color: var(--color-text);
    }

    .pyramid-summary {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      margin-bottom: 1.5rem;
      font-size: 0.875rem;
      color: var(--color-text-secondary);
    }

    .summary-total { font-weight: 600; color: var(--color-text); }
    .summary-sep { opacity: 0.4; }
    .summary-vigente { color: var(--color-success, #16a34a); }

    .level-meta {
      display: flex;
      align-items: baseline;
      gap: 0.375rem;
    }

    .level-count {
      font-size: 0.9375rem;
      color: var(--color-text);
      font-weight: 600;
    }

    .level-vigente {
      font-size: 0.75rem;
      color: var(--color-text-secondary);
      font-weight: 400;
    }
  `]
})
export class NormativePyramidComponent implements OnInit {
  private statuteService = inject(StatuteService);
  private router = inject(Router);

  levels = signal<PyramidLevel[]>([]);
  loading = signal(true);
  totalCount = computed(() => this.levels().reduce((sum, l) => sum + l.count, 0));
  totalVigente = computed(() => this.levels().reduce((sum, l) => sum + l.vigenteCount, 0));

  ngOnInit() {
    this.statuteService.getPyramid().subscribe({
      next: levels => { this.levels.set(levels); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  pyramidWidth(index: number): string {
    const total = this.levels().length || 5;
    const minPct = 35;
    const maxPct = 100;
    const step = (maxPct - minPct) / Math.max(total - 1, 1);
    return `${minPct + step * index}%`;
  }

  filterByLevel(level: string) {
    this.router.navigate(['/ordenamiento'], { queryParams: { normativeLevel: level } });
  }
}
