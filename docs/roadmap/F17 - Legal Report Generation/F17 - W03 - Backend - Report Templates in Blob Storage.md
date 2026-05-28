# F17 - W03 - Backend - Templates de Informes en Blob Storage

> **Feature:** F17 - Generacion de Informes Legales
> **Release:** 3.0 | **Sprint:** S08-S09
> **Tipo:** backend | **Prioridad:** Alta
> **Estimación:** 5 story points
> **Asignable a:** Dev Fullstack (Backend)

---

## Descripción

Implementar upload/download de documentos a Azure Blob Storage para Generacion de Informes Legales.

---

## Tareas

- [ ] Crear BlobStorageService en `Infrastructure/Storage/`
- [ ] Implementar upload con nombre: `{container}/{id}/{guid}_{filename}`
- [ ] Implementar download con SAS token de corta duración
- [ ] Configurar container y access policy
- [ ] Agregar connection string al Key Vault
- [ ] Escribir tests con Azurite (emulador local)

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
- Referir a la documentación integral (F17-W01) para modelo de datos y endpoints

---

## Archivos a Crear/Modificar

```
src/Infrastructure/Storage/BlobStorageService.cs
src/Infrastructure/Storage/IBlobStorageService.cs
tests/Infrastructure.Tests/Storage/BlobStorageServiceTests.cs
```

---

## Dependencias

- Depende de: F17-W01 (Documentación integral)

---

*F17 - W03 - Backend - Templates de Informes en Blob Storage — Legal Ai Ar*
