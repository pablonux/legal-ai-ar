import { Component, inject, signal, DestroyRef } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ProceedingService } from '../../../services/proceeding.service';
import { SkeletonDetailComponent } from '@legal-ai-ar/shared-common/components/skeletons/skeleton-detail.component';
import { BreadcrumbComponent } from '@legal-ai-ar/shared-common/components/breadcrumb/breadcrumb.component';
import { RulingDatePipe } from '@legal-ai-ar/shared-common/pipes/ruling-date.pipe';
import { EntityPreviewDirective } from '@legal-ai-ar/shared-common/directives/entity-preview.directive';
import type { ProceedingDetail, AppealChain } from '../../../models/proceeding-space.models';

type State = 'loading' | 'loaded' | 'notFound' | 'error';
type Tab = 'resumen' | 'instancias' | 'partes' | 'impugnacion';

const STATUS_LABELS: Record<string, string> = {
  EnTramite: 'En trámite', ConSentencia: 'Con sentencia', Firme: 'Firme', Archivado: 'Archivado',
};
const TYPE_LABELS: Record<string, string> = {
  Civil: 'Civil', Penal: 'Penal', Laboral: 'Laboral',
  ContenciosoAdministrativo: 'Cont. Administrativo', Familia: 'Familia', Constitucional: 'Constitucional',
};
const ROLE_LABELS: Record<string, string> = {
  PLAINTIFF: 'Actor', DEFENDANT: 'Demandado', THIRD_PARTY: 'Tercero', AMICUS_CURIAE: 'Amicus Curiae',
};
const REMEDY_LABELS: Record<string, string> = {
  RecursoExtraordinarioFederal: 'Recurso Extraordinario Federal',
  RecursoDeQueja: 'Recurso de Queja',
  RecursoDeApelacion: 'Recurso de Apelación',
  RecursoDeNulidad: 'Recurso de Nulidad',
  RecursoDeCasacion: 'Recurso de Casación',
  RecursoDeInaplicabilidadDeLey: 'Recurso de Inaplicabilidad de Ley',
  RecursoDeRevision: 'Recurso de Revisión',
  RecursoDeReposicion: 'Recurso de Reposición',
  RecursoDeAclaratoria: 'Recurso de Aclaratoria',
  RecursoDeInconstitucionalidad: 'Recurso de Inconstitucionalidad',
  Otro: 'Otro',
};

@Component({
  selector: 'app-proceeding-detail',
  standalone: true,
  imports: [RouterLink, SkeletonDetailComponent, BreadcrumbComponent, RulingDatePipe, EntityPreviewDirective],
  template: `
    @if (state() === 'loading') { <app-skeleton-detail /> }
    @if (state() === 'notFound') {
      <div class="empty-state"><p>Proceso no encontrado.</p><a routerLink="/procesos" class="back-link">Volver</a></div>
    }
    @if (state() === 'error') {
      <div class="empty-state"><p>Error al cargar.</p><a routerLink="/procesos" class="back-link">Volver</a></div>
    }

    @if (state() === 'loaded' && proc(); as p) {
      <app-breadcrumb [items]="[{ label: 'Procesos', route: '/procesos' }, { label: p.displayName ?? p.caseNumber }]" />

      <div class="detail">
        <div class="header">
          <div class="badges">
            @if (p.processType) { <span class="badge badge-type">{{ typeLabel(p.processType) }}</span> }
            @if (p.status) { <span class="badge" [attr.data-status]="p.status">{{ statusLabel(p.status) }}</span> }
          </div>
          <h1 class="title">{{ p.displayName ?? p.caseNumber }}</h1>
          <p class="meta">
            Expediente: <strong>{{ p.caseNumber }}</strong>
            @if (p.courtName) { · {{ p.courtName }} }
            @if (p.jurisdictionArea) { · {{ p.jurisdictionArea }} }
          </p>
        </div>

        <div class="tabs">
          <button type="button" class="tab" [class.active]="activeTab() === 'resumen'" (click)="activeTab.set('resumen')">Resumen</button>
          <button type="button" class="tab" [class.active]="activeTab() === 'instancias'" (click)="activeTab.set('instancias')">Instancias ({{ p.rulings.length }})</button>
          <button type="button" class="tab" [class.active]="activeTab() === 'partes'" (click)="activeTab.set('partes')">Partes ({{ p.parties.length }})</button>
          <button type="button" class="tab" [class.active]="activeTab() === 'impugnacion'" (click)="loadAppealChain(p.id)">Cadena de impugnación</button>
        </div>

        @if (activeTab() === 'resumen') {
          <div class="tab-content">
            <div class="info-card">
              <dl class="info-list">
                <div class="info-row"><dt>Tipo de proceso</dt><dd>{{ p.processType ? typeLabel(p.processType) : '—' }}</dd></div>
                @if (p.processSubtype) { <div class="info-row"><dt>Subtipo</dt><dd>{{ p.processSubtype }}</dd></div> }
                <div class="info-row"><dt>Rama del derecho</dt><dd>{{ p.legalBranch ?? '—' }}</dd></div>
                <div class="info-row"><dt>Estado</dt><dd>{{ p.status ? statusLabel(p.status) : '—' }}</dd></div>
                @if (p.courtName && p.courtId) { <div class="info-row"><dt>Tribunal</dt><dd><a [routerLink]="['/organismos', p.courtId]" [entityPreview]="{ type: 'court', id: p.courtId }">{{ p.courtName }}</a></dd></div> }
                <div class="info-row"><dt>Primer fallo</dt><dd>{{ p.firstRulingDate ? (p.firstRulingDate | rulingDate) : '—' }}</dd></div>
                <div class="info-row"><dt>Último fallo</dt><dd>{{ p.lastRulingDate ? (p.lastRulingDate | rulingDate) : '—' }}</dd></div>
                <div class="info-row"><dt>Total de fallos</dt><dd class="stat-value">{{ p.rulingCount }}</dd></div>
              </dl>
            </div>
          </div>
        }

        @if (activeTab() === 'instancias') {
          <div class="tab-content">
            <div class="timeline">
              @for (r of p.rulings; track r.id) {
                <a [routerLink]="['/jurisprudencia', r.id]" [entityPreview]="{ type: 'ruling', id: r.id }" class="timeline-item hover-lift">
                  <span class="timeline-dot"></span>
                  <div class="timeline-body">
                    <span class="timeline-title">{{ r.caseTitle }}</span>
                    <span class="timeline-meta">
                      {{ r.courtName ?? '' }}
                      @if (r.instance) { · {{ r.instance }} }
                      · {{ r.rulingDate | rulingDate }}
                    </span>
                  </div>
                </a>
              }
            </div>
          </div>
        }

        @if (activeTab() === 'partes') {
          <div class="tab-content">
            @if (p.parties.length > 0) {
              <div class="info-card">
                <h3 class="card-title">Partes procesales</h3>
                @for (party of p.parties; track party.personId) {
                  <div class="party-row">
                    <span class="party-role">{{ roleLabel(party.role) }}</span>
                    <a [routerLink]="['/sujetos', party.personId]" [entityPreview]="{ type: 'person', id: party.personId }" class="party-name">{{ party.personName }}</a>
                  </div>
                }
              </div>
            }
            @if (p.representations.length > 0) {
              <div class="info-card">
                <h3 class="card-title">Representación legal</h3>
                @for (rep of p.representations; track rep.lawyerId + '-' + rep.partyId) {
                  <div class="rep-row">
                    <a [routerLink]="['/sujetos', rep.lawyerId]" [entityPreview]="{ type: 'person', id: rep.lawyerId }">{{ rep.lawyerName }}</a>
                    <span class="rep-arrow">representa a</span>
                    <a [routerLink]="['/sujetos', rep.partyId]" [entityPreview]="{ type: 'person', id: rep.partyId }">{{ rep.partyName }}</a>
                  </div>
                }
              </div>
            }
            @if (p.parties.length === 0 && p.representations.length === 0) {
              <div class="empty-state"><p>No se han identificado partes para este proceso.</p></div>
            }
          </div>
        }

        @if (activeTab() === 'impugnacion') {
          <div class="tab-content">
            @if (appealChainLoading()) {
              <div class="empty-state"><p>Cargando cadena de impugnación...</p></div>
            } @else if (appealChain()) {
              @if (appealChain()!.nodes.length === 0) {
                <div class="empty-state"><p>No hay fallos vinculados a este proceso.</p></div>
              } @else {
                <div class="appeal-chain">
                  @for (node of appealChain()!.nodes; track node.rulingId; let i = $index; let last = $last) {
                    <div class="chain-node">
                      <div class="chain-instance">{{ node.instance ?? 'Instancia ' + (i + 1) }}</div>
                      <a [routerLink]="['/jurisprudencia', node.rulingId]" class="chain-ruling hover-lift">
                        <span class="chain-title">{{ node.caseTitle }}</span>
                        <span class="chain-meta">{{ node.courtName }} · {{ node.rulingDate | rulingDate }}</span>
                      </a>
                      @if (node.remediesFromHere.length > 0) {
                        @for (rem of node.remediesFromHere; track rem.id) {
                          <div class="chain-arrow">
                            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="5" x2="12" y2="19"/><polyline points="19 12 12 19 5 12"/></svg>
                            <span class="remedy-label">{{ remedyLabel(rem.remedyType) }}</span>
                            @if (rem.outcome) { <span class="remedy-outcome">{{ rem.outcome }}</span> }
                          </div>
                        }
                      } @else if (!last) {
                        <div class="chain-arrow">
                          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="5" x2="12" y2="19"/><polyline points="19 12 12 19 5 12"/></svg>
                        </div>
                      }
                    </div>
                  }
                </div>
              }
            } @else {
              <div class="empty-state"><p>No se pudo cargar la cadena de impugnación.</p></div>
            }
          </div>
        }
      </div>
    }
  `,
  styles: [`
    .empty-state { text-align: center; padding: 3rem; color: var(--color-text-secondary); }
    .back-link { color: var(--color-primary); text-decoration: none; font-size: 0.875rem; }
    .detail { max-width: 900px; }
    .header { margin-bottom: 1.25rem; }
    .badges { display: flex; gap: 6px; margin-bottom: 0.5rem; }
    .badge { display: inline-block; padding: 2px 8px; border-radius: var(--radius-xs); font-size: 0.6875rem; font-weight: 600; border: 1px solid var(--color-border); background: var(--color-bg-subtle); }
    .badge-type { background: rgba(208,74,2,.08); border-color: rgba(208,74,2,.2); color: var(--color-primary); }
    .badge[data-status="Firme"] { background: rgba(22,163,74,.08); border-color: rgba(22,163,74,.2); color: #16a34a; }
    .badge[data-status="EnTramite"] { background: rgba(59,130,246,.08); border-color: rgba(59,130,246,.2); color: #2563eb; }
    .title { font-size: 1.375rem; font-weight: 600; margin: 0; }
    .meta { font-size: 0.8125rem; color: var(--color-text-secondary); margin: 0.25rem 0 0; }
    .meta strong { color: var(--color-text); }

    .tabs { display: flex; gap: 0; border-bottom: 1px solid var(--color-border); margin-bottom: 1rem; }
    .tab { padding: 0.625rem 1rem; background: none; border: none; border-bottom: 2px solid transparent; font-size: 0.8125rem; font-weight: 500; color: var(--color-text-secondary); cursor: pointer; font-family: inherit; transition: all 0.15s; }
    .tab:hover { color: var(--color-text); }
    .tab.active { border-bottom-color: var(--color-primary); color: var(--color-primary); font-weight: 600; }

    .tab-content { }

    .info-card { padding: 1.25rem; border: 1px solid var(--color-border); border-radius: var(--radius-md); background: var(--color-bg-surface); margin-bottom: 1rem; }
    .card-title { font-size: 0.8125rem; font-weight: 700; text-transform: uppercase; letter-spacing: .03em; color: var(--color-text-secondary); margin: 0 0 .75rem; }
    .info-list { margin: 0; }
    .info-row { display: flex; justify-content: space-between; padding: .375rem 0; border-bottom: 1px solid var(--color-border); font-size: .8125rem; }
    .info-row:last-child { border-bottom: none; }
    .info-row dt { color: var(--color-text-secondary); }
    .info-row dd { margin: 0; font-weight: 500; }
    .info-row dd a { color: var(--color-primary); text-decoration: none; }
    .info-row dd a:hover { text-decoration: underline; }
    .stat-value { font-weight: 700; color: var(--color-primary); }

    .timeline { display: flex; flex-direction: column; gap: 0; padding-left: 1rem; border-left: 2px solid var(--color-border); }
    .timeline-item { display: flex; gap: .75rem; padding: .75rem 0; text-decoration: none; color: var(--color-text); position: relative; }
    .timeline-dot { width: 10px; height: 10px; border-radius: 50%; background: var(--color-primary); border: 2px solid var(--color-bg-surface); flex-shrink: 0; margin-top: 4px; margin-left: -1.35rem; }
    .timeline-body { display: flex; flex-direction: column; gap: 2px; }
    .timeline-title { font-size: .8125rem; font-weight: 500; }
    .timeline-meta { font-size: .6875rem; color: var(--color-text-secondary); }

    .party-row { display: flex; gap: .75rem; align-items: center; padding: .5rem 0; border-bottom: 1px solid var(--color-border); font-size: .8125rem; }
    .party-row:last-child { border-bottom: none; }
    .party-role { font-size: .6875rem; font-weight: 600; color: var(--color-text-secondary); min-width: 80px; }
    .party-name { color: var(--color-primary); text-decoration: none; }
    .party-name:hover { text-decoration: underline; }

    .rep-row { display: flex; gap: .5rem; align-items: center; padding: .5rem 0; border-bottom: 1px solid var(--color-border); font-size: .8125rem; }
    .rep-row:last-child { border-bottom: none; }
    .rep-row a { color: var(--color-primary); text-decoration: none; }
    .rep-row a:hover { text-decoration: underline; }
    .rep-arrow { font-size: .6875rem; color: var(--color-text-secondary); }

    .appeal-chain { display: flex; flex-direction: column; align-items: center; gap: 0; padding: 1rem 0; }
    .chain-node { display: flex; flex-direction: column; align-items: center; width: 100%; max-width: 480px; }
    .chain-instance { font-size: .6875rem; font-weight: 700; text-transform: uppercase; letter-spacing: .06em; color: var(--color-text-secondary); margin-bottom: .25rem; }
    .chain-ruling { display: flex; flex-direction: column; text-align: center; padding: .75rem 1.25rem; border: 1px solid var(--color-border); border-radius: var(--radius-md); background: var(--color-bg-surface); text-decoration: none; color: var(--color-text); width: 100%; transition: all .15s; }
    .chain-ruling:hover { border-color: var(--color-primary); background: var(--color-bg-subtle); }
    .chain-title { font-size: .8125rem; font-weight: 500; }
    .chain-meta { font-size: .6875rem; color: var(--color-text-secondary); margin-top: 2px; }
    .chain-arrow { display: flex; flex-direction: column; align-items: center; gap: 2px; padding: .5rem 0; color: var(--color-primary); }
    .remedy-label { font-size: .6875rem; font-weight: 600; color: var(--color-primary); }
    .remedy-outcome { font-size: .625rem; color: var(--color-text-secondary); }
  `]
})
export class ProceedingDetailComponent {
  private route = inject(ActivatedRoute);
  private procService = inject(ProceedingService);
  private destroyRef = inject(DestroyRef);

  state = signal<State>('loading');
  proc = signal<ProceedingDetail | null>(null);
  activeTab = signal<Tab>('resumen');
  appealChain = signal<AppealChain | null>(null);
  appealChainLoading = signal(false);
  private appealChainLoadedFor: number | null = null;

  constructor() {
    this.route.params.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {
      const id = parseInt(params['id'], 10);
      if (isNaN(id)) { this.state.set('notFound'); return; }
      this.load(id);
    });
  }

  private load(id: number) {
    this.state.set('loading');
    this.procService.getById(id).subscribe({
      next: p => { this.proc.set(p); this.state.set('loaded'); },
      error: err => this.state.set(err.status === 404 ? 'notFound' : 'error'),
    });
  }

  statusLabel(s: string) { return STATUS_LABELS[s] ?? s; }
  typeLabel(t: string) { return TYPE_LABELS[t] ?? t; }
  roleLabel(r: string) { return ROLE_LABELS[r] ?? r; }

  loadAppealChain(proceedingId: number) {
    this.activeTab.set('impugnacion');
    if (this.appealChainLoadedFor === proceedingId) return;
    this.appealChainLoading.set(true);
    this.procService.getAppealChain(proceedingId).subscribe({
      next: chain => {
        this.appealChain.set(chain);
        this.appealChainLoadedFor = proceedingId;
        this.appealChainLoading.set(false);
      },
      error: () => this.appealChainLoading.set(false),
    });
  }

  remedyLabel(type: string): string {
    return REMEDY_LABELS[type] ?? type;
  }
}
