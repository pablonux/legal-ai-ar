import type { LegalAiArEnvironment } from './environment.model';

/**
 * GCaaS cloud build (Entra ID via global-caas domain).
 */
const engagementId = 'ddf6b108-ced9-4827-a133-9c82141ebf29';
const releaseName = 'legal-ai-ar-main';
const entraHost = 'https://global-caas-us.pwcglb.com';

export const environment: LegalAiArEnvironment = {
  production: true,
  apiUrl: `${entraHost}/${engagementId}/${releaseName}-backend`,
  platformAuthFailurePath: 'sesion-requerida',
  platformLoginUrl: `${entraHost}/${engagementId}/${releaseName}-frontend/`,
  usePlatformCredentials: true,
  gcaasEngagementId: engagementId,
  platformSessionRefreshPath: `/${engagementId}/refresh`,
  platformLogoutPath: `/${engagementId}/logout`,
  platformSessionRefreshIntervalMs: 45 * 60 * 1000
};
