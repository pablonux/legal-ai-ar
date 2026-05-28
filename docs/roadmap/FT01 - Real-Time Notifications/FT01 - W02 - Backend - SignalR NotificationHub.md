# FT01 - W02 - Backend - SignalR NotificationHub

> **Feature:** FT01 - Real-Time Notifications
> **Release:** Transversal | **Sprint:** S03-S04
> **Type:** backend | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Fullstack Dev (Backend)

---

## Description

Implement real-time communication with SignalR for Real-Time Notifications.

---

## Tasks

- [ ] Create the Hub class in `Api/Hubs/`
- [ ] Registrar Hub en Program.cs con `.MapHub<>()`
- [ ] Implement the Hub methods
- [ ] Configure Hub authentication with a Bearer token
- [ ] Implement a service that publishes messages to the Hub
- [ ] Write integration tests

---

## Acceptance Criteria

- [ ] The implemented functionality meets the W01 acceptance criteria
- [ ] Tests pass
- [ ] The code is reviewed by at least 1 peer

---

## Technical Notes

- Framework: .NET 10 LTS with ASP.NET Core Minimal API
- ORM: Entity Framework Core 10
- Validation: FluentValidation 12.x
- Logging: Serilog with an Application Insights sink
- Refer to the comprehensive documentation (FT01-W01) for the data model and endpoints

---

## Files to Create/Modify

```
src/Api/Hubs/{Nombre}Hub.cs
src/Api/Program.cs (modificar)
src/Application/Services/{Nombre}NotificationService.cs
```

---

## Dependencies

- Depends on: FT01-W01 (Comprehensive Documentation)

---

*FT01 - W02 - Backend - SignalR NotificationHub — Legal Ai Ar*
