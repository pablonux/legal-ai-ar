# F20 - W03 - Backend - Azure Function Evaluacion de Condiciones

> **Feature:** F20 - Configuracion de Alertas Avanzadas
> **Release:** 4.0 | **Sprint:** S10
> **Tipo:** backend | **Prioridad:** Alta
> **Estimación:** 5 story points
> **Asignable a:** Dev Fullstack (Backend)

---

## Descripción

Implementar Azure Function (.NET 10 isolated worker) para Backend - Evaluacion de Condiciones.

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
- Referir a la documentación integral (F20-W01) para modelo de datos y endpoints

---

## Archivos a Crear/Modificar

```
src/Functions/{NombreFunction}.cs
src/Functions/local.settings.json (modificar)
tests/Functions.Tests/{NombreFunction}Tests.cs
```

---

## Dependencias

- Depende de: F20-W01 (Documentación integral)

---

*F20 - W03 - Backend - Azure Function Evaluacion de Condiciones — Legal Ai Ar*
