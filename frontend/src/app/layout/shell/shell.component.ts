import { Component, HostListener, inject, signal } from '@angular/core';
import { Router, NavigationEnd, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { CommandPaletteComponent } from '../../shared/components/command-palette/command-palette.component';
import { StatusBarComponent } from '../status-bar/status-bar.component';
import { OnboardingTipComponent } from '../../shared/components/onboarding-tip/onboarding-tip.component';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommandPaletteComponent, StatusBarComponent, OnboardingTipComponent],
  template: `
    <div class="shell">
      <header class="header">
        <div class="header-left">
          <button
            type="button"
            class="hamburger-btn"
            (click)="toggleSidebar()"
            [attr.aria-expanded]="sidebarOpen()"
            aria-label="Abrir menú de navegación"
          >
            <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
              @if (sidebarOpen()) {
                <line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>
              } @else {
                <line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="18" x2="21" y2="18"/>
              }
            </svg>
          </button>
          <div class="header-brand">
            <img src="pwc.svg" alt="PwC" class="header-logo" />
            <span class="header-title">Legal AI AR</span>
          </div>
        </div>
        <div class="header-options">
          <button type="button" class="logout-btn" (click)="logout()" [title]="'Cerrar sesión (' + (user()?.email ?? '') + ')'">
            <span class="avatar">{{ initials() }}</span>
            <span class="logout-text">Cerrar sesión</span>
          </button>
        </div>
      </header>

      @if (sidebarOpen()) {
        <div class="sidebar-backdrop" (click)="closeSidebar()"></div>
      }

      <div class="main">
        <nav class="sidebar" [class.open]="sidebarOpen()" aria-label="Navegación principal">
          <div class="nav-section-label">Consulta</div>
          <a routerLink="/bienvenida" routerLinkActive="active" class="nav-item" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/>
                <polyline points="9 22 9 12 15 12 15 22"/>
              </svg>
            </span>
            <span class="nav-label">Inicio</span>
            <span class="shortcut-hint" aria-hidden="true">G H</span>
          </a>
          <a routerLink="/jurisprudencia" routerLinkActive="active" class="nav-item" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <circle cx="11" cy="11" r="8"/>
                <line x1="21" y1="21" x2="16.65" y2="16.65"/>
              </svg>
            </span>
            <span class="nav-label">Jurisprudencia</span>
            <span class="shortcut-hint" aria-hidden="true">G J</span>
          </a>

          <div class="nav-section-label">Proceso</div>
          <a routerLink="/procesos" routerLinkActive="active" class="nav-item" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <path d="M16 4h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2"/><rect x="8" y="2" width="8" height="4" rx="1" ry="1"/>
              </svg>
            </span>
            <span class="nav-label">Causas</span>
            <span class="shortcut-hint" aria-hidden="true">G P</span>
          </a>
          <a routerLink="/organismos" routerLinkActive="active" class="nav-item" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <path d="M3 21h18"/><path d="M5 21V7l8-4v18"/><path d="M19 21V11l-6-4"/><path d="M9 9h.01"/><path d="M9 13h.01"/><path d="M9 17h.01"/>
              </svg>
            </span>
            <span class="nav-label">Tribunales</span>
            <span class="shortcut-hint" aria-hidden="true">G O</span>
          </a>

          <div class="nav-section-label">Personas</div>
          <a routerLink="/sujetos" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: false }" class="nav-item" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/>
              </svg>
            </span>
            <span class="nav-label">Intervinientes</span>
            <span class="shortcut-hint" aria-hidden="true">G S</span>
          </a>

          <div class="nav-section-label">Normas</div>
          <a routerLink="/ordenamiento" routerLinkActive="active" class="nav-item" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/>
              </svg>
            </span>
            <span class="nav-label">Ordenamiento</span>
            <span class="shortcut-hint" aria-hidden="true">G R</span>
          </a>
          <a routerLink="/vocabulario" routerLinkActive="active" class="nav-item" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"/>
                <line x1="8" y1="7" x2="16" y2="7"/><line x1="8" y1="11" x2="14" y2="11"/>
              </svg>
            </span>
            <span class="nav-label">Descriptores</span>
            <span class="shortcut-hint" aria-hidden="true">G V</span>
          </a>

          <div class="nav-section-label">Herramientas</div>
          <a routerLink="/asistente" routerLinkActive="active" class="nav-item" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/>
              </svg>
            </span>
            <span class="nav-label">Asistente</span>
            <span class="shortcut-hint" aria-hidden="true">G A</span>
          </a>
          <a routerLink="/explorador" routerLinkActive="active" class="nav-item" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <circle cx="18" cy="5" r="3"/><circle cx="6" cy="12" r="3"/><circle cx="18" cy="19" r="3"/>
                <line x1="8.59" y1="13.51" x2="15.42" y2="17.49"/><line x1="15.41" y1="6.51" x2="8.59" y2="10.49"/>
              </svg>
            </span>
            <span class="nav-label">Explorador</span>
            <span class="shortcut-hint" aria-hidden="true">G E</span>
          </a>
          <a routerLink="/estadisticas" routerLinkActive="active" class="nav-item" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                <path d="M21 12V7H5a2 2 0 0 1 0-4h14v4"/>
                <path d="M3 5v14a2 2 0 0 0 2 2h16v-5"/>
                <path d="M18 12a2 2 0 0 0 0 4h4v-4Z"/>
              </svg>
            </span>
            <span class="nav-label">Estadísticas</span>
            <span class="shortcut-hint" aria-hidden="true">G D</span>
          </a>

          <div class="nav-section-label">Avanzado</div>
          <a routerLink="/ontologia" routerLinkActive="active" class="nav-item" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                <polygon points="12 2 2 7 12 12 22 7 12 2"/><polyline points="2 17 12 22 22 17"/><polyline points="2 12 12 17 22 12"/>
              </svg>
            </span>
            <span class="nav-label">Ontología</span>
            <span class="shortcut-hint" aria-hidden="true">G N</span>
          </a>

          <div class="nav-spacer"></div>

          <div class="nav-section-label">Administración</div>
          <a routerLink="/admin" routerLinkActive="active" class="nav-item" [routerLinkActiveOptions]="{ exact: false }" (click)="closeSidebar()">
            <span class="nav-icon" aria-hidden="true">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <circle cx="12" cy="12" r="3"/><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z"/>
              </svg>
            </span>
            Admin
          </a>
        </nav>

        <main class="content">
          <router-outlet />
        </main>
      </div>

      <app-status-bar />

      <app-command-palette />
      <app-onboarding-tip />
    </div>
  `,
  styles: [`
    .shell {
      display: flex;
      flex-direction: column;
      height: 100vh;
      --header-height: 68px;
    }

    .header {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      height: var(--header-height);
      padding: 0 1rem 0 1.5rem;
      display: flex;
      align-items: center;
      justify-content: space-between;
      background: color-mix(in srgb, var(--color-bg-surface) 88%, transparent);
      backdrop-filter: blur(12px);
      -webkit-backdrop-filter: blur(12px);
      box-shadow: var(--shadow-sm);
      z-index: var(--z-header);
    }

    .header-left {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .hamburger-btn {
      display: none;
      align-items: center;
      justify-content: center;
      width: 2.25rem;
      height: 2.25rem;
      padding: 0;
      background: none;
      border: none;
      border-radius: var(--radius-sm);
      color: var(--color-text-body);
      cursor: pointer;
      transition: color var(--transition-fast), background var(--transition-fast);
    }

    .hamburger-btn:hover {
      color: var(--color-primary);
      background: var(--color-bg-subtle);
    }

    .hamburger-btn:focus-visible {
      outline: 2px solid var(--color-primary);
      outline-offset: 2px;
    }

    .header-brand {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .header-logo {
      height: 1.5rem;
      width: auto;
    }

    .header-title {
      font-size: 1rem;
      font-weight: 500;
      color: var(--color-text);
    }

    .header-options {
      display: flex;
      gap: 1rem;
      align-items: center;
      font-size: 0.875rem;
      color: var(--color-text-body);
    }

    .logout-btn {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.375rem 0.5rem;
      margin: 0 -0.25rem;
      background: none;
      border: none;
      border-radius: var(--radius-sm);
      cursor: pointer;
      font-size: 0.875rem;
      color: var(--color-text-body);
      transition: color var(--transition-fast), background var(--transition-fast);
    }

    .logout-btn:hover {
      color: var(--color-primary);
      background: var(--color-bg-subtle);
    }

    .logout-btn:focus-visible {
      outline: 2px solid var(--color-primary);
      outline-offset: 2px;
    }

    .user-info {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.875rem;
      color: var(--color-text-body);
    }

    .avatar {
      width: 2.25rem;
      height: 2.25rem;
      border-radius: 50%;
      background: var(--color-primary);
      color: #fff;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 0.75rem;
      font-weight: 600;
      box-shadow: var(--shadow-xs);
    }

    .sidebar-backdrop {
      display: none;
    }

    .main {
      display: flex;
      flex: 1;
      min-height: 0;
      padding-top: var(--header-height);
      background: var(--color-bg-page);
    }

    .sidebar {
      width: var(--nav-width);
      background: var(--color-bg-surface);
      border-right: 1px solid var(--color-border-input);
      padding: 1rem 1rem 1.75rem;
      flex-shrink: 0;
      display: flex;
      flex-direction: column;
      gap: 2px;
      overflow-y: auto;
      min-height: 0;
    }

    .nav-section-label {
      font-size: 0.5625rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.08em;
      color: var(--color-text-secondary);
      padding: 0.75rem 0.75rem 0.25rem;
      margin-top: 0.25rem;
    }

    .nav-section-label:first-child {
      margin-top: 0;
      padding-top: 0.5rem;
    }

    .nav-item {
      display: flex;
      align-items: center;
      padding: 0.75rem 1rem 0.75rem calc(0.75rem - 4px);
      height: 2.75rem;
      border-radius: var(--radius-sm);
      border-left: 4px solid transparent;
      color: var(--color-text);
      gap: 1.25rem;
      text-decoration: none;
      font-size: 0.875rem;
      transition: background var(--transition-base), border-color var(--transition-base), color var(--transition-base);
    }

    .nav-item:hover {
      background: var(--color-bg-subtle);
    }

    .nav-item.active {
      border-left-color: var(--color-primary);
      background: var(--color-nav-active-bg);
      font-weight: 600;
      color: var(--color-primary);
    }

    .nav-item .nav-icon {
      width: 24px;
      height: 24px;
      flex-shrink: 0;
      display: flex;
      align-items: center;
      justify-content: center;
      color: var(--color-text-body);
    }

    .nav-item.active .nav-icon {
      color: var(--color-primary);
    }

    .nav-item .nav-icon svg {
      display: block;
    }

    .nav-label {
      flex: 1;
      min-width: 0;
    }

    .shortcut-hint {
      font-size: 0.6875rem;
      font-weight: 500;
      font-family: var(--font-mono, ui-monospace, monospace);
      color: var(--color-text-secondary);
      background: #f3f3f3;
      padding: 1px 5px;
      border-radius: 3px;
      letter-spacing: 0.04em;
      opacity: 0.6;
      flex-shrink: 0;
      transition: opacity var(--transition-fast);
    }

    .nav-item:hover .shortcut-hint {
      opacity: 1;
    }

    .nav-item.active .shortcut-hint {
      opacity: 0.7;
      background: rgba(208, 74, 2, 0.08);
      color: var(--color-primary);
    }

    .nav-spacer {
      flex: 1;
    }

    .content {
      flex: 1;
      padding: 2rem 2.5rem;
      overflow-y: auto;
      min-width: 0;
      min-height: 0;
    }


    @media (max-width: 768px) {
      .hamburger-btn {
        display: flex;
      }

      .sidebar {
        position: fixed;
        top: var(--header-height);
        left: 0;
        bottom: 0;
        z-index: 150;
        transform: translateX(-100%);
        transition: transform 0.25s ease;
        box-shadow: none;
        width: 280px;
      }

      .sidebar.open {
        transform: translateX(0);
        box-shadow: 4px 0 24px rgba(0, 0, 0, 0.12);
      }

      .sidebar-backdrop {
        display: block;
        position: fixed;
        inset: 0;
        top: var(--header-height);
        background: rgba(0, 0, 0, 0.3);
        z-index: 140;
      }

      .content {
        padding: 1.5rem 1rem;
      }

      .logout-text, .user-email {
        display: none;
      }

      .shortcut-hint {
        display: none;
      }
    }
  `]
})
export class ShellComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  user = this.authService.user;
  sidebarOpen = signal(false);

  private gPending = false;
  private gTimeout: ReturnType<typeof setTimeout> | null = null;

  private static shortcuts: Record<string, string> = {
    h: '/bienvenida',
    j: '/jurisprudencia',
    o: '/organismos',
    s: '/sujetos',
    v: '/vocabulario',
    r: '/ordenamiento',
    p: '/procesos',
    a: '/asistente',
    e: '/explorador',
    d: '/estadisticas',
    n: '/ontologia',
  };

  constructor() {
    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe(() => this.sidebarOpen.set(false));
  }

  @HostListener('document:keydown', ['$event'])
  onKeydown(e: KeyboardEvent) {
    const tag = (e.target as HTMLElement)?.tagName;
    if (tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT' || (e.target as HTMLElement)?.isContentEditable) return;
    if (e.ctrlKey || e.metaKey || e.altKey) return;

    const key = e.key.toLowerCase();

    if (this.gPending) {
      this.gPending = false;
      if (this.gTimeout) { clearTimeout(this.gTimeout); this.gTimeout = null; }
      const route = ShellComponent.shortcuts[key];
      if (route) {
        e.preventDefault();
        this.router.navigate([route]);
      }
      return;
    }

    if (key === 'g') {
      this.gPending = true;
      this.gTimeout = setTimeout(() => { this.gPending = false; }, 500);
    }
  }

  toggleSidebar() {
    this.sidebarOpen.update(v => !v);
  }

  closeSidebar() {
    this.sidebarOpen.set(false);
  }

  initials(): string {
    const u = this.authService.getUser();
    if (!u?.email) return '?';
    const parts = u.email.split('@')[0].split(/[._-]/);
    if (parts.length >= 2) {
      return (parts[0][0] + parts[1][0]).toUpperCase().slice(0, 2);
    }
    return u.email.slice(0, 2).toUpperCase();
  }

  logout() {
    this.authService.logout();
  }
}
