# FT05 - W04 - GCaaS - Platform Authentication id_token Cookie

> **Feature:** FT05 - Delivery and Hosting
> **Release:** Cross-cutting | **Sprint:** S01
> **Type:** backend / frontend | **Priority:** Critical (blocking)
> **Estimate:** 5 story points
> **Assignable to:** Backend Dev + Frontend Dev

---

## Description

Production authentication is **platform-managed**: GCaaS Envoy completes Microsoft Entra SSO and issues an **`id_token` HTTP-only cookie**. The API authorizes only requests carrying a valid `id_token`; it does NOT accept `X-User-*` headers, `access_token` for authorization, app-owned login endpoints, or Bearer tokens in `localStorage`. There is no MSAL in the SPA. This work item formalizes that model (already implemented in the MVP) and its app-level role mapping. It reconciles feature **F01 — Authentication and Authorization**.

---

## Tasks

### Backend (`Auth:Platform`)

- [ ] `PlatformGatewayTokenResolver` reads the `id_token` cookie
- [ ] `PlatformAuthenticationHandler` validates the JWT against Entra OIDC metadata (issuer/audience from Vault `TenantId` + `ValidAudience`)
- [ ] `PlatformJwtPrincipalNormalizer` + `PlatformRoleResolver` map claims (`email`, `roles`) → app role (Lawyer / Administrative; fallback `DefaultRole`)
- [ ] `GET /api/auth/me` returns identity from the validated principal; `POST /api/auth/logout` acks (platform logout is an SPA redirect)
- [ ] Local dev: `DevelopmentPlatformAuthMiddleware` + `DevelopmentSessionTokenIssuer` inject a signed `id_token` when `Auth:Development:InjectIdentity=true`

### Frontend (no MSAL)

- [ ] `APP_INITIALIZER` → `auth.service.bootstrapSession()` → `GET /api/auth/me`
- [ ] `platformCredentialsInterceptor` adds `withCredentials: true` when `usePlatformCredentials` is true
- [ ] `startGcaasSessionRefresh()` — periodic `GET /{engagementId}/refresh` (~45 min)
- [ ] `error.interceptor` 401 → navigate to `sesion-requerida` gate (SSO link to `platformLoginUrl`)
- [ ] No `/login` route, no Bearer token storage

---

## API configuration (`Auth:Platform`)

| Setting | Env var | Description |
|---------|---------|-------------|
| `TenantId` | `Auth__Platform__TenantId` | Entra tenant → OIDC metadata |
| `ValidAudience` | `Auth__Platform__ValidAudience` | Expected `aud` in `id_token` |
| `IdTokenCookie` | `Auth__Platform__IdTokenCookie` | Cookie name (default `id_token`) |
| `DefaultRole` | `Auth__Platform__DefaultRole` | Fallback role if JWT has none |

---

## Acceptance Criteria

- [ ] `GET /api/auth/me` returns 200 with a valid cookie and 401 without
- [ ] Roles correctly mapped from claims to Lawyer / Administrative
- [ ] No MSAL, `/login`, or Bearer token paths exist in the SPA
- [ ] Session refresh runs and a refresh 401 routes to `sesion-requerida`
- [ ] Local dev can toggle `InjectIdentity` to test 200/401

---

## Dependencies

- **Blocks:** F01 (reconciled to this model), all authorized endpoints
- **Prerequisites:** FT05-W05 (Vault `TenantId`/`ValidAudience`), FT05-W03 (Entra VirtualService)

---

*FT05 - W04 - GCaaS - Platform Authentication id_token Cookie — Legal Ai Ar*
