# Endpoint: getSintesisAnalisis (NUEVO — no usado por parser)

**URL**: `{baseUrl}/fallos/getSintesisAnalisis.html?idAnalisis={analysisId}`
**Cache key**: `_cache/csjn/api/getSintesisAnalisis/{analysisId}.json`
**Tamano**: 2 bytes (vacio) a 100+ KB
**Formato**: JSON array `[]` cuando vacio, JSON array de objetos cuando tiene datos
**Poblacion**: 1,414/1,811 (78%) tienen datos — 397 vacios

## Descripcion

Contiene documentos de tipo "sintesis" o "resena" asociados al analisis. Es la contraparte de `getDictamenesAnalisis` pero para documentos de tipo RESENA (no dictamen). Cada item tiene la misma estructura de documento que getAllDocumentos y getDictamenesAnalisis: metadata basica + copia del `analisisDocumental` + opcionalmente el blob del documento.

## Estructura (cuando hay datos)

Cada item del array:

| Campo | Tipo | Ejemplo | Valor KB |
|-------|------|---------|----------|
| `codigo` | number | `7189645` | ID del documento resena |
| `titulo` | string | `"RESENA A. 721. XLIII. RHE"` | Titulo con tipo + expediente |
| `tipoDocumento.valor` | string | `"RESENA"` | Tipo — siempre RESENA en muestras |
| `fecha` | number (epoch ms) | `1331607600000` | Fecha del documento |
| `fechaString` | string | `"13/03/2012"` | Fecha legible |
| `fechaLong` | number | `20120313` | Fecha yyyyMMdd |
| `anioFallo` | number | `2012` | Ano |
| `publico` | string | `"S"` | Acceso publico |
| `digital` | null | `null` | |
| `doc` | string/null | (base64 o null) | Blob del documento resena |
| `citas[]` | array | `[]` | Generalmente vacio |
| `referencias[]` | array | `[]` | Generalmente vacio |
| `analisisDocumental` | object | (copia de abrirAnalisis) | Redundante |
| `tomo` | number/null | null | Tomo |
| `pagina` | number/null | null | Pagina |
| `numeroID` | number | Varia | ID interno |

## Diferencia con getDictamenesAnalisis

| Aspecto | getDictamenesAnalisis | getSintesisAnalisis |
|---------|----------------------|---------------------|
| Tipo de documento | `DICTAMEN MPF` | `RESENA` |
| Poblacion | 1,097 (61%) | 1,414 (78%) |
| Contenido | Metadata del dictamen fiscal | Metadata del documento de resena/sintesis |
| Estructura | Identica | Identica |

## Valor para la KB

### Presencia como indicador

La presencia de un documento RESENA indica que el fallo tiene un documento de sintesis asociado (generalmente elaborado por la Secretaria de Jurisprudencia). Esto es distinto del sumario (getSumariosAnalisis) que es un headnote textual; la RESENA es un documento completo.

### Comparacion con getSumariosAnalisis

| Aspecto | getSumariosAnalisis | getSintesisAnalisis |
|---------|---------------------|---------------------|
| Contenido | Headnote textual (`texto`) | Documento completo (blob) |
| Poblacion | 98% | 78% |
| Formato | JSON con texto | JSON con blob base64 |
| Valor directo | Alto — texto listo para usar | Medio — requiere extraccion de texto |

### Evaluacion pragmatica

Dado que `getSumariosAnalisis` ya provee el texto doctrinal con 98% de poblacion, y que `getSintesisAnalisis` requiere extraer texto de un blob, el valor incremental de este endpoint es **limitado para la KB actual**. Su principal utilidad seria:

1. **Confirmar tipos de documentos asociados** al fallo
2. **Fuente adicional de texto** si se procesa el blob en una fase futura
3. **Metadata de completitud**: saber que un fallo tiene FALLO + DICTAMEN + RESENA

## Resumen de acciones

| Prioridad | Accion | Impacto |
|-----------|--------|---------|
| **BAJA** | Agregar endpoint al parser | 78% de fallos tienen datos pero valor incremental limitado |
| **BAJA** | Registrar presencia de RESENA | Metadata de completitud documental |
| **FUTURA** | Procesar blob de RESENA | Extraccion de texto adicional (scope futuro) |
