# F15 - W01 - Documentacion Integral

> **Feature:** F15 - Analisis de Riesgo Legal
> **Release:** 3.0 | **Sprint:** S08-S09
> **Tipo:** Documentación | **Prioridad:** Crítica (bloqueante)
> **Estimación:** 3 story points

---

## 1. Descripción General

El usuario describe un caso y el sistema genera análisis de riesgo estructurado con score, normativa, jurisprudencia y recomendaciones.

---

## 2. Diagrama de Arquitectura

```mermaid
graph TB
    subgraph "Ingreso del Caso"
        FORM[Formulario: descripción + rama + jurisdicción]
    end
    subgraph "Procesamiento (Semantic Kernel)"
        AR[AgenteRiesgo Plugin]
        AN[AgenteNormativo]
        AJ[AgenteJurisprudencial]
    end
    subgraph "Fuentes"
        AIS[AI Search]
        SQL[Azure SQL Graph]
        OAI[Azure OpenAI]
    end
    subgraph "Output"
        SCORE[Score de Riesgo]
        NORM[Análisis Normativo]
        JURIS[Jurisprudencia Favorable/Desfavorable]
        RECOM[Recomendaciones]
        DOCX[Informe .docx]
    end
    FORM --> AR
    AR --> AN & AJ
    AN --> AIS & SQL
    AJ --> AIS & SQL
    AN & AJ --> OAI
    OAI --> SCORE & NORM & JURIS & RECOM
    SCORE & NORM & JURIS & RECOM --> DOCX
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

- El AgenteRiesgo orquesta llamadas a AgenteNormativo y AgenteJurisprudencial via Semantic Kernel
- El score de riesgo es un valor 0-100 con categorías: bajo (0-30), medio (31-60), alto (61-80), crítico (81-100)
- La taxonomía de factores de riesgo se configura como JSON en Azure SQL (tabla ConfiguracionRiesgo)
- Los análisis se persisten con snapshot de las fuentes consultadas para reproducibilidad
- El prompt del agente incluye instrucciones de formato JSON para el output estructurado

---

## 9. Work Items de esta Feature

| ID | Nombre | Tipo | Sprint |
|----|--------|------|--------|
| F15-W01 | Documentacion Integral | doc | S08-S09 |
| F15-W02 | Backend - Plugin AgenteRiesgo Semantic Kernel | backend | S08-S09 |
| F15-W03 | Backend - Modelo de Taxonomia de Riesgos | backend | S08-S09 |
| F15-W04 | Backend - Endpoint POST Analizar Riesgo | backend | S08-S09 |
| F15-W05 | Backend - Persistencia de Analisis en SQL | backend | S08-S09 |
| F15-W06 | Frontend - Formulario de Ingreso de Caso | frontend | S08-S09 |
| F15-W07 | Frontend - Vista de Resultado con Score Visual | frontend | S08-S09 |
| F15-W08 | Testing - Evaluacion de Analisis de Riesgo | testing | S08-S09 |

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

*F15 - Analisis de Riesgo Legal — Documentación integral — Legal Ai Ar*
