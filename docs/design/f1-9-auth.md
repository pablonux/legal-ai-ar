# Simulated Authentication — API

| Field | Value |
|---|---|
| **ID** | E090 |
| **Feature** | F1-9 · Autenticación — Simulada (API) |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the simulated authentication flow for Phase 1: mock JWT issuance with `role: admin` claim, API validation, and login/logout endpoints. It serves as the design reference for T-01 to T-05 and is consumed by the API and SPA (F1-13). Entra ID replaces this in Phase 3 (F3-5).

**Reference**: ADR-003 (Authentication), ADR-013 (Phase 1: all users admin); Architecture section 5.

---

## 1. Overview

Phase 1 uses a **simulated** authentication provider. No Azure Entra ID. The API issues its own JWT tokens signed with a symmetric key. Any valid credentials (or a fixed mock user) produce a token with `role: admin`. All authenticated users have full access.

---

## 2. Login Endpoint

### 2.1 POST /api/auth/login

**Route**: `POST /api/auth/login`  
**Authentication**: None (public).

**Request body**:

```json
{
  "email": "admin@legal-ai.local",
  "password": "admin"
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| email | string | Yes | Mock identifier. Phase 1: any non-empty value accepted. |
| password | string | Yes | Mock credential. Phase 1: any non-empty value accepted. |

**Validation**: Both fields non-empty. No credential verification in Phase 1 (accept any pair).

**Response (200 OK)**:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-03-15T16:30:00Z",
  "user": {
    "email": "admin@legal-ai.local",
    "displayName": "Admin (simulado)",
    "role": "admin"
  }
}
```

| Field | Type | Description |
|---|---|---|
| token | string | JWT Bearer token. Client stores and sends in `Authorization: Bearer {token}` |
| expiresAt | datetime | ISO 8601. Token expiration. |
| user | object | User info for UI. |

**Errors**: 400 if email or password empty.

---

## 3. Logout Endpoint

### 3.1 POST /api/auth/logout

**Route**: `POST /api/auth/logout`  
**Authentication**: Optional (Bearer token). Server does not invalidate tokens (stateless JWT).

**Request body**: None.

**Response (200 OK)**:

```json
{
  "success": true
}
```

**Behavior**: Server always returns success. Logout is client-side: discard token. No server-side session or token blacklist in Phase 1.

---

## 4. JWT Structure

### 4.1 Claims

| Claim | Value | Description |
|---|---|---|
| `sub` | email from request | Subject (user identifier) |
| `email` | email from request | User email |
| `role` | `admin` | Phase 1: always admin (ADR-013) |
| `iss` | `LegalAI-Ar-Simulated` | Issuer (configurable) |
| `aud` | `LegalAI-Ar-API` | Audience (configurable) |
| `exp` | Unix timestamp | Expiration (e.g. 24 hours from issuance) |
| `iat` | Unix timestamp | Issued at |

### 4.2 Signing

- **Algorithm**: HS256
- **Key**: Symmetric key from configuration (e.g. `Auth__Simulated__SigningKey`). Min 32 bytes (256 bits) for HS256.
- **Key storage**: Environment variable or `appsettings.Development.json`. Never commit real keys.

### 4.3 Configuration

| Variable | Description | Example |
|---|---|---|
| `Auth__Simulated__SigningKey` | Base64-encoded key for HS256 (min 32 bytes) | *(secret, e.g. 44-char base64)* |
| `Auth__Simulated__Issuer` | JWT `iss` claim | `LegalAI-Ar-Simulated` |
| `Auth__Simulated__Audience` | JWT `aud` claim | `LegalAI-Ar-API` |
| `Auth__Simulated__ExpirationMinutes` | Token lifetime | `1440` (24 hours) |

---

## 5. API Validation

### 5.1 JWT Bearer Configuration

Configure `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)` with:

| Setting | Value |
|---|---|
| TokenValidationParameters.ValidIssuer | Same as `Auth__Simulated__Issuer` |
| TokenValidationParameters.ValidAudience | Same as `Auth__Simulated__Audience` |
| TokenValidationParameters.IssuerSigningKey | Symmetric key from config |
| TokenValidationParameters.ValidateLifetime | true |

### 5.2 Protected Endpoints

| Endpoint | Auth |
|---|---|
| `GET /api/health` | Public |
| `POST /api/auth/login` | Public |
| `POST /api/auth/logout` | Optional (no validation required) |
| All other endpoints | `[Authorize]` required |

### 5.3 Unauthorized Response (401)

When request has no token or invalid token:

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Invalid or missing token.",
  "instance": "/api/rulings/search"
}
```

---

## 6. CORS

Configure CORS for the SPA origin:

| Setting | Value |
|---|---|
| AllowedOrigins | SPA URL (e.g. `https://legal-ai-ar.azurestaticapps.net`, `http://localhost:4200`) |
| AllowedMethods | GET, POST, PUT, PATCH, DELETE, OPTIONS |
| AllowedHeaders | Authorization, Content-Type |
| AllowCredentials | true (if SPA sends cookies; Phase 1 typically uses Bearer only) |

**Development**: Allow `http://localhost:4200` (Angular dev server).

---

## 7. Flow Summary

1. **Login**: SPA sends `POST /api/auth/login` with email/password → API returns JWT + user.
2. **Storage**: SPA stores token (e.g. `localStorage` or memory). SPA sends `Authorization: Bearer {token}` on all API requests.
3. **Validation**: API validates JWT on each request (except health and login). Invalid/missing → 401.
4. **Logout**: SPA calls `POST /api/auth/logout` (optional) and discards token.

---

## 8. References

- `docs/architecture/legal-ai-ar-architecture.md` — ADR-003, ADR-013
- `docs/design/f1-13-auth-sim.md` — SPA login flow (E114)
- `docs/design/f1-9-auth-flow.mermaid` — Sequence diagram (E091)
