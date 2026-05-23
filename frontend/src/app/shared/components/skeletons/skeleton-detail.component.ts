import { Component } from '@angular/core';

@Component({
  selector: 'app-skeleton-detail',
  standalone: true,
  template: `
    <div class="sk-detail">
      <div class="sk-badges">
        <div class="skeleton sk-badge"></div>
        <div class="skeleton sk-badge sk-badge-sm"></div>
      </div>
      <div class="skeleton sk-heading"></div>
      <div class="sk-sub">
        <div class="skeleton sk-court"></div>
        <div class="skeleton sk-case"></div>
      </div>

      <div class="sk-split">
        <div class="sk-doc">
          <div class="skeleton sk-doc-area"></div>
        </div>
        <div class="sk-info">
          <div class="skeleton sk-card-block"></div>
          <div class="skeleton sk-card-block sk-card-short"></div>
          <div class="sk-tab-bar">
            <div class="skeleton sk-tab"></div>
            <div class="skeleton sk-tab"></div>
            <div class="skeleton sk-tab"></div>
          </div>
          <div class="sk-meta-grid">
            <div class="skeleton sk-meta-item"></div>
            <div class="skeleton sk-meta-item"></div>
            <div class="skeleton sk-meta-item"></div>
            <div class="skeleton sk-meta-item"></div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .sk-detail { max-width: 1400px; margin: 0 auto; }

    .sk-badges {
      display: flex;
      gap: 6px;
      margin-bottom: 1rem;
    }

    .sk-badge {
      height: 20px;
      width: 80px;
      border-radius: var(--radius-pill);
    }

    .sk-badge-sm { width: 50px; }

    .sk-heading {
      height: 22px;
      width: 55%;
      margin-bottom: 0.75rem;
    }

    .sk-sub {
      display: flex;
      gap: 1rem;
      margin-bottom: 1.75rem;
    }

    .sk-court { height: 14px; width: 200px; }
    .sk-case { height: 14px; width: 100px; }

    .sk-split {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 2rem;
    }

    .sk-doc-area {
      height: 400px;
      border-radius: var(--radius-md);
    }

    .sk-info {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .sk-card-block {
      height: 100px;
      border-radius: var(--radius-md);
    }

    .sk-card-short { height: 70px; }

    .sk-tab-bar {
      display: flex;
      gap: 1rem;
      padding-bottom: 0.5rem;
      border-bottom: 1px solid var(--color-border);
    }

    .sk-tab { height: 14px; width: 80px; }

    .sk-meta-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 0.75rem;
    }

    .sk-meta-item {
      height: 48px;
      border-radius: var(--radius-sm);
    }

    @media (max-width: 900px) {
      .sk-split { grid-template-columns: 1fr; }
    }
  `]
})
export class SkeletonDetailComponent {}
