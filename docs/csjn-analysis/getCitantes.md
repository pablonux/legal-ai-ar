# Endpoint: getCitantes

**URL**: `{baseUrl}/documentos/getCitantes.html?idDocumento={documentId}`
**Cache key**: `_cache/csjn/api/getCitantes/{documentId}.json`
**Tamano**: 2 bytes (vacio) a 34+ KB
**Formato**: JSON string con contenido HTML (no JSON puro)
**Poblacion**: 1,104/1,811 (61%) tienen datos — 707 vacios (`""`)

## Descripcion

Citas entrantes: fallos posteriores que citan al fallo actual. El endpoint retorna **HTML embebido en un JSON string** — no un array JSON estructurado. Los links estan organizados por ano en paneles Bootstrap.

## Estructura

### Cuando vacio

```json
""
```

### Cuando tiene datos

El body es un **string JSON** cuyo contenido es HTML:

```html
<div id='acordeonReferenciantesAnio2026' class='panel-group'>
  <div class='panel panel-info'>
    <h4 class='panel-title'>2026 <span class='badge'>1</span></h4>
    <div class='panel-body'>
      <a href='/sjconsulta/documentos/getDocumentosExterno.html?idAnalisis=824307'>
        CIV 021175/2022/CS001
      </a>
    </div>
  </div>
</div>
<div id='acordeonReferenciantesAnio2025' class='panel-group'>
  <div class='panel panel-info'>
    <h4 class='panel-title'>2025 <span class='badge'>3</span></h4>
    <div class='panel-body'>
      <a href='...?idAnalisis=807965'>Fallos: 348:287</a>
      <a href='...?idAnalisis=807508'>CCC 006731/2021/TO01/5/1/RH001</a>
    </div>
  </div>
</div>
```

## Datos extraibles del HTML

| Dato | Ubicacion en HTML | Ejemplo |
|------|-------------------|---------|
| AnalysisId del citante | `href` param `idAnalisis` | `824307` |
| Identificador expediente | Texto del link `<a>` | `"CIV 021175/2022/CS001"` |
| Referencia Fallos | Texto del link `<a>` | `"Fallos: 348:287"` |
| Ano del citante | Header del panel `<h4>` | `2026` |
| Cantidad por ano | Badge `<span>` | `1` |

## BUG en el parser actual

**El parser NO extrae datos de este endpoint.**

`ParseCitedBy` espera un JSON array u objeto con propiedades `idAnalisis` + `numeroCausa`:

```csharp
// ParseCitedBy busca:
if (el.ValueKind == JsonValueKind.Array) { /* procesa items */ }
if (el.ValueKind == JsonValueKind.Object) { /* busca Records/citantes */ }
```

Pero la API devuelve `JsonValueKind.String` (el HTML). Al no entrar en ningun branch, **siempre retorna una lista vacia**. Los 1,104 fallos con citantes (61%) no se procesan.

Adicionalmente, `CsjnCitedByDto` requiere `analysisId` + `caseNumber`, pero el HTML contiene `idAnalisis` + texto libre del link (que es el expediente, no siempre disponible en el formato esperado).

## Solucion propuesta

Para extraer citantes, se necesita parsear el HTML:

1. Regex o parser HTML para extraer `idAnalisis` de los `href`
2. Texto del `<a>` como identificador del citante (expediente o referencia "Fallos: tomo:pagina")
3. Ano del panel como metadata adicional

Alternativa: evaluar si existe un endpoint alternativo que retorne JSON estructurado para citantes.

## Resumen de acciones

| Prioridad | Accion | Impacto |
|-----------|--------|---------|
| **ALTA** | Implementar HTML parser para getCitantes | Habilitar extraccion de 1,104 (61%) fallos con citantes |
| **ALTA** | Extraer `idAnalisis` de links | Grafo de citas entrantes |
| **MEDIA** | Extraer texto del link como CaseNumber del citante | Identificacion del citante |
| **MEDIA** | Extraer ano del panel | Metadata temporal de la cita |
