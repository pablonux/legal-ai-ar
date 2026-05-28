import { APP_INITIALIZER, ApplicationConfig, LOCALE_ID, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, MAT_NATIVE_DATE_FORMATS } from '@angular/material/core';
import { firstValueFrom } from 'rxjs';
import localeEsAr from '@angular/common/locales/es-AR';
import { registerLocaleData } from '@angular/common';

import { ArgDateAdapter } from './shared/adapters/arg-date-adapter';

import { routes } from './app.routes';
import { errorInterceptor } from './interceptors/error.interceptor';
import { platformCredentialsInterceptor } from './interceptors/platform-credentials.interceptor';
import { AuthService } from './services/auth.service';

registerLocaleData(localeEsAr);

function initAuth(auth: AuthService) {
  return () => firstValueFrom(auth.bootstrapSession());
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([platformCredentialsInterceptor, errorInterceptor])
    ),
    provideAnimationsAsync(),
    { provide: MAT_DATE_LOCALE, useValue: 'es-AR' },
    { provide: DateAdapter, useClass: ArgDateAdapter, deps: [MAT_DATE_LOCALE] },
    { provide: MAT_DATE_FORMATS, useValue: MAT_NATIVE_DATE_FORMATS },
    { provide: LOCALE_ID, useValue: 'es-AR' },
    {
      provide: APP_INITIALIZER,
      useFactory: initAuth,
      deps: [AuthService],
      multi: true
    }
  ]
};
