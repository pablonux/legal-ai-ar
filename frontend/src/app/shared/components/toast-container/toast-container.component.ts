import { Component, inject } from '@angular/core';
import { NotificationService, type Notification } from '../../services/notification.service';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  template: `
    <div class="toast-container" aria-live="polite">
      @for (n of notificationService.notifications(); track n.id) {
        <div class="toast" [class]="'toast-' + n.type" role="alert">
          <span class="toast-icon" aria-hidden="true">
            @switch (n.type) {
              @case ('success') { <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="20 6 9 17 4 12"/></svg> }
              @case ('error') { <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg> }
              @case ('warning') { <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg> }
              @default { <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg> }
            }
          </span>
          <span class="toast-message">{{ n.message }}</span>
          <button type="button" class="toast-close" (click)="dismiss(n)" aria-label="Cerrar notificación">&times;</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      top: calc(var(--header-height, 68px) + 0.75rem);
      right: 1rem;
      z-index: 1200;
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
      max-width: 380px;
      pointer-events: none;
    }

    .toast {
      display: flex;
      align-items: flex-start;
      gap: 0.5rem;
      padding: 0.75rem 1rem;
      border-radius: var(--radius-sm, 0.25rem);
      box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
      font-size: 0.875rem;
      line-height: 1.4;
      pointer-events: auto;
      animation: toast-in 0.25s ease;
    }

    @keyframes toast-in {
      from { opacity: 0; transform: translateX(1rem); }
      to { opacity: 1; transform: translateX(0); }
    }

    .toast-success {
      background: #f0fdf4;
      color: #166534;
      border-left: 3px solid #16a34a;
    }

    .toast-error {
      background: #fef2f2;
      color: #991b1b;
      border-left: 3px solid #dc2626;
    }

    .toast-warning {
      background: #fffbeb;
      color: #92400e;
      border-left: 3px solid #f59e0b;
    }

    .toast-info {
      background: #eff6ff;
      color: #1e40af;
      border-left: 3px solid #3b82f6;
    }

    .toast-icon {
      flex-shrink: 0;
      margin-top: 1px;
    }

    .toast-message {
      flex: 1;
    }

    .toast-close {
      flex-shrink: 0;
      background: none;
      border: none;
      font-size: 1.125rem;
      line-height: 1;
      cursor: pointer;
      opacity: 0.5;
      color: inherit;
      padding: 0 2px;
    }

    .toast-close:hover {
      opacity: 1;
    }
  `]
})
export class ToastContainerComponent {
  protected notificationService = inject(NotificationService);

  dismiss(notification: Notification) {
    this.notificationService.dismiss(notification.id);
  }
}
