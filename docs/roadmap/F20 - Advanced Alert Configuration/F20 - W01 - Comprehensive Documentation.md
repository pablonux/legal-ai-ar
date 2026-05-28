# F20 - W01 - Documentacion Integral

> **Feature:** F20 - Configuracion de Alertas Avanzadas
> **Release:** 4.0 | **Sprint:** S10
> **Tipo:** Documentación | **Prioridad:** Crítica (bloqueante)
> **Estimación:** 3 story points

---

## 1. Descripción General

Alertas personalizadas: cambios normativos, vencimientos, nuevos fallos, cambios de estado. Canales: push in-app, email.

---

## 2. Diagrama de Arquitectura

```mermaid
graph TB
    subgraph "Configuracion de Alertas Avanzadas"
        FE[Frontend Angular 19]
        BE[Backend .NET 10]
        DB[(Azure SQL + Graph)]
        AIS[Azure AI Search]
    end
    FE -->|HTTP/SignalR| BE
    BE --> DB
    BE --> AIS
```

---

## 3. Modelo de Datos

> Definir modelo de datos específico durante la implementación del W01.
> Referir a la ontología en `docs/ontologia/ontologia_legal_argentina.md` para las clases base.

---

## 4. API Endpoints

> Los endpoints específicos se definirán en base al documento de features: `docs/roadmap/features.md`, sección API Endpoints.

---

## 5. Descripción de UI / UX

> Definir mockups de UI durante la implementación. Seguir las guidelines de Angular Material 19 + Tailwind CSS 4.
> Referir a `docs/roadmap/features.md` para la descripción funcional de la UI.

---

## 6. Criterios de Aceptación

- [ ] La funcionalidad descrita en la sección de Descripción está completamente implementada
- [ ] Los endpoints de API retornan los datos esperados
- [ ] La UI es responsive y funcional en desktop y tablet
- [ ] Los tests unitarios cubren > 80% del código nuevo
- [ ] El build de CI pasa sin errores
- [ ] La funcionalidad es accesible (WCAG 2.1 AA)

---

## 7. Dependencias

- **Depende de:** F01 (Auth)
- **Referir a features.md** para dependencias detalladas entre features

---

## 8. Notas Técnicas

- Stack: Angular 19 (standalone components, signals) + .NET 10 Minimal API
- Base de datos: Azure SQL con EF Core 10 + Graph Tables
- Búsqueda: Azure AI Search con scoring híbrido
- Auth: Microsoft Entra ID con MSAL Angular + Microsoft.Identity.Web
- Comunicación real-time: SignalR
- Storage: Azure Blob Storage para documentos
- Referir a la ontología (`docs/ontologia/ontologia_legal_argentina.md`) para el modelo de dominio

---

## 9. Work Items de esta Feature

| ID | Nombre | Tipo | Sprint |
|----|--------|------|--------|
| F20-W01 | Documentacion Integral | doc | S10 |
| F20-W02 | Backend - CRUD Configuracion de Alertas | backend | S10 |
| F20-W03 | Backend - Azure Function Evaluacion de Condiciones | backend | S10 |
| F20-W04 | Backend - Envio de Email con SendGrid o SMTP | backend | S10 |
| F20-W05 | Frontend - Wizard Configuracion de Alerta | frontend | S10 |
| F20-W06 | Frontend - Centro de Alertas Inbox | frontend | S10 |
| F20-W07 | Testing - Tests de Alertas | testing | S10 |

---

## 10. Definition of Done

- [ ] Código revisado por al menos 1 peer (PR aprobado)
- [ ] Tests unitarios con cobertura > 80%
- [ ] Tests de integración para endpoints
- [ ] Sin errores en build de CI
- [ ] Documentación de API actualizada (Swagger/OpenAPI)
- [ ] Componentes Angular documentados con JSDoc
- [ ] Accesibilidad validada (WCAG 2.1 AA)
- [ ] Responsive verificado en desktop y tablet
- [ ] Performance: tiempo de carga < 3 seg, API response < 2 seg
- [ ] Feature flag configurado (si aplica)

---

*F20 - Configuracion de Alertas Avanzadas — Documentación integral — Legal Ai Ar*
