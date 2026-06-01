import { Component, input } from '@angular/core';

@Component({
  selector: 'app-skeleton-card',
  standalone: true,
  template: `
    @for (_ of items; track $index) {
      <div class="sk-card">
        <div class="skeleton sk-title"></div>
        <div class="sk-meta">
          <div class="skeleton sk-badge"></div>
          <div class="skeleton sk-badge sk-badge-sm"></div>
          <div class="skeleton sk-badge"></div>
        </div>
        <div class="skeleton sk-line"></div>
        <div class="skeleton sk-line sk-line-short"></div>
      </div>
    }
  `,
  styles: [`
    :host { display: flex; flex-direction: column; gap: 0.75rem; }

    .sk-card {
      padding: 1.5rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      background: var(--color-bg-surface);
    }

    .sk-title {
      height: 16px;
      width: 65%;
      margin-bottom: 0.75rem;
    }

    .sk-meta {
      display: flex;
      gap: 0.75rem;
      margin-bottom: 0.75rem;
    }

    .sk-badge {
      height: 12px;
      width: 80px;
      border-radius: var(--radius-pill);
    }

    .sk-badge-sm {
      width: 50px;
    }

    .sk-line {
      height: 12px;
      width: 100%;
      margin-bottom: 0.375rem;
    }

    .sk-line-short {
      width: 70%;
    }
  `]
})
export class SkeletonCardComponent {
  count = input(3);
  get items() { return Array(this.count()); }
}
