import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

export interface BreadcrumbItem {
  label: string;
  route?: string;
}

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [RouterLink],
  template: `
    <nav class="breadcrumb" aria-label="Ubicación">
      @for (item of items(); track item.label; let last = $last) {
        @if (item.route && !last) {
          <a [routerLink]="item.route" class="breadcrumb-link">{{ item.label }}</a>
        } @else {
          <span class="breadcrumb-current" [attr.aria-current]="last ? 'page' : null">{{ item.label }}</span>
        }
        @if (!last) {
          <span class="breadcrumb-sep" aria-hidden="true">&rsaquo;</span>
        }
      }
    </nav>
  `,
  styles: [`
    .breadcrumb {
      display: flex;
      align-items: center;
      gap: 0.375rem;
      font-size: 0.75rem;
      color: var(--color-text-secondary);
      margin-bottom: 1rem;
      flex-wrap: wrap;
    }

    .breadcrumb-link {
      color: var(--color-primary);
      text-decoration: none;
      transition: color 0.15s;
    }

    .breadcrumb-link:hover {
      text-decoration: underline;
    }

    .breadcrumb-current {
      color: var(--color-text-secondary);
      max-width: 300px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .breadcrumb-sep {
      color: var(--color-text-secondary);
      opacity: 0.5;
      font-size: 0.875rem;
    }
  `]
})
export class BreadcrumbComponent {
  items = input.required<BreadcrumbItem[]>();
}
