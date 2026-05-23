# Endpoint: getAllDocumentos

**URL**: `{baseUrl}/documentos/getAllDocumentos.html?idAnalisis={analysisId}`
**Cache key**: `_cache/csjn/api/getAllDocumentos/{analysisId}.json`
**Tamano**: 53-300+ KB (dominado por el campo `doc` base64)
**Formato**: JSON array (root-level)
**Poblacion**: 1,811/1,811 (100%)

## Descripcion

Contiene los documentos asociados al analisis en formato binario (PDF/Word codificado en base64). Cada item del array tiene metadata basica + el blob del documento + una copia embebida del `analisisDocumental` (equivalente a `abrirAnalisis`). El tipo de documento puede ser FALLO, DICTAMEN MPF, RESENA, u otros.

## Estructura

El root es un **array**. Cada elemento tiene:

### Campos principales por item

| Campo | Tipo | Ejemplo | Parser | Status |
|-------|------|---------|:---:|--------|
| `codigo` | number | `8091091` | Si (validacion documentId) | CORRECTO |
| `titulo` | string | `"FALLO CSJ 000555/2024(60-G)/CS001"` | No (solo fallback) | El titulo sigue el patron `"{tipo} {expediente}"` |
| `doc` | string (base64) | (43K+ chars) | No | Blob del documento. Se descarga por separado via PDF endpoint. |
| `fecha` | number (epoch ms) | `1747796400000` | No | **Fecha del documento** — para `tipoDocumento=FALLO` es la fecha del fallo |
| `fechaLong` | number | `20250521` | No | Misma fecha en formato yyyyMMdd |
| `fechaString` | string | `"21/05/2025"` | No | Misma fecha legible |
| `anioFallo` | number | `2025` | No | Ano del fallo |
| `tipoDocumento.valor` | string | `"FALLO"` / `"DICTAMEN MPF"` | No | Tipo de documento |
| `publico` | string | `"S"` | No | Flag de acceso publico |
| `digital` | null | `null` | No | |
| `tomo` | number/null | `335` | No | **Tomo en coleccion Fallos** |
| `pagina` | number/null | `252` | No | **Pagina en coleccion Fallos** |
| `citas[]` | array | `[]` | No | Citas (generalmente vacio en este endpoint) |
| `referencias[]` | array | `[]` | No | Referencias (generalmente vacio) |
| `analisisDocumental` | object | (copia de abrirAnalisis) | No | Redundante con abrirAnalisis |
| `falloDestacado` | null/string | `null` | No | Flag de fallo destacado |
| `numeroID` | number | `904` | No | ID interno |

## Uso actual por el parser

El parser solo usa este endpoint para **validar que el documento existe** (`ValidateDocumentExists` busca `codigo` en el array). No extrae otros campos.

## Campos no extraidos con valor

| Campo | Valor KB | Nota |
|-------|----------|------|
| `fecha`/`fechaString` (item FALLO) | **Fecha del fallo** — fuente alternativa para RulingDate | Solo para items con `tipoDocumento.valor = "FALLO"` |
| `tomo`/`pagina` | **Referencia oficial** "Fallos: tomo:pagina" | Cita formal para coleccion de Fallos de la CSJN. No siempre poblado. |
| `titulo` | Contiene tipo + expediente | Patron: `"{FALLO|DICTAMEN MPF} {identificacionExpediente}"` — fuente alternativa para expediente |
| `tipoDocumento.valor` | Indica que tipos de documentos hay | Un analisis puede tener FALLO + DICTAMEN MPF + RESENA |

## Observaciones

- El campo `doc` es el grueso del tamano del archivo. Contiene el PDF/Word codificado en base64. No es util para extraccion de metadata (el texto se extrae del PDF descargado por separado).
- El `analisisDocumental` embebido es una copia completa del `abrirAnalisis`. Es redundante y no debe usarse (usar `abrirAnalisis` directamente).
- El campo `fecha` del item FALLO es la **fecha real del fallo**, a diferencia de `reciboEntrada.fecha` en `abrirAnalisis` que es la fecha de recibo.
- Un analisis puede tener multiples documentos: FALLO principal, DICTAMEN MPF, RESENA, etc.
- `tomo`/`pagina` cuando estan poblados proveen la cita canonica (ej: "Fallos: 348:287").

## Resumen de acciones

| Prioridad | Accion | Impacto |
|-----------|--------|---------|
| **MEDIA** | Extraer `tomo`/`pagina` del item FALLO | Referencia oficial para cita formal |
| **BAJA** | Usar `fecha` del item FALLO como fallback para RulingDate | Fuente alternativa cuando no hay hint ni falloDestacado |
| **BAJA** | Contar tipos de documentos | Saber si hay dictamen/resena asociado |
