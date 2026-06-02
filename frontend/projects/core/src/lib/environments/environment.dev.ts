import type { LegalAiArEnvironment } from './environment.model';

/**
 * Azure dev / GitHub Actions staging SPA (adjust apiUrl when FT05 pipeline is wired).
 */
export const environment: LegalAiArEnvironment = {
  production: false,
  apiUrl: 'https://legal-ai-ar-api-dev.azurewebsites.net',
  platformAuthFailurePath: 'sesion-requerida',
  platformLoginUrl: '',
  usePlatformCredentials: false,
  platformSessionRefreshIntervalMs: 45 * 60 * 1000
};
