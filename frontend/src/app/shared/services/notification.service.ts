import { Injectable, signal, computed } from '@angular/core';

export type NotificationType = 'success' | 'error' | 'info' | 'warning';

export interface Notification {
  id: number;
  message: string;
  type: NotificationType;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private _notifications = signal<Notification[]>([]);
  private _nextId = 0;

  readonly notifications = this._notifications.asReadonly();
  readonly latest = computed(() => this._notifications().at(-1) ?? null);

  show(message: string, type: NotificationType = 'info', durationMs = 4000): void {
    const id = ++this._nextId;
    this._notifications.update(list => [...list, { id, message, type }]);

    if (durationMs > 0) {
      setTimeout(() => this.dismiss(id), durationMs);
    }
  }

  success(message: string, durationMs = 4000): void {
    this.show(message, 'success', durationMs);
  }

  error(message: string, durationMs = 6000): void {
    this.show(message, 'error', durationMs);
  }

  warning(message: string, durationMs = 5000): void {
    this.show(message, 'warning', durationMs);
  }

  info(message: string, durationMs = 4000): void {
    this.show(message, 'info', durationMs);
  }

  dismiss(id: number): void {
    this._notifications.update(list => list.filter(n => n.id !== id));
  }
}
