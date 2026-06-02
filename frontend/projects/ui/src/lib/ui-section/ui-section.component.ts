import { Component, input } from '@angular/core';

/**
 * Thin layout wrapper — placeholder until PwC AppKit ap-panel is wired (F00-W03).
 * UI labels passed via `title` stay in Spanish at call sites.
 */
@Component({
  selector: 'app-ui-section',
  standalone: true,
  template: `
    <section class="la-ui-section">
      @if (title()) {
        <header class="la-ui-section__header">
          <h2 class="la-ui-section__title">{{ title() }}</h2>
        </header>
      }
      <div class="la-ui-section__body">
        <ng-content />
      </div>
    </section>
  `,
  styles: [
    `
      .la-ui-section {
        display: block;
        margin-bottom: var(--spacing-lg, 1.5rem);
      }
      .la-ui-section__header {
        margin-bottom: var(--spacing-sm, 0.75rem);
      }
      .la-ui-section__title {
        margin: 0;
        font-size: var(--font-size-lg, 1.125rem);
        font-weight: 600;
        color: var(--color-text-primary, inherit);
      }
    `
  ]
})
export class UiSectionComponent {
  readonly title = input<string | undefined>(undefined);
}
