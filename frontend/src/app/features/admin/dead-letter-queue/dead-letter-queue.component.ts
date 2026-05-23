import { Component, signal, inject, OnInit } from '@angular/core';
import { A11yModule } from '@angular/cdk/a11y';
import { NotificationService } from '../../../shared/services/notification.service';
import { DlqService, type DlqQueueName, type DlqMessage } from '../../../services/admin/dlq.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

const QUEUE_LABELS: Record<DlqQueueName, string> = {
  discoverer: 'Descubrimiento',
  fetcher: 'Descarga',
  parser: 'Parseo',
  enricher: 'Enriquecimiento',
  persister: 'Persistencia',
  indexer: 'Indexación',
  crawler: 'Crawler (legacy)',
  enrichment: 'Enrichment (legacy)'
};

@Component({
  selector: 'app-dead-letter-queue',
  standalone: true,
  imports: [LoadingSpinnerComponent, A11yModule],
  template: `
    <div class="dlq">
      <div class="tabs">
        @for (q of queues; track q) {
          <button
            type="button"
            class="tab"
            [class.active]="activeTab() === q"
            (click)="selectTab(q)"
          >
            {{ QUEUE_LABELS[q] }}
          </button>
        }
      </div>

      <div class="tab-content">
        @if (state() === 'loading') {
          <div class="state-message" aria-live="polite">
            <app-loading-spinner [message]="'Cargando ' + QUEUE_LABELS[activeTab()] + '...'" />
          </div>
        }

        @if (state() === 'error') {
          <div class="state-message error">
            <p>{{ error() }}</p>
            <button type="button" class="retry-btn" (click)="load()">Reintentar</button>
          </div>
        }

        @if (state() === 'loaded' && messages().length === 0) {
          <div class="state-message empty">
            <p>No hay mensajes en esta cola.</p>
          </div>
        }

        @if (state() === 'loaded' && messages().length > 0) {
          <div class="table-wrap">
            <table class="dlq-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Insertado</th>
                  <th>Intentos</th>
                  <th>Error</th>
                  <th>Vista previa</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                @for (msg of messages(); track msg.id) {
                  <tr>
                    <td class="id-cell">{{ truncateId(msg.id) }}</td>
                    <td>{{ formatDate(msg.insertedOn) }}</td>
                    <td>{{ msg.dequeueCount }}</td>
                    <td class="error-cell">
                      @if (msg.error) {
                        <span class="error-badge">{{ msg.error.type }}</span>
                        <span class="error-message">{{ msg.error.message }}</span>
                      } @else {
                        <span class="error-legacy">—</span>
                      }
                    </td>
                    <td class="preview-cell">{{ msg.bodyPreview }}</td>
                    <td>
                      <button
                        type="button"
                        class="requeue-btn"
                        [disabled]="requeueId() === msg.id"
                        (click)="openRequeueConfirm(msg)"
                      >
                        {{ requeueId() === msg.id ? 'Reencolando...' : 'Reencolar' }}
                      </button>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      </div>
    </div>

    @if (showRequeueConfirm()) {
      <div class="modal-overlay" (click)="cancelRequeue()" (keydown.escape)="cancelRequeue()">
        <div class="modal modal-sm" role="dialog" aria-modal="true" cdkTrapFocus cdkTrapFocusAutoCapture (click)="$event.stopPropagation()">
          <h2>¿Reencolar este mensaje?</h2>
          <p>Volverá a la cola de origen y será procesado de nuevo.</p>
          <div class="modal-actions">
            <button type="button" class="btn-secondary" (click)="cancelRequeue()">Cancelar</button>
            <button type="button" class="btn-primary" [disabled]="requeueId() !== null" (click)="confirmRequeue()">
              Reencolar
            </button>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .dlq { position: relative; }
    .tabs {
      display: flex;
      gap: 0;
      flex-wrap: wrap;
      margin-bottom: 1rem;
      border-bottom: 1px solid var(--color-border-input);
    }
    .tab {
      padding: 0.625rem 1.25rem;
      font-size: 0.8125rem;
      font-weight: 500;
      background: none;
      border: none;
      border-bottom: 2px solid transparent;
      margin-bottom: -1px;
      cursor: pointer;
      color: var(--color-text-body);
      border-radius: var(--radius-xs) var(--radius-xs) 0 0;
      transition: all var(--transition-base);
    }
    .tab:hover {
      color: var(--color-text);
      border-bottom-color: var(--color-border-input);
    }
    .tab.active {
      color: var(--color-primary);
      border-bottom-color: var(--color-primary);
      font-weight: 600;
    }
    .tab-content { min-height: 120px; }
    .state-message {
      text-align: center;
      padding: 3rem 2rem;
      color: var(--color-text-body);
    }
    .state-message.error { color: var(--color-primary); }
    .state-message.empty { color: var(--color-text-secondary); }
    .retry-btn {
      margin-top: 1rem;
      padding: 0.5rem 1rem;
      background: none;
      border: 1px solid var(--color-primary);
      color: var(--color-primary);
      border-radius: var(--radius-sm);
      cursor: pointer;
      font-size: 0.875rem;
    }
    .table-wrap { overflow-x: auto; }
    .dlq-table {
      width: 100%;
      border-collapse: collapse;
      font-size: 0.875rem;
    }
    .dlq-table th,
    .dlq-table td {
      padding: 0.875rem 1rem;
      text-align: left;
      border-bottom: 1px solid var(--color-border);
    }
    .dlq-table th {
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      color: var(--color-text-secondary);
      background: var(--color-bg-subtle);
    }
    .id-cell { font-family: var(--font-mono); font-size: 0.8125rem; max-width: 140px; }
    .error-cell { max-width: 280px; }
    .error-badge {
      display: block;
      font-size: 0.6875rem;
      font-weight: 600;
      color: #c0392b;
      background: #fdeaea;
      padding: 2px 6px;
      border-radius: var(--radius-xs);
      margin-bottom: 4px;
    }
    .error-message { font-size: 0.8125rem; line-height: 1.3; word-break: break-word; }
    .error-legacy { font-size: 0.8125rem; color: var(--color-text-secondary); font-style: italic; }
    .preview-cell { max-width: 220px; word-break: break-all; font-size: 0.8125rem; }
    .requeue-btn {
      padding: 0.35rem 0.75rem;
      font-size: 0.8125rem;
      background: var(--color-primary);
      color: #fff;
      border: none;
      border-radius: var(--radius-sm);
      cursor: pointer;
      transition: all var(--transition-base);
    }
    .requeue-btn:hover:not(:disabled) { background: var(--color-primary-hover); }
    .requeue-btn:disabled { opacity: 0.6; cursor: not-allowed; }
    .modal-overlay {
      position: fixed;
      inset: 0;
      background: rgba(0,0,0,0.4);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 200;
    }
    .modal {
      background: var(--color-bg-surface);
      border-radius: var(--radius-md);
      padding: 1.5rem;
      max-width: 400px;
      width: 90%;
      box-shadow: var(--shadow-lg);
    }
    .modal h2 { font-size: 1.1rem; margin: 0 0 1rem 0; }
    .modal.modal-sm { max-width: 360px; }
    .modal.modal-sm p { margin: 0 0 1rem 0; font-size: 0.875rem; color: var(--color-text-body); }
    .modal-actions {
      display: flex;
      gap: 0.75rem;
      justify-content: flex-end;
    }
    .btn-secondary {
      padding: 0.5rem 1rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      cursor: pointer;
      font-size: 0.875rem;
      transition: all var(--transition-base);
    }
    .btn-primary {
      padding: 0.5rem 1rem;
      background: var(--color-primary);
      color: #fff;
      border: none;
      border-radius: var(--radius-sm);
      cursor: pointer;
      font-size: 0.875rem;
      transition: all var(--transition-base);
    }
    .btn-primary:hover:not(:disabled) { background: var(--color-primary-hover); }
    .btn-primary:disabled { opacity: 0.6; cursor: not-allowed; }
  `]
})
export class DeadLetterQueueComponent implements OnInit {
  private dlqService = inject(DlqService);
  private notify = inject(NotificationService);

  readonly QUEUE_LABELS = QUEUE_LABELS;
  readonly queues: DlqQueueName[] = this.dlqService.validQueues;

  activeTab = signal<DlqQueueName>('discoverer');
  state = signal<'loading' | 'loaded' | 'error'>('loading');
  messages = signal<DlqMessage[]>([]);
  error = signal<string>('');
  showRequeueConfirm = signal(false);
  pendingMessage = signal<DlqMessage | null>(null);
  requeueId = signal<string | null>(null);

  ngOnInit() {
    this.load();
  }

  selectTab(q: DlqQueueName) {
    this.activeTab.set(q);
    this.load();
  }

  load() {
    this.state.set('loading');
    this.error.set('');
    this.dlqService.getMessages(this.activeTab()).subscribe({
      next: (res) => {
        this.messages.set(res.messages ?? []);
        this.state.set('loaded');
      },
      error: (err) => {
        this.error.set(err?.error?.detail ?? err?.error?.title ?? err?.message ?? 'Error al cargar.');
        this.state.set('error');
      }
    });
  }

  truncateId(id: string, maxLen = 24): string {
    if (!id) return '—';
    return id.length <= maxLen ? id : id.slice(0, maxLen) + '…';
  }

  formatDate(value: string): string {
    if (!value) return '—';
    const d = new Date(value);
    if (isNaN(d.getTime())) return '—';
    return d.toLocaleString('es-AR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  openRequeueConfirm(msg: DlqMessage) {
    this.pendingMessage.set(msg);
    this.showRequeueConfirm.set(true);
  }

  cancelRequeue() {
    this.showRequeueConfirm.set(false);
    this.pendingMessage.set(null);
  }

  confirmRequeue() {
    const msg = this.pendingMessage();
    if (!msg) return;

    this.requeueId.set(msg.id);
    this.dlqService.requeue(this.activeTab(), msg.id).subscribe({
      next: (res) => {
        this.cancelRequeue();
        this.requeueId.set(null);
        this.load();
        this.notify.success(res.message ?? 'Mensaje reencolado.');
      },
      error: (err) => {
        this.requeueId.set(null);
        this.notify.error(err?.error?.detail ?? err?.message ?? 'Error al reencolar.');
      }
    });
  }
}
