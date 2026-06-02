import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import type { ProceedingResponse } from '../../../models/proceeding.models';
import { RulingDatePipe } from '@legal-ai-ar/shared-common/pipes/ruling-date.pipe';

@Component({
  selector: 'app-proceeding-timeline',
  standalone: true,
  imports: [RouterLink, RulingDatePipe],
  template: `
    @if (proceeding(); as p) {
      <div class="timeline-header">
        <h3>Expediente {{ p.caseNumber }}</h3>
        @if (p.jurisdictionArea) {
          <span class="jurisdiction-badge">{{ p.jurisdictionArea }}</span>
        }
      </div>
      <div class="timeline">
        @for (r of p.rulings; track r.rulingId) {
          <div class="timeline-item" [class.current]="r.isCurrent">
            <div class="timeline-marker">
              <div class="marker-dot"></div>
              @if (!$last) {
                <div class="marker-line"></div>
              }
            </div>
            <div class="timeline-content">
              <div class="timeline-meta">
                <span class="instance-badge">{{ instanceLabel(r.instanceLevel) }}</span>
                <span class="timeline-date">{{ r.rulingDate | rulingDate }}</span>
              </div>
              <div class="timeline-court">{{ r.courtName }}</div>
              @if (r.isCurrent) {
                <div class="timeline-title current-label">{{ r.caseTitle }}</div>
              } @else {
                <a [routerLink]="['/jurisprudencia', r.rulingId]" class="timeline-title link">{{ r.caseTitle }}</a>
              }
              @if (r.rulingDirection) {
                <span class="direction-badge" [class.confirm]="isConfirm(r.rulingDirection)" [class.revoke]="isRevoke(r.rulingDirection)">
                  {{ r.rulingDirection }}
                </span>
              }
            </div>
          </div>
        }
      </div>
    }
  `,
  styles: [`
    :host { display: block; }
    .timeline-header {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin-bottom: 1rem;
    }
    .timeline-header h3 {
      font-size: 0.875rem;
      font-weight: 700;
      margin: 0;
    }
    .jurisdiction-badge {
      font-size: 0.625rem;
      padding: 0.125rem 0.5rem;
      background: var(--color-bg-subtle);
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-pill, 9999px);
      color: var(--color-text-secondary);
    }
    .timeline { display: flex; flex-direction: column; }
    .timeline-item {
      display: flex;
      gap: 0.75rem;
      position: relative;
    }
    .timeline-item.current .timeline-content {
      background: var(--color-primary-light, #fff7ed);
      border-color: var(--color-primary);
    }
    .timeline-marker {
      display: flex;
      flex-direction: column;
      align-items: center;
      flex-shrink: 0;
      width: 1.25rem;
    }
    .marker-dot {
      width: 10px;
      height: 10px;
      border-radius: 50%;
      background: var(--color-border-input);
      border: 2px solid var(--color-bg-surface);
      box-shadow: 0 0 0 2px var(--color-border-input);
      flex-shrink: 0;
      margin-top: 0.375rem;
    }
    .timeline-item.current .marker-dot {
      background: var(--color-primary);
      box-shadow: 0 0 0 2px var(--color-primary);
    }
    .marker-line {
      width: 2px;
      flex: 1;
      background: var(--color-border-input);
      min-height: 1rem;
    }
    .timeline-content {
      flex: 1;
      padding: 0.5rem 0.75rem;
      border: 1px solid var(--color-border-input);
      border-radius: var(--radius-sm);
      margin-bottom: 0.5rem;
    }
    .timeline-meta {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin-bottom: 0.25rem;
    }
    .instance-badge {
      font-size: 0.625rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      padding: 0.0625rem 0.375rem;
      background: var(--color-bg-subtle);
      border-radius: var(--radius-xs, 3px);
      color: var(--color-text-secondary);
    }
    .timeline-date {
      font-size: 0.6875rem;
      color: var(--color-text-secondary);
    }
    .timeline-court {
      font-size: 0.75rem;
      font-weight: 600;
      color: var(--color-text-body);
    }
    .timeline-title {
      font-size: 0.75rem;
      color: var(--color-text-secondary);
      margin-top: 0.125rem;
      display: block;
    }
    .timeline-title.link {
      color: var(--color-primary);
      text-decoration: none;
      cursor: pointer;
    }
    .timeline-title.link:hover { text-decoration: underline; }
    .current-label { font-weight: 600; color: var(--color-text-body); }
    .direction-badge {
      display: inline-block;
      font-size: 0.625rem;
      font-weight: 600;
      padding: 0.0625rem 0.375rem;
      border-radius: var(--radius-xs, 3px);
      margin-top: 0.25rem;
      background: var(--color-bg-subtle);
      color: var(--color-text-secondary);
    }
    .direction-badge.confirm { background: #dcfce7; color: #166534; }
    .direction-badge.revoke { background: #fee2e2; color: #991b1b; }
  `]
})
export class ProceedingTimelineComponent {
  proceeding = input<ProceedingResponse | null>(null);

  instanceLabel(level: number | null): string {
    if (level === null) return 'N/A';
    switch (level) {
      case 1: return '1ra instancia';
      case 2: return '2da instancia';
      case 3: return '3ra instancia';
      default: return `${level}ta instancia`;
    }
  }

  isConfirm(direction: string): boolean {
    const d = direction.toLowerCase();
    return d.includes('confirm') || d.includes('rechaz');
  }

  isRevoke(direction: string): boolean {
    const d = direction.toLowerCase();
    return d.includes('revoc') || d.includes('anul');
  }
}
