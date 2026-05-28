# Tutorial: Configuración IA para Legal Ai Ar

> Guía de referencia de la configuración de asistentes IA del proyecto. Cubre Cowork (Claude Desktop) y Cursor, ambos configurados para comportarse de manera idéntica.

---

## 1. El proyecto

El proyecto Cowork ya está conectado a la carpeta `legal-ai-ar/` que contiene:

```
legal-ai-ar/
├── mvp/                    # Código del MVP (backend + frontend)
│   ├── backend/            # .NET 10, Clean Architecture, 21 proyectos
│   └── frontend/           # Angular 19 SPA
├── docs/                   # Documentación del proyecto
│   ├── roadmap/            # Features (F00-F23, FT01-FT04), work items, backlog
│   ├── tecnicas/           # 9 documentos técnicos (RAG, agentes, prompts, etc.)
│   └── ontologia/          # Modelo de dominio legal argentino
├── README.md
├── CLAUDE.md               # → Instrucciones para Cowork (sección 2)
├── .claude/                # → Configuración Cowork
│   ├── memory.md           #   Memoria persistente (sección 3)
│   └── skills/             #   13 skills compartidos (sección 4)
│       ├── work-item-generator/   entity-analyzer/
│       ├── consistency-checker/   documenter/
│       ├── architect/             developer/
│       ├── designer/              reviewer/
│       ├── task-breakdown/        doc-standards/
│       ├── backend-tools/         ia-tools/
│       └── infra-tools/
└── .cursor/                # → Configuración Cursor (sección 5)
    ├── rules/              #   Reglas del proyecto para Cursor
    │   ├── proyecto.mdc    #   Reglas generales (equivale a CLAUDE.md)
    │   ├── backend-dotnet.mdc    # Convenciones backend
    │   ├── frontend-angular.mdc  # Convenciones frontend
    │   └── work-items.mdc       # Template de work items
    └── skills/             #   Los mismos 13 skills que en .claude/
```

Al abrir una sesión en Cowork con esta carpeta, Claude lee automáticamente el `CLAUDE.md` y tiene acceso a todos los archivos del repo, los skills, y la memoria.

---

## 2. CLAUDE.md — Instrucciones del proyecto

**Archivo**: `CLAUDE.md` (raíz del repo)

Es el "system prompt" del proyecto. Claude lo lee al inicio de cada sesión. Contiene las reglas y el contexto que Claude necesita para trabajar correctamente con Legal Ai Ar.

### Qué incluye

- **Identidad**: qué es Legal Ai Ar
- **Stack**: Angular 19, .NET 10, Azure OpenAI, Semantic Kernel, etc.
- **Estructura del monorepo**: mapa de carpetas y proyectos .NET
- **Convenciones de código**: Clean Architecture, naming, patterns (backend y frontend)
- **Nomenclatura Azure**: `{servicio}-legal-ai-ar-{ambiente}`
- **Releases**: R0.0 (Preparación) → R1.0 (Foundation) → R2.0 (Agents) → R3.0 (Risk) → R4.0 (Operations)
- **Reglas**: idioma español, monorepo único, nombres `LegalAiAr.*` (nunca LegalKB)

### Cómo extenderlo

Editá directamente el archivo para agregar reglas nuevas. Ejemplos:

- Preferencias de estilo: "siempre usar `var` en C#"
- Restricciones de paquetes: "no usar Dapper, usar EF Core"
- Convenciones de Git: "commits en inglés, formato conventional commits"
- Restricciones de scope: "no tocar los workers del MVP sin consultar"

---

## 3. Memoria — Contexto persistente

**Archivo**: `.claude/memory.md`

Guarda decisiones, convenciones y contexto que Claude puede consultar en cualquier sesión futura.

### Contenido actual

- **Decisiones**: monorepo existente (no crear repo nuevo), eliminación de SignalR para workers, rename de R0.0 a "Preparación"
- **Convenciones**: numeración de features (F00-F23, FT01-FT04), formato de work items, naming Azure
- **Estado**: R0.0 en progreso, pendiente crear LegalAiAr.Agents y LegalAiAr.AgentEvals
- **Cosas a evitar**: nunca "LegalKB", no repos separados, no SignalR para workers

### Cómo actualizarla

Decile a Claude en cualquier momento:
- "Recordá que decidimos usar Azure Functions en vez de BackgroundService"
- "Agregá a la memoria que el campo EstadoVigencia ahora es un enum"
- "Actualizá la memoria: R0.0 está completo"

La memoria crece con el tiempo. Cada tanto conviene revisarla y limpiar lo que ya no aplique.

---

## 4. Skills personalizados

Cuatro skills especializados para las tareas más frecuentes del proyecto. Se activan automáticamente cuando Claude detecta que tu pedido encaja con alguno de ellos.

### 4.1 Work Item Generator

**Cuándo se activa**: al pedir crear un work item, tarea, historia de usuario, o ticket para cualquier feature.

**Qué hace**:
1. Lee `features.md` para entender la feature padre
2. Verifica work items existentes para continuar la numeración
3. Genera el `.md` con el template completo: descripción, tareas con checkboxes, secciones técnicas, criterios de aceptación, dependencias
4. Lo guarda en la carpeta correcta (`docs/roadmap/F{XX} - {Nombre}/`)

**Ejemplos**:
```
"Creame el work item para el endpoint de búsqueda semántica en F03"
"Necesito un W03 en F09 para el plugin de búsqueda de normas vigentes"
"Agregá una tarea al roadmap para implementar el middleware de auth en F01"
```

### 4.2 Entity Analyzer

**Cuándo se activa**: al trabajar con entidades del dominio, modelo de datos, ontología, o grafo legal.

**Capacidades**:
- Validar una entidad contra la ontología formal (`docs/ontologia/`)
- Analizar relaciones entre dos entidades
- Proponer nuevas entidades con propiedades, relaciones, y código C#
- Auditar consistencia entre el código (`LegalAiAr.Core/Entities/`) y la ontología

**Ejemplos**:
```
"Verificá que la entidad Expediente en el código coincide con la ontología"
"Necesitamos una entidad RecursoJudicial. Proponé la estructura"
"Qué relaciones hay entre Fallo y Norma según la ontología?"
```

### 4.3 Consistency Checker

**Cuándo se activa**: al pedir auditorías de documentación, revisar consistencia, o después de cambios masivos.

**Qué verifica**:
- Features en `features.md` vs. carpetas y archivos existentes en disco
- Metadatos de work items (release, feature, ID, footer)
- Nombres correctos (`LegalAiAr`, no `LegalKB`)
- Endpoints y DTOs consistentes entre work items
- Referencias cruzadas y dependencias válidas (sin ciclos)

**Ejemplos**:
```
"Revisá la consistencia de todos los work items de F08"
"Hay inconsistencias entre features.md y las carpetas?"
"Verificá que no quedó ningún LegalKB en los docs"
```

### 4.4 Documentador

**Cuándo se activa**: al pedir documentación técnica, guías, ADRs, o documentación de API.

**Tipos de documento que genera**:
- Documento técnico numerado (para `docs/tecnicas/`)
- Guía de implementación paso a paso
- Architecture Decision Record (ADR)
- Documentación de API/endpoints

**Ejemplos**:
```
"Documentá la arquitectura del pipeline de ingesta como doc técnico"
"Creá un ADR sobre la decisión de usar Reciprocal Rank Fusion"
"Hacé una guía de implementación para el setup de Semantic Kernel"
```

---

## 5. Cursor — Configuración para el segundo dev

El otro desarrollador trabaja con Cursor. La configuración tiene dos niveles: reglas (siempre activas según contexto) y skills (roles especializados que se invocan explícitamente).

### 5.1 Reglas (`.cursor/rules/`)

Se cargan automáticamente según el archivo que se esté editando:

| Archivo | Alcance | Equivalente Cowork |
|---------|---------|-------------------|
| `proyecto.mdc` | Siempre activo | CLAUDE.md completo |
| `backend-dotnet.mdc` | Archivos `*.cs` y `*.csproj` en `mvp/backend/` | Sección "Backend (.NET)" de CLAUDE.md |
| `frontend-angular.mdc` | Archivos `*.ts`, `*.html`, `*.scss` en `mvp/frontend/` | Sección "Frontend (Angular)" de CLAUDE.md |
| `work-items.mdc` | Archivos `*.md` en `docs/roadmap/` | Skill work-item-generator |

### 5.2 Skills (`.cursor/skills/`)

Los **mismos 13 skills** están disponibles en Cowork (`.claude/skills/`) y en Cursor (`.cursor/skills/`), con nombres idénticos. Cualquiera de los dos puede invocar cualquier skill; la diferencia es el foco de cada plataforma, no las capacidades.

| Skill | Rol |
|-------|-----|
| **work-item-generator** | Genera work items siguiendo el template estándar |
| **consistency-checker** | Verifica consistencia del roadmap |
| **entity-analyzer** | Valida entidades contra la ontología |
| **documenter** | Genera documentación técnica (ADR, guías, API) |
| **architect** | Analiza impacto técnico, produce plan de implementación |
| **developer** | Implementa work items con código production-ready |
| **designer** | Crea mockups HTML con guías de diseño PwC |
| **reviewer** | Code review contra estándares del proyecto |
| **task-breakdown** | Descompone un work item en tareas concretas |
| **doc-standards** | Markdown, Mermaid, ADR templates |
| **backend-tools** | Archivos .http, migraciones EF, scaffolding CQRS |
| **ia-tools** | Prompt templates, golden sets, plugins Semantic Kernel |
| **infra-tools** | Config Azure, GitHub Actions |

### 5.3 Distribución de responsabilidades

Aunque ambas plataformas tienen los 13 skills, en la práctica cada una se enfoca en distintas etapas:

```
                    COWORK (Pablo)              CURSOR (Otro dev)
                    ──────────────              ─────────────────
Planificación       work-item-generator         (disponible)
                    consistency-checker
                    entity-analyzer

Análisis técnico    (disponible)                architect
Diseño              (disponible)                designer
Desarrollo          (disponible)                developer
Review              reviewer                    reviewer
Documentación       documenter                  (disponible)
```

Cowork se enfoca en planificación y documentación. Cursor se enfoca en análisis técnico, diseño e implementación. Ambos comparten las mismas reglas base (CLAUDE.md = proyecto.mdc), los mismos skills, y la misma restricción de no tocar código sin permiso.

### 5.4 Qué comparten ambas configuraciones

- Mismos 13 skills con nombres idénticos
- Misma regla de no tocar código sin permiso explícito
- **Mismo idioma**: todo en inglés (código, nombres, docs, commits, work items); español solo para textos visibles al usuario final
- Misma nomenclatura (LegalAiAr, nunca LegalKB)
- Misma estructura de monorepo y Clean Architecture
- Mismas convenciones de Azure y código
- Mismo template de work items

### 5.5 Cómo mantener la paridad

Cuando se agrega una regla nueva al CLAUDE.md, hay que reflejarla en `proyecto.mdc` (y viceversa). Ambos archivos deben mantenerse sincronizados.

---

## 6. Flujo de trabajo completo — Cowork + Cursor

Ejemplo: implementar **F09 - Agente Normativo**

### Fase 1 — Planificación (Cowork)

```
Pablo:  "Creame los work items para F09 - Agente Normativo"
Claude: [skill: work-item-generator → lee features.md → genera W01-W05]

Pablo:  "Revisá la consistencia"
Claude: [skill: consistency-checker → valida metadatos, dependencias, numeración]

Pablo:  "Necesitamos la entidad ConsultaNormativa. Proponé la estructura"
Claude: [skill: entity-analyzer → lee ontología → propone entidad con código C#]
```

### Fase 2 — Análisis técnico (Cursor)

```
Dev:    "Analizá el impacto técnico de F09-W02 (plugin de búsqueda de normas)"
Cursor: [skill: architect → lee features.md + docs/tecnicas/01-rag-retrieval.md
         → produce plan: archivos a crear, modificar, decisiones, riesgos]
```

### Fase 3 — Diseño (Cursor)

```
Dev:    "Creá el mockup para la vista del Agente Normativo"
Cursor: [skill: designer → lee guías PwC → produce mockup HTML con chat,
         panel de normas, citación inline → pide aprobación]
```

### Fase 4 — Desarrollo (Cursor)

```
Dev:    "Implementá F09-W02"
Cursor: [skill: developer → lee W02 + plan del architect → presenta:
         "Creá SearchLegalNormPlugin.cs en LegalAiAr.Agents/Plugins/Normativo/
          con el siguiente código: ..." → espera aprobación]

Dev:    "Dale, aprobado"
Cursor: [presenta código completo de cada archivo → pide aprobación final]
```

### Fase 5 — Review (Cursor o Cowork)

```
Dev:    "Revisá el código del PR de F09-W02"
Cursor: [skill: reviewer → revisa contra convenciones, tests,
         Clean Architecture → aprueba o reporta issues con severidad]
```

### Fase 6 — Documentación (Cowork)

```
Pablo:  "Documentá la arquitectura del Agente Normativo"
Claude: [skill: documenter → genera docs/tecnicas/10-agente-normativo.md]
```

---

## 7. Tips

- **Sé específico**: "Creame el W03 para F09 sobre el plugin de búsqueda de normas" funciona mejor que "hacé algo para F09"
- **Encadená skills**: creá un work item y después pedí que verifique la consistencia
- **Actualizá la memoria**: cuando tomes una decisión importante, pedile a Claude que la registre para futuras sesiones
- **Iterá sobre los skills**: si un skill no genera lo que esperás, editá su `SKILL.md` directamente
- **Usá el CLAUDE.md como contrato**: si Claude comete un error recurrente (ej: usa "LegalKB"), agregá una regla más explícita

---

*Tutorial: Configuración IA para Legal Ai Ar — Legal Ai Ar*
