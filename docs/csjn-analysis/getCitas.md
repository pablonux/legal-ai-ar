# Endpoint: getCitas

**URL**: `{baseUrl}/documentos/getCitas.html?idDocumento={documentId}`
**Cache key**: `_cache/csjn/api/getCitas/{documentId}.json`
**Tamano**: 200 bytes a 10+ KB
**Formato**: JSON array de objetos
**Poblacion**: 1,811/1,811 (100%) — **todos tienen datos**

## Descripcion

Citas salientes del fallo: otros fallos, sumarios y jurisprudencia citados por este documento. Cada item del array es una referencia estructurada con alias, link al sumario o fallo, y texto de cita canonica.

**CORRECCION IMPORTANTE**: El analisis previo (basado en 5 muestras antiguas) indicaba "getCitas siempre retorna cadena vacia `""`". Con 1,811 muestras de fallos destacados, **el 100% contiene datos** como array JSON. Es posible que las 5 muestras originales fueran de fallos sin citas o de una version anterior de la API.

## Estructura

Cada item del array:

| Campo | Tipo | Ejemplo | Parser | Status |
|-------|------|---------|:---:|--------|
| `id` | number | `47929` | No | ID interno de la cita |
| `alias` | string | `"Fallos: 323:1625"` o `"\"Halabi\" #Fallos: 332:111"` | Si → `Citations.Alias` | CORRECTO |
| `orden` | number | `1` | No | Orden de la cita |
| `idSumario` | number/null | `38940` | Si → `Citations.SummaryId` | CORRECTO |
| `idFallo` | number/null | `7034831` | **No** | **FALTA** — ID directo al fallo citado (para linking en KB) |
| `link` | string | `"/consultaSumarios/buscarSumariosFallo.html?idSumario=38940"` | No | Link relativo al sumario |
| `textoCita` | string | `"323:1625"` o `""` | **No** | **FALTA** — referencia canonica "tomo:pagina" |
| `replacementLink` | string | `"https://sjconsulta.csjn.gov.ar/..."` | **No** | **FALTA** — URL completa para navegacion |
| `tipoLink` | null | `null` | No | No poblado en muestras |
| `found` | null | `null` | No | No poblado en muestras |
| `usuarioCreacion` | string | `"JURC5F"` | No | Audit |
| `usuarioModificacion` | string | `"JURC5F"` | No | Audit |
| `fechaCreacion` | number | `202011241627` | No | Audit |
| `fechaModificacion` | number | `202011241627` | No | Audit |

## Patrones de alias observados

| Patron | Ejemplo | Significado |
|--------|---------|-------------|
| `Fallos: tomo:pagina` | `"Fallos: 323:1625"` | Referencia a coleccion oficial de Fallos CSJN |
| `"Nombre" #Fallos: tomo:pagina` | `"\"Halabi\" #Fallos: 332:111"` | Fallo con nombre propio + referencia |
| `"Nombre" #CSJ expediente` | `"\"Padec\" #CSJ 361/2007"` | Fallo con nombre + expediente moderno |
| `"Nombre" #CSJN expediente` | Similar | Variante |

## Logica idSumario vs idFallo

Cada cita puede tener `idSumario`, `idFallo`, o ambos:
- `idSumario != null`: la cita refiere a un sumario especifico. El `link` apunta a `buscarSumariosFallo.html`.
- `idFallo != null`: la cita refiere directamente a un fallo. El `link` apunta a `verDocumentoByIdLinksJSP.html`.
- Ambos: la cita tiene tanto sumario como fallo asociado.

El parser actual solo extrae `idSumario`. No extrae `idFallo`, lo que impide vincular citas directamente con fallos ya indexados en la KB.

## Uso actual por el parser

```csharp
// ParseCitations: busca alias + idSumario
var alias = GetString(item, "alias", "cita", "referencia", "referenciaNormativa");
var summaryId = GetIntNullable(item, "idSumario", "summaryId");
```

Funciona correctamente para la estructura. Las mejoras posibles son:

1. Extraer `idFallo` para linking directo
2. Extraer `textoCita` para la referencia canonica
3. Extraer `replacementLink` para navegacion

## Resumen de acciones

| Prioridad | Accion | Campo | Impacto |
|-----------|--------|-------|---------|
| **ALTA** | Extraer `idFallo` | `idFallo` | Permite vincular citas con fallos indexados en la KB |
| **ALTA** | Extraer `textoCita` | `textoCita` | Referencia canonica tomo:pagina |
| **MEDIA** | Extraer `replacementLink` | `replacementLink` | URL de navegacion directa |
| **BAJA** | Extraer `orden` | `orden` | Mantener orden original de citas |
