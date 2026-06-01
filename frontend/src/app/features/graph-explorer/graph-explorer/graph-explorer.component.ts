import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild, signal, inject } from '@angular/core';
import cytoscape, { Core, StylesheetStyle, NodeSingular } from 'cytoscape';
import { GraphExplorerService } from '../../../services/graph-explorer.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import {
  ENTITY_TYPE_CONFIG,
  type GraphEntityNode,
  type GraphEntityEdge,
  type GraphEntityType,
  type LayerVisibility,
} from '../../../models/graph-explorer.models';

@Component({
  selector: 'app-graph-explorer',
  standalone: true,
  imports: [LoadingSpinnerComponent],
  template: `
    <div class="graph-explorer-wrap">
      @if (loading()) {
        <div class="graph-overlay">
          <app-loading-spinner message="Cargando grafo..." />
        </div>
      }
      @if (error()) {
        <div class="graph-overlay error-overlay">
          <p>{{ error() }}</p>
          <button class="retry-btn" (click)="loadInitial()">Reintentar</button>
        </div>
      }
      <div #graphContainer class="graph-container"></div>
      <div class="graph-toolbar">
        <button type="button" title="Ajustar vista" (click)="fitGraph()">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M15 3h6v6"/><path d="M9 21H3v-6"/><path d="M21 3l-7 7"/><path d="M3 21l7-7"/></svg>
        </button>
        <button type="button" title="Redistribuir" (click)="relayout()">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="18" cy="5" r="3"/><circle cx="6" cy="12" r="3"/><circle cx="18" cy="19" r="3"/><line x1="8.59" y1="13.51" x2="15.42" y2="17.49"/><line x1="15.41" y1="6.51" x2="8.59" y2="10.49"/></svg>
        </button>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; width: 100%; height: 100%; }
    .graph-explorer-wrap {
      position: relative;
      width: 100%;
      height: 100%;
      min-height: 400px;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      overflow: hidden;
      background: var(--color-bg-surface);
    }
    .graph-container { width: 100%; height: 100%; }
    .graph-overlay {
      position: absolute;
      inset: 0;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      background: rgba(255,255,255,0.85);
      z-index: 10;
    }
    .error-overlay { color: var(--color-primary); }
    .error-overlay p { margin: 0 0 1rem; font-size: 0.875rem; }
    .retry-btn {
      padding: 0.5rem 1.25rem;
      background: var(--color-primary);
      color: #fff;
      border-radius: var(--radius-sm);
      font-size: 0.8125rem;
      font-weight: 600;
    }
    .retry-btn:hover { background: var(--color-primary-hover); }
    .graph-toolbar {
      position: absolute;
      bottom: 12px;
      right: 12px;
      display: flex;
      gap: 4px;
      z-index: 5;
    }
    .graph-toolbar button {
      width: 32px;
      height: 32px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      color: var(--color-text-secondary);
      cursor: pointer;
      transition: all 0.15s;
    }
    .graph-toolbar button:hover {
      background: var(--color-bg-subtle);
      color: var(--color-text);
    }
  `]
})
export class GraphExplorerComponent implements OnInit, OnDestroy {
  @Input() initialEntityType: string = 'ruling';
  @Input() initialEntityId: string = '';
  @Output() entitySelected = new EventEmitter<GraphEntityNode>();
  @Output() nodeCountChanged = new EventEmitter<{ nodes: number; edges: number }>();
  @ViewChild('graphContainer', { static: true }) containerRef!: ElementRef<HTMLDivElement>;

  private graphService = inject(GraphExplorerService);
  private cy: Core | null = null;
  private nodesMap = new Map<string, GraphEntityNode>();
  private edgesMap = new Map<string, GraphEntityEdge>();
  private expandedNodes = new Set<string>();
  private rootId = '';

  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit() {
    this.initCytoscape();
    if (this.initialEntityId) {
      this.loadInitial();
    }
  }

  ngOnDestroy() {
    this.cy?.destroy();
    this.cy = null;
  }

  loadInitial() {
    if (!this.initialEntityId) return;
    this.loading.set(true);
    this.error.set(null);
    this.nodesMap.clear();
    this.edgesMap.clear();
    this.expandedNodes.clear();
    this.cy?.elements().remove();

    this.graphService.getNeighborhood(this.initialEntityType, this.initialEntityId).subscribe({
      next: (resp) => {
        this.rootId = resp.center.id;
        this.addToGraph(resp.center, resp.nodes, resp.edges);
        this.expandedNodes.add(resp.center.id);
        this.relayout();
        this.loading.set(false);
      },
      error: () => {
        this.error.set('No se pudo cargar el grafo.');
        this.loading.set(false);
      },
    });
  }

  expandNode(nodeId: string) {
    if (this.expandedNodes.has(nodeId)) return;
    const node = this.nodesMap.get(nodeId);
    if (!node) return;

    const [entityType, entityId] = this.parseNodeId(nodeId);
    if (!entityType || !entityId) return;

    const cyNode = this.cy?.getElementById(nodeId) as NodeSingular | undefined;
    cyNode?.addClass('expanding');

    this.graphService.getNeighborhood(entityType, entityId).subscribe({
      next: (resp) => {
        this.expandedNodes.add(nodeId);
        this.addToGraph(null, resp.nodes, resp.edges);
        cyNode?.removeClass('expanding');
        cyNode?.addClass('expanded');
        this.relayout();
      },
      error: () => {
        cyNode?.removeClass('expanding');
      },
    });
  }

  removeNode(nodeId: string) {
    if (nodeId === this.rootId) return;
    this.cy?.getElementById(nodeId).remove();
    this.nodesMap.delete(nodeId);
    this.expandedNodes.delete(nodeId);
    const edgesToRemove: string[] = [];
    this.edgesMap.forEach((e, id) => {
      if (e.source === nodeId || e.target === nodeId) edgesToRemove.push(id);
    });
    edgesToRemove.forEach(id => this.edgesMap.delete(id));
    this.emitCounts();
  }

  resetGraph() {
    const allIds = [...this.nodesMap.keys()].filter(id => id !== this.rootId);
    allIds.forEach(id => this.nodesMap.delete(id));
    this.edgesMap.clear();
    this.expandedNodes.clear();
    this.cy?.elements().remove();
    const rootNode = this.nodesMap.get(this.rootId);
    if (rootNode) {
      this.cy?.add({ group: 'nodes', data: this.toCyNodeData(rootNode) });
    }
    this.emitCounts();
  }

  setLayerVisibility(layers: LayerVisibility) {
    if (!this.cy) return;
    Object.entries(layers.nodes).forEach(([type, visible]) => {
      const selector = `node[entityType="${type}"]`;
      this.cy!.$(selector).style('display', visible ? 'element' : 'none');
    });
    Object.entries(layers.edges).forEach(([type, visible]) => {
      const selector = `edge[edgeType="${type}"]`;
      this.cy!.$(selector).style('display', visible ? 'element' : 'none');
    });
  }

  addEntityById(entityType: string, entityId: string) {
    const fullId = `${entityType}:${entityId}`;
    if (this.nodesMap.has(fullId)) {
      this.cy?.getElementById(fullId).select();
      return;
    }
    this.graphService.getNeighborhood(entityType, entityId).subscribe({
      next: (resp) => {
        this.addToGraph(resp.center, resp.nodes, resp.edges);
        this.expandedNodes.add(resp.center.id);
        this.relayout();
      },
    });
  }

  fitGraph() {
    this.cy?.fit(undefined, 30);
  }

  relayout() {
    if (!this.cy || this.cy.nodes().length === 0) return;
    this.cy.layout({
      name: 'cose',
      animate: true,
      animationDuration: 400,
      nodeRepulsion: () => 8000,
      idealEdgeLength: () => 120,
      gravity: 0.3,
      fit: true,
      padding: 30,
    } as any).run();
  }

  private initCytoscape() {
    this.cy = cytoscape({
      container: this.containerRef.nativeElement,
      style: this.getStylesheet(),
      layout: { name: 'preset' },
      minZoom: 0.15,
      maxZoom: 3,
      wheelSensitivity: 0.3,
    });

    this.cy.on('tap', 'node', (evt) => {
      const node = evt.target;
      const data = node.data();
      this.entitySelected.emit({
        id: data.id,
        entityType: data.entityType,
        label: data.label,
        subtitle: data.subtitle,
        properties: data.properties,
      });
    });

    this.cy.on('dbltap', 'node', (evt) => {
      this.expandNode(evt.target.id());
    });
  }

  private addToGraph(center: GraphEntityNode | null, nodes: GraphEntityNode[], edges: GraphEntityEdge[]) {
    const elementsToAdd: cytoscape.ElementDefinition[] = [];

    if (center && !this.nodesMap.has(center.id)) {
      this.nodesMap.set(center.id, center);
      elementsToAdd.push({ group: 'nodes', data: this.toCyNodeData(center) });
    }

    for (const n of nodes) {
      if (!this.nodesMap.has(n.id)) {
        this.nodesMap.set(n.id, n);
        elementsToAdd.push({ group: 'nodes', data: this.toCyNodeData(n) });
      }
    }

    for (const e of edges) {
      if (!this.edgesMap.has(e.id) && this.nodesMap.has(e.source) && this.nodesMap.has(e.target)) {
        this.edgesMap.set(e.id, e);
        elementsToAdd.push({
          group: 'edges',
          data: { id: e.id, source: e.source, target: e.target, edgeType: e.type, label: e.label || '' },
        });
      }
    }

    if (elementsToAdd.length > 0) {
      this.cy?.add(elementsToAdd);
    }
    this.emitCounts();
  }

  private toCyNodeData(n: GraphEntityNode) {
    return {
      id: n.id,
      entityType: n.entityType,
      label: n.label,
      subtitle: n.subtitle || '',
      properties: n.properties,
      isRoot: n.id === this.rootId,
    };
  }

  private parseNodeId(nodeId: string): [string, string] | [null, null] {
    const idx = nodeId.indexOf(':');
    if (idx < 0) return [null, null];
    return [nodeId.substring(0, idx), nodeId.substring(idx + 1)];
  }

  private emitCounts() {
    this.nodeCountChanged.emit({ nodes: this.nodesMap.size, edges: this.edgesMap.size });
  }

  private getStylesheet(): StylesheetStyle[] {
    const styles: StylesheetStyle[] = [
      {
        selector: 'node',
        style: {
          label: 'data(label)',
          'text-wrap': 'ellipsis',
          'text-max-width': '120px',
          'font-size': '10px',
          'text-valign': 'bottom',
          'text-margin-y': 6,
          color: '#374151',
          'border-width': 2,
          'border-color': '#e5e7eb',
          width: 36,
          height: 36,
        },
      },
      {
        selector: 'node[?isRoot]',
        style: {
          'border-width': 4,
          width: 48,
          height: 48,
          'font-weight': 'bold' as any,
          'font-size': '12px',
        },
      },
      {
        selector: 'node.expanded',
        style: {
          'border-style': 'double' as any,
        },
      },
      {
        selector: 'node.expanding',
        style: {
          'border-style': 'dashed' as any,
          opacity: 0.7,
        },
      },
      {
        selector: 'edge',
        style: {
          'curve-style': 'bezier',
          'target-arrow-shape': 'triangle',
          'arrow-scale': 0.8,
          width: 1.5,
          'font-size': '8px',
          color: '#9ca3af',
          'text-rotation': 'autorotate' as any,
          'text-margin-y': -8,
        },
      },
    ];

    for (const [type, config] of Object.entries(ENTITY_TYPE_CONFIG)) {
      styles.push({
        selector: `node[entityType="${type}"]`,
        style: {
          'background-color': config.color,
          'border-color': config.color,
          shape: config.shape as any,
        },
      });
    }

    const edgeStyles: Record<string, { color: string; style: string }> = {
      cites:        { color: '#ea580c', style: 'solid' },
      citedBy:      { color: '#ea580c', style: 'solid' },
      citesStatute: { color: '#7c3aed', style: 'dashed' },
      signedBy:     { color: '#16a34a', style: 'dashed' },
      issuedBy:     { color: '#2563eb', style: 'solid' },
      hasKeyword:   { color: '#6b7280', style: 'dotted' },
      normRelation: { color: '#7c3aed', style: 'dashed' },
      memberOf:      { color: '#16a34a', style: 'solid' },
      belongsTo:     { color: '#0891b2', style: 'solid' },
      partyOf:       { color: '#0891b2', style: 'dashed' },
      adjudicatedAt: { color: '#2563eb', style: 'dashed' },
    };

    for (const [type, cfg] of Object.entries(edgeStyles)) {
      styles.push({
        selector: `edge[edgeType="${type}"]`,
        style: {
          'line-color': cfg.color,
          'target-arrow-color': cfg.color,
          'line-style': cfg.style as any,
          label: 'data(label)',
        },
      });
    }

    return styles;
  }
}
