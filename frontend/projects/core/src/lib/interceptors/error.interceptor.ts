import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { environment } from '../environments/environment';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401) {
        auth.clearSessionAfterUnauthorized();
        const path =
          (environment as { platformAuthFailurePath?: string }).platformAuthFailurePath ??
          'sesion-requerida';
        void router.navigateByUrl(`/${path}`);
      }
      return throwError(() => err);
    })
  );
};
