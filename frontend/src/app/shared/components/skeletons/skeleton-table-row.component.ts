import { Component, input } from '@angular/core';

@Component({
  selector: 'app-skeleton-table-row',
  standalone: true,
  template: `
    @for (_ of items; track $index) {
      <div class="sk-row">
        <div class="skeleton sk-cell sk-cell-wide"></div>
        <div class="skeleton sk-cell"></div>
        <div class="skeleton sk-cell sk-cell-narrow"></div>
      </div>
    }
  `,
  styles: [`
    :host { display: flex; flex-direction: column; }

    .sk-row {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.75rem 1rem;
      border-bottom: 1px solid var(--color-border);
    }

    .sk-cell {
      height: 12px;
      width: 100px;
    }

    .sk-cell-wide {
      flex: 1;
    }

    .sk-cell-narrow {
      width: 60px;
    }
  `]
})
export class SkeletonTableRowComponent {
  count = input(5);
  get items() { return Array(this.count()); }
}
