import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <div class="admin-layout">
      <div class="admin-header">
        <h1>Administración</h1>
        <nav class="admin-nav" aria-label="Admin navigation">
          <a routerLink="/admin" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Ingesta</a>
          <a routerLink="/admin/dlq" routerLinkActive="active">Cola de errores</a>
          <a routerLink="/admin/reproceso" routerLinkActive="active">Reproceso</a>
          <a routerLink="/admin/users" routerLinkActive="active">Usuarios</a>
        </nav>
      </div>
      <div class="admin-content">
        <router-outlet />
      </div>
    </div>
  `,
  styles: [`
    .admin-layout {
      padding: 0;
    }
    .admin-header {
      margin-bottom: 1.5rem;
      padding-bottom: 1rem;
    }
    .admin-header h1 {
      font-size: 1.375rem;
      font-weight: 700;
      margin: 0 0 1rem 0;
    }
    .admin-nav {
      display: flex;
      gap: 0;
      flex-wrap: wrap;
      border-bottom: 1px solid var(--color-border-input);
    }
    .admin-nav a {
      color: var(--color-text-body);
      text-decoration: none;
      font-size: 0.875rem;
      font-weight: 500;
      padding: 0.625rem 1.25rem;
      border-bottom: 2px solid transparent;
      margin-bottom: -1px;
      transition: all var(--transition-base);
    }
    .admin-nav a:hover {
      color: var(--color-text);
      border-bottom-color: var(--color-border-input);
    }
    .admin-nav a.active {
      color: var(--color-primary);
      border-bottom-color: var(--color-primary);
      font-weight: 600;
    }
    .admin-content {
      min-height: 200px;
    }
  `]
})
export class AdminLayoutComponent {}
