import type { LegalAiArEnvironment } from './environment.model';

const engagementId = 'ddf6b108-ced9-4827-a133-9c82141ebf29';

/**
 * Staging environment configuration.
 */
export const environment: LegalAiArEnvironment = {
  production: true,
  apiUrl: 'https://legal-ai-ar-api-staging.azurewebsites.net',
  platformAuthFailurePath: 'sesion-requerida',
  platformLoginUrl: '',
  usePlatformCredentials: false
};
