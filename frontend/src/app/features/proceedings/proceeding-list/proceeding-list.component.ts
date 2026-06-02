import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { ProceedingService } from '../../../services/proceeding.service';
import { SkeletonTableRowComponent } from '@legal-ai-ar/shared-common/components/skeletons/skeleton-table-row.component';
import { RulingDatePipe } from '@legal-ai-ar/shared-common/pipes/ruling-date.pipe';
import { EmptyStateComponent } from '@legal-ai-ar/shared-common/components/empty-state/empty-state.component';
import type { ProceedingListItem } from '../../../models/proceeding-space.models';

const STATUS_LABELS: Record<string, string> = {
  EnTramite: 'En trámite', ConSentencia: 'Con sentencia', Firme: 'Firme', Archivado: 'Archivado',
};

const TYPE_LABELS: Record<string, string> = {
  Civil: 'Civil', Penal: 'Penal', Laboral: 'Laboral',
  ContenciosoAdministrativo: 'Cont. Administrativo', Familia: 'Familia', Constitucional: 'Constitucional',
};

@Component({
  selector: 'app-proceeding-list',
  standalone: true,
  imports: [RouterLink, FormsModule, SkeletonTableRowComponent, RulingDatePipe, EmptyStateComponent],
  template: `
    <div class="proc-list">
      <h1 class="page-title">
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" aria-hidden="true"><path d="M16 4h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2"/><rect x="8" y="2" width="8" height="4" rx="1" ry="1"/></svg>
        Procesos Judiciales
      </h1>

      <div class="filters-row">
        <input type="text" [(ngModel)]="query" (ngModelChange)="onSearch($event)" placeholder="Buscar por expediente o carátula..." class="search-input" />
        <select [(ngModel)]="typeFilter" (ngModelChange)="reload()" class="filter-select">
          <option value="">Tipo</option>
          <option value="Civil">Civil</option>
          <option value="Penal">Penal</option>
          <option value="Laboral">Laboral</option>
          <option value="ContenciosoAdministrativo">Cont. Administrativo</option>
          <option value="Familia">Familia</option>
          <option value="Constitucional">Constitucional</option>
        </select>
        <select [(ngModel)]="statusFilter" (ngModelChange)="reload()" class="filter-select">
          <option value="">Estado</option>
          <option value="EnTramite">En trámite</option>
          <option value="ConSentencia">Con sentencia</option>
          <option value="Firme">Firme</option>
          <option value="Archivado">Archivado</option>
        </select>
      </div>

      @if (loading()) {
        <app-skeleton-table-row [count]="10" />
      } @else if (proceedings().length === 0) {
        <app-empty-state
          title="Sin procesos encontrados"
          subtitle="Los procesos judiciales se generan a partir de los fallos indexados en la KB."
          variant="space"
          [tips]="['Los procesos se crean automáticamente al procesar fallos', 'Probá ampliar los criterios de búsqueda o quitar filtros', 'Cada fallo procesado vincula las partes y expedientes relacionados']"
          [actions]="[{ label: 'Explorar jurisprudencia', route: '/jurisprudencia', primary: true }]">
          <svg esIcon xmlns="http://www.w3.org/2000/svg" width="56" height="56" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/></svg>
        </app-empty-state>
      } @else {
        <div class="proc-table">
          <div class="table-header">
            <span class="col-case">Expediente</span>
            <span class="col-name">Carátula</span>
            <span class="col-type">Tipo</span>
            <span class="col-status">Estado</span>
            <span class="col-date">Último fallo</span>
            <span class="col-count">Fallos</span>
          </div>
          @for (p of proceedings(); track p.id) {
            <a [routerLink]="['/procesos', p.id]" class="table-row hover-lift">
              <span class="col-case">{{ p.caseNumber }}</span>
              <span class="col-name row-name">{{ p.displayName ?? p.caseNumber }}</span>
              <span class="col-type">{{ p.processType ? typeLabel(p.processType) : '—' }}</span>
              <span class="col-status">
                @if (p.status) {
                  <span class="status-badge" [attr.data-status]="p.status">{{ statusLabel(p.status) }}</span>
                } @else { — }
              </span>
              <span class="col-date">{{ p.lastRulingDate ? (p.lastRulingDate | rulingDate) : '—' }}</span>
              <span class="col-count"><span class="count-badge">{{ p.rulingCount }}</span></span>
            </a>
          }
        </div>

        @if (totalCount() > proceedings().length) {
          <div class="pagination-info">Mostrando {{ proceedings().length }} de {{ totalCount() }} procesos</div>
        }
      }
    </div>
  `,
  styles: [`
    .proc-list { max-width: 1100px; margin: 0 auto; }

    .page-title {
      display: flex; align-items: center; gap: 0.5rem;
      font-size: 1.375rem; font-weight: 600; margin: 0 0 1rem; color: var(--color-text);
    }
    .page-title svg { color: var(--color-primary); }

    .filters-row { display: flex; gap: 0.75rem; margin-bottom: 1rem; flex-wrap: wrap; }

    .search-input {
      flex: 1; min-width: 200px; padding: 0.5rem 0.75rem;
      border: 1px solid var(--color-border-input); border-radius: var(--radius-sm);
      font-size: 0.875rem; font-family: inherit;
    }
    .search-input:focus { outline: none; border-color: var(--color-primary); box-shadow: 0 0 0 2px rgba(208,74,2,.08); }

    .filter-select {
      padding: 0.5rem 0.75rem; border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm); font-size: 0.8125rem; font-family: inherit;
      background: var(--color-bg-surface); cursor: pointer;
    }

    .empty-state { text-align: center; padding: 3rem; color: var(--color-text-secondary); }

    .proc-table { display: flex; flex-direction: column; }

    .table-header, .table-row {
      display: grid; grid-template-columns: 130px 1fr 110px 100px 90px 60px;
      align-items: center; padding: 0.625rem 1rem; gap: 0.5rem;
    }

    .table-header {
      font-size: 0.6875rem; font-weight: 700; text-transform: uppercase;
      letter-spacing: 0.5px; color: var(--color-text-secondary); border-bottom: 1px solid var(--color-border);
    }

    .table-row {
      text-decoration: none; color: var(--color-text); border-bottom: 1px solid var(--color-border);
      font-size: 0.8125rem;
    }
    .table-row:hover { background: var(--color-bg-subtle); }
    .row-name { font-weight: 500; }
    .col-case { font-family: var(--font-mono, monospace); font-size: 0.75rem; color: var(--color-text-secondary); }
    .col-type, .col-status, .col-date { font-size: 0.75rem; color: var(--color-text-secondary); }
    .col-count { text-align: right; }

    .status-badge {
      display: inline-block; padding: 1px 6px; border-radius: var(--radius-xs);
      font-size: 0.6875rem; font-weight: 600;
      border: 1px solid var(--color-border); background: var(--color-bg-subtle);
    }
    .status-badge[data-status="Firme"] { background: rgba(22,163,74,.08); border-color: rgba(22,163,74,.2); color: #16a34a; }
    .status-badge[data-status="EnTramite"] { background: rgba(59,130,246,.08); border-color: rgba(59,130,246,.2); color: #2563eb; }
    .status-badge[data-status="Archivado"] { background: rgba(107,114,128,.08); border-color: rgba(107,114,128,.2); color: #6b7280; }

    .count-badge {
      display: inline-block; background: var(--color-bg-subtle); border: 1px solid var(--color-border);
      border-radius: var(--radius-pill); padding: 2px 8px; font-size: 0.6875rem; font-weight: 600; color: var(--color-text-secondary);
    }

    .pagination-info { text-align: center; padding: 1rem; font-size: 0.8125rem; color: var(--color-text-secondary); }
  `]
})
export class ProceedingListComponent implements OnInit {
  private procService = inject(ProceedingService);
  private searchSubject = new Subject<string>();

  proceedings = signal<ProceedingListItem[]>([]);
  totalCount = signal(0);
  loading = signal(true);
  query = '';
  typeFilter = '';
  statusFilter = '';

  constructor() {
    this.searchSubject.pipe(debounceTime(300), distinctUntilChanged()).subscribe(() => this.reload());
  }

  ngOnInit() { this.reload(); }
  onSearch(value: string) { this.searchSubject.next(value.trim()); }

  reload() {
    this.loading.set(true);
    this.procService.search({
      q: this.query.trim() || undefined,
      processType: this.typeFilter || undefined,
      status: this.statusFilter || undefined,
      page: 1, pageSize: 50,
    }).subscribe({
      next: res => { this.proceedings.set(res.items); this.totalCount.set(res.totalCount); this.loading.set(false); },
      error: () => { this.proceedings.set([]); this.totalCount.set(0); this.loading.set(false); },
    });
  }

  statusLabel(s: string) { return STATUS_LABELS[s] ?? s; }
  typeLabel(t: string) { return TYPE_LABELS[t] ?? t; }
}
