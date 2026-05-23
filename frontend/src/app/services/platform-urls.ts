import { environment } from '../../environments/environment';
import type { LegalAiArEnvironment } from '../../environments/environment.model';

/** Resolves a path or absolute URL against the current origin when needed. */
export function resolvePlatformUrl(pathOrUrl: string): string {
  if (!pathOrUrl) return '';
  if (pathOrUrl.startsWith('http://') || pathOrUrl.startsWith('https://')) {
    return pathOrUrl;
  }
  if (typeof window === 'undefined') return pathOrUrl;
  const path = pathOrUrl.startsWith('/') ? pathOrUrl : `/${pathOrUrl}`;
  return `${window.location.origin}${path}`;
}

export function getGcaasSessionRefreshUrl(env: LegalAiArEnvironment = environment): string | null {
  if (!env.usePlatformCredentials) return null;
  const engagementId = env.gcaasEngagementId;
  if (!engagementId) return null;
  const path = env.platformSessionRefreshPath ?? `/${engagementId}/refresh`;
  return resolvePlatformUrl(path);
}

export function getGcaasLogoutUrl(env: LegalAiArEnvironment = environment): string | null {
  if (!env.usePlatformCredentials) return null;
  const engagementId = env.gcaasEngagementId;
  if (!engagementId) return null;
  const path = env.platformLogoutPath ?? `/${engagementId}/logout`;
  return resolvePlatformUrl(path);
}
