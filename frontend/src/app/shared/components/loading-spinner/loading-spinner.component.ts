import { Component, input } from '@angular/core';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [],
  template: `
    <div class="spinner-wrapper" [attr.aria-label]="ariaLabel()">
      <div class="spinner"></div>
      @if (message()) {
        <p class="spinner-message">{{ message() }}</p>
      }
    </div>
  `,
  styles: [`
    .spinner-wrapper {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 1rem;
    }
    .spinner {
      width: 36px;
      height: 36px;
      border: 2.5px solid var(--color-border);
      border-top-color: var(--color-primary);
      border-radius: 50%;
      animation: spin 0.9s linear infinite;
    }
    @keyframes spin { to { transform: rotate(360deg); } }
    .spinner-message {
      margin: 1rem 0 0;
      font-size: 0.8125rem;
      color: var(--color-text-secondary);
    }
  `]
})
export class LoadingSpinnerComponent {
  message = input<string>();
  ariaLabel = input('Cargando');
}
