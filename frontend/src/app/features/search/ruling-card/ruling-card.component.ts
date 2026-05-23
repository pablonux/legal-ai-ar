import { Component, input, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { RulingDatePipe } from '../../../shared/pipes/ruling-date.pipe';
import { EntityPreviewDirective } from '../../../shared/directives/entity-preview.directive';
import type { RulingCardData } from '../../../models/ruling.models';

@Component({
  selector: 'app-ruling-card',
  standalone: true,
  imports: [RouterLink, RulingDatePipe, EntityPreviewDirective],
  template: `
    <a [routerLink]="['/jurisprudencia', ruling().id]" [entityPreview]="{ type: 'ruling', id: ruling().id }" class="ruling-card">
      <h3 class="ruling-title">{{ ruling().caseTitle }}</h3>
      <div class="ruling-meta">
        <span class="ruling-date">{{ ruling().rulingDate | rulingDate }}</span>
        @if (ruling().jurisdictionArea || ruling().instance) {
          <span class="ruling-subtitle">{{ subtitle() }}</span>
        }
        @if (ruling().subjectArea) {
          <span class="ruling-tag">{{ ruling().subjectArea }}</span>
        }
        @if (ruling().resourceType) {
          <span class="ruling-tag">{{ ruling().resourceType }}</span>
        }
        @if (ruling().isUnconstitutional) {
          <span class="ruling-tag tag-unconst">Inconstitucionalidad</span>
        }
        @if (ruling().legalBranch) {
          <span class="ruling-tag">{{ ruling().legalBranch }}</span>
        }
        @if (ruling().precedentWeight) {
          <span class="ruling-tag">{{ ruling().precedentWeight }}</span>
        }
        @if (ruling().isPlenario) {
          <span class="ruling-tag">Plenario</span>
        }
        @if (ruling().isLeadingCase) {
          <span class="ruling-tag">Leading case</span>
        }
      </div>
      @if (ruling().summary) {
        <p class="ruling-summary">{{ truncate(ruling().summary ?? '', 150) }}</p>
      }
      @if (scoreLabel()) {
        <span class="ruling-score">{{ scoreLabel() }}</span>
      }
      @if (ruling().highlightedText) {
        <p class="ruling-highlight" [innerHTML]="ruling().highlightedText"></p>
      }
    </a>
  `,
  styles: [`
    .ruling-card {
      display: block;
      padding: 1.5rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      background: var(--color-bg-surface);
      text-decoration: none;
      color: inherit;
      transition:
        border-color var(--transition-fast),
        box-shadow var(--transition-fast),
        transform var(--transition-fast);
    }
    .ruling-card:hover {
      border-color: var(--color-primary);
      box-shadow: var(--shadow-md);
      transform: translateY(-1px);
    }
    .ruling-title {
      margin: 0 0 0.5rem;
      font-size: 0.9375rem;
      font-weight: 600;
      color: var(--color-primary);
      line-height: 1.3;
    }
    .ruling-meta {
      display: flex;
      gap: 1rem;
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
      margin-bottom: 0.5rem;
    }
    .ruling-subtitle { font-style: italic; }
    .ruling-tag {
      font-size: 0.6875rem;
      font-weight: 500;
      padding: 1px 8px;
      border-radius: var(--radius-pill);
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      white-space: nowrap;
    }
    .tag-unconst {
      background: rgba(208, 74, 2, 0.08);
      border-color: rgba(208, 74, 2, 0.25);
      color: var(--color-primary);
      font-weight: 600;
    }
    .ruling-summary {
      margin: 0.5rem 0;
      font-size: 0.875rem;
      color: var(--color-text-body);
      line-height: 1.5;
    }
    .ruling-score {
      display: inline-block;
      font-size: 0.6875rem;
      font-weight: 600;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border);
      color: var(--color-text-secondary);
      padding: 2px 10px;
      border-radius: var(--radius-pill);
      margin-top: 0.5rem;
    }
    .ruling-highlight {
      margin: 0.5rem 0 0;
      padding-left: 0.75rem;
      border-left: 3px solid var(--color-primary-amber);
      background: var(--color-bg-subtle);
      font-size: 0.875rem;
      color: var(--color-text-body);
      font-style: italic;
    }
  `]
})
export class RulingCardComponent {
  ruling = input.required<RulingCardData>();
  maxScore = input<number>(0);

  subtitle = computed(() => {
    const r = this.ruling();
    return [r.jurisdictionArea, r.instance].filter(Boolean).join(' · ');
  });

  scoreLabel = computed(() => {
    const r = this.ruling();
    const max = this.maxScore();

    if (r.relevanceScore != null && r.relevanceScore > 0 && max > 0) {
      const pct = Math.round((r.relevanceScore / max) * 100);
      return `${pct}% relevancia`;
    }
    if (r.similarityScore != null && r.similarityScore > 0) {
      return `${(r.similarityScore * 100).toFixed(0)}% similitud`;
    }
    return null;
  });

  truncate(text: string, maxLen: number): string {
    if (!text || text.length <= maxLen) return text;
    return text.slice(0, maxLen).trim() + '…';
  }
}
