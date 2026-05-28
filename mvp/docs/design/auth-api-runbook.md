# API authentication and health probes

Identity is always a **validated `id_token` cookie**. No `Auth:Mode`, no login endpoint, no `X-User-*` headers.

## How it works

| Environment | Identity source |
|-------------|-----------------|
| **GCaaS / production** | Cookie `id_token` validated against Entra (`TenantId` + `ValidAudience` in Vault) |
| **Local Development** | `DevelopmentPlatformAuthMiddleware` injects a signed `id_token` test cookie when `Auth:Development:InjectIdentity` is true |

## Configuration

### Platform (`Auth:Platform`)

| Setting | Description |
|---------|-------------|
| `TenantId` | Entra tenant GUID → OIDC metadata URL |
| `ValidAudience` | App Registration client id (`aud` from `id_token`) |
| `IdTokenCookie` | Cookie name (default `id_token`) |
| `MetadataAddress` | Optional explicit OIDC discovery URL |
| `SigningKeyBase64` | Local dev / tests only |
| `DefaultRole` | Fallback when JWT has no app role (`admin`) |

### Development (`Auth:Development`)

| Setting | Default | Description |
|---------|---------|-------------|
| `InjectIdentity` | `true` | Inject signed `id_token` cookie when session JWT is missing |
| `Email` | `dev@legal-ai.local` | Email claim in dev token |
| `DisplayName` | `Dev User (local)` | Name claim in dev token |
| `Role` | `admin` | Role claim in dev token |

Set `Auth:Development:InjectIdentity=false` to test unauthorized responses locally.

Environment variables use `__` (e.g. `Auth__Development__Email`).

## Endpoints

| Endpoint | Auth |
|----------|------|
| `GET /api/auth/me` | Authenticated (valid `id_token`) |
| `POST /api/auth/logout` | Authenticated |
| `GET /api/health/live` | Anonymous |
| `GET /api/health` | Authenticated |

## Worker SignalR hub (`/hubs/worker-control`)

- Policy **`WorkerControlHub`**: authenticated user **or** header `X-Worker-Hub-Key` matching `WorkerControl:HubAccessKey`.

## Angular SPA

- **`APP_INITIALIZER`**: `AuthService.bootstrapSession()` → `GET /api/auth/me`.
- **`usePlatformCredentials`**: `true` on GCaaS/cloud builds (`withCredentials` for cookies).
- **`sesion-requerida`**: branded gate with SSO button → `platformLoginUrl`.
- **Session refresh**: every 45 min, `GET /{engagementId}/refresh` on the GCaaS host (cookies). Configured via `platformSessionRefreshPath` in [`environment.development.ts`](../../frontend/src/environments/environment.development.ts).
- **Logout**: redirects to `/{engagementId}/logout` on GCaaS when `usePlatformCredentials` is true.
- No `/login` route; no Bearer token in localStorage.

### Environment fields (GCaaS)

| Field | Example |
|-------|---------|
| `gcaasEngagementId` | `ddf6b108-ced9-4827-a133-9c82141ebf29` |
| `platformSessionRefreshPath` | `/ddf6b108-.../refresh` |
| `platformLogoutPath` | `/ddf6b108-.../logout` |
| `platformSessionRefreshIntervalMs` | `2700000` (45 min) |

## Tests

`LegalAiAr.Api.Tests` covers anonymous `health/live`, unauthorized requests without `id_token` cookie, and development cookie injection.

## GCaaS Entra ID deployment (Helm)

Configured in [`deployment/values.yaml`](../../deployment/values.yaml):

| Value | Purpose |
|-------|---------|
| `authentication.entra: true` | Enables GCaaS Entra provisioning (App Registration, EnvoyFilter, etc.) |
| `metadata.entraHostName` | Entra ingress host (e.g. `global-caas-us.pwcglb.com`), set at deploy time |
| `metadata.hostName` | Legacy PWC Identity host (e.g. `hosted-apps-us.pwclabs.pwcglb.com`) |

[`deployment/templates/ksvc.yaml`](../../deployment/templates/ksvc.yaml) adds a second `VirtualService` (`*-vs-entra`) with gateway `istio-system/entra-ingress-gateway` when `authentication.entra` is true.

### Post-deploy verification checklist

1. Open Entra URL: `https://{entraHostName}/{engagementId}/{release}-frontend/`
2. Complete Microsoft Entra login (stage may require `@testenv.pwc.com`)
3. Confirm browser cookies: `id_token`, `access_token`, `refresh_token`
4. Confirm `GET .../api/auth/me` returns 200 (Network tab)
5. Confirm legacy URL on `hosted-apps-*` still works until PWC Identity decommission
6. If 503 on Entra domain: verify `*-vs-entra` VirtualService exists in the namespace

### Session refresh / logout (GCaaS)

- Refresh: `GET /{engagementId}/refresh` (SPA must call periodically; token lifetime 1 h)
- Logout: `GET /{engagementId}/logout` on the `global-caas-*` host

## Session JWT (GCaaS)

See [`auth-jwt-hardening.md`](auth-jwt-hardening.md) for `TenantId` / `ValidAudience` from a decoded browser `id_token`.
