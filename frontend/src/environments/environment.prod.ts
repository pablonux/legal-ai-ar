import type { LegalAiArEnvironment } from './environment.model';

const engagementId = 'ddf6b108-ced9-4827-a133-9c82141ebf29';

/**
 * Production: same origin as SPA on GCaaS (API at sibling path under engagement).
 */
export const environment: LegalAiArEnvironment = {
  production: true,
  apiUrl: '',
  platformAuthFailurePath: 'sesion-requerida',
  platformLoginUrl: '',
  usePlatformCredentials: true,
  gcaasEngagementId: engagementId,
  platformSessionRefreshPath: `/${engagementId}/refresh`,
  platformLogoutPath: `/${engagementId}/logout`,
  platformSessionRefreshIntervalMs: 45 * 60 * 1000
};
