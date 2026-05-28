# F17 - W07 - Testing - Tests de Generacion de Informes

> **Feature:** F17 - Generacion de Informes Legales
> **Release:** 3.0 | **Sprint:** S08-S09
> **Tipo:** testing | **Prioridad:** Alta
> **Estimación:** 3 story points
> **Asignable a:** Dev Fullstack (QA)

---

## Descripción

Escribir tests unitarios, de integración y E2E para la feature Generacion de Informes Legales.

---

## Tareas

- [ ] Tests unitarios de servicios backend (xUnit + Moq)
- [ ] Tests de integración de API endpoints (WebApplicationFactory)
- [ ] Tests unitarios de componentes Angular (Jest)
- [ ] Tests E2E del flujo completo (Cypress)
- [ ] Verificar cobertura > 80%
- [ ] Verificar que todos los tests pasan en CI

---

## Criterios de Aceptación

- [ ] Cobertura de tests > 80% en código nuevo
- [ ] Todos los tests pasan en CI
- [ ] Los tests E2E cubren el happy path completo de la feature
- [ ] Los tests de integración cubren todos los endpoints de la feature

---

## Notas Técnicas

- Backend: xUnit + Moq + WebApplicationFactory para integration tests
- Frontend: Jest para unit tests + Cypress para E2E
- CI: Los tests deben correr en el pipeline de GitHub Actions / Azure DevOps

---

## Archivos a Crear/Modificar

```
tests/Application.Tests/{Feature}/
tests/Api.Tests/{Feature}/
src/app/features/{feature}/**/*.spec.ts
cypress/e2e/{feature}.cy.ts
```

---

## Dependencias

- Depende de: Todos los work items de la feature F17 completados

---

*F17 - W07 - Testing - Tests de Generacion de Informes — Legal Ai Ar*
