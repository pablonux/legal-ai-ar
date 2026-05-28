# F03 - W02 - Backend - Indice AI Search para Normas

> **Feature:** F03 - Busqueda de Normas
> **Release:** 1.0 | **Sprint:** S02-S03
> **Tipo:** backend | **Prioridad:** Alta
> **Estimación:** 5 story points
> **Asignable a:** Dev Fullstack (Backend)

---

## Descripción

Configurar el índice de Azure AI Search para Busqueda de Normas. Incluye definición del schema, scoring profile, suggester y skillset.

---

## Tareas

- [ ] Definir el schema del índice (campos, tipos, atributos)
- [ ] Crear el scoring profile con pesos para BM25 y vectores
- [ ] Configurar el suggester para autocompletado
- [ ] Configurar el skillset para extracción de texto de PDFs (si aplica)
- [ ] Configurar las projections vectoriales
- [ ] Crear el indexer conectado a Azure SQL como data source
- [ ] Implementar el SearchService en C# con Azure.Search.Documents
- [ ] Probar búsquedas con datos de prueba

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
- Referir a la documentación integral (F03-W01) para modelo de datos y endpoints

---

## Archivos a Crear/Modificar

```
src/Infrastructure/Search/{Feature}SearchService.cs
src/Infrastructure/Search/IndexDefinitions/{feature}-index.json
tests/Infrastructure.Tests/Search/{Feature}SearchServiceTests.cs
```

---

## Dependencias

- Depende de: F03-W01 (Documentación integral)

---

*F03 - W02 - Backend - Indice AI Search para Normas — Legal Ai Ar*
