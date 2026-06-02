import { Component, inject, signal, computed, DestroyRef } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PersonService } from '../../../services/person.service';
import { SkeletonDetailComponent } from '@legal-ai-ar/shared-common/components/skeletons/skeleton-detail.component';
import { RulingDatePipe } from '@legal-ai-ar/shared-common/pipes/ruling-date.pipe';
import { BreadcrumbComponent } from '@legal-ai-ar/shared-common/components/breadcrumb/breadcrumb.component';
import { EntityPreviewDirective } from '@legal-ai-ar/shared-common/directives/entity-preview.directive';
import type { PersonDetail } from '../../../models/catalog.models';

type State = 'loading' | 'loaded' | 'notFound' | 'error';
type Tab = 'fallos' | 'carrera' | 'procesos';

const ROLE_LABELS: Record<string, string> = {
  SIGNATORY: 'Firmante', DISSENT: 'Disidente', MAJORITY: 'Mayoría',
  MAJORITY_AUTHOR: 'Autor del voto', CONCURRENCE: 'Concurrencia',
  PROSECUTOR: 'Fiscal', PUBLIC_DEFENDER: 'Defensor',
};

const PARTY_ROLE_LABELS: Record<string, string> = {
  PLAINTIFF: 'Actor', DEFENDANT: 'Demandado', THIRD_PARTY: 'Tercero', AMICUS_CURIAE: 'Amicus Curiae',
};

const POSITION_LABELS: Record<string, string> = {
  Unknown: 'Desconocido', Minister: 'Ministro', Conjuez: 'Conjuez',
  CamaristaNacional: 'Camarista Nacional', CamaristaFederal: 'Camarista Federal',
  JuezPrimeraInstancia: 'Juez de 1ra Instancia', JuezDePaz: 'Juez de Paz',
  ProcuradorGeneral: 'Procurador General', FiscalGeneral: 'Fiscal General',
  Fiscal: 'Fiscal', DefensorGeneral: 'Defensor General',
  Defensor: 'Defensor', Secretario: 'Secretario',
};

const TYPE_LABELS: Record<string, string> = {
  Physical: 'Persona física', LegalPublic: 'Persona jurídica pública',
  LegalPrivate: 'Persona jurídica privada', StateEntity: 'Entidad estatal',
  Indeterminate: 'Indeterminado',
};

@Component({
  selector: 'app-person-detail',
  standalone: true,
  imports: [RouterLink, SkeletonDetailComponent, RulingDatePipe, BreadcrumbComponent, EntityPreviewDirective],
  template: `
    @if (state() === 'loading') { <app-skeleton-detail /> }
    @if (state() === 'notFound') {
      <div class="empty-state"><p>Persona no encontrada.</p><a routerLink="/sujetos" class="back-link">Volver</a></div>
    }

    @if (state() === 'loaded' && person(); as p) {
      <app-breadcrumb [items]="[{ label: 'Consulta' }, { label: 'Intervinientes', route: '/sujetos' }, { label: p.displayName }]" />

      <div class="detail-header">
        <div class="person-hero">
          <span class="hero-avatar" [class.legal]="p.personType !== 'Physical'">{{ p.displayName[0] }}</span>
          <div>
            <h2>{{ p.displayName }}</h2>
            <div class="hero-meta">
              <span class="type-badge" [attr.data-type]="p.personType">{{ typeLabel(p.personType) }}</span>
              @if (p.legalEntityType) { <span class="type-badge sub">{{ p.legalEntityType }}</span> }
              @if (p.courtName) { <span class="court-label">{{ p.courtName }}</span> }
            </div>
          </div>
        </div>
      </div>

      <div class="stats-row">
        <div class="stat-card"><span class="stat-value">{{ p.rulingCount }}</span><span class="stat-label">Fallos</span></div>
        @if (p.judicialOffices.length) {
          <div class="stat-card"><span class="stat-value">{{ p.judicialOffices.length }}</span><span class="stat-label">Cargos</span></div>
        }
        @if (p.proceedings.length) {
          <div class="stat-card"><span class="stat-value">{{ p.proceedings.length }}</span><span class="stat-label">Procesos</span></div>
        }
      </div>

      <div class="tabs">
        <button type="button" class="tab" [class.active]="activeTab() === 'fallos'" (click)="activeTab.set('fallos')">Fallos ({{ p.recentRulings.length }})</button>
        @if (showCarrera()) {
          <button type="button" class="tab" [class.active]="activeTab() === 'carrera'" (click)="activeTab.set('carrera')">Carrera judicial ({{ p.judicialOffices.length }})</button>
        }
        @if (p.proceedings.length) {
          <button type="button" class="tab" [class.active]="activeTab() === 'procesos'" (click)="activeTab.set('procesos')">Procesos ({{ p.proceedings.length }})</button>
        }
      </div>

      @if (activeTab() === 'fallos') {
        <div class="tab-content">
          @if (p.recentRulings.length) {
            <div class="rulings-list">
              @for (r of p.recentRulings; track r.rulingId) {
                <a [routerLink]="['/jurisprudencia', r.rulingId]" [entityPreview]="{ type: 'ruling', id: r.rulingId }" class="ruling-row hover-lift">
                  <div class="ruling-main">
                    <span class="ruling-title">{{ r.caseTitle }}</span>
                    <span class="ruling-meta">{{ r.rulingDate | rulingDate }}@if (r.instance) { · {{ r.instance }} }</span>
                  </div>
                  <span class="participation-badge">{{ roleLabel(r.rulingRole) }}</span>
                </a>
              }
            </div>
          } @else {
            <div class="empty-tab"><p>No hay fallos registrados.</p></div>
          }
        </div>
      }

      @if (activeTab() === 'carrera') {
        <div class="tab-content">
          @for (o of p.judicialOffices; track o.courtId + '-' + o.startDate) {
            <a [routerLink]="['/organismos', o.courtId]" [entityPreview]="{ type: 'court', id: o.courtId }" class="office-row hover-lift">
              <span class="office-position">{{ positionLabel(o.position) }}</span>
              <span class="office-court">{{ o.courtName }}</span>
              <span class="office-date">
                @if (o.startDate && o.endDate) { {{ o.startDate | rulingDate }} – {{ o.endDate | rulingDate }} }
                @else if (o.startDate) { desde {{ o.startDate | rulingDate }} }
              </span>
              @if (o.isCurrent) { <span class="current-badge">Activo</span> }
            </a>
          }
        </div>
      }

      @if (activeTab() === 'procesos') {
        <div class="tab-content">
          @for (proc of p.proceedings; track proc.proceedingId) {
            <a [routerLink]="['/procesos', proc.proceedingId]" [entityPreview]="{ type: 'proceeding', id: proc.proceedingId }" class="proc-row hover-lift">
              <div class="proc-main">
                <span class="proc-title">{{ proc.displayName ?? proc.caseNumber }}</span>
                <span class="proc-case">{{ proc.caseNumber }}</span>
              </div>
              <span class="participation-badge">{{ partyRoleLabel(proc.role) }}</span>
            </a>
          }
        </div>
      }
    }
  `,
  styles: [`
    .detail-header { margin-bottom: 1.25rem; }
    .person-hero { display: flex; align-items: center; gap: 14px; }
    .hero-avatar { width: 48px; height: 48px; border-radius: 50%; background: var(--color-primary-light); color: var(--color-primary); display: flex; align-items: center; justify-content: center; font-size: 1.25rem; font-weight: 700; flex-shrink: 0; }
    .hero-avatar.legal { border-radius: var(--radius-sm); }
    .detail-header h2 { font-size: 1.25rem; font-weight: 700; margin: 0; }
    .hero-meta { display: flex; align-items: center; gap: 6px; margin-top: 4px; flex-wrap: wrap; }
    .court-label { font-size: 0.8125rem; color: var(--color-text-secondary); }

    .type-badge { display: inline-block; padding: 2px 8px; border-radius: var(--radius-xs); font-size: 0.625rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.03em; border: 1px solid var(--color-border); background: var(--color-bg-subtle); color: var(--color-text-secondary); }
    .type-badge[data-type="Physical"] { background: rgba(59,130,246,.08); border-color: rgba(59,130,246,.2); color: #2563eb; }
    .type-badge[data-type="LegalPublic"] { background: rgba(22,163,74,.08); border-color: rgba(22,163,74,.2); color: #16a34a; }
    .type-badge[data-type="LegalPrivate"] { background: rgba(124,58,237,.08); border-color: rgba(124,58,237,.2); color: #7c3aed; }
    .type-badge[data-type="StateEntity"] { background: rgba(208,74,2,.08); border-color: rgba(208,74,2,.2); color: var(--color-primary); }
    .type-badge.sub { text-transform: none; }

    .stats-row { display: flex; gap: 1rem; margin-bottom: 1.25rem; flex-wrap: wrap; }
    .stat-card { min-width: 100px; max-width: 160px; padding: 1rem; background: var(--color-bg-surface); border: 1px solid var(--color-border); border-radius: var(--radius-md); text-align: center; }
    .stat-value { display: block; font-size: 1.5rem; font-weight: 700; color: var(--color-primary); }
    .stat-label { display: block; font-size: 0.6875rem; color: var(--color-text-secondary); margin-top: 2px; text-transform: uppercase; letter-spacing: 0.5px; }

    .tabs { display: flex; gap: 0; border-bottom: 1px solid var(--color-border); margin-bottom: 1rem; }
    .tab { padding: 0.625rem 1rem; background: none; border: none; border-bottom: 2px solid transparent; font-size: 0.8125rem; font-weight: 500; color: var(--color-text-secondary); cursor: pointer; font-family: inherit; transition: all 0.15s; }
    .tab:hover { color: var(--color-text); }
    .tab.active { border-bottom-color: var(--color-primary); color: var(--color-primary); font-weight: 600; }

    .rulings-list { display: flex; flex-direction: column; }
    .ruling-row { display: flex; align-items: center; justify-content: space-between; gap: 1rem; padding: 0.75rem 1rem; border-bottom: 1px solid var(--color-border); text-decoration: none; color: var(--color-text); transition: background 0.1s; }
    .ruling-row:hover { background: var(--color-bg-subtle); }
    .ruling-main { display: flex; flex-direction: column; min-width: 0; }
    .ruling-title { font-size: 0.8125rem; font-weight: 500; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
    .ruling-meta { font-size: 0.6875rem; color: var(--color-text-secondary); margin-top: 2px; }
    .participation-badge { flex-shrink: 0; padding: 2px 8px; background: var(--color-bg-subtle); border-radius: var(--radius-xs); font-size: 0.625rem; font-weight: 600; color: var(--color-text-secondary); text-transform: uppercase; }

    .office-row { display: flex; align-items: center; gap: 0.75rem; padding: 0.625rem 1rem; border-bottom: 1px solid var(--color-border); text-decoration: none; color: var(--color-text); font-size: 0.8125rem; }
    .office-row:last-child { border-bottom: none; }
    .office-position { font-size: 0.6875rem; font-weight: 600; color: var(--color-text-secondary); min-width: 130px; }
    .office-court { font-weight: 500; flex: 1; color: var(--color-primary); }
    .office-date { font-size: 0.6875rem; color: var(--color-text-secondary); }
    .current-badge { display: inline-block; padding: 1px 6px; border-radius: var(--radius-xs); font-size: 0.625rem; font-weight: 700; background: rgba(22,163,74,.08); border: 1px solid rgba(22,163,74,.2); color: #16a34a; }

    .proc-row { display: flex; align-items: center; justify-content: space-between; gap: 1rem; padding: 0.75rem 1rem; border-bottom: 1px solid var(--color-border); text-decoration: none; color: var(--color-text); }
    .proc-row:hover { background: var(--color-bg-subtle); }
    .proc-main { display: flex; flex-direction: column; min-width: 0; }
    .proc-title { font-size: 0.8125rem; font-weight: 500; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
    .proc-case { font-size: 0.6875rem; color: var(--color-text-secondary); font-family: var(--font-mono, monospace); }

    .empty-state { text-align: center; padding: 3rem; color: var(--color-text-secondary); }
    .empty-tab { text-align: center; padding: 2rem; color: var(--color-text-secondary); font-size: 0.875rem; }
    .back-link { color: var(--color-primary); text-decoration: none; font-size: 0.875rem; }
  `]
})
export class PersonDetailComponent {
  private route = inject(ActivatedRoute);
  private personService = inject(PersonService);
  private destroyRef = inject(DestroyRef);

  person = signal<PersonDetail | null>(null);
  state = signal<State>('loading');
  activeTab = signal<Tab>('fallos');

  showCarrera = computed(() => {
    const p = this.person();
    return p && p.personType === 'Physical' && p.judicialOffices.length > 0;
  });

  constructor() {
    this.route.paramMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        const id = Number(params.get('id'));
        if (!id) { this.state.set('notFound'); return; }
        this.activeTab.set('fallos');
        this.loadPerson(id);
      });
  }

  roleLabel(type: string) { return ROLE_LABELS[type] ?? type; }
  partyRoleLabel(role: string) { return PARTY_ROLE_LABELS[role] ?? role; }
  positionLabel(pos: string) { return POSITION_LABELS[pos] ?? pos; }
  typeLabel(type: string) { return TYPE_LABELS[type] ?? type; }

  private loadPerson(id: number) {
    this.state.set('loading');
    this.personService.getById(id).subscribe({
      next: p => { this.person.set(p); this.state.set('loaded'); },
      error: (err: { status?: number }) => {
        this.state.set(err?.status === 404 ? 'notFound' : 'error');
      }
    });
  }
}
