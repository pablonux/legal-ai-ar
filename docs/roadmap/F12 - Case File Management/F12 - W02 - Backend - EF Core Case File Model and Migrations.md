# F12 - W02 - Backend - Modelo EF Core Expediente y Migraciones

> **Feature:** F12 - Gestion de Expedientes
> **Release:** 2.0 | **Sprint:** S05-S06
> **Tipo:** backend | **Prioridad:** Alta
> **Estimación:** 5 story points
> **Asignable a:** Dev Fullstack (Backend)

---

## Descripción

Crear el modelo de datos con Entity Framework Core 10 para la feature Gestion de Expedientes. Incluye entidades, configuración Fluent API, y migration inicial.

---

## Tareas

- [ ] Crear entidades C# en `Domain/Entities/`
- [ ] Crear configuración Fluent API en `Infrastructure/Persistence/Configurations/`
- [ ] Agregar DbSet al `ApplicationDbContext`
- [ ] Crear migration con `dotnet ef migrations add {Nombre}`
- [ ] Verificar que la migration genera el SQL esperado
- [ ] Ejecutar migration en la base de desarrollo
- [ ] Crear seed data si corresponde

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
- Referir a la documentación integral (F12-W01) para modelo de datos y endpoints

---

## Archivos a Crear/Modificar

```
src/Domain/Entities/{Entidad}.cs
src/Infrastructure/Persistence/Configurations/{Entidad}Configuration.cs
src/Infrastructure/Persistence/ApplicationDbContext.cs (modificar)
src/Infrastructure/Persistence/Migrations/{timestamp}_{Nombre}.cs
```

---

## Dependencias

- Depende de: F12-W01 (Documentación integral)

---

*F12 - W02 - Backend - Modelo EF Core Expediente y Migraciones — Legal Ai Ar*
