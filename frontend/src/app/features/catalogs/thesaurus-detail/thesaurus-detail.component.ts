import { Component, inject, signal, DestroyRef } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ThesaurusService } from '../../../services/thesaurus.service';
import { SkeletonDetailComponent } from '../../../shared/components/skeletons/skeleton-detail.component';
import { BreadcrumbComponent } from '../../../shared/components/breadcrumb/breadcrumb.component';
import type { ThesaurusTermDetail } from '../../../models/thesaurus.models';

type State = 'loading' | 'loaded' | 'notFound' | 'error';

@Component({
  selector: 'app-thesaurus-detail',
  standalone: true,
  imports: [RouterLink, SkeletonDetailComponent, BreadcrumbComponent],
  template: `
    @if (state() === 'loading') {
      <app-skeleton-detail />
    }

    @if (state() === 'notFound') {
      <div class="empty-state">
        <p>Descriptor no encontrado.</p>
        <a routerLink="/vocabulario" class="back-link">Volver al tesauro</a>
      </div>
    }

    @if (state() === 'loaded' && term(); as t) {
      <app-breadcrumb [items]="[{ label: 'Vocabulario', route: '/vocabulario' }, { label: t.label }]" />

      <div class="detail-header">
        <h2>{{ t.label }}</h2>
        <div class="detail-badges">
          @if (t.branch) {
            <span class="badge branch">{{ t.branch }}</span>
          }
          <span class="badge depth">Nivel {{ t.depth }}</span>
        </div>
      </div>

      <a [routerLink]="['/jurisprudencia/resultados']" [queryParams]="{ keywords: t.label }" class="cta-link">
        Buscar fallos con este descriptor
        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="9 18 15 12 9 6"/></svg>
      </a>

      @if (t.synonyms.length) {
        <section class="relation-section">
          <h3>Sinónimos (UF)</h3>
          <div class="chips">
            @for (s of t.synonyms; track s.id) {
              <span class="chip synonym">{{ s.label }}</span>
            }
          </div>
        </section>
      }

      @if (t.broaderTerms.length) {
        <section class="relation-section">
          <h3>Término genérico (TG)</h3>
          <div class="chips">
            @for (bt of t.broaderTerms; track bt.id) {
              <a [routerLink]="['/vocabulario', bt.id]" class="chip link">{{ bt.label }}</a>
            }
          </div>
        </section>
      }

      @if (t.narrowerTerms.length) {
        <section class="relation-section">
          <h3>Términos específicos (TE)</h3>
          <div class="chips">
            @for (nt of t.narrowerTerms; track nt.id) {
              <a [routerLink]="['/vocabulario', nt.id]" class="chip link">{{ nt.label }}</a>
            }
          </div>
        </section>
      }

      @if (t.relatedTerms.length) {
        <section class="relation-section">
          <h3>Términos relacionados (TR)</h3>
          <div class="chips">
            @for (rt of t.relatedTerms; track rt.id) {
              <a [routerLink]="['/vocabulario', rt.id]" class="chip link">{{ rt.label }}</a>
            }
          </div>
        </section>
      }
    }
  `,
  styles: [`
    .detail-header { margin-bottom: 1rem; }
    .detail-header h2 { font-size: 1.25rem; font-weight: 700; margin: 0 0 0.5rem; }
    .detail-badges { display: flex; gap: 6px; flex-wrap: wrap; }
    .badge {
      display: inline-block;
      padding: 3px 10px;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-pill);
      font-size: 0.6875rem;
      font-weight: 600;
      color: var(--color-text-secondary);
    }
    .badge.branch { background: rgba(208, 74, 2, 0.06); color: var(--color-primary); border-color: rgba(208, 74, 2, 0.2); }

    .cta-link {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      color: var(--color-primary);
      font-size: 0.8125rem;
      font-weight: 600;
      text-decoration: none;
      margin-bottom: 1.5rem;
    }
    .cta-link:hover { text-decoration: underline; }

    .relation-section { margin-bottom: 1.25rem; }
    .relation-section h3 {
      font-size: 0.75rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.8px;
      color: var(--color-text-secondary);
      margin: 0 0 0.5rem;
    }
    .chips { display: flex; flex-wrap: wrap; gap: 6px; }
    .chip {
      padding: 4px 12px;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-pill);
      font-size: 0.75rem;
      color: var(--color-text);
    }
    .chip.synonym { font-style: italic; color: var(--color-text-secondary); }
    .chip.link {
      text-decoration: none;
      cursor: pointer;
      transition: border-color 0.15s, background 0.15s;
    }
    .chip.link:hover {
      border-color: rgba(208, 74, 2, 0.3);
      background: rgba(208, 74, 2, 0.04);
      color: var(--color-primary);
    }

    .empty-state { text-align: center; padding: 3rem; color: var(--color-text-secondary); }
    .back-link { color: var(--color-primary); text-decoration: none; font-size: 0.875rem; }
    .back-link:hover { text-decoration: underline; }
  `]
})
export class ThesaurusDetailComponent {
  private route = inject(ActivatedRoute);
  private thesaurusService = inject(ThesaurusService);
  private destroyRef = inject(DestroyRef);

  term = signal<ThesaurusTermDetail | null>(null);
  state = signal<State>('loading');

  constructor() {
    this.route.paramMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        const id = Number(params.get('id'));
        if (!id) { this.state.set('notFound'); return; }
        this.loadTerm(id);
      });
  }

  private loadTerm(id: number) {
    this.state.set('loading');
    this.thesaurusService.getById(id).subscribe({
      next: t => { this.term.set(t); this.state.set('loaded'); },
      error: (err: { status?: number }) => {
        this.state.set(err?.status === 404 ? 'notFound' : 'error');
      }
    });
  }
}
