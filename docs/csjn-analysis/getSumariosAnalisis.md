# Endpoint: getSumariosAnalisis

**URL**: `{baseUrl}/sumarios/getSumariosAnalisis.html?idAnalisis={analysisId}`
**Cache key**: `_cache/csjn/api/getSumariosAnalisis/{analysisId}.json`
**Tamano**: 2 bytes (vacio) a 1.4 MB
**Formato**: JSON array `[]` cuando vacio, JSON array de objetos cuando tiene datos
**Poblacion**: 1,774/1,811 (98%) tienen datos — solo 37 vacios

## Descripcion

Sumarios juridicos elaborados por la Secretaria de Jurisprudencia de la CSJN. Cada sumario contiene un headnote doctrinal (`texto`), voces tematicas, referencia tomo:pagina, y metadata completa. Es la fuente de texto juridico de mayor calidad disponible en la API.

**NOTA**: El analisis previo basado en 5 muestras indicaba "~90% vacios". Con 1,811 muestras de fallos destacados, el resultado real es **98% con datos**. La poblacion alta se debe a que los fallos destacados tienen prioridad de sumario.

## Estructura (cuando hay datos)

### Campos principales por sumario

| Campo | Tipo | Ejemplo | Parser | Status |
|-------|------|---------|:---:|--------|
| `id` | number | `171613` | No | ID interno del sumario |
| `orden` | number | `1` | No | Orden del sumario dentro del fallo |
| `texto` | string | `"La Corte Suprema dejo sin efecto la sentencia..."` (859+ chars) | **No** | **FALTA** — **campo mas valioso: headnote doctrinal** |
| `fullText` | string | `"<VOCES>...</VOCES><TEXTO>...</TEXTO>"` | No | Texto con XML tags (voces + texto combinados) |
| `voces` | string | `"MINISTERIO PUPILAR - ACCIDENTES DEL TRABAJO..."` | **No** | **FALTA** — headings tematicos concatenados con " - " |
| `autos` | string | `"VILLEGAS MARCELA ALEJANDRA c/ PREFECTURA..."` | No | Caratula (redundante con abrirAnalisis) |
| `caratulaWeb` | string | Igual a `autos` | No | Variante web |
| `caratulaLex` | string | Igual a `autos` | No | Variante Lex |
| `numeroExpediente` | string | `"V. 54. XLV. RHE"` | No | Otra fuente de CaseNumber |
| `fechaFallo` | number (epoch ms) | `1747796400000` | No | **Fecha real del fallo** (coincide con `falloDestacado.fecha`) |
| `fechaString` | string | `"21/05/2025"` | No | Fecha del fallo legible |
| `tomo` | number | `335` | **No** | **FALTA** — tomo en coleccion Fallos |
| `pagina` | number | `252` | **No** | **FALTA** — pagina en coleccion Fallos |
| `anioFallo` | number | `2025` | No | Ano del fallo |
| `idFallo` | number | `8091091` | No | documentId del fallo (cross-reference) |
| `holding` | number | `1` | Si (BUG) | **Es un FLAG NUMERICO, no texto** |

### Voces Sumario (array estructurado)

```json
"vocesSumario": [
  {
    "tipoVozSumario": {
      "codigoValor": 8756,
      "valor": "RECURSO EXTRAORDINARIO"
    },
    "tipoVoz": {
      "codigoValor": 8756,
      "valor": "RECURSO EXTRAORDINARIO"
    }
  }
]
```

| Poblacion | Parser | Valor KB |
|-----------|:---:|----------|
| 98% | **No** | Keywords estructuradas con codigos — complementa voces de abrirAnalisis |

### Flags

| Campo | Tipo | Ejemplo | Valor KB |
|-------|------|---------|----------|
| `sentenciaArbitraria` | bool | `false` | Marca sentencia arbitraria |
| `inconstitucional` | bool | `false` | Flag redundante con abrirAnalisis |
| `tieneDictamenes` | bool | `true` | **Indica que existe dictamen** — sin necesidad de llamar getDictamenesAnalisis |
| `conAnalisis` | bool | `true` | Tiene analisis documental |
| `conAnalisisPrivado` | bool | `false` | Analisis privado |
| `conVotos` | bool | `false` | Tiene votos en sumario |
| `tieneDocumento` | bool | `true` | Tiene documento asociado |
| `desvinculado` | bool | `false` | Flag de desvinculacion |

### Otros campos

| Campo | Tipo | Contenido | Poblacion |
|-------|------|-----------|-----------|
| `notas[]` | array | Notas adicionales | Raro |
| `suplementos[]` | array | Suplementos | Raro |
| `referencias[]` | array | Referencias normativas | Variable |
| `citasRex[]` | array | Citas REX | Raro |
| `stringVotosAnalisis` | string | Votos string | Variable |
| `stringVotosMayoria` | string | Mayoria string | Variable |
| `stringVotosDisidencia` | string | Disidencia string | Variable |

## Uso actual por el parser

El parser busca:
1. **Holding**: campos `considerando`, `holding`, `sintesis` → `GetString` y `GetStringFromFirstRecord`
2. **Summary** (fallback): campos `sintesis`, `summary`, `resumen`
3. **Keywords** (fallback): `ParseKeywords` si voces de abrirAnalisis estan vacias

### BUG: campo `holding`

El parser busca `holding` como campo de texto, pero en la API es un **flag numerico** (valor `1`). `GetStringFromElement` lo convierte a string `"1"`. Esto puede resultar en `Holding = "1"` en la KB.

### BUG: campo `texto` no buscado

El campo con el texto doctrinal real es **`texto`**, no `sintesis` ni `considerando`. El parser no incluye `texto` en su lista de fallbacks. Resultado: el Holding real no se extrae nunca de este endpoint.

## Ejemplo de `texto` (lo que deberia ser el Holding)

```
"La Corte Suprema dejo sin efecto la sentencia que habia rechazado el recurso
de amparo interpuesto contra la resolucion de la Direccion Nacional de
Migraciones que denego la solicitud de radicacion permanente. Senalo que el
tribunal a quo omitio considerar que el art. 22 de la ley 25.871 establece..."
```

## Resumen de acciones

| Prioridad | Accion | Campo | Impacto |
|-----------|--------|-------|---------|
| **CRITICA** | Extraer `texto` como Holding | `texto` | Headnote doctrinal de calidad profesional |
| **CRITICA** | Corregir bug de `holding` numerico | `holding` | Evitar `Holding = "1"` en KB |
| **ALTA** | Extraer `tomo`/`pagina` | `tomo`, `pagina` | Referencia oficial "Fallos: tomo:pagina" |
| **ALTA** | Extraer `voces` concatenado | `voces` | Keywords tematicos adicionales |
| **ALTA** | Extraer `fechaFallo` | `fechaFallo` | Fuente confiable de fecha del fallo |
| **MEDIA** | Extraer `vocesSumario[]` | `vocesSumario[]` | Keywords estructuradas con codigos |
| **MEDIA** | Extraer flags | `sentenciaArbitraria`, `tieneDictamenes` | Clasificacion adicional |
| **BAJA** | Extraer `referencias[]` | `referencias[]` | Normas citadas en sumario |
