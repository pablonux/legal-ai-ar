# Guía del Desarrollador — Legal Ai Ar

> Cómo trabajar con los asistentes IA (Claude en Cowork / Cursor) para implementar work items del proyecto.

---

## 1. Antes de empezar

### Entorno de desarrollo

El setup del entorno está documentado en el work item **F00 - W07 - Setup Entorno Local y Onboarding Guide** (`docs/roadmap/F00 - Entorno y Estructura de Desarrollo/`). En resumen necesitás:

- .NET 10 SDK
- Node.js 20+ y Angular CLI 19
- Azure subscription configurada (SQL, AI Search, OpenAI, Storage)
- Azure Entra ID (autenticación)
- Git + acceso al repo `legal-ai-ar`

### Herramientas IA

Trabajamos con dos herramientas de asistencia IA. Ambas están configuradas para comportarse de manera idéntica — mismas reglas, misma nomenclatura, mismas convenciones:

| Herramienta | Quién la usa | Foco principal |
|-------------|-------------|----------------|
| **Cowork** (Claude Desktop) | Pablo | Planificación, documentación, consistencia |
| **Cursor** | Desarrolladores | Análisis técnico, diseño, implementación, review |

Las IAs **nunca crean ni modifican código directamente**. Siempre te indican qué archivo crear, en qué ruta, y te dan el código para que vos lo coloques. Esto es una regla de proyecto.

### Estructura del repo

```
legal-ai-ar/
├── mvp/
│   ├── backend/src/
│   │   ├── api/LegalAiAr.Api/              # Minimal API, endpoints
│   │   ├── api/LegalAiAr.Application/      # CQRS, handlers, DTOs
│   │   ├── shared/LegalAiAr.Core/          # Entidades, interfaces
│   │   ├── shared/LegalAiAr.Infrastructure/ # EF Core, Azure, AI
│   │   ├── shared/LegalAiAr.Agents/        # Semantic Kernel
│   │   ├── workers/                         # BackgroundServices
│   │   └── tools/                           # CLI auxiliares
│   ├── backend/tests/                       # xUnit + NSubstitute
│   └── frontend/                            # Angular 19 SPA
├── docs/
│   ├── roadmap/                             # Work items por feature
│   ├── tecnicas/                            # 9 docs técnicos
│   └── ontologia/                           # Modelo de dominio
├── CLAUDE.md                                # Instrucciones IA (Cowork)
└── .cursor/
    ├── rules/                               # Instrucciones IA (Cursor)
    └── skills/                              # Skills de Cursor
```

---

## 2. Anatomía de un work item

Cada work item es un archivo `.md` en `docs/roadmap/{Feature}/`. Esta es la estructura:

```
Header          → Feature, Release, Sprint, Tipo, Prioridad, Estimación
Descripción     → Qué hay que hacer y por qué
Tareas          → Checklist (genérico al crear, detallado después del breakdown)
Notas técnicas  → Stack, patrones, configuración
Archivos        → Qué crear/modificar
Criterios       → Cuándo está "listo"
Dependencias    → Qué bloquea y qué necesita
```

Cuando te asignan un work item, lo primero que hacés es **desglosar las tareas genéricas en tasks de implementación concretas** usando el skill `task-breakdown`.

---

## 3. Skills disponibles en Cursor

Estos son los skills que tenés disponibles en Cursor. Los invocás desde el chat escribiendo lo que necesitás — Cursor detecta automáticamente cuál usar.

### task-breakdown — Desglosar tareas

**Cuándo**: al recibir un work item asignado, antes de empezar a codear.

**Qué hace**: lee el work item, la documentación técnica relevante, y reemplaza las tareas genéricas con un checklist detallado de archivos a crear/modificar, con rutas exactas, nombres de clases, y métodos.

```
Tú:     "Desglosá las tareas del work item F08-W03"
Cursor: [lee W03, W01, features.md, docs técnicos relevantes
         → genera checklist: 1. Modelo (DTOs), 2. Servicios, 
         3. CQRS, 4. Validación, 5. Endpoints, 6. Tests, 7. Verificación
         → actualiza la sección Tareas del work item]
```

### architect — Análisis técnico

**Cuándo**: antes del breakdown, cuando necesitás entender el impacto técnico de un work item en el sistema.

**Qué hace**: analiza qué capas, servicios, y componentes se ven afectados. Produce un plan con decisiones técnicas, dependencias, y riesgos.

```
Tú:     "Analizá el impacto técnico de F09-W02"
Cursor: [lee W02 + docs técnicos → plan: archivos a crear, 
         decisiones (patrón Strategy vs directo), riesgos, orden sugerido]
```

### developer — Implementación

**Cuándo**: después del breakdown, cuando estás listo para codear.

**Qué hace**: para cada task del breakdown, te presenta el código completo de cada archivo con su ruta. Vos lo colocás.

```
Tú:     "Implementemos la task 1 de F08-W03: crear los DTOs"
Cursor: [presenta: "Creá LegalNormSearchRequestDto.cs en 
         backend/src/api/LegalAiAr.Application/DTOs/LegalNorms/ 
         con el siguiente código: ..."]
```

### designer — Mockups

**Cuándo**: cuando necesitás mockups HTML antes de implementar un componente frontend.

```
Tú:     "Creá el mockup para la vista de búsqueda de normas"
Cursor: [lee guías de diseño → produce mockup HTML]
```

### reviewer — Code review

**Cuándo**: antes de hacer PR, para verificar que tu código cumple con los estándares.

```
Tú:     "Revisá los archivos que creé para F08-W03"
Cursor: [revisa contra convenciones → reporte con issues por severidad]
```

---

## 4. Flujo completo: del work item al PR

Este es el flujo paso a paso para implementar un work item asignado.

### Paso 1 — Entender el contexto

Leé el work item y su W01 (Documentación Integral). Si algo no queda claro, preguntale a la IA:

```
Tú:     "Explicame qué necesita F08-W03 y cómo encaja con el resto de la feature"
```

### Paso 2 — Análisis técnico (opcional pero recomendado)

Si el work item es complejo (5+ story points), pedí un análisis técnico primero:

```
Tú:     "Analizá el impacto técnico de F08-W03"
Cursor: [skill: architect → produce plan de implementación]
```

Revisá el plan. Si hay decisiones técnicas que tomar, discutirlas ahora.

### Paso 3 — Desglosar tareas

Pedí el breakdown de tareas. Esto reemplaza las tareas genéricas del work item:

```
Tú:     "Desglosá las tareas de F08-W03"
Cursor: [skill: task-breakdown → genera checklist detallado en el work item]
```

Revisá el checklist. Si falta algo o algo no tiene sentido, pedí ajustes.

### Paso 4 — Crear branch

```bash
git checkout main
git pull
git checkout -b feature/f08-w03-chat-endpoint
```

### Paso 5 — Implementar task por task

Seguí el checklist en orden. Para cada task, pedí el código:

```
Tú:     "Implementá la task 1: crear los DTOs de chat"
Cursor: [skill: developer → presenta código completo con ruta]
```

Creá el archivo en la ruta indicada y pegá el código. Repetí para cada task.

A medida que completás cada task, marcala con `[x]` en el work item.

### Paso 6 — Verificación

Una vez que todas las tasks están completas:

```bash
dotnet build
dotnet test
```

Verificá los criterios de aceptación del work item uno por uno.

### Paso 7 — Review

Antes del PR, pedí un review:

```
Tú:     "Revisá todo lo que implementé para F08-W03"
Cursor: [skill: reviewer → reporte de issues]
```

Corregí los issues críticos e importantes.

### Paso 8 — PR

```bash
git add .
git commit -m "feat(F08): implementar endpoint POST chat con SignalR streaming"
git push -u origin feature/f08-w03-chat-endpoint
```

Creá el PR a `main` con:
- Título: `feat(F08): implementar endpoint POST chat con SignalR streaming`
- Descripción: lista de archivos creados/modificados y work item referenciado
- Referencia al work item: `Closes F08-W03`

---

## 5. Convenciones que la IA aplica (y vos también)

Estas reglas están configuradas en la IA y se aplican automáticamente, pero es bueno conocerlas:

| Tema | Convención |
|------|-----------|
| **Idioma** | Todo en inglés: código, nombres, comentarios, documentación, commits, work items. Español solo para textos visibles al usuario final (UI labels, mensajes de error, tooltips) |
| **Nombres** | `LegalAiAr.*` para proyectos .NET (nunca "LegalKB") |
| **Arquitectura** | Clean Architecture: Core → Application → Infrastructure → Api |
| **Core** | NUNCA referencia a otro proyecto |
| **API** | Minimal API (no Controllers) |
| **CQRS** | Commands y Queries separados, handlers con MediatR |
| **DI** | Solo constructor injection |
| **async** | Todo I/O async con CancellationToken |
| **Logging** | `ILogger<T>` structured, nunca `Console.WriteLine` |
| **Config** | `IOptions<T>`, sin secretos en appsettings |
| **Tests** | xUnit + NSubstitute + FluentAssertions. Naming: `{Método}_{Escenario}_{Resultado}` |
| **Angular** | Standalone components, signals, no `any`, interfaces en `core/models/` |
| **Git** | Branch: `feature/{fXX}-{desc}`. Commits: `feat/fix/refactor(F{XX}): descripción` |
| **Azure** | Recursos: `{servicio}-legal-ai-ar-{ambiente}` |

---

## 6. Documentación de referencia

Estos son los documentos que la IA consulta y que vos también podés leer cuando necesitás contexto:

| Documento | Para qué |
|-----------|----------|
| `docs/roadmap/features.md` | Roadmap completo, endpoints, KPIs |
| `docs/roadmap/{Feature}/` | Work items de cada feature |
| `docs/tecnicas/01-rag-retrieval.md` | Búsqueda híbrida, embeddings, re-ranking |
| `docs/tecnicas/02-arquitectura-agentica.md` | Agentes, router, tool calling |
| `docs/tecnicas/03-prompt-engineering.md` | Templates, system prompts |
| `docs/tecnicas/04-ingesta-procesamiento.md` | Pipeline de ingesta |
| `docs/tecnicas/05-evaluacion-calidad-ia.md` | Métricas y evaluación |
| `docs/tecnicas/06-seguridad-compliance-ia.md` | Seguridad y compliance |
| `docs/tecnicas/07-observabilidad-llmops.md` | Observabilidad |
| `docs/tecnicas/08-ux-ia-legal.md` | UX del chat y citación |
| `docs/tecnicas/09-data-knowledge-management.md` | Gestión de datos |
| `docs/ontologia/ontologia_legal_argentina.md` | Modelo de dominio legal |

---

## 7. Preguntas frecuentes

**¿Puedo pedirle a la IA que escriba el código directo en los archivos?**
No. La regla del proyecto es que la IA indica qué crear y dónde, y vos lo colocás. Esto es intencional para que siempre revises lo que va al repo.

**¿Qué hago si la IA sugiere algo que no tiene sentido?**
Decíselo. "Eso no me parece correcto porque X". La IA ajusta. Si es una decisión de arquitectura, consultá con Pablo.

**¿Puedo saltarme el task-breakdown?**
Para work items simples (1-2 story points) podés ir directo al developer. Para 3+ story points, el breakdown te ahorra tiempo y errores.

**¿Cómo sé qué docs técnicos leer?**
No necesitás leerlos vos — la IA los lee automáticamente cuando le pedís analizar o implementar algo. Pero si querés contexto, están en `docs/tecnicas/`.

**¿Qué pasa si necesito un endpoint que no está en features.md?**
Avisale a Pablo. Puede ser que falte un work item o que haya que ajustar el roadmap.

---

*Guía del Desarrollador — Legal Ai Ar*
