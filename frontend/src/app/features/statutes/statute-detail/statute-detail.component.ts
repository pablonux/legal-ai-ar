import { Component, inject, signal, DestroyRef } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { StatuteService } from '../../../services/statute.service';
import { SkeletonDetailComponent } from '../../../shared/components/skeletons/skeleton-detail.component';
import { BreadcrumbComponent } from '../../../shared/components/breadcrumb/breadcrumb.component';
import { RulingDatePipe } from '../../../shared/pipes/ruling-date.pipe';
import { EntityPreviewDirective } from '../../../shared/directives/entity-preview.directive';
import type { StatuteDetail } from '../../../models/statute.models';

type State = 'loading' | 'loaded' | 'notFound' | 'error';

const NORM_TYPE_LABELS: Record<string, string> = {
  CONSTITUTION: 'Constitución', TREATY: 'Tratado', LAW: 'Ley', DECREE: 'Decreto',
  DNU: 'DNU', RESOLUTION: 'Resolución', ACORDADA: 'Acordada', ORDINANCE: 'Ordenanza',
};

const LEVEL_LABELS: Record<string, string> = {
  CONSTITUTIONAL: 'Constitucional', SUPRALEGAL: 'Supralegal', LEGAL: 'Legal',
  REGULATORY: 'Reglamentario', INDIVIDUAL: 'Individual',
};

const RELATION_LABELS: Record<string, string> = {
  DEROGATES: 'Deroga', AMENDS: 'Modifica', REGULATES: 'Reglamenta', COMPLEMENTS: 'Complementa',
};

@Component({
  selector: 'app-statute-detail',
  standalone: true,
  imports: [RouterLink, SkeletonDetailComponent, BreadcrumbComponent, RulingDatePipe, EntityPreviewDirective],
  template: `
    @if (state() === 'loading') {
      <app-skeleton-detail />
    }

    @if (state() === 'notFound') {
      <div class="empty-state">
        <p>Norma no encontrada.</p>
        <a routerLink="/ordenamiento" class="back-link">Volver al ordenamiento</a>
      </div>
    }

    @if (state() === 'error') {
      <div class="empty-state">
        <p>Error al cargar la norma.</p>
        <a routerLink="/ordenamiento" class="back-link">Volver al ordenamiento</a>
      </div>
    }

    @if (state() === 'loaded' && statute(); as s) {
      <app-breadcrumb [items]="[{ label: 'Ordenamiento', route: '/ordenamiento' }, { label: s.name }]" />

      <div class="detail-layout">
        <div class="detail-header">
          <div class="badges">
            @if (s.normType) {
              <span class="badge badge-type">{{ normTypeLabel(s.normType) }}</span>
            }
            @if (s.normativeLevel) {
              <span class="badge badge-level">{{ levelLabel(s.normativeLevel) }}</span>
            }
            <span class="badge" [class.badge-vigente]="s.isVigente" [class.badge-no-vigente]="!s.isVigente">
              {{ s.isVigente ? 'Vigente' : 'No vigente' }}
            </span>
          </div>
          <h1 class="statute-title">{{ s.name }}</h1>
          @if (s.number) {
            <p class="statute-number">N° {{ s.number }}</p>
          }
        </div>

        <div class="detail-grid">
          <div class="info-card">
            <h3 class="card-title">Información</h3>
            <dl class="info-list">
              @if (s.issuingBody) {
                <div class="info-row"><dt>Organismo emisor</dt><dd>{{ s.issuingBody }}</dd></div>
              }
              @if (s.sanctionDate) {
                <div class="info-row"><dt>Fecha de sanción</dt><dd>{{ s.sanctionDate | rulingDate }}</dd></div>
              }
              @if (s.effectiveFrom) {
                <div class="info-row"><dt>Vigente desde</dt><dd>{{ s.effectiveFrom | rulingDate }}</dd></div>
              }
              @if (s.effectiveTo) {
                <div class="info-row"><dt>Vigente hasta</dt><dd>{{ s.effectiveTo | rulingDate }}</dd></div>
              }
              @if (s.legalBranch) {
                <div class="info-row"><dt>Rama del derecho</dt><dd>{{ s.legalBranch }}</dd></div>
              }
              <div class="info-row"><dt>Fallos que la citan</dt><dd class="stat-value">{{ s.rulingCount }}</dd></div>
            </dl>
            @if (s.officialUrl) {
              <a [href]="s.officialUrl" target="_blank" class="official-link">Ver texto oficial</a>
            }
          </div>

          @if (s.recentRulings.length > 0) {
            <div class="info-card">
              <h3 class="card-title">Jurisprudencia que cita esta norma</h3>
              <div class="rulings-list">
                @for (r of s.recentRulings; track r.rulingId) {
                  <a [routerLink]="['/jurisprudencia', r.rulingId]" [entityPreview]="{ type: 'ruling', id: r.rulingId }" class="ruling-item hover-lift">
                    <span class="ruling-title">{{ r.caseTitle }}</span>
                    <span class="ruling-meta">
                      {{ r.courtName ?? '' }} · {{ r.rulingDate | rulingDate }}
                      @if (r.articles) { · Art. {{ r.articles }} }
                    </span>
                  </a>
                }
              </div>
            </div>
          }

          @if (s.relations.length > 0) {
            <div class="info-card">
              <h3 class="card-title">Relaciones normativas</h3>
              <div class="relations-list">
                @for (rel of s.relations; track rel.relatedStatuteId) {
                  <a [routerLink]="['/ordenamiento', rel.relatedStatuteId]" class="relation-item">
                    <span class="relation-type">{{ rel.isOutbound ? '' : '← ' }}{{ relationLabel(rel.relationType) }}{{ rel.isOutbound ? ' →' : '' }}</span>
                    <span class="relation-name">{{ rel.relatedStatuteName }} ({{ rel.relatedStatuteNumber }})</span>
                  </a>
                }
              </div>
            </div>
          }
        </div>
      </div>
    }
  `,
  styles: [`
    .empty-state { text-align: center; padding: 3rem; color: var(--color-text-secondary); }
    .back-link { color: var(--color-primary); text-decoration: none; font-size: 0.875rem; }
    .back-link:hover { text-decoration: underline; }

    .detail-layout { max-width: 900px; }

    .detail-header { margin-bottom: 1.5rem; }

    .badges { display: flex; gap: 6px; margin-bottom: 0.5rem; flex-wrap: wrap; }

    .badge {
      display: inline-block;
      padding: 2px 8px;
      border-radius: var(--radius-xs);
      font-size: 0.6875rem;
      font-weight: 600;
      border: 1px solid var(--color-border);
      background: var(--color-bg-subtle);
    }

    .badge-type { background: rgba(208, 74, 2, 0.08); border-color: rgba(208, 74, 2, 0.2); color: var(--color-primary); }
    .badge-level { background: rgba(59, 130, 246, 0.08); border-color: rgba(59, 130, 246, 0.2); color: #2563eb; }
    .badge-vigente { background: rgba(22, 163, 74, 0.08); border-color: rgba(22, 163, 74, 0.2); color: #16a34a; }
    .badge-no-vigente { background: rgba(220, 38, 38, 0.08); border-color: rgba(220, 38, 38, 0.2); color: #dc2626; }

    .statute-title { font-size: 1.375rem; font-weight: 600; margin: 0; color: var(--color-text); }
    .statute-number { font-size: 0.875rem; color: var(--color-text-secondary); margin: 0.25rem 0 0; font-family: var(--font-mono, monospace); }

    .detail-grid { display: flex; flex-direction: column; gap: 1.25rem; }

    .info-card {
      padding: 1.25rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      background: var(--color-bg-surface);
    }

    .card-title {
      font-size: 0.8125rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.03em;
      color: var(--color-text-secondary);
      margin: 0 0 0.75rem;
    }

    .info-list { margin: 0; }
    .info-row { display: flex; justify-content: space-between; padding: 0.375rem 0; border-bottom: 1px solid var(--color-border); font-size: 0.8125rem; }
    .info-row:last-child { border-bottom: none; }
    .info-row dt { color: var(--color-text-secondary); }
    .info-row dd { margin: 0; font-weight: 500; }
    .stat-value { font-weight: 700; color: var(--color-primary); }

    .official-link {
      display: inline-block;
      margin-top: 0.75rem;
      font-size: 0.8125rem;
      color: var(--color-primary);
      text-decoration: none;
    }
    .official-link:hover { text-decoration: underline; }

    .rulings-list { display: flex; flex-direction: column; gap: 0; }
    .ruling-item {
      display: flex;
      flex-direction: column;
      gap: 2px;
      padding: 0.5rem 0;
      border-bottom: 1px solid var(--color-border);
      text-decoration: none;
      color: var(--color-text);
      transition: background 0.1s;
    }
    .ruling-item:last-child { border-bottom: none; }
    .ruling-title { font-size: 0.8125rem; font-weight: 500; }
    .ruling-meta { font-size: 0.6875rem; color: var(--color-text-secondary); }

    .relations-list { display: flex; flex-direction: column; gap: 0; }
    .relation-item {
      display: flex;
      gap: 0.75rem;
      align-items: center;
      padding: 0.5rem 0;
      border-bottom: 1px solid var(--color-border);
      text-decoration: none;
      color: var(--color-text);
      font-size: 0.8125rem;
    }
    .relation-item:last-child { border-bottom: none; }
    .relation-item:hover { color: var(--color-primary); }
    .relation-type { font-size: 0.6875rem; font-weight: 600; color: var(--color-text-secondary); white-space: nowrap; min-width: 90px; }
  `]
})
export class StatuteDetailComponent {
  private route = inject(ActivatedRoute);
  private statuteService = inject(StatuteService);
  private destroyRef = inject(DestroyRef);

  state = signal<State>('loading');
  statute = signal<StatuteDetail | null>(null);

  constructor() {
    this.route.params
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        const id = parseInt(params['id'], 10);
        if (isNaN(id)) { this.state.set('notFound'); return; }
        this.load(id);
      });
  }

  private load(id: number) {
    this.state.set('loading');
    this.statuteService.getById(id).subscribe({
      next: s => { this.statute.set(s); this.state.set('loaded'); },
      error: (err) => this.state.set(err.status === 404 ? 'notFound' : 'error'),
    });
  }

  normTypeLabel(type: string): string { return NORM_TYPE_LABELS[type] ?? type; }
  levelLabel(level: string): string { return LEVEL_LABELS[level] ?? level; }
  relationLabel(type: string): string { return RELATION_LABELS[type] ?? type; }
}
