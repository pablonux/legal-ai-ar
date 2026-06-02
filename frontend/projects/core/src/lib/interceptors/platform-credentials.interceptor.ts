import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../environments/environment';

/** Sends cookies on API calls when targeting a corporate / GCaaS host. */
export const platformCredentialsInterceptor: HttpInterceptorFn = (req, next) => {
  if (!environment.usePlatformCredentials) {
    return next(req);
  }
  return next(req.clone({ withCredentials: true }));
};
