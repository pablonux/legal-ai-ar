import type { LegalAiArEnvironment } from './environment.model';

/**
 * Staging environment configuration.
 */
export const environment: LegalAiArEnvironment = {
  production: true,
  apiUrl: 'https://legal-ai-ar-api-staging.azurewebsites.net',
  platformAuthFailurePath: 'sesion-requerida',
  platformLoginUrl: '',
  usePlatformCredentials: false,
  platformSessionRefreshIntervalMs: 45 * 60 * 1000
};
