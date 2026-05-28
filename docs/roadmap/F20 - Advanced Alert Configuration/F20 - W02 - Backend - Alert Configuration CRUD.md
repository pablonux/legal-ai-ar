# F20 - W02 - Backend - CRUD Configuracion de Alertas

> **Feature:** F20 - Configuracion de Alertas Avanzadas
> **Release:** 4.0 | **Sprint:** S10
> **Tipo:** backend | **Prioridad:** Alta
> **Estimación:** 5 story points
> **Asignable a:** Dev Fullstack (Backend)

---

## Descripción

Implementar los endpoints de API para Configuracion de Alertas Avanzadas usando ASP.NET Core 10 Minimal API.

---

## Tareas

- [ ] Crear DTOs de request y response en `Application/DTOs/`
- [ ] Crear validadores con FluentValidation en `Application/Validators/`
- [ ] Implementar service/handler en `Application/Services/`
- [ ] Registrar endpoints en `Api/Endpoints/{Feature}Endpoints.cs`
- [ ] Agregar autorización por rol donde corresponda
- [ ] Documentar endpoints con `.WithOpenApi()`
- [ ] Escribir tests unitarios del service
- [ ] Escribir tests de integración de los endpoints

---

## Criterios de Aceptación

- [ ] La funcionalidad implementada cumple con los criterios de aceptación del W01
- [ ] Los tests pasan
- [ ] El código está revisado por al menos 1 peer

---

## Notas Técnicas

- Framework: .NET 10 LTS con ASP.NET Core Minimal API
- ORM: Entity Framework Core 10
- Validación: FluentValidation 12.x
- Logging: Serilog con sink a Application Insights
- Referir a la documentación integral (F20-W01) para modelo de datos y endpoints

---

## Archivos a Crear/Modificar

```
src/Application/DTOs/{Feature}Dto.cs
src/Application/Validators/{Feature}Validator.cs
src/Application/Services/{Feature}Service.cs
src/Api/Endpoints/{Feature}Endpoints.cs
tests/Application.Tests/{Feature}ServiceTests.cs
tests/Api.Tests/{Feature}EndpointTests.cs
```

---

## Dependencias

- Depende de: F20-W01 (Documentación integral)

---

*F20 - W02 - Backend - CRUD Configuracion de Alertas — Legal Ai Ar*
