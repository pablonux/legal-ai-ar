import { Component, input, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import type { OntologyClass, GraphNode, GraphEdge, EntityStats } from '../../../models/ontology.models';

@Component({
  selector: 'app-class-detail-panel',
  standalone: true,
  imports: [RouterLink],
  template: `
    @if (selectedNode(); as node) {
      @if (classData(); as cls) {
        <div class="panel">
          <div class="panel-header">
            <span class="category-badge" [class]="node.category">{{ categoryLabel(node.category) }}</span>
            <h3>{{ cls.name }}</h3>
            <code class="namespace">{{ cls.namespace }}</code>
          </div>

          <p class="description">{{ cls.description }}</p>

          @if (entityStat(); as stat) {
            <div class="stats-section">
              <div class="stat-row">
                <span class="stat-label">Instancias en la KB</span>
                <span class="stat-value">{{ formatNumber(stat.totalCount) }}</span>
              </div>
              @if (node.kbRoute) {
                <a [routerLink]="node.kbRoute" class="view-data-link">Ver datos →</a>
              }
              @for (breakdown of stat.breakdowns; track breakdown.taxonomyId) {
                <div class="breakdown">
                  <span class="breakdown-title">{{ breakdown.taxonomyName }}</span>
                  @for (val of breakdown.values; track val.code) {
                    <div class="breakdown-bar">
                      <span class="bar-label">{{ val.label }}</span>
                      <div class="bar-track">
                        <div class="bar-fill" [style.width.%]="barWidth(val.count, stat.totalCount)"></div>
                      </div>
                      <span class="bar-count">{{ val.count }}</span>
                    </div>
                  }
                </div>
              }
            </div>
          }

          @if (outboundEdges().length > 0 || inboundEdges().length > 0) {
            <div class="section">
              <h4>Relaciones</h4>
              @for (edge of outboundEdges(); track edge.id) {
                <div class="relation-row">
                  <span class="relation-arrow">→</span>
                  <span class="relation-label">{{ edge.label }}</span>
                  <span class="relation-target">{{ edge.target }}</span>
                  @if (edge.instanceCount > 0) {
                    <span class="relation-count">{{ formatNumber(edge.instanceCount) }}</span>
                  }
                </div>
              }
              @for (edge of inboundEdges(); track edge.id) {
                <div class="relation-row inbound">
                  <span class="relation-arrow">←</span>
                  <span class="relation-label">{{ edge.label }}</span>
                  <span class="relation-target">{{ edge.source }}</span>
                  @if (edge.instanceCount > 0) {
                    <span class="relation-count">{{ formatNumber(edge.instanceCount) }}</span>
                  }
                </div>
              }
            </div>
          }

          @if (cls.properties.length > 0) {
            <div class="section">
              <h4>Propiedades</h4>
              <table class="props-table">
                <thead>
                  <tr><th>Nombre</th><th>Tipo</th><th>Descripción</th></tr>
                </thead>
                <tbody>
                  @for (prop of cls.properties; track prop.name) {
                    <tr>
                      <td><code>{{ prop.name }}</code></td>
                      <td>{{ prop.type }}</td>
                      <td>{{ prop.description }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          }

          @if (cls.children.length > 0) {
            <div class="section">
              <h4>Subclases</h4>
              <div class="children-list">
                @for (child of cls.children; track child) {
                  <span class="child-badge">{{ child }}</span>
                }
              </div>
            </div>
          }

          @if (cls.parentId) {
            <div class="section">
              <h4>Clase padre</h4>
              <span class="child-badge parent">{{ cls.parentId }}</span>
            </div>
          }
        </div>
      }
    } @else {
      <div class="panel empty">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" class="empty-icon">
          <circle cx="12" cy="12" r="10"/><path d="M12 16v-4"/><path d="M12 8h.01"/>
        </svg>
        <p>Seleccioná un nodo del grafo para ver sus detalles.</p>
      </div>
    }
  `,
  styles: [`
    :host { display: block; height: 100%; overflow-y: auto; }
    .panel { padding: 0; }
    .panel-header { margin-bottom: 0.75rem; }
    .panel-header h3 {
      font-size: 1.125rem;
      font-weight: 700;
      margin: 0.375rem 0 0.125rem;
    }
    .namespace {
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
      background: var(--color-bg-subtle);
      padding: 0.125rem 0.375rem;
      border-radius: var(--radius-xs, 3px);
    }
    .category-badge {
      display: inline-block;
      font-size: 0.625rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      padding: 0.125rem 0.5rem;
      border-radius: 9999px;
      color: #fff;
    }
    .category-badge.core { background: #2563eb; }
    .category-badge.subclass { background: #60a5fa; color: #1e3a5f; }
    .category-badge.kb-entity { background: #ea580c; }
    .category-badge.taxonomy { background: #16a34a; }
    .description {
      font-size: 0.8125rem;
      color: var(--color-text-body);
      line-height: 1.5;
      margin: 0 0 1rem;
    }
    .stats-section {
      background: var(--color-bg-subtle);
      border-radius: var(--radius-sm);
      padding: 0.75rem;
      margin-bottom: 1rem;
    }
    .stat-row {
      display: flex;
      justify-content: space-between;
      align-items: baseline;
      margin-bottom: 0.25rem;
    }
    .stat-label { font-size: 0.75rem; color: var(--color-text-secondary); }
    .stat-value { font-size: 1.25rem; font-weight: 700; color: var(--color-primary); }
    .view-data-link {
      display: inline-block;
      font-size: 0.75rem;
      color: var(--color-primary);
      text-decoration: none;
      margin-bottom: 0.5rem;
    }
    .view-data-link:hover { text-decoration: underline; }
    .breakdown { margin-top: 0.5rem; }
    .breakdown-title {
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-text-secondary);
      text-transform: uppercase;
      letter-spacing: 0.04em;
    }
    .breakdown-bar {
      display: grid;
      grid-template-columns: 1fr 4fr 2.5rem;
      gap: 0.375rem;
      align-items: center;
      font-size: 0.6875rem;
      margin-top: 0.25rem;
    }
    .bar-label { color: var(--color-text-body); white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .bar-track {
      height: 6px;
      background: var(--color-border-input);
      border-radius: 3px;
      overflow: hidden;
    }
    .bar-fill {
      height: 100%;
      background: var(--color-primary);
      border-radius: 3px;
      min-width: 2px;
      transition: width 0.3s ease;
    }
    .bar-count { text-align: right; color: var(--color-text-secondary); font-variant-numeric: tabular-nums; }
    .section {
      margin-bottom: 1rem;
    }
    .section h4 {
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      color: var(--color-text-secondary);
      margin: 0 0 0.375rem;
    }
    .props-table {
      width: 100%;
      border-collapse: collapse;
      font-size: 0.75rem;
    }
    .props-table th {
      text-align: left;
      font-weight: 600;
      color: var(--color-text-secondary);
      padding: 0.25rem 0.375rem;
      border-bottom: 1px solid var(--color-border-input);
    }
    .props-table td {
      padding: 0.25rem 0.375rem;
      border-bottom: 1px solid var(--color-border-input);
      color: var(--color-text-body);
    }
    .props-table code {
      font-size: 0.6875rem;
      background: var(--color-bg-subtle);
      padding: 0.0625rem 0.25rem;
      border-radius: 2px;
    }
    .children-list { display: flex; flex-wrap: wrap; gap: 0.25rem; }
    .child-badge {
      font-size: 0.6875rem;
      padding: 0.125rem 0.5rem;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      color: var(--color-text-body);
    }
    .child-badge.parent { border-color: var(--color-primary); color: var(--color-primary); }
    .relation-row {
      display: flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.25rem 0;
      font-size: 0.75rem;
      border-bottom: 1px solid var(--color-border-input);
    }
    .relation-row:last-child { border-bottom: none; }
    .relation-arrow {
      font-weight: 700;
      color: #b45309;
      flex-shrink: 0;
      width: 1rem;
      text-align: center;
    }
    .relation-row.inbound .relation-arrow { color: #6b7280; }
    .relation-label {
      color: var(--color-text-body);
      font-style: italic;
    }
    .relation-target {
      font-weight: 600;
      color: var(--color-text);
    }
    .relation-count {
      margin-left: auto;
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-primary);
      background: var(--color-primary-light);
      padding: 0.0625rem 0.375rem;
      border-radius: var(--radius-pill);
      font-variant-numeric: tabular-nums;
    }
    .empty {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      color: var(--color-text-secondary);
      text-align: center;
      gap: 0.75rem;
    }
    .empty-icon { opacity: 0.3; }
    .empty p { font-size: 0.8125rem; max-width: 200px; }
  `]
})
export class ClassDetailPanelComponent {
  selectedNode = input<GraphNode | null>(null);
  classes = input<OntologyClass[]>([]);
  stats = input<EntityStats[]>([]);
  edges = input<GraphEdge[]>([]);

  classData = computed(() => {
    const node = this.selectedNode();
    if (!node) return null;
    return this.classes().find(c => c.id === node.id) ?? null;
  });

  outboundEdges = computed(() => {
    const node = this.selectedNode();
    if (!node) return [];
    return this.edges().filter(e => e.type === 'relationship' && e.source === node.id);
  });

  inboundEdges = computed(() => {
    const node = this.selectedNode();
    if (!node) return [];
    return this.edges().filter(e => e.type === 'relationship' && e.target === node.id && e.source !== node.id);
  });

  entityStat = computed(() => {
    const cls = this.classData();
    if (!cls?.kbEntity) return null;
    return this.stats().find(s => s.classId === cls.id) ?? null;
  });

  categoryLabel(category: string): string {
    switch (category) {
      case 'core': return 'Clase principal';
      case 'subclass': return 'Subclase';
      case 'kb-entity': return 'Entidad KB';
      case 'taxonomy': return 'Taxonomía';
      default: return category;
    }
  }

  formatNumber(n: number): string {
    return n.toLocaleString('es-AR');
  }

  barWidth(value: number, total: number): number {
    if (total === 0) return 0;
    return Math.max(2, (value / total) * 100);
  }
}
