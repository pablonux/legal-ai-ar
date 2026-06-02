import type { LegalAiArEnvironment } from './environment.model';

/**
 * Local development: API via dev-server proxy (/api → backend).
 */
export const environment: LegalAiArEnvironment = {
  production: false,
  apiUrl: '',
  platformAuthFailurePath: 'sesion-requerida',
  platformLoginUrl: '',
  usePlatformCredentials: false,
  platformSessionRefreshIntervalMs: 45 * 60 * 1000
};
