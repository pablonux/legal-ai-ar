# M-08: Ingesta del Tesauro SAIJ — Plan de Implementación

**Fecha**: 2026-04-10
**Rama**: `feature/saij-thesaurus`
**Dependencias**: search-improves (M-01 a M-10)

---

## 1. Contexto

El Tesauro SAIJ de Derecho Argentino es un vocabulario controlado del Sistema Argentino de Información Jurídica (Ministerio de Justicia). Organiza ~12,000 descriptores en 27 ramas temáticas con relaciones jerárquicas (TG/TE), de sinonimia (USE/UP) y asociativas (TR). Es la fuente autoritativa de terminología legal argentina.

**Fuentes de datos**:
- API REST TemaTres 3.5: `http://vocabularios.saij.gob.ar/saij/services.php`
  - Soporta output JSON (`&output=json`)
  - Endpoints: `fetchTopTerms`, `fetchDown`, `fetchTerm`, `fetchAlt`, `fetchRelated`, `fetchDirectTerms`, `letter`
- PDFs (backup): lista alfabética y lista sistemática

**Relaciones del tesauro**:
| Código | Significado | Ejemplo |
|--------|-------------|---------|
| TG | Término General (padre) | "abandono de trabajo" → TG: "despido con causa" |
| TE | Término Específico (hijo) | "aborto" → TE: "aborto no punible" |
| UP | Usado Por (sinónimo no preferido) | "aborto" ← UP: "delito de aborto" |
| USE | Redirección al preferido | "cuatrerismo" → USE: "abigeato" |
| TR | Término Relacionado | "aborto" → TR: "interrupción del embarazo" |
| UPAB | Abreviatura sinónima | "ABL" → USE: "alumbrado, barrido y limpieza" |

---

## 2. Objetivos

1. **Modelar el tesauro como entidad de primer nivel** en la KB (igual que Rulings, Courts, Judges)
2. **Pipeline de ingesta dedicado** que consuma la API TemaTres y persista en Azure SQL
3. **Generar synonym maps** para Azure AI Search automáticamente desde las relaciones USE/UP
4. **Enriquecer el buscador**: usar el tesauro para expandir queries y mejorar el LLM preprocessing
5. **Vincular keywords de rulings** con descriptores del tesauro (normalización)

---

## 3. Modelo de datos

### 3.1 Entidades nuevas (Azure SQL)

```
ThesaurusTerm
├── Id (int, PK, auto)
├── ExternalId (int, unique) ← ID en TemaTres
├── PreferredLabel (nvarchar 500) ← descriptor preferido
├── IsPreferred (bit) ← true si es descriptor, false si es no-preferido
├── BranchCode (nvarchar 50, nullable) ← código de rama (ej: "01.03")
├── BranchName (nvarchar 200, nullable) ← "Derecho laboral"
├── Depth (int) ← profundidad en la jerarquía
├── CreatedAt (datetime2)
├── UpdatedAt (datetime2)

ThesaurusRelation
├── Id (int, PK, auto)
├── SourceTermId (int, FK → ThesaurusTerm.Id)
├── TargetTermId (int, FK → ThesaurusTerm.Id)
├── RelationType (nvarchar 10) ← 'BT' (broader), 'NT' (narrower), 'UF' (use for), 'RT' (related)
├── UNIQUE (SourceTermId, TargetTermId, RelationType)

RulingKeywordMapping (vinculación ruling ↔ tesauro)
├── RulingId (uniqueidentifier, FK → Ruling.Id)
├── ThesaurusTermId (int, FK → ThesaurusTerm.Id)
├── MatchType (nvarchar 20) ← 'exact', 'synonym', 'broader'
├── PK (RulingId, ThesaurusTermId)
```

### 3.2 Relaciones SKOS → SQL

| TemaTres | RelationType | Dirección |
|----------|-------------|-----------|
| TG (padre) | BT (broader term) | hijo → padre |
| TE (hijo) | NT (narrower term) | padre → hijo |
| UP (sinónimo) | UF (use for) | preferido → no-preferido |
| TR (relacionado) | RT (related term) | bidireccional |

---

## 4. Fases de implementación

### Fase 1: Modelo de datos e ingesta (esfuerzo: 3-4 días)

#### F-THES-1: Modelo y migración

| Task | Descripción | Deliverable |
|------|-------------|-------------|
| T-00 | Documentación de diseño | E260: `docs/design/thesaurus-data-model.md` |
| T-01 | Entidades `ThesaurusTerm`, `ThesaurusRelation` en Core | `[ ] DEV` `[ ] AUD` |
| T-02 | EF Core configurations y migración | `[ ] DEV` `[ ] AUD` |
| T-03 | `IThesaurusRepository` + implementación | `[ ] DEV` `[ ] AUD` |

#### F-THES-2: Crawler del tesauro

| Task | Descripción | Deliverable |
|------|-------------|-------------|
| T-04 | `IThesaurusCrawler` interface en Core | `[ ] DEV` `[ ] AUD` |
| T-05 | `SaijThesaurusCrawler` — consume API TemaTres con JSON | `[ ] DEV` `[ ] AUD` |
| T-06 | CLI tool `LegalAiAr.IngestThesaurus` | `[ ] DEV` `[ ] AUD` |

**Estrategia de crawling**:
1. `fetchTopTerms` → obtener las 27 ramas raíz
2. Para cada raíz, `fetchDown` recursivo hasta agotar la jerarquía
3. Para cada término, `fetchAlt` para obtener sinónimos (UP)
4. Para cada término, `fetchRelated` para obtener términos relacionados (TR)
5. Throttling: 200ms entre requests (cortesía con servidor SAIJ)
6. Idempotencia: upsert por `ExternalId`, `ON CONFLICT UPDATE`

**Output esperado**: ~12,000 términos preferidos + ~5,000 sinónimos + ~20,000 relaciones

### Fase 2: Synonym maps y búsqueda (esfuerzo: 2 días)

#### F-THES-3: Generación de synonym maps

| Task | Descripción | Deliverable |
|------|-------------|-------------|
| T-07 | Generador de synonym maps desde relaciones UF/RT | `[ ] DEV` `[ ] AUD` |
| T-08 | Integrar en `SetupSearch` — crear/actualizar synonym map en Azure AI Search | `[ ] DEV` `[ ] AUD` |
| T-09 | Configurar campos del índice para usar el synonym map | `[ ] DEV` `[ ] AUD` |

**Formato Azure AI Search Synonym Map** (Solr format):
```
despido, cesantía, distracto, desvinculación
cuatrerismo, abigeato, hurto de ganado, hurto campestre
recurso extraordinario federal, REF
```

Las relaciones USE/UP generan líneas de equivalencia. Las relaciones TR se pueden agregar como expansiones unidireccionales:
```
libertad de expresión => libertad de expresión, libertad de prensa, derecho a la información
```

**Límite Azure Basic tier**: 3 synonym maps × 5,000 reglas cada uno.

### Fase 3: Vinculación con rulings (esfuerzo: 2 días)

#### F-THES-4: Normalización de keywords

| Task | Descripción | Deliverable |
|------|-------------|-------------|
| T-10 | `KeywordNormalizationService` — mapea keywords de rulings a descriptores del tesauro | `[ ] DEV` `[ ] AUD` |
| T-11 | Backfill tool para normalizar keywords existentes (~8,300 rulings) | `[ ] DEV` `[ ] AUD` |
| T-12 | Integrar normalización en IndexerWorker pipeline | `[ ] DEV` `[ ] AUD` |

**Estrategia de matching**:
1. Match exacto (keyword = PreferredLabel)
2. Match por sinónimo (keyword = label de un UP del descriptor)
3. Match fuzzy (Levenshtein ≤ 2 + mismo TG) para variantes ortográficas

### Fase 4: Enriquecimiento del LLM preprocessing (esfuerzo: 1 día)

#### F-THES-5: Query expansion con tesauro

| Task | Descripción | Deliverable |
|------|-------------|-------------|
| T-13 | Actualizar `SearchQueryPreprocessor` para consultar el tesauro antes del LLM | `[ ] DEV` `[ ] AUD` |
| T-14 | Inyectar contexto del tesauro en el prompt del preprocessor | `[ ] DEV` `[ ] AUD` |

**Flujo mejorado**:
```
Query "despido" 
  → Tesauro lookup: broader="despido con causa", synonyms=["cesantía","distracto"], related=["indemnización por despido"]
  → LLM prompt incluye este contexto
  → keywordQuery: "despido cesantía distracto desvinculación indemnización"
  → semanticQuery: "Jurisprudencia sobre despido laboral, incluyendo cesantía y distracto, con indemnización por despido sin justa causa"
```

### Fase 5: API y frontend (esfuerzo: 1-2 días)

#### F-THES-6: Exposición del tesauro

| Task | Descripción | Deliverable |
|------|-------------|-------------|
| T-15 | `GET /api/thesaurus/search?q=` — autocomplete de descriptores | `[ ] DEV` `[ ] AUD` |
| T-16 | `GET /api/thesaurus/{id}` — detalle de descriptor con relaciones | `[ ] DEV` `[ ] AUD` |
| T-17 | Frontend: autocomplete de keywords en filtro de búsqueda | `[ ] DEV` `[ ] AUD` |
| T-18 | Frontend: chips de descriptores en ruling detail | `[ ] DEV` `[ ] AUD` |

---

## 5. Resumen

| Fase | Alcance | Esfuerzo | Impacto |
|------|---------|----------|---------|
| 1 | Modelo + ingesta | 3-4 días | Fundacional |
| 2 | Synonym maps | 2 días | Alto — mejora directa en recall del buscador |
| 3 | Vinculación con rulings | 2 días | Alto — normalización de keywords |
| 4 | LLM preprocessing | 1 día | Medio — query expansion más precisa |
| 5 | API + frontend | 1-2 días | UX — autocomplete y navegación del tesauro |

**Total estimado**: 9-11 días de desarrollo

---

## 6. Riesgos

| Riesgo | Mitigación |
|--------|------------|
| API SAIJ no disponible o throttled | Backup: parsear PDFs descargados. Caché local de responses. |
| Límite de 5,000 reglas por synonym map (Basic tier) | Priorizar sinónimos más usados. Agrupar por rama temática si necesario. |
| Keywords de rulings no matchean con tesauro | Match fuzzy + revisión manual de los no-mapeados |
| Tesauro se actualiza (última: 2025-06-03) | `termsSince` endpoint de TemaTres permite ingesta incremental |
