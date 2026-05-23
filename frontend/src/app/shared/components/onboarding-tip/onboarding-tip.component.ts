import { Component, inject } from '@angular/core';
import { OnboardingService } from '../../../services/onboarding.service';

@Component({
  selector: 'app-onboarding-tip',
  standalone: true,
  template: `
    @if (onboarding.activeTip(); as tip) {
      <div class="ob-overlay" (click)="onboarding.dismiss()">
        <div class="ob-tooltip" (click)="$event.stopPropagation()">
          <div class="ob-header">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 16v-4"/><path d="M12 8h.01"/></svg>
            <h3 class="ob-title">{{ tip.title }}</h3>
            <button type="button" class="ob-close" (click)="onboarding.dismiss()" title="Cerrar">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
            </button>
          </div>
          <p class="ob-body">{{ tip.body }}</p>
          @if (tip.shortcut) {
            <div class="ob-shortcut">
              <kbd>{{ tip.shortcut }}</kbd>
            </div>
          }
          <button type="button" class="ob-dismiss" (click)="onboarding.dismiss()">Entendido</button>
        </div>
      </div>
    }
  `,
  styles: [`
    .ob-overlay {
      position: fixed;
      inset: 0;
      z-index: 10000;
      background: rgba(0,0,0,.3);
      display: flex;
      align-items: center;
      justify-content: center;
      animation: obFade .2s ease-out;
    }
    @keyframes obFade { from { opacity: 0; } to { opacity: 1; } }
    .ob-tooltip {
      background: var(--color-bg, #fff);
      border-radius: 12px;
      padding: 20px 24px;
      max-width: 380px;
      width: 90vw;
      box-shadow: 0 12px 40px rgba(0,0,0,.15);
      animation: obSlideUp .25s ease-out;
    }
    @keyframes obSlideUp { from { transform: translateY(12px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }
    .ob-header {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 10px;
      color: var(--color-primary, #d04a02);
    }
    .ob-title {
      flex: 1;
      margin: 0;
      font-size: .9375rem;
      font-weight: 600;
      color: var(--color-text);
    }
    .ob-close {
      background: none; border: none; cursor: pointer;
      color: var(--color-text-secondary, #78716c);
      padding: 4px;
      border-radius: 4px;
      display: flex;
    }
    .ob-close:hover { background: var(--color-bg-subtle, #f5f5f4); }
    .ob-body {
      margin: 0 0 14px;
      font-size: .8125rem;
      line-height: 1.6;
      color: var(--color-text-secondary, #57534e);
    }
    .ob-shortcut {
      margin-bottom: 14px;
    }
    .ob-shortcut kbd {
      display: inline-block;
      padding: 3px 8px;
      border-radius: 5px;
      border: 1px solid var(--color-border, #e7e5e4);
      background: var(--color-bg-subtle, #fafaf9);
      font-family: inherit;
      font-size: .75rem;
      font-weight: 600;
      color: var(--color-text);
      box-shadow: 0 1px 2px rgba(0,0,0,.06);
    }
    .ob-dismiss {
      display: block;
      width: 100%;
      padding: 8px;
      border: none;
      border-radius: 8px;
      background: var(--color-primary, #d04a02);
      color: #fff;
      font-size: .8125rem;
      font-weight: 600;
      cursor: pointer;
      transition: opacity .15s;
    }
    .ob-dismiss:hover { opacity: .9; }
  `]
})
export class OnboardingTipComponent {
  onboarding = inject(OnboardingService);
}
