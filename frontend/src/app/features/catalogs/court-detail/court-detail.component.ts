import { Component, inject, signal, DestroyRef } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CourtService } from '../../../services/court.service';
import { SkeletonDetailComponent } from '@legal-ai-ar/shared-common/components/skeletons/skeleton-detail.component';
import { BreadcrumbComponent } from '@legal-ai-ar/shared-common/components/breadcrumb/breadcrumb.component';
import { RulingDatePipe } from '@legal-ai-ar/shared-common/pipes/ruling-date.pipe';
import { EntityPreviewDirective } from '@legal-ai-ar/shared-common/directives/entity-preview.directive';
import type { CourtDetail } from '../../../models/catalog.models';

type State = 'loading' | 'loaded' | 'notFound' | 'error';
type Tab = 'info' | 'composicion' | 'jerarquia';

const POSITION_LABELS: Record<string, string> = {
  Unknown: 'Desconocido', Minister: 'Ministro', Conjuez: 'Conjuez',
  CamaristaNacional: 'Camarista Nacional', CamaristaFederal: 'Camarista Federal',
  JuezPrimeraInstancia: 'Juez de 1ra Instancia', JuezDePaz: 'Juez de Paz',
  ProcuradorGeneral: 'Procurador General', FiscalGeneral: 'Fiscal General',
  Fiscal: 'Fiscal', DefensorGeneral: 'Defensor General',
  Defensor: 'Defensor', Secretario: 'Secretario',
};

@Component({
  selector: 'app-court-detail',
  standalone: true,
  imports: [RouterLink, SkeletonDetailComponent, BreadcrumbComponent, RulingDatePipe, EntityPreviewDirective],
  template: `
    @if (state() === 'loading') {
      <app-skeleton-detail />
    }

    @if (state() === 'notFound') {
      <div class="empty-state">
        <p>Tribunal no encontrado.</p>
        <a routerLink="/organismos" class="back-link">Volver a tribunales</a>
      </div>
    }

    @if (state() === 'loaded' && court(); as c) {
      <app-breadcrumb [items]="[{ label: 'Organismos', route: '/organismos' }, { label: c.name }]" />

      <div class="detail-header">
        <h2>{{ c.name }}</h2>
        <div class="detail-badges">
          <span class="badge">{{ c.jurisdictionArea }}</span>
          <span class="badge">{{ c.instance }}</span>
          @if (c.territory) { <span class="badge">{{ c.territory }}</span> }
          @if (c.courtCategory) { <span class="badge">{{ c.courtCategory }}</span> }
          @if (c.fuero) { <span class="badge">{{ c.fuero }}</span> }
          @if (c.instanceLevel != null) { <span class="badge">Nivel {{ c.instanceLevel }}</span> }
          @if (c.governmentLevel) { <span class="badge">{{ c.governmentLevel }}</span> }
        </div>
      </div>

      <div class="stats-row">
        <div class="stat-card"><span class="stat-value">{{ c.rulingCount }}</span><span class="stat-label">Fallos indexados</span></div>
        <div class="stat-card"><span class="stat-value">{{ c.topPersons.length }}</span><span class="stat-label">Participantes</span></div>
        <div class="stat-card"><span class="stat-value">{{ c.judicialOffices.length }}</span><span class="stat-label">Cargos registrados</span></div>
        <div class="stat-card"><span class="stat-value">{{ c.childCourts.length }}</span><span class="stat-label">Subordinados</span></div>
      </div>

      <a [routerLink]="['/jurisprudencia/resultados']" [queryParams]="{ query: '', court: c.name }" class="cta-link">
        Ver todos los fallos de este tribunal
        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="9 18 15 12 9 6"/></svg>
      </a>

      <div class="tabs">
        <button type="button" class="tab" [class.active]="activeTab() === 'info'" (click)="activeTab.set('info')">Información</button>
        <button type="button" class="tab" [class.active]="activeTab() === 'composicion'" (click)="activeTab.set('composicion')">Composición ({{ c.judicialOffices.length }})</button>
        <button type="button" class="tab" [class.active]="activeTab() === 'jerarquia'" (click)="activeTab.set('jerarquia')">Jerarquía</button>
      </div>

      @if (activeTab() === 'info') {
        <div class="tab-content">
          @if (c.topPersons.length) {
            <section class="section">
              <h3>Personas que participaron</h3>
              <div class="persons-grid">
                @for (p of c.topPersons; track p.id) {
                  <a [routerLink]="['/sujetos', p.id]" [entityPreview]="{ type: 'person', id: p.id }" class="person-card hover-lift">
                    <span class="person-avatar">{{ p.displayName[0] }}</span>
                    <div class="person-info">
                      <span class="person-name">{{ p.displayName }}</span>
                      <span class="person-count">{{ p.rulingCount }} fallos</span>
                    </div>
                  </a>
                }
              </div>
            </section>
          } @else {
            <div class="empty-tab"><p>No hay participantes registrados.</p></div>
          }
        </div>
      }

      @if (activeTab() === 'composicion') {
        <div class="tab-content">
          @if (c.judicialOffices.length) {
            @if (currentOffices(c).length) {
              <section class="section">
                <h3>Composición actual</h3>
                @for (o of currentOffices(c); track o.personId) {
                  <a [routerLink]="['/sujetos', o.personId]" [entityPreview]="{ type: 'person', id: o.personId }" class="office-row hover-lift">
                    <span class="office-position">{{ positionLabel(o.position) }}</span>
                    <span class="office-name">{{ o.personName }}</span>
                    @if (o.startDate) { <span class="office-date">desde {{ o.startDate | rulingDate }}</span> }
                    <span class="current-badge">Activo</span>
                  </a>
                }
              </section>
            }
            @if (pastOffices(c).length) {
              <section class="section">
                <h3>Histórico</h3>
                @for (o of pastOffices(c); track o.personId + '-' + o.startDate) {
                  <a [routerLink]="['/sujetos', o.personId]" [entityPreview]="{ type: 'person', id: o.personId }" class="office-row hover-lift past">
                    <span class="office-position">{{ positionLabel(o.position) }}</span>
                    <span class="office-name">{{ o.personName }}</span>
                    <span class="office-date">
                      @if (o.startDate && o.endDate) { {{ o.startDate | rulingDate }} – {{ o.endDate | rulingDate }} }
                      @else if (o.startDate) { desde {{ o.startDate | rulingDate }} }
                    </span>
                  </a>
                }
              </section>
            }
          } @else {
            <div class="empty-tab"><p>No hay cargos registrados para este organismo.</p></div>
          }
        </div>
      }

      @if (activeTab() === 'jerarquia') {
        <div class="tab-content">
          <div class="hierarchy">
            @if (c.parentCourt) {
              <section class="section">
                <h3>Organismo superior</h3>
                <a [routerLink]="['/organismos', c.parentCourt.id]" [entityPreview]="{ type: 'court', id: c.parentCourt.id }" class="hierarchy-node parent hover-lift">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 21h18"/><path d="M5 21V7l8-4v18"/><path d="M19 21V11l-6-4"/></svg>
                  <div>
                    <span class="hier-name">{{ c.parentCourt.name }}</span>
                    @if (c.parentCourt.instance) { <span class="hier-meta">{{ c.parentCourt.instance }}</span> }
                  </div>
                </a>
              </section>
            }
            <section class="section">
              <h3>Este organismo</h3>
              <div class="hierarchy-node current">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 21h18"/><path d="M5 21V7l8-4v18"/><path d="M19 21V11l-6-4"/></svg>
                <div>
                  <span class="hier-name">{{ c.name }}</span>
                  <span class="hier-meta">{{ c.instance }} · Nivel {{ c.instanceLevel ?? '—' }}</span>
                </div>
              </div>
            </section>
            @if (c.childCourts.length) {
              <section class="section">
                <h3>Organismos subordinados ({{ c.childCourts.length }})</h3>
                @for (child of c.childCourts; track child.id) {
                  <a [routerLink]="['/organismos', child.id]" [entityPreview]="{ type: 'court', id: child.id }" class="hierarchy-node child hover-lift">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 21h18"/><path d="M5 21V7l8-4v18"/><path d="M19 21V11l-6-4"/></svg>
                    <div>
                      <span class="hier-name">{{ child.name }}</span>
                      @if (child.instance) { <span class="hier-meta">{{ child.instance }}</span> }
                    </div>
                  </a>
                }
              </section>
            }
            @if (!c.parentCourt && !c.childCourts.length) {
              <div class="empty-tab"><p>No hay relaciones jerárquicas registradas.</p></div>
            }
          </div>
        </div>
      }
    }
  `,
  styles: [`
    .detail-header { margin-bottom: 1.25rem; }
    .detail-header h2 { font-size: 1.25rem; font-weight: 700; margin: 0 0 0.5rem; }
    .detail-badges { display: flex; gap: 6px; flex-wrap: wrap; }
    .badge { display: inline-block; padding: 3px 10px; background: var(--color-bg-subtle); border: 1px solid var(--color-border); border-radius: var(--radius-pill); font-size: 0.6875rem; font-weight: 600; color: var(--color-text-secondary); }

    .stats-row { display: flex; gap: 1rem; margin-bottom: 1.25rem; flex-wrap: wrap; }
    .stat-card { flex: 1; min-width: 120px; max-width: 200px; padding: 1rem; background: var(--color-bg-surface); border: 1px solid var(--color-border); border-radius: var(--radius-md); text-align: center; }
    .stat-value { display: block; font-size: 1.5rem; font-weight: 700; color: var(--color-primary); }
    .stat-label { display: block; font-size: 0.6875rem; color: var(--color-text-secondary); margin-top: 2px; text-transform: uppercase; letter-spacing: 0.5px; }

    .cta-link { display: inline-flex; align-items: center; gap: 4px; color: var(--color-primary); font-size: 0.8125rem; font-weight: 600; text-decoration: none; margin-bottom: 1.25rem; }
    .cta-link:hover { text-decoration: underline; }

    .tabs { display: flex; gap: 0; border-bottom: 1px solid var(--color-border); margin-bottom: 1rem; }
    .tab { padding: 0.625rem 1rem; background: none; border: none; border-bottom: 2px solid transparent; font-size: 0.8125rem; font-weight: 500; color: var(--color-text-secondary); cursor: pointer; font-family: inherit; transition: all 0.15s; }
    .tab:hover { color: var(--color-text); }
    .tab.active { border-bottom-color: var(--color-primary); color: var(--color-primary); font-weight: 600; }

    .section { margin-top: 1rem; }
    .section h3 { font-size: 0.75rem; font-weight: 700; text-transform: uppercase; letter-spacing: 0.8px; color: var(--color-text-secondary); margin: 0 0 0.75rem; }

    .persons-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(240px, 1fr)); gap: 0.5rem; }
    .person-card { display: flex; align-items: center; gap: 10px; padding: 10px 12px; background: var(--color-bg-surface); border: 1px solid var(--color-border); border-radius: var(--radius-sm); text-decoration: none; color: var(--color-text); transition: border-color 0.15s; }
    .person-card:hover { border-color: rgba(208, 74, 2, 0.3); }
    .person-avatar { width: 32px; height: 32px; border-radius: 50%; background: var(--color-bg-subtle); color: var(--color-text-secondary); display: flex; align-items: center; justify-content: center; font-size: 0.75rem; font-weight: 600; flex-shrink: 0; }
    .person-info { display: flex; flex-direction: column; }
    .person-name { font-size: 0.8125rem; font-weight: 500; }
    .person-count { font-size: 0.6875rem; color: var(--color-text-secondary); }

    /* Composición */
    .office-row { display: flex; align-items: center; gap: 0.75rem; padding: 0.625rem 1rem; border-bottom: 1px solid var(--color-border); text-decoration: none; color: var(--color-text); font-size: 0.8125rem; }
    .office-row:last-child { border-bottom: none; }
    .office-row.past { opacity: 0.7; }
    .office-position { font-size: 0.6875rem; font-weight: 600; color: var(--color-text-secondary); min-width: 130px; }
    .office-name { font-weight: 500; flex: 1; color: var(--color-primary); }
    .office-date { font-size: 0.6875rem; color: var(--color-text-secondary); }
    .current-badge { display: inline-block; padding: 1px 6px; border-radius: var(--radius-xs); font-size: 0.625rem; font-weight: 700; background: rgba(22,163,74,.08); border: 1px solid rgba(22,163,74,.2); color: #16a34a; }

    /* Jerarquía */
    .hierarchy-node { display: flex; align-items: center; gap: 0.75rem; padding: 0.75rem 1rem; border: 1px solid var(--color-border); border-radius: var(--radius-sm); background: var(--color-bg-surface); margin-bottom: 0.5rem; text-decoration: none; color: var(--color-text); }
    .hierarchy-node svg { color: var(--color-text-secondary); flex-shrink: 0; }
    .hierarchy-node.current { border-color: var(--color-primary); border-left: 3px solid var(--color-primary); }
    .hierarchy-node.parent svg { color: var(--color-primary); }
    .hier-name { display: block; font-size: 0.8125rem; font-weight: 500; }
    .hier-meta { display: block; font-size: 0.6875rem; color: var(--color-text-secondary); }

    .empty-state { text-align: center; padding: 3rem; color: var(--color-text-secondary); }
    .empty-tab { text-align: center; padding: 2rem; color: var(--color-text-secondary); font-size: 0.875rem; }
    .back-link { color: var(--color-primary); text-decoration: none; font-size: 0.875rem; }
    .back-link:hover { text-decoration: underline; }
  `]
})
export class CourtDetailComponent {
  private route = inject(ActivatedRoute);
  private courtService = inject(CourtService);
  private destroyRef = inject(DestroyRef);

  court = signal<CourtDetail | null>(null);
  state = signal<State>('loading');
  activeTab = signal<Tab>('info');

  constructor() {
    this.route.paramMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        const id = Number(params.get('id'));
        if (!id) { this.state.set('notFound'); return; }
        this.activeTab.set('info');
        this.loadCourt(id);
      });
  }

  private loadCourt(id: number) {
    this.state.set('loading');
    this.courtService.getById(id).subscribe({
      next: c => { this.court.set(c); this.state.set('loaded'); },
      error: (err: { status?: number }) => {
        this.state.set(err?.status === 404 ? 'notFound' : 'error');
      }
    });
  }

  currentOffices(c: CourtDetail) { return c.judicialOffices.filter(o => o.isCurrent); }
  pastOffices(c: CourtDetail) { return c.judicialOffices.filter(o => !o.isCurrent); }
  positionLabel(pos: string) { return POSITION_LABELS[pos] ?? pos; }
}
