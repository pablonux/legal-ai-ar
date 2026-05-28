# Análisis UX — Top 10 Web Apps & Aplicabilidad a Legal AI AR

> **Fecha:** 2026-04-30
> **Contexto:** Investigación de las mejores web apps del mercado rankeadas por experiencia de usuario, identificación de diferenciadores clave y plan de aplicación a Legal AI AR manteniendo la identidad PwC (isologo, paleta `#D04A02` / `#EB8C00` / `#191919` / `#f3f3f3`, tipografía Georgia + Arial).

---

## 1. Las 10 mejores web apps por experiencia de usuario

### 1.1 Linear (Gestión de proyectos)

**¿Por qué es referencia?**
Linear redefinió lo que "rápido" significa en una web app. No hay spinners, no hay estados de carga visibles. Todo se siente nativo.

**Diferenciadores:**

| Patrón | Descripción |
|--------|-------------|
| Zero-latency UI | Optimistic updates + prefetching. Las acciones se reflejan instantáneamente en la UI antes de confirmarse en el servidor. |
| Command Palette (`Cmd+K`) | Punto de entrada unificado para buscar, navegar y ejecutar acciones desde cualquier pantalla. |
| Keyboard-first | Cada acción tiene shortcut. Los atajos se muestran inline junto a los items del menú. |
| Calm design | Reducción radical de ruido visual. Tipografía y spacing hacen todo el trabajo de jerarquía, sin depender de colores saturados o bordes gruesos. |
| Smooth transitions | Animaciones CSS de 150-200ms en cambios de estado que orientan la atención sin distraer. |

**¿Qué aprende el usuario?** Linear entrena al usuario a ser más productivo: cuanto más lo usás, más rápido te volvés gracias a los shortcuts progresivos.

---

### 1.2 Notion (Productividad / Knowledge Base)

**¿Por qué es referencia?**
Notion hizo accesible la complejidad. Un sistema de bloques infinitamente componible que no intimida al usuario nuevo pero escala para power users.

**Diferenciadores:**

| Patrón | Descripción |
|--------|-------------|
| Breadcrumb enriquecido | Navegación jerárquica siempre visible que muestra exactamente dónde estás y permite saltar a cualquier nivel. |
| Hover previews | Al pasar el cursor sobre un link interno, un popover muestra preview del contenido sin navegar. |
| Slash commands | `/` abre un menú contextual de acciones rápidas inline. |
| Block architecture | Todo es un bloque: texto, tabla, imagen, embed. Composición flexible sin templates rígidos. |
| Inline editing | El contenido se edita en contexto, sin modals ni pantallas intermedias. |

**¿Qué aprende el usuario?** Notion demuestra que la complejidad se domina con composición, no con menús profundos.

---

### 1.3 Perplexity (AI Search)

**¿Por qué es referencia?**
Perplexity resolvió el problema fundamental de la IA generativa: la confianza. Lo hizo poniendo las fuentes al frente, no al final.

**Diferenciadores:**

| Patrón | Descripción |
|--------|-------------|
| Citation-forward | Las fuentes se muestran inline con numeración `[1][2][3]` y se revelan progresivamente mientras el modelo razona. |
| Streaming por fases | UI muestra fases explícitas: "Buscando..." → "Leyendo 5 fuentes..." → Respuesta con citas. |
| Panel de fuentes lateral | Las citas se expanden para ver el pasaje original del documento fuente. |
| Respuestas estructuradas | Headers, listas y tablas dentro de las respuestas de IA. No un bloque de texto plano. |
| Intent-based design | El sistema interpreta la intención del usuario más allá de la consulta literal. |

**¿Qué aprende el usuario?** La IA genera confianza cuando muestra su trabajo. Las citas verificables son el puente entre "generación" y "conocimiento".

---

### 1.4 Stripe (Dashboard de pagos)

**¿Por qué es referencia?**
Stripe hace legible lo complejo. Transacciones, disputas, webhooks — data densa presentada con claridad absoluta.

**Diferenciadores:**

| Patrón | Descripción |
|--------|-------------|
| Progressive disclosure | La información se revela en capas. Overview limpio → detalle a un click → raw data si lo necesitás. |
| Data density sin ruido | Tablas densas jerarquizadas con tipografía, spacing y color — sin bordes innecesarios. |
| Contextual actions | Las acciones aparecen on-hover sobre cada fila, no en un toolbar genérico. |
| Filter chips removibles | Los filtros activos se representan como pills sobre los resultados, cada una con `×` para remover. |
| KPI cards | Métricas principales como números grandes y prominentes arriba de cada vista. |

**¿Qué aprende el usuario?** La densidad de información no es enemiga de la claridad. La jerarquía tipográfica + progressive disclosure resuelven el problema.

---

### 1.5 Vercel (Plataforma de deployment)

**¿Por qué es referencia?**
Vercel eliminó la fricción. Cada interacción tiene el mínimo de pasos necesarios y máximo feedback en tiempo real.

**Diferenciadores:**

| Patrón | Descripción |
|--------|-------------|
| Real-time feedback | Logs en vivo, estados que cambian sin recargar. WebSockets para todo lo que tiene progreso. |
| Minimalism funcional | Cada pantalla tiene un propósito claro. Cero elementos decorativos que no comuniquen algo. |
| Smooth transitions | Cambios de estado animados (200ms ease) que orientan la atención. |
| Empty states productivos | Los estados vacíos invitan a la acción con CTA claro, no son mensajes de error. |
| Zero-config defaults | Valores sensatos por defecto que funcionan para el 80% de casos sin configuración. |

**¿Qué aprende el usuario?** Los defaults inteligentes respetan el tiempo del usuario. El estado vacío es una oportunidad de onboarding.

---

### 1.6 Arc Browser

**¿Por qué es referencia?**
Arc reimaginó el browser como espacio de trabajo. La UI desaparece cuando no la necesitás y aparece exactamente cuando la necesitás.

**Diferenciadores:**

| Patrón | Descripción |
|--------|-------------|
| Spaces | Contextos separados que permiten switch rápido entre "modos" de trabajo. |
| Command Bar | Todo accesible desde un único input al tope de la pantalla. |
| Peek mode | Links se abren como preview flotante antes de navegar — preserva contexto. |
| Split view | Dos paneles lado a lado para comparar o trabajar en paralelo. |
| Minimalist chrome | La interfaz del browser se oculta, maximizando el área de contenido. |

**¿Qué aprende el usuario?** Preservar el contexto del usuario es más importante que navegar rápido. Preview > Navigate.

---

### 1.7 Figma (Herramienta de diseño)

**¿Por qué es referencia?**
Figma demostró que una app web puede tener performance nativa. La colaboración en tiempo real cambió el paradigma de "archivo compartido".

**Diferenciadores:**

| Patrón | Descripción |
|--------|-------------|
| Multiplayer indicators | Sabés quién más está activo con avatars y cursores coloreados. |
| Contextual tooling | Las herramientas y propiedades cambian según lo que tenés seleccionado. |
| Auto-save permanente | No hay botón "Guardar". El estado siempre está sincronizado. |
| Community ecosystem | Templates y plugins de la comunidad extienden la funcionalidad core. |
| Performance-first | Canvas WebGL con 60fps en documentos complejos. |

**¿Qué aprende el usuario?** La transparencia del estado del sistema (quién está, qué cambió, cuándo) genera confianza en herramientas colaborativas.

---

### 1.8 ChatGPT (Asistente AI)

**¿Por qué es referencia?**
ChatGPT definió el patrón de interacción conversacional con IA. Cada detalle de la UI está optimizado para que la conversación sea el centro.

**Diferenciadores:**

| Patrón | Descripción |
|--------|-------------|
| Streaming word-by-word | La respuesta aparece progresivamente, reduciendo la ansiedad de espera. |
| Markdown rendering | Headers, listas, tablas, código con syntax highlighting dentro de las respuestas. |
| Conversation history | Sidebar con historial de conversaciones persistido y buscable. |
| Message actions | Copiar, regenerar, editar — acciones sobre cada mensaje individual. |
| Model selector | Selector visible del modelo/capacidad, dando control al usuario. |

**¿Qué aprende el usuario?** El streaming convierte una espera en una experiencia. El historial convierte una sesión en una herramienta de trabajo.

---

### 1.9 Raycast (Launcher de productividad)

**¿Por qué es referencia?**
Raycast es la demostración definitiva de que velocidad = UX. Todo en <50ms. Si algo tarda, hay progress indicator inmediato.

**Diferenciadores:**

| Patrón | Descripción |
|--------|-------------|
| Speed obsession | Resultados en <50ms. Si algo tarda más de 100ms, se muestra indicador de progreso. |
| Extensible commands | El usuario puede agregar sus propios workflows y scripts. |
| Visual consistency | Cada item: icono + título + metadata + acción. Siempre la misma estructura. |
| Fuzzy search | Búsqueda tolerante a errores de tipeo. "trbunal" encuentra "tribunal". |
| Recent/frequent first | Los items más usados y recientes aparecen primero. |

**¿Qué aprende el usuario?** La velocidad percibida es tan importante como la velocidad real. Mostrar resultados frecuentes primero ahorra tiempo.

---

### 1.10 Superhuman (Email client)

**¿Por qué es referencia?**
Superhuman demostró que se puede cobrar premium por UX. La velocidad y los shortcuts no son features — son la propuesta de valor.

**Diferenciadores:**

| Patrón | Descripción |
|--------|-------------|
| Shortcut hints visibles | Cada acción muestra su atajo de teclado inline para entrenar al usuario. |
| Onboarding guiado | Tutorial contextual que enseña los shortcuts mientras usás la app. |
| Status bar | Barra inferior con información contextual y acciones rápidas. |
| Split inbox | Categorización automática e inteligente del contenido. |
| Speed indicators | Indicadores visuales de qué tan rápido fue una acción (ej. "sent in 0.3s"). |

**¿Qué aprende el usuario?** Los shortcuts inline son la mejor forma de training progresivo. El usuario se vuelve power user sin leer documentación.

---

## 2. Patrones transversales identificados

Del análisis de las 10 apps emergen **7 patrones transversales** que aparecen en 3+ productos:

```
┌─────────────────────────────────────────────────────────────────┐
│                  PATRONES TRANSVERSALES                         │
├──────────────────────────┬──────────────────────────────────────┤
│ Patrón                   │ Presente en                          │
├──────────────────────────┼──────────────────────────────────────┤
│ Command Palette          │ Linear, Arc, Raycast, Notion, Figma  │
│ Keyboard shortcuts       │ Linear, Superhuman, Raycast, Figma   │
│ Progressive disclosure   │ Stripe, Vercel, Notion, Perplexity   │
│ Streaming / real-time    │ Perplexity, ChatGPT, Vercel, Figma   │
│ Microinteractions        │ Linear, Arc, Vercel, Stripe          │
│ Skeleton / optimistic UI │ Linear, Notion, Stripe, Vercel       │
│ Hover previews           │ Notion, Arc, Figma, Stripe           │
└──────────────────────────┴──────────────────────────────────────┘
```

---

## 3. Aplicación a Legal AI AR

### 3.1 Contexto actual

- **Framework:** Angular 18 + Angular Material (selectivo) + Cytoscape (grafos)
- **Design system:** PwC Design Ganador v3.0 — CSS custom properties, no design-system package
- **Paleta:** Primary `#D04A02`, hover `#EB8C00`, bg `#f3f3f3`, surface `#ffffff`, text `#252525`
- **Tipografía:** Georgia (headings) + Arial (body)
- **Shell:** Header glassmorphism + sidebar fijo + content area + footer
- **Rutas:** ~18 pantallas en 9 áreas (auth, welcome, search, chat, KB dashboard, catálogos, ontología, explorador, admin)

### 3.2 Plan de implementación por prioridad

#### Tier 1 — Alto impacto, bajo/medio esfuerzo

##### T1-A: Command Palette Global (`Ctrl+K`)

**Inspiración:** Linear + Raycast + Arc

**Qué es:** Un overlay modal con input de búsqueda fuzzy que aparece desde cualquier pantalla y permite:
- Navegar a cualquier sección (Búsqueda, Chat, Explorador, Admin...)
- Buscar fallos por texto libre
- Buscar tribunales, personas, keywords
- Acceder a acciones rápidas ("Nueva búsqueda", "Limpiar filtros")

**Estructura visual:**

```
┌────────────────────────────────────────────┐
│  🔍  Buscar en Legal AI AR...        Esc   │
├────────────────────────────────────────────┤
│  RECIENTES                                 │
│    📄 CSJN - Libertad de expresión    →    │
│    📄 Cámara Civil Sala A - Daños     →    │
│                                            │
│  NAVEGACIÓN                                │
│    🏠 Inicio                          B    │
│    🔍 Búsqueda                        S    │
│    💬 Asistente                       A    │
│    📊 Estadísticas                    D    │
│                                            │
│  ACCIONES                                  │
│    ➕ Nueva búsqueda                       │
│    📋 Copiar última cita                   │
└────────────────────────────────────────────┘
```

**Implementación Angular:**
- Componente standalone `CommandPaletteComponent`
- Listener global de `Ctrl+K` / `Cmd+K` en el `ShellComponent`
- Service `CommandPaletteService` que registra commands desde features
- Fuzzy search con [`fuse.js`](https://www.fusejs.io/) (2KB gzipped)
- Animación: fade-in 150ms + scale(0.98→1)

**Integración con PwC:**
- Background: `rgba(0,0,0,0.5)` backdrop
- Surface: `#ffffff` con `border-radius: 12px` y `box-shadow: 0 16px 48px rgba(0,0,0,0.16)`
- Input: sin borde, font-size `1.125rem`, placeholder en `#696969`
- Items activos: `background: rgba(208, 74, 2, 0.06)` + left border `#D04A02`
- Shortcut badges: `background: #f3f3f3`, `border-radius: 4px`, `font-size: 0.6875rem`

---

##### T1-B: Page Transitions + Microinteracciones

**Inspiración:** Linear + Vercel + Arc

**Qué es:** Animaciones sutiles que dan sensación de fluidez y orientan la atención:

1. **Route transition:** Fade-in 200ms del contenido al cambiar de ruta
2. **Card hover:** Translate-y -2px + shadow elevation en hover (150ms)
3. **Button feedback:** Scale 0.97 on press (100ms)
4. **List stagger:** Items aparecen con delay escalonado (50ms entre items)
5. **Collapse/expand:** Height transition suave en filtros y paneles

**CSS base (compatible con PwC tokens):**

```css
/* Route enter */
@keyframes page-enter {
  from { opacity: 0; transform: translateY(8px); }
  to   { opacity: 1; transform: translateY(0); }
}

.content router-outlet + * {
  animation: page-enter 0.2s ease-out;
}

/* Card hover elevation */
.feature-card, .ruling-card {
  transition: transform 0.15s ease, box-shadow 0.15s ease;
}
.feature-card:hover, .ruling-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(0,0,0,0.08);
}

/* Stagger list items */
@keyframes stagger-in {
  from { opacity: 0; transform: translateY(6px); }
  to   { opacity: 1; transform: translateY(0); }
}
```

---

##### T1-C: Skeleton Loaders Universales

**Inspiración:** Linear + Notion + Stripe

**Qué es:** Reemplazar spinners por skeleton placeholders que imitan la forma del contenido que viene.

**Variantes necesarias:**
- `skeleton-card` — para ruling cards en resultados de búsqueda
- `skeleton-table-row` — para tablas de catálogos
- `skeleton-detail` — para la vista de detalle de fallo
- `skeleton-stat` — para KPI cards del dashboard
- `skeleton-chat-message` — para mensajes del chat

**Cada skeleton usa:**
- `background: linear-gradient(90deg, #e8e8e8 25%, #f3f3f3 50%, #e8e8e8 75%)`
- `background-size: 200% 100%`
- `animation: shimmer 1.5s ease-in-out infinite`
- `border-radius` matching del componente real

---

##### T1-D: Status Bar con Métricas Vivas

**Inspiración:** Superhuman + Figma

**Qué es:** Reemplazar el footer estático actual por una status bar que muestra información contextual del sistema:

```
┌─────────────────────────────────────────────────────────────────┐
│ 📊 12,847 fallos │ 342 tribunales │ Última sync: hace 2h │ ⌨ Ctrl+K │
└─────────────────────────────────────────────────────────────────┘
```

**Comportamiento:**
- Polling cada 60s al endpoint de stats (o WebSocket si disponible)
- Indicador de ingesta activa cuando hay jobs corriendo (dot pulsante + "Ingesta en progreso")
- Click en el contador de fallos navega a `/dashboard`
- El hint `Ctrl+K` recuerda la existencia del command palette

---

##### T1-E: Keyboard Shortcuts con Hints Visibles

**Inspiración:** Superhuman + Linear

**Qué es:** Shortcuts globales que aceleran la navegación, con hints visibles en el sidebar:

```
┌──────────────────────────────┐
│  PRINCIPAL                   │
│  🏠 Inicio              G H │
│  🔍 Búsqueda            G S │
│  💬 Asistente            G A │
│                              │
│  CONOCIMIENTO                │
│  🔗 Explorador          G E │
│  📚 Catálogos            G C │
│  📊 Estadísticas         G D │
│                              │
│  GLOBAL                      │
│  ⌨  Command Palette   Ctrl+K │
│  🔍 Focus búsqueda        / │
└──────────────────────────────┘
```

**Esquema de shortcuts:**
- `G` + letra = **Go to** (navegación). `G H` = Home, `G S` = Search, `G A` = Assistant
- `Ctrl+K` = Command Palette
- `/` = Focus en el input de búsqueda de la pantalla actual
- `Esc` = Cerrar modal/panel/overlay

---

#### Tier 2 — Alto impacto, esfuerzo medio-alto

##### T2-A: Chat con Citas estilo Perplexity

**Inspiración:** Perplexity + ChatGPT

**Qué es:** Rediseño del Asistente para mostrar citas verificables inline con panel de fuentes.

**Flujo visual:**

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          ASISTENTE                                     │
├────────────────────────────────────────────┬────────────────────────────┤
│                                            │  FUENTES (3)               │
│  👤 ¿Cuál es el criterio de la CSJN       │                            │
│     sobre libertad de expresión?           │  [1] CSJN, "Campillay"    │
│                                            │      1986-05-15            │
│  🤖 La CSJN ha establecido tres           │      "...la simple         │
│     estándares principales:                │      atribución de la      │
│                                            │      noticia a la fuente   │
│     1. **Doctrina Campillay** [1]:         │      pertinente..."        │
│        La atribución de la noticia a       │      ───────────────────   │
│        la fuente pertinente...             │  [2] CSJN, "Ponzetti de   │
│                                            │      Balbín" 1984-12-11   │
│     2. **Real malicia** [2][3]:            │      "...el derecho a la   │
│        Aplicable cuando...                 │      intimidad prevalece   │
│                                            │      cuando..."            │
│  ──────────────────────────────────────    │      ───────────────────   │
│  📝 Escriba su consulta...                │  [3] CSJN, "Morales Solá" │
│                                            │      2004-11-12            │
└────────────────────────────────────────────┴────────────────────────────┘
```

**Elementos clave:**
- Citas numeradas `[1]` con hover que resalta la fuente en el panel derecho
- Panel de fuentes colapsable (toggle button)
- Cada fuente muestra: tribunal, carátula, fecha, extracto relevante, link a `/fallos/:id`
- Streaming por fases con indicadores visuales:
  - Fase 1: `Buscando en la base de jurisprudencia...` (animated dots)
  - Fase 2: `Analizando 5 fallos relevantes...` (con mini-cards de los fallos)
  - Fase 3: Respuesta streamed con citas

---

##### T2-B: Hover Previews de Entidades

**Inspiración:** Notion + Arc

**Qué es:** Popovers informativos que aparecen al hacer hover sobre links a entidades (fallos, tribunales, personas) en cualquier parte de la app.

**Ejemplo — Hover sobre tribunal:**

```
┌───────────────────────────────────────┐
│  Cámara Nacional de Apelaciones       │
│  en lo Civil, Sala A                  │
│  ─────────────────────────────────    │
│  📍 CABA • Fuero Civil               │
│  📊 1,234 fallos indexados            │
│  📅 Último fallo: 2026-04-15         │
│                                       │
│  Ver detalle →                        │
└───────────────────────────────────────┘
```

**Ejemplo — Hover sobre fallo citado:**

```
┌───────────────────────────────────────┐
│  "Rodríguez c/ García"                │
│  CSJN • 2024-03-12                    │
│  ─────────────────────────────────    │
│  Daños y perjuicios, responsabilidad  │
│  civil, factor de atribución          │
│  ─────────────────────────────────    │
│  "...el factor de atribución          │
│   objetivo no requiere la prueba      │
│   de la culpa del demandado..."       │
│                                       │
│  Abrir fallo →                        │
└───────────────────────────────────────┘
```

**Implementación:**
- Directiva `[entityPreview]` que se aplica a cualquier link
- Delay de 300ms antes de mostrar (evitar flash en mouse pass-through)
- Posicionamiento inteligente (arriba/abajo según espacio disponible)
- Fade-in 150ms
- Datos cargados lazy via servicio existente

---

#### Tier 3 — Impacto medio, esfuerzo medio

##### T3-A: Empty States Diseñados

**Inspiración:** Vercel + Stripe

**Qué es:** Estados vacíos que guían al usuario en lugar de mostrar "No hay resultados".

**Casos:**
- Búsqueda sin resultados → Sugerencias de búsquedas relacionadas + tips de filtros
- Chat sin historial → "Haga su primera consulta" con ejemplos sugeridos
- Dashboard con KB vacía → Progreso de la ingesta + qué esperar
- Catálogos vacíos → Explicación de qué es cada catálogo + CTA a admin

##### T3-B: Onboarding Contextual

**Inspiración:** Superhuman

**Qué es:** Tooltips guiados que aparecen la primera vez que el usuario entra a cada sección. Se persisten en localStorage para no repetirse.

**Flujo:**
1. Primera visita a Welcome → Tooltip sobre el search box + sobre Ctrl+K
2. Primera visita a Búsqueda → Tooltip sobre filtros avanzados
3. Primera visita a Chat → Tooltip sobre citas verificables
4. Primera visita a Explorador → Tooltip sobre zoom/pan + capas

##### T3-C: Búsquedas Recientes Persistidas

**Inspiración:** Raycast + Notion

**Qué es:** Las últimas 10 búsquedas del usuario se guardan en localStorage y aparecen en:
- La pantalla de Búsqueda (debajo de búsquedas sugeridas)
- El Command Palette (sección "Recientes")
- El Welcome (si el usuario ya buscó antes, reemplaza o complementa las feature cards)

---

## 4. Resumen ejecutivo

### Matriz impacto / esfuerzo

```
        IMPACTO
        ▲
  ALTO  │  T1-A Command Palette ●    T2-A Chat Perplexity ●
        │  T1-C Skeletons ●          T2-B Hover Previews ●
        │  T1-D Status Bar ●
        │  T1-B Transitions ●
  MEDIO │  T1-E Shortcuts ●          T3-A Empty States ●
        │                            T3-B Onboarding ●
        │                            T3-C Recientes ●
  BAJO  │
        └──────────────────────────────────────────────→
          BAJO              MEDIO              ALTO
                          ESFUERZO
```

### Orden de implementación recomendado

| Orden | ID | Feature | Esfuerzo est. | Dependencias |
|-------|----|---------|---------------|--------------|
| 1 | T1-B | Page transitions + microinteracciones | 2-3h | Ninguna |
| 2 | T1-C | Skeleton loaders universales | 3-4h | Ninguna |
| 3 | T1-D | Status bar con métricas vivas | 2-3h | Endpoint de stats |
| 4 | T1-E | Keyboard shortcuts + hints | 3-4h | Ninguna |
| 5 | T1-A | Command Palette (`Ctrl+K`) | 6-8h | `fuse.js` dependency |
| 6 | T2-B | Hover previews de entidades | 6-8h | Services existentes |
| 7 | T2-A | Chat citas estilo Perplexity | 8-12h | Refactor chat view |
| 8 | T3-C | Búsquedas recientes persistidas | 2-3h | localStorage |
| 9 | T3-A | Empty states diseñados | 3-4h | Ninguna |
| 10 | T3-B | Onboarding contextual | 4-6h | localStorage |

### Compatibilidad con PwC Design System

Todas las implementaciones propuestas respetan:

- **Colores:** Primary `#D04A02`, hover `#EB8C00`, backgrounds `#f3f3f3` / `#ffffff`
- **Tipografía:** Georgia para headings, Arial para body/UI
- **Radii:** `0.25rem` para componentes funcionales, `100px` para badges/pills
- **Shadows:** `0 2px 8px rgba(0,0,0,0.08)` base, elevación proporcional
- **Focus states:** `outline: 2px solid #D04A02; outline-offset: 2px` (WCAG AA)
- **Glassmorphism:** Header con `backdrop-filter: blur(12px)` mantiene consistencia

### Principios de diseño aplicados

1. **Velocidad percibida** — Skeletons + optimistic UI + transitions hacen que todo se sienta instantáneo
2. **Confianza verificable** — Citas numeradas con fuentes expandibles (Perplexity pattern)
3. **Progressive mastery** — Shortcuts + command palette + onboarding que entrenan al usuario progresivamente
4. **Información en contexto** — Hover previews + status bar reducen la necesidad de navegar
5. **Calm design** — La tipografía Georgia + Arial con la paleta PwC ya da un tono editorial serio; las transiciones suaves refuerzan la calma profesional
