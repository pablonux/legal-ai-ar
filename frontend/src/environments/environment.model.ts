/**
 * Shared environment shape for Legal AI AR builds.
 */
export interface LegalAiArEnvironment {
  production: boolean;
  apiUrl: string;
  platformAuthFailurePath: string;
  /** Full URL or path to re-enter SSO (GCaaS frontend entry). */
  platformLoginUrl: string;
  /** Send cookies on API calls (GCaaS / corporate host). */
  usePlatformCredentials: boolean;
  /** GCaaS engagement UUID; required for session refresh/logout paths. */
  gcaasEngagementId?: string;
  /** e.g. /{engagementId}/refresh — resolved against window origin when relative. */
  platformSessionRefreshPath?: string;
  /** e.g. /{engagementId}/logout */
  platformLogoutPath?: string;
  /** Interval in ms between GCaaS session refresh calls (default 45 min). */
  platformSessionRefreshIntervalMs?: number;
}
