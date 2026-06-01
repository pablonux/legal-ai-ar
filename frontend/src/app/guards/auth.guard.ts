import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated()) {
    return true;
  }

  const path =
    (environment as { platformAuthFailurePath?: string }).platformAuthFailurePath ??
    'sesion-requerida';
  return router.createUrlTree([`/${path}`]);
};
