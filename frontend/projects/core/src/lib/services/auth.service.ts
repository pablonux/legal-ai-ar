import { Injectable, inject, OnDestroy, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, of, catchError, finalize, map, timeout } from 'rxjs';
import { environment } from '../environments/environment';
import { getGcaasLogoutUrl, getGcaasSessionRefreshUrl } from './platform-urls';

const AUTH_BASE = `${environment.apiUrl}/api/auth`;
const USER_KEY = 'legalaiar_user';
/** Avoid indefinite APP_INITIALIZER wait when the API is cold or unreachable in cloud. */
const BOOTSTRAP_TIMEOUT_MS = 20_000;
const DEFAULT_REFRESH_INTERVAL_MS = 45 * 60 * 1000;

@Injectable({ providedIn: 'root' })
export class AuthService implements OnDestroy {
  private http = inject(HttpClient);
  private router = inject(Router);

  private _user = signal<AuthUser | null>(this.loadUser());
  readonly user = this._user.asReadonly();

  private _sessionReady = false;
  private _bootstrapComplete = signal(false);
  private refreshTimerId: ReturnType<typeof setInterval> | null = null;

  readonly bootstrapComplete = this._bootstrapComplete.asReadonly();

  get usePlatformCredentials(): boolean {
    return environment.usePlatformCredentials;
  }

  /**
   * Runs once at startup: loads identity via GET /api/auth/me (platform headers or dev injection).
   */
  bootstrapSession(): Observable<void> {
    return this.http.get<MeResponse>(`${AUTH_BASE}/me`).pipe(
      timeout(BOOTSTRAP_TIMEOUT_MS),
      tap((res) => {
        this.applyMeResponse(res);
        this._sessionReady = true;
        this.startGcaasSessionRefresh();
      }),
      catchError(() => {
        this.clearLocalSession();
        return of(null);
      }),
      finalize(() => this._bootstrapComplete.set(true)),
      map(() => void 0)
    );
  }

  logout(): void {
    const gcaasLogout = getGcaasLogoutUrl();
    if (gcaasLogout) {
      this.stopGcaasSessionRefresh();
      this.clearLocalSession();
      window.location.href = gcaasLogout;
      return;
    }

    this.http
      .post(`${AUTH_BASE}/logout`, {}, { observe: 'response' })
      .pipe(catchError(() => of(null)))
      .subscribe();

    this.clearLocalSession();
  }

  clearSessionAfterUnauthorized(): void {
    this.clearLocalSession();
  }

  getUser(): AuthUser | null {
    return this._user();
  }

  isAuthenticated(): boolean {
    return this._bootstrapComplete() && this._sessionReady;
  }

  ngOnDestroy(): void {
    this.stopGcaasSessionRefresh();
  }

  /**
   * GCaaS does not auto-refresh sessions; call /{engagementId}/refresh before the 1h id_token expires.
   */
  private startGcaasSessionRefresh(): void {
    this.stopGcaasSessionRefresh();
    const refreshUrl = getGcaasSessionRefreshUrl();
    if (!refreshUrl) return;

    const intervalMs =
      environment.platformSessionRefreshIntervalMs ?? DEFAULT_REFRESH_INTERVAL_MS;

    const runRefresh = () => {
      void fetch(refreshUrl, { method: 'GET', credentials: 'include' }).then((res) => {
        if (res.ok) return;
        if (res.status === 401) {
          this.clearLocalSession();
          const path = environment.platformAuthFailurePath ?? 'sesion-requerida';
          void this.router.navigateByUrl(`/${path}`);
        }
      });
    };

    this.refreshTimerId = setInterval(runRefresh, intervalMs);
  }

  private stopGcaasSessionRefresh(): void {
    if (this.refreshTimerId !== null) {
      clearInterval(this.refreshTimerId);
      this.refreshTimerId = null;
    }
  }

  private applyMeResponse(res: MeResponse): void {
    const user: AuthUser = {
      email: res.email,
      displayName: res.displayName,
      role: res.role
    };
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    this._user.set(user);
  }

  private clearLocalSession(): void {
    this.stopGcaasSessionRefresh();
    localStorage.removeItem(USER_KEY);
    this._user.set(null);
    this._sessionReady = false;
  }

  private loadUser(): AuthUser | null {
    try {
      const raw = localStorage.getItem(USER_KEY);
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;
    }
  }
}

export interface AuthUser {
  email: string;
  displayName?: string;
  role: string;
}

export interface MeResponse {
  email: string;
  displayName: string;
  role: string;
  groups: string;
}
