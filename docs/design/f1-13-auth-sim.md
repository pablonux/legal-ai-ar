# Simulated Authentication — SPA

| Field | Value |
|---|---|
| **ID** | E114 |
| **Feature** | F1-13 · Frontend — Autenticación simulada |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the SPA-side simulated login: login form, call to mock login API, token storage, and session handling. It serves as the design reference for T-01 to T-07 and complements the API spec (E090).

**Reference**: E090 (API auth); Architecture section 6; ADR-013.

---

## 1. Login Form

### 1.1 Route

`/login` — LoginComponent. **Public** (no AuthGuard).

### 1.2 Fields

| Field | Type | Required | Validation |
|---|---|---|---|
| email | string | Yes | Non-empty |
| password | string | Yes | Non-empty |

### 1.3 Layout

- **Title**: "Iniciar sesión" (or "Login")
- **Email input**: type="email" or "text", placeholder "Correo electrónico"
- **Password input**: type="password", placeholder "Contraseña"
- **Submit button**: "Iniciar sesión". Disabled while loading.
- **Error message**: Inline below form if API returns 400 or network error.

### 1.4 Phase 1 Credentials

Any non-empty email/password is accepted. Suggested defaults for development: `admin@legal-ai.local` / `admin`. No pre-fill in production; optional placeholder in dev.

---

## 2. AuthService

### 2.1 Methods

| Method | Description |
|---|---|
| `login(email, password)` | `POST /api/auth/login`. Returns `Observable<{ token, expiresAt, user }>`. On success: store token and user; emit logged-in state. |
| `logout()` | Clear token and user from storage. Optionally call `POST /api/auth/logout`. Emit logged-out state. |
| `getToken()` | Return stored token or null. |
| `getUser()` | Return stored user or null. |
| `isAuthenticated()` | Return true if token exists and not expired (optional: check `exp` claim). |

### 2.2 Token Storage

| Option | Pros | Cons |
|---|---|---|
| **localStorage** | Persists across tabs, survives refresh | Accessible to XSS |
| **sessionStorage** | Cleared on tab close | Same XSS risk |
| **Memory only** | No persistence | Lost on refresh |

**Recommendation**: `localStorage` for Phase 1. Token is short-lived (24h). SPA is internal; XSS risk is mitigated by Angular's sanitization. Phase 3 (Entra ID) may use MSAL with different storage.

**Keys**: `legalaiar_token`, `legalaiar_user` (or namespaced).

### 2.3 Expiration Check

- **Optional**: Decode JWT `exp` claim. If expired, treat as unauthenticated (clear storage, redirect to login).
- **Simple**: Rely on API 401. When any request returns 401, clear token and redirect to login.

---

## 3. Login Flow

1. User enters email/password, clicks "Iniciar sesión".
2. LoginComponent calls `AuthService.login(email, password)`.
3. AuthService sends `POST /api/auth/login`.
4. On success: store `token` and `user`; navigate to **return URL** (if any) or `/buscar`.
5. On error (400, 500, network): show error message. Do not navigate.

### 3.1 Return URL

When user is redirected to `/login` because of 401 or AuthGuard:
- Store intended URL in query param: `/login?returnUrl=/admin/crawlers`.
- After successful login: `router.navigateByUrl(returnUrl)` or default `/buscar`.

---

## 4. AuthGuard

### 4.1 Behavior

- **CanActivateFn**: Before activating a protected route, check `AuthService.isAuthenticated()`.
- If **true**: allow navigation.
- If **false**: redirect to `/login?returnUrl={currentUrl}`.

### 4.2 Protected Routes

All routes except `/login` and (optionally) public landing. Apply AuthGuard to: `/buscar`, `/buscar/resultados`, `/fallos/:id`, `/chat`, `/admin`, `/admin/*`.

---

## 5. AuthInterceptor

### 5.1 Request Interceptor

For every outgoing HTTP request (except login):
- If token exists: add header `Authorization: Bearer {token}`.
- Skip for `/api/auth/login` and `/api/health`.

### 5.2 Response Interceptor (Error Handling)

When response is **401 Unauthorized**:
- Clear token and user from storage.
- Navigate to `/login?returnUrl={currentUrl}`.
- Optionally show toast: "Sesión expirada. Inicia sesión de nuevo."

---

## 6. Logout

### 6.1 Trigger

- **Menu**: "Cerrar sesión" link or button in header/sidebar.
- **Action**: Call `AuthService.logout()`. Navigate to `/login`.

### 6.2 Logout Steps

1. Clear `localStorage` (token, user).
2. Optionally: `POST /api/auth/logout` (no-op server-side; for consistency).
3. Navigate to `/login`.

---

## 7. Session Persistence

| Scenario | Behavior |
|---|---|
| **Page refresh** | Token in localStorage → still authenticated. AuthGuard allows. |
| **New tab** | Same origin → token available. Authenticated. |
| **Token expired** | Next API call returns 401 → ErrorInterceptor clears and redirects. |
| **User closes browser** | localStorage persists. Next visit: still logged in until token expires. |

---

## 8. References

- `docs/design/f1-9-auth.md` — API spec (E090)
- `docs/design/f1-9-auth-flow.mermaid` — Sequence diagram (E091)
- `docs/design/f1-13-auth-guard-flow.mermaid` — AuthGuard flow (E115)
