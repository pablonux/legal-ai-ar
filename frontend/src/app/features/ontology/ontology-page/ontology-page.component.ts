import { Component, inject, signal, computed } from '@angular/core';
import { OntologyService } from '../../../services/ontology.service';
import { OntologyGraphComponent } from '../ontology-graph/ontology-graph.component';
import { ClassDetailPanelComponent } from '../class-detail-panel/class-detail-panel.component';
import { TaxonomyBrowserComponent } from '../taxonomy-browser/taxonomy-browser.component';
import { LoadingSpinnerComponent } from '@legal-ai-ar/shared-common/components/loading-spinner/loading-spinner.component';
import { forkJoin } from 'rxjs';
import type { OntologyClass, OntologyGraphResponse, GraphNode, EntityStats } from '../../../models/ontology.models';

type ViewMode = 'graph' | 'taxonomies';

@Component({
  selector: 'app-ontology-page',
  standalone: true,
  imports: [
    OntologyGraphComponent,
    ClassDetailPanelComponent,
    TaxonomyBrowserComponent,
    LoadingSpinnerComponent,
  ],
  template: `
    <div class="ontology-page">
      <div class="page-header">
        <h1>Ontología Legal Argentina</h1>
        <p class="subtitle">Modelo ontológico formal del sistema legal argentino — clases, propiedades, relaciones y vocabularios controlados.</p>
        <div class="view-toggle">
          <button type="button" class="toggle-btn" [class.active]="viewMode() === 'graph'" (click)="viewMode.set('graph')">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="18" cy="5" r="3"/><circle cx="6" cy="12" r="3"/><circle cx="18" cy="19" r="3"/><line x1="8.59" y1="13.51" x2="15.42" y2="17.49"/><line x1="15.41" y1="6.51" x2="8.59" y2="10.49"/></svg>
            Grafo
          </button>
          <button type="button" class="toggle-btn" [class.active]="viewMode() === 'taxonomies'" (click)="viewMode.set('taxonomies')">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="8" y1="6" x2="21" y2="6"/><line x1="8" y1="12" x2="21" y2="12"/><line x1="8" y1="18" x2="21" y2="18"/><line x1="3" y1="6" x2="3.01" y2="6"/><line x1="3" y1="12" x2="3.01" y2="12"/><line x1="3" y1="18" x2="3.01" y2="18"/></svg>
            Taxonomías
          </button>
        </div>
      </div>

      @if (state() === 'loading') {
        <div class="loading-state">
          <app-loading-spinner />
          <p>Cargando ontología...</p>
        </div>
      } @else if (state() === 'error') {
        <div class="error-state">
          <p>Error al cargar la ontología.</p>
          <button type="button" class="retry-btn" (click)="load()">Reintentar</button>
        </div>
      } @else {
        @if (viewMode() === 'graph') {
          <div class="graph-layout">
            <div class="graph-main">
              @if (graphData(); as g) {
                <app-ontology-graph [data]="g" (nodeSelected)="onNodeSelected($event)" />
              }
            </div>
            <aside class="detail-aside">
              <app-class-detail-panel
                [selectedNode]="selectedNode()"
                [classes]="classes()"
                [stats]="entityStats()"
                [edges]="graphData()?.edges ?? []" />
            </aside>
          </div>
        } @else {
          <app-taxonomy-browser />
        }
      }
    </div>
  `,
  styles: [`
    .ontology-page { display: flex; flex-direction: column; height: calc(100vh - var(--header-height, 68px) - 6rem); }
    .page-header { margin-bottom: 1rem; flex-shrink: 0; }
    .page-header h1 {
      font-size: 1.375rem;
      font-weight: 700;
      margin: 0 0 0.25rem;
    }
    .subtitle {
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
      margin: 0 0 0.75rem;
    }
    .view-toggle { display: flex; gap: 0; }
    .toggle-btn {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.5rem 1rem;
      border: 1px solid var(--color-border-input);
      background: var(--color-bg-surface);
      color: var(--color-text-body);
      font-size: 0.8125rem;
      font-weight: 500;
      cursor: pointer;
      transition: all var(--transition-fast);
    }
    .toggle-btn:first-child { border-radius: var(--radius-sm) 0 0 var(--radius-sm); }
    .toggle-btn:last-child { border-radius: 0 var(--radius-sm) var(--radius-sm) 0; border-left: none; }
    .toggle-btn.active {
      background: var(--color-primary);
      color: #fff;
      border-color: var(--color-primary);
    }
    .toggle-btn:hover:not(.active) { background: var(--color-bg-subtle); }
    .graph-layout {
      display: grid;
      grid-template-columns: 1fr 320px;
      gap: 1rem;
      flex: 1;
      min-height: 0;
    }
    .graph-main { min-height: 0; display: flex; flex-direction: column; }
    .detail-aside {
      border-left: 1px solid var(--color-border-input);
      padding-left: 1rem;
      overflow-y: auto;
      min-height: 0;
    }
    .loading-state, .error-state {
      flex: 1;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 0.75rem;
      color: var(--color-text-secondary);
      font-size: 0.875rem;
    }
    .retry-btn {
      padding: 0.5rem 1.25rem;
      background: var(--color-primary);
      color: #fff;
      border: none;
      border-radius: var(--radius-sm);
      cursor: pointer;
      font-size: 0.8125rem;
    }
    .retry-btn:hover { opacity: 0.9; }

    @media (max-width: 900px) {
      .graph-layout { grid-template-columns: 1fr; }
      .detail-aside { border-left: none; border-top: 1px solid var(--color-border-input); padding-left: 0; padding-top: 1rem; }
    }
  `]
})
export class OntologyPageComponent {
  private svc = inject(OntologyService);

  state = signal<'loading' | 'loaded' | 'error'>('loading');
  viewMode = signal<ViewMode>('graph');
  classes = signal<OntologyClass[]>([]);
  graphData = signal<OntologyGraphResponse | null>(null);
  entityStats = signal<EntityStats[]>([]);
  selectedNode = signal<GraphNode | null>(null);

  ngOnInit() {
    this.load();
  }

  load() {
    this.state.set('loading');
    forkJoin({
      classes: this.svc.getClasses(),
      graph: this.svc.getGraph(),
      stats: this.svc.getStats(),
    }).subscribe({
      next: ({ classes, graph, stats }) => {
        this.classes.set(classes.classes);
        this.graphData.set(graph);
        this.entityStats.set(stats.entities);
        this.state.set('loaded');
      },
      error: () => {
        this.state.set('error');
      },
    });
  }

  onNodeSelected(node: GraphNode) {
    this.selectedNode.set(node);
  }
}
