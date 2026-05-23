# Exportar la ontología a Microsoft Visio

Diagramas actualizados de la ontología **v2.1** (2026-05-21), alineados con:

- `docs/ontology/legal-ai-ar-ontology.md`
- `backend/src/api/LegalAiAr.Application/Ontology/OntologyModelProvider.cs`

## Archivos generados

| Archivo | Descripción |
|---|---|
| `legal-ai-ar-ontology-v2.1.visio.vdx` | **Recomendado.** Abrir directamente en Visio (formato XML nativo). |
| `legal-ai-ar-ontology-v2.1.svg` | Jerarquía de clases; importar con **Archivo → Abrir** en Visio. |
| `legal-ai-ar-ontology-v2.1-class-hierarchy.mermaid` | Jerarquía is-a completa (fuente editable). |
| `legal-ai-ar-ontology-v2.1-relationships.mermaid` | Grafo de relaciones de dominio. |
| `legal-ai-ar-ontology-v2.1-implementation-status.mermaid` | Mapeo KB / UI / estado de implementación. |

## Regenerar exportación Visio

```powershell
python docs/ontology/diagrams/generate_visio_export.py
```

El VDX incluye **3 páginas**:

1. **Jerarquía de clases** — relaciones `is-a` y roles (Juez, Fiscal, etc.)
2. **Relaciones de dominio** — `citaNorma`, `conduceA`, `participaEn`, etc.
3. **Estado de implementación** — mapeo a entidades KB y rutas UI

## Importar en Microsoft Visio

### Opción A — VDX (mejor calidad editable)

1. Cerrar el `.vsdx` anterior si lo tiene abierto.
2. Regenerar: `python docs/ontology/diagrams/generate_visio_export.py`
3. Abrir Visio → **Archivo → Abrir** → `legal-ai-ar-ontology-v2.1.visio.vdx` (no el vsdx viejo).
4. La página **1-Jerarquia-Clases** es apaisada (17×11 in): 7 columnas, una por rama de `owl:Thing`.
5. Guardar como `.vsdx`: **Archivo → Guardar como → Visio (*.vsdx)**.

Si las formas aparecen superpuestas, está usando un export anterior. Regenere el VDX y vuelva a abrirlo.

### Opción B — SVG

1. **Archivo → Abrir** → `legal-ai-ar-ontology-v2.1.svg`.
2. Visio convierte el SVG a formas editables (puede requerir desagrupar).
3. Ajustar conectores y estilos según la guía corporativa.

### Opción C — Mermaid → PNG/SVG (diagramas más ricos)

Si tiene [Mermaid CLI](https://github.com/mermaid-js/mermaid-cli):

```powershell
npx -y @mermaid-js/mermaid-cli -i docs/ontology/diagrams/legal-ai-ar-ontology-v2.1-class-hierarchy.mermaid -o docs/ontology/diagrams/out/class-hierarchy.svg
npx -y @mermaid-js/mermaid-cli -i docs/ontology/diagrams/legal-ai-ar-ontology-v2.1-relationships.mermaid -o docs/ontology/diagrams/out/relationships.svg
```

Luego abrir los SVG en Visio.

### Opción D — draw.io → VSDX

1. Pegar el contenido de un archivo `.mermaid` en [diagrams.net](https://app.diagrams.net) (soporte Mermaid limitado) o recrear el diagrama.
2. **Archivo → Exportar como → VSDX**.
3. Abrir el `.vsdx` en Visio.

## Leyenda de colores (diagramas)

| Color | Significado |
|---|---|
| Azul oscuro `#1E3A5F` | Clase core del dominio |
| Naranja `#D04A02` | Entidad implementada en KB |
| Azul medio `#4A7CB5` | Subclase |
| Naranja claro `#EB8C00` | Rol (no subclase is-a) |
| Gris `#9E9E9E` | Conceptual / no implementada |

## Notas

- Los roles **Juez, Fiscal, Defensor, Abogado** se modelan con aristas punteadas (`rol`), no como subclases disjuntas de `PersonaHumana`, según la doctrina del documento ontológico.
- `Sentencia`, `ThesaurusTerm`, `PalabraClave` y `Fuente` son entidades KB sin padre `owl:Thing` en el viewer, pero aparecen en el grafo de relaciones.
