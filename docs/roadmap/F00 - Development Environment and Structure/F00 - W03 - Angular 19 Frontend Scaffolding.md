# F00 - W03 - Scaffolding Frontend Angular 19

> **Feature:** F00 - Entorno y Estructura de Desarrollo
> **Release:** 0.0 | **Sprint:** S00
> **Tipo:** frontend | **Prioridad:** Crítica (bloqueante)
> **Estimación:** 5 story points
> **Asignable a:** Dev Frontend

---

## Descripción

Inicializar el proyecto Angular 19 dentro del monorepo con la estructura de carpetas definida, configurar PwC AppKit 4 (o placeholder hasta tener la librería), y dejar el proyecto listo para desarrollo de features.

---

## Tareas

- [ ] Generar proyecto Angular 19 con `ng new` (standalone, SCSS, routing)
- [ ] Mover el proyecto generado a la carpeta `frontend/`
- [ ] Configurar estructura de carpetas: `core/`, `shared/`, `features/`, `layout/`
- [ ] Configurar `angular.json` con build configurations por ambiente (dev, qa, staging, prod)
- [ ] Crear archivos `environment.ts` para cada ambiente
- [ ] Instalar y configurar PwC AppKit 4 (o placeholder con Angular Material temporalmente)
- [ ] Configurar SCSS base: `_variables.scss`, `_mixins.scss`, `_typography.scss`
- [ ] Crear componente `AppComponent` con router-outlet
- [ ] Crear componente `LayoutComponent` (shell con sidebar placeholder y navbar)
- [ ] Configurar `app.routes.ts` con lazy loading para features placeholder
- [ ] Configurar proxy para API backend en `proxy.conf.json`
- [ ] Configurar Jest como test runner (reemplazar Karma)
- [ ] Instalar y configurar ESLint + angular-eslint
- [ ] Instalar y configurar Prettier
- [ ] Agregar script npm: `start`, `build`, `test`, `lint`, `e2e`
- [ ] Verificar que `ng build` compila sin errores
- [ ] Verificar que `ng test` ejecuta tests placeholder
- [ ] Verificar que `ng serve` levanta la app en `localhost:4200`

---

## Configuración de Proxy (desarrollo local)

```json
// proxy.conf.json
{
  "/api": {
    "target": "https://localhost:5001",
    "secure": false,
    "changeOrigin": true
  }
}
```

---

## Scripts npm

```json
{
  "start": "ng serve --proxy-config proxy.conf.json",
  "build": "ng build",
  "build:dev": "ng build --configuration=dev",
  "build:qa": "ng build --configuration=qa",
  "build:staging": "ng build --configuration=staging",
  "build:prod": "ng build --configuration=production",
  "test": "jest",
  "test:watch": "jest --watch",
  "test:coverage": "jest --coverage",
  "lint": "ng lint",
  "lint:fix": "ng lint --fix",
  "e2e": "npx playwright test",
  "format": "prettier --write \"src/**/*.{ts,html,scss}\""
}
```

---

## Paquetes npm Base

```json
{
  "dependencies": {
    "@angular/core": "^19.0.0",
    "@azure/msal-angular": "^4.0.0",
    "@azure/msal-browser": "^4.0.0",
    "chart.js": "^4.x",
    "ng2-charts": "^6.x",
    "cytoscape": "^3.x",
    "@fullcalendar/angular": "^6.x",
    "@fullcalendar/core": "^6.x",
    "@fullcalendar/daygrid": "^6.x",
    "@fullcalendar/timegrid": "^6.x",
    "marked": "^12.x"
  },
  "devDependencies": {
    "jest": "^29.x",
    "@angular-builders/jest": "^19.x",
    "@testing-library/angular": "^17.x",
    "jest-preset-angular": "^14.x",
    "playwright": "^1.x",
    "@playwright/test": "^1.x",
    "eslint": "^9.x",
    "angular-eslint": "^19.x",
    "prettier": "^3.x"
  }
}
```

> **Nota:** PwC AppKit 4 se agregará cuando se proporcione la documentación y acceso al paquete. Hasta entonces, usar Angular Material como placeholder para validar estructura.

---

## Layout Shell Inicial

```
┌──────────────────────────────────────────────┐
│  Navbar [Logo] [Título] [Usuario] [Logout]   │
├──────────┬───────────────────────────────────┤
│          │                                   │
│ Sidebar  │       <router-outlet>             │
│          │                                   │
│ - Home   │       (Feature content)           │
│ - Normas │                                   │
│ - Jurisp.│                                   │
│ - Exped. │                                   │
│ - Chat   │                                   │
│ - Config │                                   │
│          │                                   │
└──────────┴───────────────────────────────────┘
```

---

## Criterios de Aceptación

- [ ] `ng build --configuration=production` compila sin errores ni warnings
- [ ] `ng serve` levanta la app y muestra el layout shell con sidebar
- [ ] `jest` ejecuta al menos 1 test placeholder exitosamente
- [ ] `ng lint` no reporta errores
- [ ] Las rutas lazy-loaded funcionan (al menos 1 feature placeholder)
- [ ] El proxy redirige correctamente a la API backend
- [ ] Los 5 archivos de environment están configurados

---

## Dependencias

- **Bloquea:** F00-W04 (CI frontend), F01-W04 (MSAL setup)
- **Depende de:** F00-W02 (repo ya creado en GitHub)

---

*F00 - W03 - Scaffolding Frontend Angular 19 — Legal Ai Ar*
