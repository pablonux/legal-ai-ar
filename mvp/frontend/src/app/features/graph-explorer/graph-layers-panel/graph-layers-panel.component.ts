import { Component, EventEmitter, Input, Output, signal, computed } from '@angular/core';
import {
  ENTITY_TYPE_CONFIG,
  EDGE_TYPE_CONFIG,
  type GraphEntityType,
  type LayerVisibility,
} from '../../../models/graph-explorer.models';

@Component({
  selector: 'app-graph-layers-panel',
  standalone: true,
  template: `
    <div class="layers-panel">
      <div class="panel-header">
        <h3>Capas del grafo</h3>
        <span class="counter">{{ nodeCount() }} nodos · {{ edgeCount() }} rel.</span>
      </div>

      <div class="section">
        <h4>Tipos de nodo</h4>
        @for (entry of nodeTypes; track entry.type) {
          <label class="layer-toggle">
            <input type="checkbox" [checked]="nodeVisibility()[entry.type]"
                   (change)="toggleNode(entry.type, $any($event.target).checked)" />
            <span class="color-dot" [style.background]="entry.config.color"></span>
            {{ entry.config.label }}
          </label>
        }
      </div>

      <div class="section">
        <h4>Tipos de relación</h4>
        @for (entry of edgeTypes; track entry.type) {
          <label class="layer-toggle">
            <input type="checkbox" [checked]="edgeVisibility()[entry.type]"
                   (change)="toggleEdge(entry.type, $any($event.target).checked)" />
            <span class="edge-indicator" [style.border-bottom-color]="entry.config.color"
                  [style.border-bottom-style]="entry.config.style"></span>
            {{ entry.config.label }}
          </label>
        }
      </div>

      <div class="section actions">
        @if (selectedNodeId()) {
          <button class="action-btn danger" (click)="removeSelected.emit(selectedNodeId()!)">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
            Quitar nodo seleccionado
          </button>
        }
        <button class="action-btn" (click)="clearGraph.emit()">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="1 4 1 10 7 10"/><path d="M3.51 15a9 9 0 1 0 2.13-9.36L1 10"/></svg>
          Limpiar grafo
        </button>
      </div>
    </div>
  `,
  styles: [`
    .layers-panel {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: 1rem;
      font-size: 0.8125rem;
    }
    .panel-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 1rem;
      padding-bottom: 0.75rem;
      border-bottom: 1px solid var(--color-border);
    }
    .panel-header h3 {
      margin: 0;
      font-size: 0.75rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.8px;
      color: var(--color-text-secondary);
    }
    .counter {
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      background: var(--color-bg-subtle);
      padding: 2px 8px;
      border-radius: var(--radius-pill);
    }
    .section { margin-bottom: 1rem; }
    .section h4 {
      margin: 0 0 0.5rem;
      font-size: 0.6875rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      color: var(--color-text-secondary);
    }
    .layer-toggle {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 4px 0;
      cursor: pointer;
      color: var(--color-text-body);
    }
    .layer-toggle input[type="checkbox"] {
      width: 14px;
      height: 14px;
      accent-color: var(--color-primary);
      cursor: pointer;
    }
    .color-dot {
      width: 10px;
      height: 10px;
      border-radius: 50%;
      flex-shrink: 0;
    }
    .edge-indicator {
      width: 18px;
      border-bottom-width: 2px;
      flex-shrink: 0;
    }
    .actions {
      padding-top: 0.75rem;
      border-top: 1px solid var(--color-border);
      display: flex;
      flex-direction: column;
      gap: 6px;
    }
    .action-btn {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 6px 10px;
      font-size: 0.75rem;
      font-weight: 500;
      color: var(--color-text-secondary);
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      cursor: pointer;
      transition: all 0.15s;
    }
    .action-btn:hover {
      background: var(--color-bg-surface);
      color: var(--color-text);
    }
    .action-btn.danger { color: #dc2626; }
    .action-btn.danger:hover { background: #fef2f2; }
  `]
})
export class GraphLayersPanelComponent {
  @Input() set counts(value: { nodes: number; edges: number }) {
    this.nodeCount.set(value.nodes);
    this.edgeCount.set(value.edges);
  }
  @Input() selectedNodeId = signal<string | null>(null);
  @Output() layerChanged = new EventEmitter<LayerVisibility>();
  @Output() removeSelected = new EventEmitter<string>();
  @Output() clearGraph = new EventEmitter<void>();

  nodeCount = signal(0);
  edgeCount = signal(0);

  nodeTypes = Object.entries(ENTITY_TYPE_CONFIG).map(([type, config]) => ({ type: type as GraphEntityType, config }));
  edgeTypes = Object.entries(EDGE_TYPE_CONFIG).map(([type, config]) => ({ type, config }));

  private _nodeVis: Record<string, boolean> = Object.fromEntries(this.nodeTypes.map(t => [t.type, true]));
  private _edgeVis: Record<string, boolean> = Object.fromEntries(this.edgeTypes.map(t => [t.type, true]));

  nodeVisibility = signal<Record<string, boolean>>({ ...this._nodeVis });
  edgeVisibility = signal<Record<string, boolean>>({ ...this._edgeVis });

  toggleNode(type: string, visible: boolean) {
    this._nodeVis[type] = visible;
    this.nodeVisibility.set({ ...this._nodeVis });
    this.emitLayers();
  }

  toggleEdge(type: string, visible: boolean) {
    this._edgeVis[type] = visible;
    this.edgeVisibility.set({ ...this._edgeVis });
    this.emitLayers();
  }

  private emitLayers() {
    this.layerChanged.emit({
      nodes: this._nodeVis as Record<GraphEntityType, boolean>,
      edges: this._edgeVis,
    });
  }
}
