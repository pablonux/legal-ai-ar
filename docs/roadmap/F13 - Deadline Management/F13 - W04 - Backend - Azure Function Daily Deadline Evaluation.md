# F13 - W04 - Backend - Azure Function Evaluacion Diaria de Plazos

> **Feature:** F13 - Gestion de Plazos
> **Release:** 2.0 | **Sprint:** S06-S07
> **Tipo:** backend | **Prioridad:** Alta
> **Estimación:** 3 story points
> **Asignable a:** Dev Fullstack (Backend)

---

## Descripción

Implementar Azure Function (.NET 10 isolated worker) para Backend - Evaluacion Diaria de Plazos.

---

## Tareas

- [ ] Crear la function class en `Functions/`
- [ ] Configurar el trigger (Timer/Queue/Blob según corresponda)
- [ ] Implementar la lógica de negocio
- [ ] Configurar bindings de output (SQL, Queue, etc.)
- [ ] Agregar connection strings en local.settings.json
- [ ] Agregar secretos al Key Vault para producción
- [ ] Escribir tests unitarios
- [ ] Probar localmente con Azure Functions Core Tools

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
- Referir a la documentación integral (F13-W01) para modelo de datos y endpoints

---

## Archivos a Crear/Modificar

```
src/Functions/{NombreFunction}.cs
src/Functions/local.settings.json (modificar)
tests/Functions.Tests/{NombreFunction}Tests.cs
```

---

## Dependencias

- Depende de: F13-W01 (Documentación integral)

---

*F13 - W04 - Backend - Azure Function Evaluacion Diaria de Plazos — Legal Ai Ar*
