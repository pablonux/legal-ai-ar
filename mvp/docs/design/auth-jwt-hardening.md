# Sesión GCaaS (`id_token` cookie)

## Regla

Solo entra quien trae cookie **`id_token`** válida. Nada más:

- No `X-User-Email`
- No `X-User-Jwt`
- No `access_token`

## Configuración (Vault)

```bash
Auth__Platform__TenantId=<tid del id_token>
Auth__Platform__ValidAudience=<aud del id_token>
```

## Local

Middleware de desarrollo inyecta cookie `id_token` firmada (`SigningKeyBase64` en `appsettings.Development.json`).

## Errores

| Mensaje | Causa |
|---------|--------|
| Missing id_token session cookie | Sin cookie en el pod |
| Invalid or expired id_token | Token vencido o TenantId/ValidAudience incorrectos |
