import { Component, Input, computed, signal } from '@angular/core';

export type ToolChipState = 'running' | 'done' | 'error';

const TOOL_LABELS: Record<string, { running: string; done: string }> = {
  search_rulings: { running: 'Buscando jurisprudencia...', done: 'Búsqueda completada' },
  get_ruling_detail: { running: 'Obteniendo detalles del fallo...', done: 'Detalles obtenidos' },
  get_ruling_citations: { running: 'Consultando citas...', done: 'Citas obtenidas' },
  get_related_rulings: { running: 'Buscando fallos relacionados...', done: 'Fallos relacionados obtenidos' },
  search_by_statute: { running: 'Buscando por norma...', done: 'Búsqueda por norma completada' },
  count_rulings: { running: 'Contando fallos...', done: 'Conteo completado' },
  list_courts: { running: 'Listando tribunales...', done: 'Tribunales obtenidos' },
  list_persons: { running: 'Listando personas...', done: 'Personas obtenidas' },
};

@Component({
  selector: 'app-tool-status-chip',
  standalone: true,
  template: `
    <span class="tool-chip" [class.running]="state === 'running'" [class.done]="state === 'done'" [class.error]="state === 'error'">
      @if (state === 'running') {
        <span class="spinner" aria-hidden="true"></span>
      } @else if (state === 'done') {
        <span class="check" aria-hidden="true">&#10003;</span>
      } @else {
        <span class="warn" aria-hidden="true">&#9888;</span>
      }
      <span class="label">{{ label }}</span>
      @if (state === 'done' && resultCount !== undefined && resultCount !== null) {
        <span class="count">({{ resultCount }})</span>
      }
    </span>
  `,
  styles: [`
    .tool-chip {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 4px 12px;
      border-radius: 8px;
      font-size: 0.75rem;
      font-weight: 500;
      line-height: 1.4;
    }
    .tool-chip.running {
      background: #fef7ef;
      color: #c2410c;
      border: 1px solid #fed7aa;
    }
    .tool-chip.done {
      background: #f0fdf4;
      color: #15803d;
      border: 1px solid #bbf7d0;
    }
    .tool-chip.error {
      background: #fef2f2;
      color: #b91c1c;
      border: 1px solid #fecaca;
    }
    .spinner {
      width: 11px;
      height: 11px;
      border: 1.5px solid #fed7aa;
      border-top-color: #c2410c;
      border-radius: 50%;
      animation: spin 0.7s linear infinite;
    }
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
    .check {
      font-size: 0.75rem;
      line-height: 1;
    }
    .warn {
      font-size: 0.75rem;
      line-height: 1;
    }
    .count {
      opacity: 0.7;
      font-size: 0.6875rem;
    }
  `]
})
export class ToolStatusChipComponent {
  @Input() toolName = '';
  @Input() state: ToolChipState = 'running';
  @Input() resultCount?: number;

  get label(): string {
    const labels = TOOL_LABELS[this.toolName];
    if (!labels) return this.state === 'running' ? 'Ejecutando herramienta...' : 'Herramienta completada';
    return this.state === 'running' ? labels.running : labels.done;
  }
}
