import type { LegalAiArEnvironment } from './environment.model';

/**
 * QA environment (same-origin or dedicated QA API host).
 */
export const environment: LegalAiArEnvironment = {
  production: true,
  apiUrl: 'https://legal-ai-ar-api-qa.azurewebsites.net',
  platformAuthFailurePath: 'sesion-requerida',
  platformLoginUrl: '',
  usePlatformCredentials: false,
  platformSessionRefreshIntervalMs: 45 * 60 * 1000
};
