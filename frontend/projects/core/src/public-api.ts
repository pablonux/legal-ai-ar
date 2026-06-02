/*
 * Public API Surface of core
 */

export type { LegalAiArEnvironment } from './lib/environments/environment.model';
export { environment } from './lib/environments/environment';

export { AuthService } from './lib/services/auth.service';
export type { AuthUser, MeResponse } from './lib/services/auth.service';
export {
  resolvePlatformUrl,
  getGcaasSessionRefreshUrl,
  getGcaasLogoutUrl
} from './lib/services/platform-urls';

export { authGuard } from './lib/guards/auth.guard';
export { platformCredentialsInterceptor } from './lib/interceptors/platform-credentials.interceptor';
export { errorInterceptor } from './lib/interceptors/error.interceptor';
