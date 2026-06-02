import { Component, input, signal, inject, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { RulingService } from '@legal-ai-ar/app/services/ruling.service';
import { CourtService } from '@legal-ai-ar/app/services/court.service';
import { PersonService } from '@legal-ai-ar/app/services/person.service';
import { StatuteService } from '@legal-ai-ar/app/services/statute.service';
import { ProceedingService } from '@legal-ai-ar/app/services/proceeding.service';
import type { PreviewEntityType } from '../../directives/entity-preview.directive';

interface PreviewData {
  title: string;
  type: string;
  fields: { label: string; value: string }[];
}

@Component({
  selector: 'app-entity-preview-popup',
  standalone: true,
  template: `
    @if (loading()) {
      <div class="ep-card ep-loading">
        <div class="ep-skel ep-skel-title"></div>
        <div class="ep-skel ep-skel-line"></div>
        <div class="ep-skel ep-skel-line short"></div>
      </div>
    } @else if (data()) {
      <div class="ep-card">
        <div class="ep-header">
          <span class="ep-badge">{{ data()!.type }}</span>
        </div>
        <h4 class="ep-title">{{ data()!.title }}</h4>
        @for (f of data()!.fields; track f.label) {
          <div class="ep-field">
            <span class="ep-label">{{ f.label }}</span>
            <span class="ep-value">{{ f.value }}</span>
          </div>
        }
      </div>
    } @else if (error()) {
      <div class="ep-card ep-error">No se pudo cargar la vista previa</div>
    }
  `,
  styles: [`
    :host { display: block; animation: epFadeIn .15s ease-out; }
    @keyframes epFadeIn { from { opacity: 0; transform: translateY(4px); } to { opacity: 1; transform: translateY(0); } }
    .ep-card {
      background: var(--color-bg, #fff);
      border: 1px solid var(--color-border, #e7e5e4);
      border-radius: 10px;
      padding: 14px 16px;
      box-shadow: 0 8px 24px rgba(0,0,0,.12), 0 2px 6px rgba(0,0,0,.06);
      max-width: 320px;
    }
    .ep-header { margin-bottom: 6px; }
    .ep-badge {
      display: inline-block;
      font-size: .625rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: .06em;
      padding: 2px 7px;
      border-radius: 4px;
      background: var(--color-primary, #d04a02);
      color: #fff;
    }
    .ep-title {
      margin: 0 0 8px;
      font-size: .875rem;
      font-weight: 600;
      line-height: 1.3;
      color: var(--color-text);
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }
    .ep-field {
      display: flex;
      justify-content: space-between;
      gap: 8px;
      padding: 3px 0;
      font-size: .75rem;
      line-height: 1.4;
    }
    .ep-label { color: var(--color-text-secondary, #78716c); flex-shrink: 0; }
    .ep-value {
      color: var(--color-text);
      font-weight: 500;
      text-align: right;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
    .ep-loading { min-height: 80px; }
    .ep-skel {
      border-radius: 4px;
      background: linear-gradient(90deg, #e7e5e4 25%, #f5f5f4 50%, #e7e5e4 75%);
      background-size: 200% 100%;
      animation: epShimmer 1.2s infinite;
    }
    .ep-skel-title { height: 16px; width: 60%; margin-bottom: 10px; }
    .ep-skel-line { height: 12px; width: 100%; margin-bottom: 6px; }
    .ep-skel-line.short { width: 45%; }
    @keyframes epShimmer { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
    .ep-error { font-size: .8125rem; color: var(--color-text-secondary, #78716c); text-align: center; }
  `]
})
export class EntityPreviewPopupComponent implements OnInit, OnDestroy {
  entityType = input.required<PreviewEntityType>();
  entityId = input.required<string | number>();

  loading = signal(true);
  data = signal<PreviewData | null>(null);
  error = signal(false);

  private rulingService = inject(RulingService);
  private courtService = inject(CourtService);
  private personService = inject(PersonService);
  private statuteService = inject(StatuteService);
  private proceedingService = inject(ProceedingService);
  private sub?: Subscription;

  ngOnInit() {
    const id = this.entityId();
    switch (this.entityType()) {
      case 'ruling': this.loadRuling(String(id)); break;
      case 'court': this.loadCourt(Number(id)); break;
      case 'person': this.loadPerson(Number(id)); break;
      case 'statute': this.loadStatute(Number(id)); break;
      case 'proceeding': this.loadProceeding(Number(id)); break;
    }
  }

  private loadRuling(id: string) {
    this.sub = this.rulingService.getById(id).subscribe({
      next: r => {
        this.data.set({
          title: r.caseTitle,
          type: 'Fallo',
          fields: [
            { label: 'Tribunal', value: r.court?.name ?? '—' },
            { label: 'Fecha', value: r.rulingDate ? new Date(r.rulingDate).toLocaleDateString('es-AR') : '—' },
            ...(r.legalBranch ? [{ label: 'Rama', value: r.legalBranch }] : []),
            ...(r.keywords?.length ? [{ label: 'Temas', value: r.keywords.slice(0, 3).join(', ') }] : []),
          ],
        });
        this.loading.set(false);
      },
      error: () => { this.error.set(true); this.loading.set(false); },
    });
  }

  private loadCourt(id: number) {
    this.sub = this.courtService.getById(id).subscribe({
      next: c => {
        this.data.set({
          title: c.name,
          type: 'Organismo',
          fields: [
            { label: 'Jurisdicción', value: c.jurisdictionArea || '—' },
            { label: 'Fuero', value: c.fuero || c.territory || '—' },
            { label: 'Fallos', value: String(c.rulingCount) },
          ],
        });
        this.loading.set(false);
      },
      error: () => { this.error.set(true); this.loading.set(false); },
    });
  }

  private loadPerson(id: number) {
    this.sub = this.personService.getById(id).subscribe({
      next: p => {
        const fields: { label: string; value: string }[] = [
          { label: 'Tipo', value: p.personType || '—' },
        ];
        if (p.judicialOffices?.length) {
          fields.push({ label: 'Cargo actual', value: p.judicialOffices[0].position + ' - ' + p.judicialOffices[0].courtName });
        }
        if (p.recentRulings?.length) {
          fields.push({ label: 'Fallos', value: String(p.recentRulings.length) + '+' });
        }
        this.data.set({ title: p.displayName, type: 'Sujeto', fields });
        this.loading.set(false);
      },
      error: () => { this.error.set(true); this.loading.set(false); },
    });
  }

  private loadStatute(id: number) {
    this.sub = this.statuteService.getById(id).subscribe({
      next: s => {
        this.data.set({
          title: s.name || `${s.normType ?? 'Norma'} ${s.number}`,
          type: 'Norma',
          fields: [
            { label: 'Número', value: s.number },
            ...(s.normType ? [{ label: 'Tipo', value: s.normType }] : []),
            ...(s.normativeLevel ? [{ label: 'Nivel', value: s.normativeLevel }] : []),
            ...(s.sanctionDate ? [{ label: 'Sanción', value: new Date(s.sanctionDate).toLocaleDateString('es-AR') }] : []),
          ],
        });
        this.loading.set(false);
      },
      error: () => { this.error.set(true); this.loading.set(false); },
    });
  }

  private loadProceeding(id: number) {
    this.sub = this.proceedingService.getById(id).subscribe({
      next: p => {
        this.data.set({
          title: p.displayName || p.caseNumber,
          type: 'Proceso',
          fields: [
            { label: 'Tribunal', value: p.courtName || '—' },
            { label: 'Estado', value: p.status || '—' },
            ...(p.processType ? [{ label: 'Tipo', value: p.processType }] : []),
          ],
        });
        this.loading.set(false);
      },
      error: () => { this.error.set(true); this.loading.set(false); },
    });
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
  }
}
