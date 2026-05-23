import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-session-required',
  standalone: true,
  imports: [MatButtonModule],
  template: `
    <div class="gate-page">
      <div class="gate-card">
        <div class="brand">
          <img src="pwc-light.svg" alt="PwC" class="brand-logo" />
          <div class="brand-divider"></div>
          <div class="brand-app">
            <span class="brand-name">Legal AI AR</span>
            <span class="brand-tag">Jurisprudencia con Inteligencia Artificial</span>
          </div>
        </div>

        <div class="gate-body">
          <h1 class="gate-title">Sesión requerida</h1>
          <p class="gate-sub">
            Tu sesión expiró o no está disponible. Iniciá sesión con tu cuenta corporativa para continuar.
          </p>
          @if (platformLoginUrl) {
            <a class="sso-btn" [href]="platformLoginUrl">Iniciar sesión con SSO</a>
          }
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .gate-page {
        min-height: 100vh;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 1.5rem;
        background: linear-gradient(145deg, #f5f7fa 0%, #e8ecf1 50%, #dfe6ed 100%);
      }

      .gate-card {
        width: 100%;
        max-width: 440px;
        background: #fff;
        border-radius: 12px;
        box-shadow: 0 4px 24px rgba(0, 0, 0, 0.08);
        overflow: hidden;
      }

      .brand {
        display: flex;
        align-items: center;
        gap: 1rem;
        padding: 1.5rem 1.75rem;
        background: #2d2d2d;
        color: #fff;
      }

      .brand-logo {
        height: 28px;
        width: auto;
      }

      .brand-divider {
        width: 1px;
        height: 32px;
        background: rgba(255, 255, 255, 0.25);
      }

      .brand-app {
        display: flex;
        flex-direction: column;
        gap: 0.15rem;
      }

      .brand-name {
        font-size: 1.125rem;
        font-weight: 600;
        letter-spacing: -0.02em;
      }

      .brand-tag {
        font-size: 0.75rem;
        opacity: 0.85;
      }

      .gate-body {
        padding: 1.75rem;
        display: flex;
        flex-direction: column;
        gap: 1rem;
      }

      .gate-title {
        margin: 0;
        font-size: 1.35rem;
        font-weight: 600;
        color: #1a1a1a;
      }

      .gate-sub {
        margin: 0;
        font-size: 0.95rem;
        line-height: 1.5;
        color: #5c5c5c;
      }

      .sso-btn {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        margin-top: 0.5rem;
        padding: 0.75rem 1.25rem;
        background: #d04a02;
        color: #fff;
        font-weight: 600;
        font-size: 0.95rem;
        text-decoration: none;
        border-radius: 6px;
        transition: background 0.15s ease;
      }

      .sso-btn:hover {
        background: #b84202;
      }
    `
  ]
})
export class SessionRequiredComponent {
  readonly platformLoginUrl = environment.platformLoginUrl ?? '';
}
