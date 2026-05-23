import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { OnboardingService } from '../../services/onboarding.service';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [RouterLink, FormsModule],
  template: `
    <div class="welcome-layout">

      <div class="hero">
        <div class="hero-content">
          <h1 class="hero-title">Bienvenido a Legal AI AR{{ displayName() ? ', ' + displayName() : '' }}</h1>
          <p class="hero-desc">Plataforma de jurisprudencia argentina. Busque fallos en lenguaje natural, consulte el asistente con citas verificables o explore el grafo de relaciones.</p>
          <div class="hero-search">
            <div class="hero-search-box">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
              </svg>
              <input
                type="text"
                class="hero-search-input"
                placeholder="Buscar jurisprudencia..."
                [ngModel]="quickQuery()"
                (ngModelChange)="quickQuery.set($event)"
                (keydown.enter)="onQuickSearch()"
                aria-label="Búsqueda rápida"
                autocomplete="off"
              />
            </div>
            <button type="button" class="hero-search-btn" [disabled]="!quickQuery().trim()" (click)="onQuickSearch()">Buscar</button>
          </div>
        </div>
      </div>

      <nav class="quick-links" aria-label="Accesos procesales">
        <a routerLink="/jurisprudencia" class="quick-link">Buscar fallos</a>
        <span class="quick-sep" aria-hidden="true">·</span>
        <a routerLink="/procesos" class="quick-link">Causas</a>
        <span class="quick-sep" aria-hidden="true">·</span>
        <a routerLink="/organismos" class="quick-link">Tribunales</a>
        <span class="quick-sep" aria-hidden="true">·</span>
        <a routerLink="/sujetos" class="quick-link">Intervinientes</a>
        <span class="quick-sep" aria-hidden="true">·</span>
        <a routerLink="/ordenamiento" class="quick-link">Ordenamiento</a>
      </nav>

      <div class="feature-grid">

        <a routerLink="/jurisprudencia" class="feature-card">
          <div class="card-icon" aria-hidden="true">
            <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
            </svg>
          </div>
          <h2 class="card-title">Búsqueda semántica</h2>
          <p class="card-desc">Busque fallos usando lenguaje natural. El motor híbrido encuentra los resultados más relevantes.</p>
          <span class="card-cta">
            Ir a buscar
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="9 18 15 12 9 6"/></svg>
          </span>
        </a>

        <a routerLink="/asistente" class="feature-card">
          <div class="card-icon" aria-hidden="true">
            <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
              <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/>
            </svg>
          </div>
          <h2 class="card-title">Asistente jurisprudencial</h2>
          <p class="card-desc">Consultas legales con citas verificables a fallos. El asistente RAG responde con jurisprudencia.</p>
          <span class="card-cta">
            Abrir asistente
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="9 18 15 12 9 6"/></svg>
          </span>
        </a>

        <a routerLink="/explorador" class="feature-card">
          <div class="card-icon" aria-hidden="true">
            <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="18" cy="5" r="3"/><circle cx="6" cy="12" r="3"/><circle cx="18" cy="19" r="3"/>
              <line x1="8.59" y1="13.51" x2="15.42" y2="17.49"/><line x1="15.41" y1="6.51" x2="8.59" y2="10.49"/>
            </svg>
          </div>
          <h2 class="card-title">Grafo de relaciones</h2>
          <p class="card-desc">Explore las conexiones entre fallos, tribunales, leyes y conceptos legales.</p>
          <span class="card-cta">
            Explorar
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="9 18 15 12 9 6"/></svg>
          </span>
        </a>

        <a routerLink="/estadisticas" class="feature-card">
          <div class="card-icon" aria-hidden="true">
            <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
              <path d="M21 12V7H5a2 2 0 0 1 0-4h14v4"/>
              <path d="M3 5v14a2 2 0 0 0 2 2h16v-5"/>
              <path d="M18 12a2 2 0 0 0 0 4h4v-4Z"/>
            </svg>
          </div>
          <h2 class="card-title">Estadísticas</h2>
          <p class="card-desc">Composición, cobertura y calidad de la base de jurisprudencia.</p>
          <span class="card-cta">
            Ver estadísticas
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="9 18 15 12 9 6"/></svg>
          </span>
        </a>

      </div>

    </div>
  `,
  styles: [`
    :host {
      display: block;
      margin: -2rem -2.5rem;
    }

    .welcome-layout {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1.5rem;
      padding: 0 2rem 3rem;
    }

    .hero {
      width: 100%;
      background: linear-gradient(135deg, #fff 0%, #fff8f5 100%);
      border-bottom: 1px solid var(--color-border);
      padding: 2.5rem 2rem;
      display: flex;
      justify-content: center;
    }

    .hero-content {
      max-width: 640px;
      width: 100%;
    }

    .hero-title {
      font-family: var(--font-heading);
      font-size: 1.5rem;
      font-weight: 600;
      letter-spacing: -0.03em;
      color: var(--color-text);
      margin: 0 0 0.5rem 0;
    }

    .hero-desc {
      font-size: 0.9375rem;
      line-height: 1.55;
      color: var(--color-text-secondary);
      margin: 0 0 1.25rem 0;
    }

    .hero-search {
      display: flex;
      gap: 0.625rem;
    }

    .hero-search-box {
      flex: 1;
      display: flex;
      align-items: center;
      gap: 0.5rem;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border-input);
      border-radius: 10px;
      padding: 0 1rem;
      transition: border-color 0.15s, box-shadow 0.15s;
    }

    .hero-search-box:focus-within {
      border-color: var(--color-primary);
      box-shadow: 0 0 0 2px rgba(208, 74, 2, 0.08);
    }

    .hero-search-box svg {
      color: var(--color-text-secondary);
      flex-shrink: 0;
    }

    .hero-search-box:focus-within svg {
      color: var(--color-primary);
    }

    .hero-search-input {
      flex: 1;
      border: none;
      outline: none;
      background: transparent;
      font-family: inherit;
      font-size: 0.9375rem;
      color: var(--color-text);
      padding: 0.625rem 0;
    }

    .hero-search-input::placeholder {
      color: var(--color-text-secondary);
    }

    .hero-search-btn {
      padding: 0 1.5rem;
      border: none;
      border-radius: 10px;
      font-size: 0.875rem;
      font-weight: 600;
      background: var(--color-primary);
      color: #fff;
      cursor: pointer;
      white-space: nowrap;
      transition: background 0.15s, opacity 0.15s;
    }

    .hero-search-btn:hover:not(:disabled) {
      background: var(--color-primary-hover);
    }

    .hero-search-btn:disabled {
      opacity: 0.4;
      cursor: default;
    }

    .quick-links {
      display: flex;
      flex-wrap: wrap;
      align-items: center;
      justify-content: center;
      gap: 0.25rem 0.5rem;
      max-width: 640px;
      width: 100%;
      padding: 0 1rem;
      font-size: 0.8125rem;
    }
    .quick-link {
      color: var(--color-primary);
      font-weight: 600;
      text-decoration: none;
    }
    .quick-link:hover { text-decoration: underline; }
    .quick-sep { color: var(--color-text-secondary); user-select: none; }

    .feature-grid {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: 14px;
      width: 100%;
      max-width: 940px;
    }

    @media (max-width: 900px) {
      .feature-grid {
        grid-template-columns: repeat(2, 1fr);
      }
    }

    @media (max-width: 520px) {
      .feature-grid {
        grid-template-columns: 1fr;
        max-width: 360px;
      }

      .hero {
        padding: 1.5rem 1rem;
      }

      .hero-search {
        flex-direction: column;
      }

      .hero-search-btn {
        padding: 0.625rem 1.5rem;
      }
    }

    .feature-card {
      display: flex;
      flex-direction: column;
      gap: 10px;
      padding: 22px 20px 18px;
      background: var(--color-bg-surface);
      border: 1px solid var(--color-border);
      border-radius: 12px;
      text-align: left;
      text-decoration: none;
      color: inherit;
      transition: border-color 0.15s, box-shadow 0.15s;
      cursor: pointer;
    }

    .feature-card:hover {
      border-color: var(--color-primary);
      box-shadow: 0 0 0 1px rgba(208, 74, 2, 0.08);
    }

    .feature-card:focus-visible {
      outline: none;
      border-color: var(--color-primary);
      box-shadow: 0 0 0 3px rgba(208, 74, 2, 0.12);
    }

    .card-icon {
      flex-shrink: 0;
      width: 38px;
      height: 38px;
      border-radius: 10px;
      background: rgba(208, 74, 2, 0.08);
      color: var(--color-primary);
      display: flex;
      align-items: center;
      justify-content: center;
      transition: background 0.15s;
    }

    .feature-card:hover .card-icon {
      background: rgba(208, 74, 2, 0.13);
    }

    .card-title {
      font-size: 0.9375rem;
      font-weight: 600;
      color: var(--color-text);
      margin: 0;
    }

    .card-desc {
      font-size: 0.8125rem;
      line-height: 1.55;
      color: var(--color-text-secondary);
      margin: 0;
      flex: 1;
    }

    .card-cta {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      font-size: 0.8125rem;
      font-weight: 600;
      color: var(--color-primary);
      margin-top: 2px;
      transition: gap 0.15s;
    }

    .feature-card:hover .card-cta {
      gap: 7px;
    }
  `]
})
export class WelcomeComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private onboarding = inject(OnboardingService);

  ngOnInit() { this.onboarding.tryShow('welcome-palette'); }

  quickQuery = signal('');

  displayName(): string {
    const u = this.authService.getUser();
    if (u?.displayName) return u.displayName;
    if (u?.email) {
      const name = u.email.split('@')[0].replace(/[._-]/g, ' ');
      return name.split(' ').map(w => w.charAt(0).toUpperCase() + w.slice(1)).join(' ');
    }
    return '';
  }

  onQuickSearch() {
    const q = this.quickQuery().trim();
    if (!q) return;
    this.router.navigate(['/jurisprudencia/resultados'], { queryParams: { query: q, page: '1' } });
  }
}
