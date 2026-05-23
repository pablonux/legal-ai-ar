import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

export interface EmptyStateAction {
  label: string;
  route?: string;
  queryParams?: Record<string, string>;
  action?: () => void;
  primary?: boolean;
}

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="es-container" [class]="'es-' + variant()">
      <div class="es-illustration" aria-hidden="true">
        <ng-content select="[esIcon]"></ng-content>
      </div>
      <h2 class="es-title">{{ title() }}</h2>
      @if (subtitle()) {
        <p class="es-subtitle">{{ subtitle() }}</p>
      }
      @if (tips().length) {
        <ul class="es-tips">
          @for (tip of tips(); track tip) {
            <li>
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"/><circle cx="12" cy="12" r="10"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>
              <span>{{ tip }}</span>
            </li>
          }
        </ul>
      }
      @if (actions().length) {
        <div class="es-actions">
          @for (act of actions(); track act.label) {
            @if (act.route) {
              <a [routerLink]="act.route" [queryParams]="act.queryParams ?? {}" class="es-btn" [class.es-btn-primary]="act.primary">{{ act.label }}</a>
            } @else {
              <button type="button" class="es-btn" [class.es-btn-primary]="act.primary" (click)="act.action?.()">{{ act.label }}</button>
            }
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .es-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      padding: 3rem 2rem;
      max-width: 480px;
      margin: 0 auto;
    }
    .es-illustration { margin-bottom: 16px; color: var(--color-text-secondary, #a8a29e); }
    .es-title {
      font-size: 1.125rem;
      font-weight: 600;
      color: var(--color-text);
      margin: 0 0 6px;
    }
    .es-subtitle {
      font-size: .875rem;
      color: var(--color-text-secondary, #78716c);
      margin: 0 0 16px;
      line-height: 1.5;
    }
    .es-tips {
      list-style: none;
      padding: 0;
      margin: 0 0 20px;
      text-align: left;
      width: 100%;
    }
    .es-tips li {
      display: flex;
      align-items: flex-start;
      gap: 8px;
      font-size: .8125rem;
      color: var(--color-text-secondary, #78716c);
      padding: 6px 12px;
      border-radius: 8px;
      background: var(--color-bg-subtle, #fafaf9);
      margin-bottom: 4px;
      line-height: 1.4;
    }
    .es-tips li svg { flex-shrink: 0; margin-top: 2px; color: var(--color-primary, #d04a02); }
    .es-actions { display: flex; gap: 8px; flex-wrap: wrap; justify-content: center; }
    .es-btn {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 8px 18px;
      border-radius: 8px;
      font-size: .8125rem;
      font-weight: 600;
      cursor: pointer;
      border: 1px solid var(--color-border, #e7e5e4);
      background: var(--color-bg, #fff);
      color: var(--color-text);
      text-decoration: none;
      transition: border-color .15s, background .15s;
    }
    .es-btn:hover { border-color: var(--color-primary, #d04a02); }
    .es-btn-primary {
      background: var(--color-primary, #d04a02);
      color: #fff;
      border-color: var(--color-primary, #d04a02);
    }
    .es-btn-primary:hover { opacity: .9; }
  `]
})
export class EmptyStateComponent {
  title = input.required<string>();
  subtitle = input('');
  tips = input<string[]>([]);
  actions = input<EmptyStateAction[]>([]);
  variant = input<'default' | 'search' | 'space'>('default');
}
