import { Routes } from '@angular/router';

export const healthCheckRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/health-check-page.component').then((m) => m.HealthCheckPageComponent)
  }
];
