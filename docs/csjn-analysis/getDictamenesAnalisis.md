# Endpoint: getDictamenesAnalisis (NUEVO — no usado por parser)

**URL**: `{baseUrl}/fallos/getDictamenesAnalisis.html?idAnalisis={analysisId}`
**Cache key**: `_cache/csjn/api/getDictamenesAnalisis/{analysisId}.json`
**Tamano**: 2 bytes (vacio) a 100+ KB
**Formato**: JSON array `[]` cuando vacio, JSON array de objetos cuando tiene datos
**Poblacion**: 1,097/1,811 (61%) tienen datos — 714 vacios

## Descripcion

Contiene metadata de los dictamenes del Ministerio Publico Fiscal (MPF) asociados al analisis. Cuando un fallo tiene dictamen del Procurador General o del fiscal interviniente, este endpoint devuelve la metadata completa del dictamen, incluyendo tipo de documento, fecha, titulo, y una copia del `analisisDocumental`.

No contiene el texto del dictamen (para eso se necesita el PDF via `getAllDocumentos` o el enlace externo via `getEnlacesAnalisis`).

## Estructura (cuando hay datos)

Cada item del array:

| Campo | Tipo | Ejemplo | Valor KB |
|-------|------|---------|----------|
| `codigo` | number | `7189644` | ID del documento dictamen |
| `titulo` | string | `"DICTAMEN MPF A. 721. XLIII. RHE"` | Titulo del dictamen con expediente |
| `tipoDocumento.valor` | string | `"DICTAMEN MPF"` | Tipo — confirma que es dictamen |
| `fecha` | number (epoch ms) | `1331607600000` | Fecha del dictamen |
| `fechaString` | string | `"13/03/2012"` | Fecha legible |
| `fechaLong` | number | `20120313` | Fecha yyyyMMdd |
| `anioFallo` | number | `2012` | Ano |
| `publico` | string | `"S"` | Acceso publico |
| `digital` | null | `null` | |
| `doc` | string/null | (base64 o null) | Blob del dictamen (si disponible) |
| `citas[]` | array | `[]` | Generalmente vacio |
| `referencias[]` | array | `[]` | Generalmente vacio |
| `analisisDocumental` | object | (copia de abrirAnalisis) | Redundante — misma estructura que abrirAnalisis |
| `tomo` | number/null | null | Tomo (raramente poblado para dictamenes) |
| `pagina` | number/null | null | Pagina |

## Multiples dictamenes

Un fallo puede tener mas de un dictamen. Ejemplo: `719582.json` contiene 2 items en el array (dos dictamenes MPF del mismo caso). Cada uno tiene su propio `codigo`, `titulo`, y `fecha`.

## Valor para la KB

### 1. Deteccion de dictamen sin LLM

Actualmente, el pipeline usa una llamada LLM opcional (`prosecutor_opinion` en `missingFields`) para detectar y extraer informacion del dictamen desde el texto del PDF. Con este endpoint:

- **Presencia**: Si `getDictamenesAnalisis` retorna datos, hay dictamen. Si retorna `[]`, no hay.
- **Fecha**: Se obtiene la fecha del dictamen directamente.
- **Titulo**: Identifica el tipo y expediente del dictamen.

Esto permite **gatear la llamada LLM**: solo invocar `ProsecutorOpinionPrompt` cuando sabemos que hay dictamen, ahorrando ~39% de llamadas LLM innecesarias.

### 2. Cross-reference con getEnlacesAnalisis

Cuando `getDictamenesAnalisis` indica que hay dictamen, `getEnlacesAnalisis` puede proveer el URL al PDF del dictamen en mpf.gob.ar. Combinados, permiten:

1. Detectar existencia (getDictamenesAnalisis)
2. Obtener URL del PDF (getEnlacesAnalisis)
3. Descargar y procesar el texto del dictamen (futuro)

### 3. Flag `tieneDictamenes` en getSumariosAnalisis

Alternativamente, el flag `tieneDictamenes` en `getSumariosAnalisis` (98% poblado) tambien indica la presencia de dictamen. Pero `getDictamenesAnalisis` provee metadata adicional (fecha, titulo, codigo).

## Resumen de acciones

| Prioridad | Accion | Impacto |
|-----------|--------|---------|
| **ALTA** | Agregar endpoint al parser | Detectar dictamenes desde API |
| **ALTA** | Usar presencia para gatear LLM prosecutor_opinion | ~39% menos llamadas LLM |
| **MEDIA** | Extraer fecha y titulo del dictamen | Metadata del dictamen en KB |
| **BAJA** | Cross-reference con getEnlacesAnalisis para URL | Acceso al PDF del dictamen |
