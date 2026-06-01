import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { RulingReprocessService, type RulingReprocessRequestItem } from '../../../services/admin/ruling-reprocess.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

type FilterStatus = '' | 'Queued' | 'Running' | 'Completed' | 'Failed';

@Component({
  selector: 'app-reprocess-queue',
  standalone: true,
  imports: [RouterLink, LoadingSpinnerComponent],
  template: `
    <div class="reprocess-queue">
      <p class="intro">
        Fallos enviados a reprocesamiento completo (Fetcher → Indexer). Mientras están en curso no aparecen en búsqueda ni asistente.
      </p>

      <div class="filters">
        <label>
          Estado
          <select [value]="statusFilter()" (change)="onFilterChange($event)">
            <option value="">Todos</option>
            <option value="Queued">En cola</option>
            <option value="Running">En ejecución</option>
            <option value="Completed">Completados</option>
            <option value="Failed">Fallidos</option>
          </select>
        </label>
        <button type="button" class="btn-refresh" (click)="load()" [disabled]="loading()">Actualizar</button>
      </div>

      @if (loading()) {
        <app-loading-spinner message="Cargando cola..." />
      } @else if (items().length === 0) {
        <p class="empty">No hay solicitudes de reprocesamiento.</p>
      } @else {
        <div class="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Fallo</th>
                <th>External ID</th>
                <th>Estado</th>
                <th>Solicitado</th>
                <th>Por</th>
                <th>Error</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              @for (row of items(); track row.id) {
                <tr>
                  <td>
                    <a [routerLink]="['/jurisprudencia', row.rulingId]">{{ row.caseTitle }}</a>
                  </td>
                  <td><code>{{ row.externalId }}</code></td>
                  <td><span class="status" [attr.data-status]="row.status">{{ statusLabel(row.status) }}</span></td>
                  <td>{{ formatDate(row.requestedAt) }}</td>
                  <td>{{ row.requestedBy }}</td>
                  <td class="err">{{ row.errorMessage ?? '—' }}</td>
                  <td>
                    @if (row.status === 'Failed') {
                      <button type="button" class="btn-retry" (click)="retry(row)" [disabled]="retryBusyId() === row.id">
                        {{ retryBusyId() === row.id ? '…' : 'Reintentar' }}
                      </button>
                    }
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
        <p class="total">{{ total() }} solicitud(es)</p>
      }
    </div>
  `,
  styles: [`
    .intro { color: var(--color-text-body); margin: 0 0 1rem; font-size: 0.9rem; }
    .filters { display: flex; gap: 1rem; align-items: end; margin-bottom: 1rem; flex-wrap: wrap; }
    .filters label { display: flex; flex-direction: column; gap: 0.25rem; font-size: 0.8rem; font-weight: 600; }
    .filters select { padding: 0.4rem 0.6rem; border-radius: 6px; border: 1px solid var(--color-border-input); }
    .btn-refresh, .btn-retry {
      padding: 0.45rem 0.9rem; border-radius: 6px; border: none; cursor: pointer; font-size: 0.85rem;
    }
    .btn-refresh { background: var(--color-surface-alt); }
    .btn-retry { background: var(--color-primary); color: #fff; }
    .table-wrap { overflow-x: auto; }
    table { width: 100%; border-collapse: collapse; font-size: 0.875rem; }
    th, td { text-align: left; padding: 0.6rem 0.75rem; border-bottom: 1px solid var(--color-border-input); }
    th { font-weight: 600; color: var(--color-text-muted); }
    .err { max-width: 280px; font-size: 0.8rem; color: var(--color-danger, #c00); }
    .status[data-status="Running"], .status[data-status="Queued"] { color: var(--color-warning, #b8860b); }
    .status[data-status="Completed"] { color: var(--color-success, #2d6a4f); }
    .status[data-status="Failed"] { color: var(--color-danger, #c00); }
    .empty, .total { color: var(--color-text-muted); font-size: 0.875rem; }
    a { color: var(--color-primary); text-decoration: none; }
    a:hover { text-decoration: underline; }
  `]
})
export class ReprocessQueueComponent implements OnInit {
  private service = inject(RulingReprocessService);
  private notify = inject(NotificationService);

  items = signal<RulingReprocessRequestItem[]>([]);
  total = signal(0);
  loading = signal(false);
  statusFilter = signal<FilterStatus>('');
  retryBusyId = signal<string | null>(null);

  ngOnInit() {
    this.load();
  }

  onFilterChange(event: Event) {
    const v = (event.target as HTMLSelectElement).value as FilterStatus;
    this.statusFilter.set(v);
    this.load();
  }

  load() {
    this.loading.set(true);
    const status = this.statusFilter() || undefined;
    this.service.list(status).subscribe({
      next: res => {
        this.items.set(res.items);
        this.total.set(res.total);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.notify.error('No se pudo cargar la cola de reprocesamiento.');
      }
    });
  }

  retry(row: RulingReprocessRequestItem) {
    if (!confirm(`¿Reintentar reprocesamiento de «${row.caseTitle}»?`)) return;
    this.retryBusyId.set(row.id);
    this.service.retry(row.id).subscribe({
      next: res => {
        this.notify.success(res.message);
        this.retryBusyId.set(null);
        this.load();
      },
      error: err => {
        this.retryBusyId.set(null);
        this.notify.error(err?.error?.message ?? 'Error al reintentar.');
      }
    });
  }

  statusLabel(status: string): string {
    const map: Record<string, string> = {
      Queued: 'En cola',
      Running: 'En ejecución',
      Completed: 'Completado',
      Failed: 'Fallido',
      Cancelled: 'Cancelado'
    };
    return map[status] ?? status;
  }

  formatDate(iso: string): string {
    try {
      return new Date(iso).toLocaleString('es-AR');
    } catch {
      return iso;
    }
  }
}
