# Plan: Navegación alineada al mapa mental del abogado

| Campo | Valor |
|-------|--------|
| Estado | Fase 1 implementada (sidebar, hub intervinientes con `vista`, API `vista`, detalle de fallo, palette, bienvenida, redirect `/causas`) |
| Alcance | Solo IA / UX y rutas de presentación; **sin** cambiar ontología ni modelo de datos de `Person` (identidad) vs roles contextuales (`RulingParticipation`, `ProceedingParty`, oficinas judiciales). |
| Contexto | La KB y la ontología son correctas al modelar roles en relaciones; la fricción es la **arquitectura de información** del menú y las pantallas de entrada. |

---

## 1. Objetivo

Rediseñar menú lateral, hubs y flujos para que un abogado reconozca **tareas y vocabulario procesal** (jurisprudencia, causas, tribunales, personas por rol, normas) sin renunciar al modelo actual de datos.

---

## 2. Mapa mental del usuario (orden de uso típico)

1. **Jurisprudencia** — buscar, leer, citar fallos.
2. **Causas / expedientes** — seguimiento del proceso y cadena de resoluciones.
3. **Personas según rol** — magistrados que firman, partes, otros intervinientes; no solo “sujeto abstracto”.
4. **Tribunales / organismos** — competencia, instancia, fuero.
5. **Normativa** — leyes y jerarquía.
6. **Herramientas** — asistente, estadísticas, explorador.
7. **Meta** — ontología, administración (según rol).

Rutas existentes que mapean bien: `/jurisprudencia`, `/procesos`, `/sujetos`, `/organismos`, `/ordenamiento`, `/vocabulario`, `/asistente`, `/explorador`, `/estadisticas`, `/ontologia`, `/admin`.

---

## 3. Menú izquierdo propuesto

**Principio:** pocas secciones con etiquetas **procesales**; un solo espacio “Personas” con **subnavegación por rol** (vistas filtradas, no nuevas entidades persistentes).

| Sección | Ítems | Rutas actuales |
|---------|--------|----------------|
| **Consulta** | Inicio · Jurisprudencia | `/bienvenida`, `/jurisprudencia` |
| **Proceso** | Causas (o Expedientes) · Tribunales | `/procesos`, `/organismos` |
| **Personas** | Hub “Personas” / “Intervinientes” con subpestañas o submenú: **Magistrados y firmas** · **Partes** · **Todos** | Base `/sujetos` + query o rutas hijas (ej. `?view=`, `/sujetos/magistrados`) |
| **Normas** | Ordenamiento · Vocabulario / descriptores | `/ordenamiento`, `/vocabulario` |
| **Herramientas** | Asistente · Explorador · Estadísticas | `/asistente`, `/explorador`, `/estadisticas` |
| **Avanzado** | Ontología · Admin | `/ontologia`, `/admin` |

### Copy sugerido (menú)

| Actual | Propuesta |
|--------|-----------|
| Sujetos | **Personas** o **Intervinientes** |
| Procesos | **Causas** o **Expedientes** |
| Organismos | **Tribunales** o **Tribunales y organismos** |

Mantener URLs `/sujetos` y `/procesos` salvo que se decidan **redirects opcionales** amigables (`/causas` → `/procesos`).

---

## 4. Navegación general (fuera del sidebar)

1. **Hub al entrar en Personas** (`/sujetos`): pestañas o chips *Magistrados · Partes · Todos* + búsqueda; misma ficha de detalle de persona.
2. **Detalle de fallo como centro de gravedad**: bloques explícitos Tribunal, Firmas/votos, Expediente, Partes con enlaces a personas en contexto.
3. **Breadcrumbs** alineados a la nueva agrupación (ej. Consulta › Jurisprudencia › Fallo › Persona).
4. **Command palette** y atajos `G *`: mismos nombres que el menú renombrado.
5. **Bienvenida**: accesos directos a “Buscar fallos”, “Causas”, “Tribunales” (y opcionalmente “Personas”) según mapa mental.

---

## 5. Intencionalmente fuera de alcance (fase 1)

- Cambiar clases ontológicas o hacer “Juez” subtipo de `Person`.
- Duplicar entidades en BD por rol.

Los roles siguen en relaciones; la UI solo **materializa vistas y filtros**.

---

## 6. Orden de implementación sugerido

1. **Sidebar + labels** — reagrupar secciones y renombrar (bajo riesgo).
2. **Hub `/sujetos` con vistas por rol** — alto impacto UX; puede requerir filtros en API de listado de personas si aún no existen.
3. **Detalle de fallo** — anclas y bloques por rol hacia personas.
4. **Palette, bienvenida, breadcrumbs** — coherencia global.
5. **Opcional** — redirects `/causas`, documentar en changelog interno.

---

## 7. Archivos probablemente tocados (referencia)

- `frontend/src/app/layout/shell/shell.component.ts` — estructura y textos del nav.
- `frontend/src/app/app.routes.ts` — rutas hijas o redirects opcionales.
- `frontend/src/app/features/catalogs/persons-list/` — hub, tabs, query params.
- `frontend/src/app/services/command-palette.service.ts` — etiquetas de comandos.
- Componente de bienvenida y detalle de fallo según diseño de bloques.

---

## 8. Criterios de aceptación (borrador)

- Un usuario puede llegar a “personas que actúan como magistrado en fallos” sin tener que adivinar el término “Sujetos”.
- Navegación principal no exige abrir “Ontología” para entender roles.
- URLs existentes compartidas siguen funcionando (o redirigen de forma explícita).

---

## API (personas)

`GET /api/persons?vista=magistrados|partes` — opcional; sin parámetro o valor no reconocido = vista «Todos».

- **magistrados**: participación en fallos con rol de integración del tribunal (`SIGNATORY`, `DISSENT`, `CONCURRENCE`, `MAJORITY_AUTHOR`; no incluye `PROSECUTOR` ni `PUBLIC_DEFENDER`).
- **partes**: al menos un registro en `ProceedingParty`. La columna numérica devuelve cantidad de **causas** (participaciones en expediente).

---

## Referencias en el repo

- Ontología y mapeo UI: `docs/ontology/legal-ai-ar-ontology.md`
- Persona sin rol global, roles contextuales: `docs/design/kb-domain-model-v2.md`
- Restructura UI previa: `docs/design/f2-1-ui-restructure.md`
