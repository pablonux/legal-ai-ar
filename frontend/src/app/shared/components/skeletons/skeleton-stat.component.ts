import { Component, input } from '@angular/core';

@Component({
  selector: 'app-skeleton-stat',
  standalone: true,
  template: `
    <div class="sk-stats">
      @for (_ of items; track $index) {
        <div class="sk-stat-card">
          <div class="skeleton sk-value"></div>
          <div class="skeleton sk-label"></div>
        </div>
      }
    </div>
  `,
  styles: [`
    .sk-stats {
      display: flex;
      gap: 1rem;
      flex-wrap: wrap;
    }

    .sk-stat-card {
      flex: 1;
      min-width: 140px;
      max-width: 200px;
      padding: 1.25rem 1rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      background: var(--color-bg-surface);
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.5rem;
    }

    .sk-value {
      height: 28px;
      width: 60px;
    }

    .sk-label {
      height: 10px;
      width: 80px;
    }
  `]
})
export class SkeletonStatComponent {
  count = input(4);
  get items() { return Array(this.count()); }
}
