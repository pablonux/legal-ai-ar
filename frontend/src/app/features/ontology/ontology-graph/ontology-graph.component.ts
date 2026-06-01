import { Component, ElementRef, ViewChild, AfterViewInit, OnDestroy, inject, input, output, effect } from '@angular/core';
import { Router } from '@angular/router';
import cytoscape, { Core, StylesheetStyle } from 'cytoscape';
import type { OntologyGraphResponse, GraphNode } from '../../../models/ontology.models';

@Component({
  selector: 'app-ontology-graph',
  standalone: true,
  template: `
    <div class="graph-toolbar">
      <button type="button" class="toolbar-btn" [class.active]="currentLayout === 'dagre'" (click)="setLayout('dagre')">Jerárquico</button>
      <button type="button" class="toolbar-btn" [class.active]="currentLayout === 'cose'" (click)="setLayout('cose')">Orgánico</button>
      <button type="button" class="toolbar-btn" (click)="fit()">
        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M15 3h6v6"/><path d="M9 21H3v-6"/><path d="M21 3l-7 7"/><path d="M3 21l7-7"/></svg>
        Ajustar
      </button>
    </div>
    <div #graphContainer class="graph-container"></div>
    <div class="graph-legend">
      <span class="legend-item"><span class="legend-dot core"></span> Clase principal</span>
      <span class="legend-item"><span class="legend-dot kb-entity"></span> Entidad KB (navegar)</span>
      <span class="legend-item"><span class="legend-dot taxonomy"></span> Taxonomía</span>
      <span class="legend-item"><span class="legend-dot coming-soon"></span> Próximamente</span>
      <span class="legend-hint">Doble-click en un nodo para navegar</span>
    </div>
  `,
  styles: [`
    :host { display: flex; flex-direction: column; height: 100%; }
    .graph-toolbar {
      display: flex;
      gap: 0.375rem;
      padding: 0.5rem 0;
      flex-shrink: 0;
    }
    .toolbar-btn {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.375rem 0.75rem;
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      background: var(--color-bg-surface);
      color: var(--color-text-body);
      font-size: 0.75rem;
      cursor: pointer;
      transition: all var(--transition-fast);
    }
    .toolbar-btn:hover { background: var(--color-bg-subtle); }
    .toolbar-btn.active {
      background: var(--color-primary);
      color: #fff;
      border-color: var(--color-primary);
    }
    .graph-container {
      flex: 1;
      min-height: 0;
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-md);
      background: var(--color-bg-page);
    }
    .graph-legend {
      display: flex;
      gap: 1rem;
      padding: 0.5rem 0;
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      flex-shrink: 0;
    }
    .legend-item { display: flex; align-items: center; gap: 0.25rem; }
    .legend-dot {
      width: 10px;
      height: 10px;
      border-radius: 2px;
      flex-shrink: 0;
    }
    .legend-dot.core { background: #2563eb; }
    .legend-dot.subclass { background: #60a5fa; }
    .legend-dot.kb-entity { background: #ea580c; }
    .legend-dot.taxonomy { background: #16a34a; }
    .legend-dot.coming-soon { background: #d1d5db; border: 1px dashed #9ca3af; }
    .legend-hint {
      font-size: 0.625rem;
      color: var(--color-text-secondary);
      opacity: 0.7;
      margin-left: auto;
    }
  `]
})
export class OntologyGraphComponent implements AfterViewInit, OnDestroy {
  data = input.required<OntologyGraphResponse>();
  nodeSelected = output<GraphNode>();

  @ViewChild('graphContainer', { static: true }) containerRef!: ElementRef<HTMLElement>;
  private router = inject(Router);

  currentLayout: 'dagre' | 'cose' = 'dagre';
  private cy: Core | null = null;

  constructor() {
    effect(() => {
      const d = this.data();
      if (this.cy && d) {
        this.renderGraph(d);
      }
    });
  }

  ngAfterViewInit() {
    this.initCytoscape();
    const d = this.data();
    if (d) {
      this.renderGraph(d);
    }
  }

  ngOnDestroy() {
    this.cy?.destroy();
  }

  setLayout(layout: 'dagre' | 'cose') {
    this.currentLayout = layout;
    this.runLayout();
  }

  fit() {
    this.cy?.fit(undefined, 40);
  }

  private initCytoscape() {
    this.cy = cytoscape({
      container: this.containerRef.nativeElement,
      style: this.getStylesheet(),
      minZoom: 0.2,
      maxZoom: 3,
      wheelSensitivity: 0.3,
    });

    this.cy.on('tap', 'node', (evt) => {
      const nodeData = evt.target.data();
      this.nodeSelected.emit({
        id: nodeData.id,
        label: nodeData.label,
        category: nodeData.category,
        instanceCount: nodeData.instanceCount ?? 0,
        kbRoute: nodeData.kbRoute ?? null,
      });
    });

    this.cy.on('dbltap', 'node', (evt) => {
      const route = evt.target.data().kbRoute;
      if (route) this.router.navigate([route]);
    });
  }

  private renderGraph(data: OntologyGraphResponse) {
    if (!this.cy) return;

    this.cy.elements().remove();

    const elements: cytoscape.ElementDefinition[] = [];

    for (const node of data.nodes) {
      const size = this.nodeSize(node.instanceCount, node.category);
      const hasRoute = !!node.kbRoute;
      const label = hasRoute
        ? `${node.label}\n(${node.instanceCount.toLocaleString('es-AR')})`
        : node.label;
      elements.push({
        group: 'nodes',
        data: {
          id: node.id,
          label,
          category: node.category,
          instanceCount: node.instanceCount,
          kbRoute: node.kbRoute,
          hasRoute: hasRoute ? 'yes' : 'no',
          nodeWidth: size,
          nodeHeight: node.category === 'taxonomy' ? size * 0.7 : size * 0.45,
        },
      });
    }

    for (const edge of data.edges) {
      const countLabel = edge.type === 'relationship' && edge.instanceCount > 0
        ? `${edge.label} (${edge.instanceCount.toLocaleString('es-AR')})`
        : (edge.type === 'relationship' ? edge.label : '');
      const edgeWidth = edge.type === 'relationship' && edge.instanceCount > 0
        ? Math.min(1 + Math.log10(edge.instanceCount + 1) * 1.5, 6)
        : 1;
      elements.push({
        group: 'edges',
        data: {
          id: edge.id,
          source: edge.source,
          target: edge.target,
          edgeType: edge.type,
          label: countLabel,
          instanceCount: edge.instanceCount ?? 0,
          edgeWidth,
        },
      });
    }

    this.cy.add(elements);
    this.runLayout();
  }

  private runLayout() {
    if (!this.cy) return;

    const options = this.currentLayout === 'dagre'
      ? {
          name: 'breadthfirst' as const,
          directed: true,
          spacingFactor: 1.2,
          avoidOverlap: true,
          animate: true,
          animationDuration: 400,
        }
      : {
          name: 'cose' as const,
          idealEdgeLength: () => 120,
          nodeOverlap: 20,
          animate: true,
          animationDuration: 600,
          randomize: false,
          gravity: 0.25,
        };

    this.cy.layout(options).run();
    setTimeout(() => this.cy?.fit(undefined, 40), this.currentLayout === 'dagre' ? 450 : 700);
  }

  private nodeSize(count: number, category: string): number {
    if (category === 'taxonomy') return 50;
    const base = 80;
    if (count <= 0) return base;
    return base + Math.log10(count + 1) * 30;
  }

  private getStylesheet(): StylesheetStyle[] {
    return [
      {
        selector: 'node',
        style: {
          'label': 'data(label)',
          'text-valign': 'center',
          'text-halign': 'center',
          'font-size': '9px',
          'font-family': 'Arial, sans-serif',
          'text-wrap': 'wrap',
          'text-max-width': '70px',
          'width': 'data(nodeWidth)',
          'height': 'data(nodeHeight)',
          'border-width': 1,
          'border-color': '#d1d5db',
        },
      },
      {
        selector: 'node[category="core"]',
        style: {
          'background-color': '#2563eb',
          'color': '#ffffff',
          'shape': 'round-rectangle',
          'font-weight': 'bold' as const,
          'font-size': '10px',
        },
      },
      {
        selector: 'node[category="subclass"]',
        style: {
          'background-color': '#60a5fa',
          'color': '#1e3a5f',
          'shape': 'round-rectangle',
        },
      },
      {
        selector: 'node[category="kb-entity"]',
        style: {
          'background-color': '#ea580c',
          'color': '#ffffff',
          'shape': 'ellipse',
          'font-weight': 'bold' as const,
          'font-size': '10px',
        },
      },
      {
        selector: 'node[category="taxonomy"]',
        style: {
          'background-color': '#16a34a',
          'color': '#ffffff',
          'shape': 'diamond',
          'font-size': '8px',
        },
      },
      {
        selector: 'node[hasRoute="yes"]',
        style: {
          'border-width': 2,
          'border-color': '#D04A02',
        },
      },
      {
        selector: 'node[hasRoute="no"][category="kb-entity"]',
        style: {
          'background-color': '#d1d5db',
          'color': '#6b7280',
          'border-style': 'dashed',
          'border-color': '#9ca3af',
        },
      },
      {
        selector: 'node:selected',
        style: {
          'border-width': 3,
          'border-color': '#D04A02',
          'overlay-opacity': 0.1,
          'overlay-color': '#D04A02',
        },
      },
      {
        selector: 'edge[edgeType="is-a"]',
        style: {
          'line-color': '#9ca3af',
          'width': 1.5,
          'curve-style': 'bezier',
          'target-arrow-shape': 'triangle',
          'target-arrow-color': '#9ca3af',
          'arrow-scale': 0.8,
        },
      },
      {
        selector: 'edge[edgeType="relationship"]',
        style: {
          'line-color': '#d97706',
          'line-style': 'dashed',
          'width': 'data(edgeWidth)',
          'curve-style': 'bezier',
          'target-arrow-shape': 'vee',
          'target-arrow-color': '#d97706',
          'arrow-scale': 0.7,
          'label': 'data(label)',
          'font-size': '7px',
          'color': '#92400e',
          'text-rotation': 'autorotate',
          'text-background-color': '#ffffff',
          'text-background-opacity': 0.8,
          'text-background-padding': '2px',
        },
      },
      {
        selector: 'edge[instanceCount > 0]',
        style: {
          'line-style': 'solid',
          'line-color': '#b45309',
          'target-arrow-color': '#b45309',
        },
      },
    ];
  }
}
