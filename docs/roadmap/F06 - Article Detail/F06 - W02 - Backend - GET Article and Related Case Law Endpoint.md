# F06 - W02 - Backend - Endpoint GET Articulo y Jurisprudencia Relacionada

> **Feature:** F06 - Detalle de Articulo
> **Release:** 1.0 | **Sprint:** S04
> **Tipo:** backend | **Prioridad:** Alta
> **Estimación:** 5 story points
> **Asignable a:** Dev Fullstack (Backend)

---

## Descripción

Implementar los endpoints de API para Detalle de Articulo usando ASP.NET Core 10 Minimal API.

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
- Referir a la documentación integral (F06-W01) para modelo de datos y endpoints

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

- Depende de: F06-W01 (Documentación integral)

---

*F06 - W02 - Backend - Endpoint GET Articulo y Jurisprudencia Relacionada — Legal Ai Ar*
