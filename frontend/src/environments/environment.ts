import type { LegalAiArEnvironment } from './environment.model';

/**
 * Local development: API injects platform headers (Auth:Development); no login form.
 */
export const environment: LegalAiArEnvironment = {
  production: false,
  apiUrl: 'http://localhost:5088',
  platformAuthFailurePath: 'sesion-requerida',
  platformLoginUrl: '',
  usePlatformCredentials: false
};
