import { Component, ViewChild, signal, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { OnboardingService } from '../../../services/onboarding.service';
import { GraphExplorerComponent } from '../graph-explorer/graph-explorer.component';
import { GraphLayersPanelComponent } from '../graph-layers-panel/graph-layers-panel.component';
import { EntitySearchPopoverComponent } from '../entity-search-popover/entity-search-popover.component';
import type { EntitySearchResult, GraphEntityNode, LayerVisibility } from '../../../models/graph-explorer.models';

@Component({
  selector: 'app-graph-explorer-page',
  standalone: true,
  imports: [RouterLink, GraphExplorerComponent, GraphLayersPanelComponent, EntitySearchPopoverComponent],
  template: `
    <div class="explorer-page">
      <header class="page-header">
        <div class="header-left">
          <h1>
            <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
              <circle cx="18" cy="5" r="3"/><circle cx="6" cy="12" r="3"/><circle cx="18" cy="19" r="3"/>
              <line x1="8.59" y1="13.51" x2="15.42" y2="17.49"/><line x1="15.41" y1="6.51" x2="8.59" y2="10.49"/>
            </svg>
            Explorador de Relaciones
          </h1>
        </div>
        <div class="header-search">
          <app-entity-search-popover (entityChosen)="onEntityChosen($event)" />
        </div>
      </header>

      <div class="explorer-body">
        <div class="graph-area" [class.has-start]="hasStarted()">
          @if (!hasStarted()) {
            <div class="empty-state">
              <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.2">
                <circle cx="18" cy="5" r="3"/><circle cx="6" cy="12" r="3"/><circle cx="18" cy="19" r="3"/>
                <line x1="8.59" y1="13.51" x2="15.42" y2="17.49"/><line x1="15.41" y1="6.51" x2="8.59" y2="10.49"/>
              </svg>
              <h2>Explorar relaciones de la KB</h2>
              <p>Buscá una entidad (fallo, juez, tribunal, norma) en el buscador de arriba para empezar a explorar sus relaciones.</p>
            </div>
          }
          <app-graph-explorer
            #graphExplorer
            [initialEntityType]="startEntityType()"
            [initialEntityId]="startEntityId()"
            (entitySelected)="onEntitySelected($event)"
            (nodeCountChanged)="graphCounts = $event" />
        </div>

        <aside class="side-panel">
          <app-graph-layers-panel
            [counts]="graphCounts"
            [selectedNodeId]="selectedNodeId"
            (layerChanged)="onLayerChanged($event)"
            (removeSelected)="onRemoveNode($event)"
            (clearGraph)="onClearGraph()" />

          @if (selectedEntity(); as ent) {
            <div class="entity-detail-card">
              <div class="entity-header">
                <span class="entity-type-badge" [style.background]="entityTypeColor(ent.entityType)">
                  {{ ent.entityType }}
                </span>
                <span class="entity-label">{{ ent.label }}</span>
              </div>
              @if (ent.subtitle) {
                <p class="entity-subtitle">{{ ent.subtitle }}</p>
              }
              @if (ent.entityType === 'ruling') {
                <a class="entity-link" [routerLink]="['/jurisprudencia', extractId(ent.id)]">Ver detalle del fallo →</a>
              }
              @if (ent.entityType === 'court') {
                <a class="entity-link" [routerLink]="['/organismos', extractId(ent.id)]">Ver tribunal →</a>
              }
              @if (ent.entityType === 'person') {
                <a class="entity-link" [routerLink]="['/sujetos', extractId(ent.id)]">Ver persona →</a>
              }
              @if (ent.entityType === 'statute') {
                <a class="entity-link" [routerLink]="['/ordenamiento', extractId(ent.id)]">Ver norma →</a>
              }
              @if (ent.entityType === 'proceeding') {
                <a class="entity-link" [routerLink]="['/procesos', extractId(ent.id)]">Ver proceso →</a>
              }
            </div>
          }
        </aside>
      </div>
    </div>
  `,
  styles: [`
    .explorer-page {
      display: flex;
      flex-direction: column;
      height: calc(100vh - 4rem);
      max-width: 100%;
    }
    .page-header {
      display: flex;
      align-items: center;
      gap: 1.5rem;
      padding-bottom: 1rem;
      border-bottom: 1px solid var(--color-border);
      margin-bottom: 1rem;
    }
    .header-left h1 {
      display: flex;
      align-items: center;
      gap: 8px;
      margin: 0;
      font-size: 1.125rem;
      white-space: nowrap;
    }
    .header-left h1 svg { color: var(--color-primary); }
    .header-search { flex: 1; max-width: 480px; }
    .explorer-body {
      display: grid;
      grid-template-columns: 1fr 280px;
      gap: 1rem;
      flex: 1;
      min-height: 0;
    }
    .graph-area {
      position: relative;
      min-height: 500px;
    }
    .graph-area:not(.has-start) app-graph-explorer { visibility: hidden; }
    .empty-state {
      position: absolute;
      inset: 0;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      text-align: center;
      padding: 2rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      z-index: 5;
    }
    .empty-state svg { color: var(--color-border-input); margin-bottom: 1rem; }
    .empty-state h2 {
      margin: 0 0 0.5rem;
      font-size: 1rem;
      color: var(--color-text);
    }
    .empty-state p {
      margin: 0;
      font-size: 0.875rem;
      color: var(--color-text-secondary);
      max-width: 400px;
    }
    .side-panel {
      display: flex;
      flex-direction: column;
      gap: 1rem;
      overflow-y: auto;
    }
    .entity-detail-card {
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: 1rem;
    }
    .entity-header {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 0.5rem;
    }
    .entity-type-badge {
      padding: 2px 8px;
      border-radius: var(--radius-pill);
      font-size: 0.625rem;
      font-weight: 700;
      text-transform: uppercase;
      color: #fff;
    }
    .entity-label {
      font-size: 0.875rem;
      font-weight: 600;
      color: var(--color-text);
    }
    .entity-subtitle {
      margin: 0 0 0.5rem;
      font-size: 0.75rem;
      color: var(--color-text-secondary);
    }
    .entity-link {
      display: inline-block;
      font-size: 0.75rem;
      font-weight: 500;
      color: var(--color-primary);
      text-decoration: none;
    }
    .entity-link:hover { text-decoration: underline; }
    @media (max-width: 900px) {
      .explorer-body { grid-template-columns: 1fr; }
      .page-header { flex-direction: column; align-items: stretch; }
      .header-search { max-width: 100%; }
    }
  `]
})
export class GraphExplorerPageComponent implements OnInit {
  private onboarding = inject(OnboardingService);
  @ViewChild('graphExplorer') graphExplorer!: GraphExplorerComponent;

  ngOnInit() { this.onboarding.tryShow('explorer-controls'); }

  hasStarted = signal(false);
  startEntityType = signal('');
  startEntityId = signal('');
  selectedEntity = signal<GraphEntityNode | null>(null);
  selectedNodeId = signal<string | null>(null);
  graphCounts = { nodes: 0, edges: 0 };

  private entityColors: Record<string, string> = {
    ruling: '#ea580c', court: '#2563eb', person: '#16a34a', statute: '#7c3aed', keyword: '#6b7280',
  };

  onEntityChosen(result: EntitySearchResult) {
    const [type, id] = this.parseId(result.id);
    if (!this.hasStarted()) {
      this.startEntityType.set(type);
      this.startEntityId.set(id);
      this.hasStarted.set(true);
      setTimeout(() => this.graphExplorer.loadInitial(), 0);
    } else {
      this.graphExplorer.addEntityById(type, id);
    }
  }

  onEntitySelected(entity: GraphEntityNode) {
    this.selectedEntity.set(entity);
    this.selectedNodeId.set(entity.id);
  }

  onLayerChanged(layers: LayerVisibility) {
    this.graphExplorer.setLayerVisibility(layers);
  }

  onRemoveNode(nodeId: string) {
    this.graphExplorer.removeNode(nodeId);
    if (this.selectedNodeId() === nodeId) {
      this.selectedEntity.set(null);
      this.selectedNodeId.set(null);
    }
  }

  onClearGraph() {
    this.graphExplorer.resetGraph();
    this.selectedEntity.set(null);
    this.selectedNodeId.set(null);
  }

  extractId(compositeId: string): string {
    const idx = compositeId.indexOf(':');
    return idx >= 0 ? compositeId.substring(idx + 1) : compositeId;
  }

  entityTypeColor(type: string): string {
    return this.entityColors[type] ?? '#6b7280';
  }

  private parseId(compositeId: string): [string, string] {
    const idx = compositeId.indexOf(':');
    return [compositeId.substring(0, idx), compositeId.substring(idx + 1)];
  }
}
