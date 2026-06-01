import { Component, input } from '@angular/core';

@Component({
  selector: 'app-skeleton-loader',
  standalone: true,
  template: `
    <div class="skeleton-host" [attr.aria-label]="'Cargando'" role="status">
      @switch (variant()) {
        @case ('card') {
          <div class="sk-card">
            <div class="sk-line sk-line-title"></div>
            <div class="sk-line sk-line-meta"></div>
            <div class="sk-line sk-line-body"></div>
            <div class="sk-line sk-line-body sk-line-short"></div>
          </div>
        }
        @case ('text') {
          @for (_ of lines(); track $index) {
            <div class="sk-line sk-line-body" [style.width]="$last ? '60%' : '100%'"></div>
          }
        }
        @case ('table-row') {
          <div class="sk-table-row">
            @for (_ of cols(); track $index) {
              <div class="sk-cell"></div>
            }
          </div>
        }
        @default {
          <div class="sk-line sk-line-body"></div>
        }
      }
    </div>
  `,
  styles: [`
    .skeleton-host {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .sk-line {
      height: 0.75rem;
      background: linear-gradient(90deg, #eee 25%, #f5f5f5 50%, #eee 75%);
      background-size: 200% 100%;
      border-radius: 4px;
      animation: shimmer 1.5s infinite;
    }

    .sk-line-title {
      height: 1rem;
      width: 60%;
    }

    .sk-line-meta {
      height: 0.625rem;
      width: 40%;
    }

    .sk-line-body {
      width: 100%;
    }

    .sk-line-short {
      width: 75%;
    }

    .sk-card {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
      padding: 1.25rem;
      background: var(--color-bg-surface, #fff);
      border: 1px solid var(--color-border, #e8e8e8);
      border-radius: 12px;
    }

    .sk-table-row {
      display: flex;
      gap: 1rem;
      padding: 0.75rem 0;
      border-bottom: 1px solid var(--color-border, #e8e8e8);
    }

    .sk-cell {
      flex: 1;
      height: 0.75rem;
      background: linear-gradient(90deg, #eee 25%, #f5f5f5 50%, #eee 75%);
      background-size: 200% 100%;
      border-radius: 4px;
      animation: shimmer 1.5s infinite;
    }

    @keyframes shimmer {
      0% { background-position: 200% 0; }
      100% { background-position: -200% 0; }
    }
  `]
})
export class SkeletonLoaderComponent {
  variant = input<'card' | 'text' | 'table-row' | 'line'>('line');
  count = input(1);

  lines(): number[] {
    return Array.from({ length: this.count() }, (_, i) => i);
  }

  cols(): number[] {
    return Array.from({ length: this.count() }, (_, i) => i);
  }
}
