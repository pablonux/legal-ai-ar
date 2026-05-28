# FT01 - W03 - Backend - Worker Storage Queue a SignalR

> **Feature:** FT01 - Notificaciones en Tiempo Real
> **Release:** Transversal | **Sprint:** S03-S04
> **Tipo:** backend | **Prioridad:** Alta
> **Estimación:** 5 story points
> **Asignable a:** Dev Fullstack (Backend)

---

## Descripción

Implementar comunicación real-time con SignalR para Notificaciones en Tiempo Real.

---

## Tareas

- [ ] Crear Hub class en `Api/Hubs/`
- [ ] Registrar Hub en Program.cs con `.MapHub<>()`
- [ ] Implementar métodos del Hub
- [ ] Configurar autenticación del Hub con Bearer token
- [ ] Implementar service que publica mensajes al Hub
- [ ] Escribir tests de integración

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
- Referir a la documentación integral (FT01-W01) para modelo de datos y endpoints

---

## Archivos a Crear/Modificar

```
src/Api/Hubs/{Nombre}Hub.cs
src/Api/Program.cs (modificar)
src/Application/Services/{Nombre}NotificationService.cs
```

---

## Dependencias

- Depende de: FT01-W01 (Documentación integral)

---

*FT01 - W03 - Backend - Worker Storage Queue a SignalR — Legal Ai Ar*
