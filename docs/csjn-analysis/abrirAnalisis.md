# Endpoint: abrirAnalisis

**URL**: `{baseUrl}/fallos/abrirAnalisis.html?idAnalisis={analysisId}`
**Cache key**: `_cache/csjn/api/abrirAnalisis/{analysisId}.json`
**Tamano**: 7-300 KB
**Formato**: JSON objeto (root-level)
**Poblacion**: 1,811/1,811 (100%)

## Descripcion

Metadata principal del fallo. Es el endpoint mas rico en informacion estructurada. Contiene datos del expediente, clasificacion juridica, voces (keywords), jueces intervinientes, formulas aplicadas, referencias normativas, cuestion federal, y fallo destacado.

## Estructura completa

### Identificacion

| Campo | Tipo | Ejemplo | Parser | Status |
|-------|------|---------|:---:|--------|
| `id` | number | `809109` | Si (analysisId) | CORRECTO |
| `caratula` | string | `"FLOGAR S.A.C.I. c/ MUNICIPALIDAD..."` | Si → `CaseTitle` | CORRECTO |
| `identificacionExpediente` | string | `"F. 23. XLVII. RHE"` | **No** | **FALTA** — campo real para CaseNumber |
| `alias` | string/null | `null` | No | Raramente poblado |
| `caratulaFallo` | string/null | `null` | No | Alternativa a caratula, raramente poblado |

### Expediente (objeto `expediente`)

| Campo | Tipo | Ejemplo | Parser | Valor KB |
|-------|------|---------|:---:|----------|
| `expediente.id` | number | `23526352` | No | ID interno |
| `expediente.numeroExpediente` | number | `23` | No | Numero |
| `expediente.anioExpediente` | number | `2011` | No | Ano de inicio |
| `expediente.tomoExpediente` | number | `47` | No | Tomo |
| `expediente.letraExpediente` | string | `"F"` | No | Letra |
| `expediente.camara.abreviatura` | string | `"CSJ"` | No | Tribunal de origen |
| `expediente.camara.descripcion` | string | `"Corte Suprema de Justicia de la Nacion"` | No | Nombre completo |
| `expediente.objetoJuicio.descripcion` | string | `"A DETERMINAR"` | No | Objeto del juicio |
| `expediente.caratula` | string | (igual a root `caratula`) | No | Redundante |
| `expediente.oldIdentificadorExpediente` | string | `"F. 23. XLVII. "` | No | ID legacy |

### Recurso Expediente (objeto `recursoExpediente`)

| Campo | Tipo | Ejemplo | Parser | Valor KB |
|-------|------|---------|:---:|----------|
| `recursoExpediente.id` | number | `3102171` | No | ID interno |
| `recursoExpediente.tipoRecurso.codigo` | string | `"RHE"` | No | Codigo tipo recurso |
| `recursoExpediente.tipoRecurso.descripcion` | string | `"RECURSO DE HECHO"` | No | Descripcion |
| `recursoExpediente.claveRecurso` | string | `"CSJ 000023/2011(47-F)/CS001"` | **No** | **FALTA** — ID moderno de expediente |
| `recursoExpediente.identificacionExpediente` | string | `"F. 23. XLVII. RHE"` | No | Igual a root |

### Clasificacion juridica (objetos `{codigoValor, valor}`)

| Campo | Tipo | Ejemplo | Parser | Status |
|-------|------|---------|:---:|--------|
| `competencia.valor` | string | `"APELACION EXTRAORDINARIA"` | Si → `Jurisdiction` | **PARCIAL** — es tipo procesal, no jurisdiccion territorial |
| `tipoAccion.valor` | string | `"ACCION CONTENCIOSO ADMINISTRATIVA"` | **No** | **FALTA** — util para clasificacion (~75% poblado) |
| `sentidoPronunciamiento.valor` | string | `"DESESTIMA"` | Si → `RulingDirection` | CORRECTO |
| `materia.valor` | string | `"@HONORARIOS"` | **No** | **FALTA** — clasificacion interna CSJN (diferente de materiaSecretaria) |
| `tipoRecurso.valor` | string | `"RECURSO DE QUEJA"` | Si → `ResourceType` | CORRECTO |
| `materiaSecretaria.valor` | string | `"Administrativo"` | Si → `SubjectArea` | CORRECTO |

### Flags

| Campo | Tipo | Ejemplo | Parser | Valor KB |
|-------|------|---------|:---:|----------|
| `inconstitucional` | bool | `false` | Si → `IsUnconstitutional` | CORRECTO |
| `publico` | string | `"S"` | No | Bajo valor |
| `audPublica` | string | `"N"` | No | Bajo valor |
| `articulo16Ley48` | string | `"N"` | No | Bajo valor |
| `amicue_curie` | string | `"N"` | No | Posible interes |
| `votosOpinion` | string | `"N"` | No | Bajo valor |
| `cambiaJurisp` | null/string | `null` | No | **Alto valor** si fuera poblado (indica cambio de jurisprudencia) |

### Fallo Destacado (objeto `falloDestacado`)

| Campo | Tipo | Ejemplo | Parser | Status |
|-------|------|---------|:---:|--------|
| `falloDestacado.id` | number | `8723` | No | ID interno |
| `falloDestacado.titulo` | string | `"Comercio interprovincial y discriminacion tributaria"` | **No** | **FALTA** — titulo editorial del fallo |
| `falloDestacado.cabecilla` | string (HTML) | `"<p>La Corte hizo lugar a la demanda..."` | **No** | **FALTA** — resumen HTML rico |
| `falloDestacado.resumen` | string/null | Texto corto de resumen | **No** | **FALTA** — resumen limpio |
| `falloDestacado.fecha` | number (epoch ms) | `1747796400000` | **No** | **FALTA** — **fecha real del fallo** (diferente de reciboEntrada) |
| `falloDestacado.esNovedad` | bool | `true` | No | Flag de novedad |
| `falloDestacado.relevancia` | number | `1` | No | Nivel de relevancia |
| `falloDestacado.fechaCreacion` | number | `1747858029362` | No | Audit |
| `falloDestacado.fechaModificacion` | number | `1747935266112` | No | Audit |

**NOTA CRITICA**: El parser actual trata `falloDestacado` como un string via `GetStringFromElement`, que busca `valor`/`descripcion`/`texto` dentro de objetos. Pero el objeto real tiene `titulo`/`cabecilla`/`resumen` — ninguno coincide. Resultado: **el Summary que sale de este campo es siempre null**. Para fallos no-destacados, `falloDestacado` es `null`.

### Observaciones y Comentarios

| Campo | Tipo | Poblacion | Parser | Valor KB |
|-------|------|-----------|:---:|----------|
| `observaciones` | string/null | ~50% | **No** | Notas del caso, contexto doctrinal (HTML a veces) |
| `comentarios` | string/null | ~0% | No | No poblado en muestras |

### Voces (keywords juridicas)

```json
"voces": [
  {
    "tipoVoz": {
      "codigoValor": 1671,
      "valor": "EMPLEADOS PROVINCIALES"
    },
    "orden": 1
  }
]
```

| Parser | Status | Promedio | Nota |
|:---:|--------|---------|------|
| Si → `Keywords` | CORRECTO | ~7 voces/fallo (min 4, max 11+) | Algunos valores son placeholders: `"(*)"`, `"(**)"`, `"(.)"` |

### Jueces — Strings concatenados

| Campo | Ejemplo | Poblacion | Parser | Status |
|-------|---------|-----------|:---:|--------|
| `stringMayoria` | `"MAQUEDA, LORENZETTI, HIGHTON de NOLASCO"` | ~75% | **No** | **FALTA** — nombres de jueces de mayoria |
| `stringVoto` | `""` | ~0% | No | Raramente poblado |
| `stringDisidencia` | `"HIGHTON de NOLASCO"` | ~12% | **No** | **FALTA** — jueces en disidencia |
| `stringDisidenciaParcial` | `""` | Raro | No | |
| `stringAbstencion` | `""` | Raro | No | |

### Votos Analisis Documental (array estructurado)

```json
"votosAnalisisDocumental": [
  {
    "tipoVoto": { "valor": "MAYORIA" },
    "vocales": "ROSATTI, LORENZETTI",
    "paginas": "1-8"
  },
  {
    "tipoVoto": { "valor": "VOTO PROPIO QUE ADHIERE A LA MAYORIA" },
    "vocales": "ROSENKRANTZ",
    "paginas": "9-12"
  }
]
```

| Poblacion | Parser | Status | Impacto |
|-----------|:---:|--------|---------|
| 100% | **No** | **FALTA** | **Puede reemplazar la llamada LLM de judges** — datos estructurados con tipo de voto + nombres |

### HTML Votos

| Campo | Tipo | Poblacion | Valor KB |
|-------|------|-----------|----------|
| `htmlVotos` | string (HTML) | 100% | HTML con paneles Bootstrap, paginas de votos. Orientado a presentacion, no a datos. Preferir `votosAnalisisDocumental`. |

### Fecha — Recibo de Entrada

```json
"reciboEntrada": {
  "fecha": 1747278000000,
  "fechaLong": 20250515,
  "fechaString": "15/05/2025",
  "secretaria": { "descripcion": "Sec. Judicial N 4" },
  "reciboEntradaExpedientes": [...]
}
```

| Parser | Status |
|:---:|--------|
| Si (fallback) → `RulingDate` | **INCORRECTO como fecha del fallo** |

**DETALLE CRITICO**: `reciboEntrada.fecha` es la **fecha de recibo/alta del registro en el sistema**, NO la fecha del fallo. Comparaciones en 3 muestras:

| Sample | reciboEntrada.fechaString | falloDestacado.fecha (epoch→date) | Diferencia |
|--------|--------------------------|-----------------------------------|------------|
| 809109 | 15/05/2025 | ~21/05/2025 | +6 dias |
| 776580 | 28/06/2022 | ~05/07/2022 | +7 dias |
| 718919 | 06/02/2015 | ~10/02/2015 | +4 dias |

La **fecha real del fallo** es `falloDestacado.fecha` o `getSumariosAnalisis[].fechaFallo`. El parser se salva porque el crawler pasa `rulingDateHint` (fecha de acuerdo), pero si se re-procesa desde cache sin hint, la fecha queda incorrecta.

### Referencias Normativas

```json
"referenciasNormativas": [
  {
    "norma": {
      "tipoNorma": { "valor": "LEY" },
      "numero": "25675",
      "descripcion": "LEY GENERAL DEL AMBIENTE"
    },
    "articulo": "30",
    "inciso": null
  }
]
```

| Poblacion | Parser | Impacto |
|-----------|:---:|---------|
| ~37% | **No** | **Puede suplementar la llamada LLM de cited_statutes** |

### Cuestiones Federales

```json
"cuestionesFederales": [
  {
    "tipoCuestionFederal": {
      "valor": "ARTICULO 14 DE LA LEY 48"
    }
  }
]
```

| Poblacion | Parser | Valor KB |
|-----------|:---:|----------|
| ~50% | **No** | Tipo de cuestion federal: clasificacion procesal |

### Formulas

```json
"formulas": [
  {
    "tipoFormula": { "valor": "ART. 280 CPCCN" }
  }
]
```

| Poblacion | Parser | Valor KB |
|-----------|:---:|----------|
| ~12% | **No** | Tags procesales (art. 280, acordadas). Bajo volumen pero alto valor cuando presente. |

### Otros arrays (generalmente vacios)

| Campo | Contenido tipico | Poblacion |
|-------|------------------|-----------|
| `enlaces[]` | Enlaces a documentos | ~0.5% (ver getEnlacesAnalisis) |
| `recusExcus[]` | Recusaciones/excusaciones | ~0% |
| `votosSumario[]` | Votos en sumario | ~0% |
| `materias[]` | Materias adicionales | Variable |
| `dictamenesProcuracion` | Dictamenes | null |

### Campos de auditoria (no utiles para KB)

| Campo | Tipo | Nota |
|-------|------|------|
| `fechaCreacion` | number (yyyyMMddHHmm) | Fecha de creacion del registro |
| `fechaModificacion` | number | Fecha de ultima modificacion |
| `fechaControl` | null | No poblado |
| `usuarioCreacion` | object | Usuario CSJN |
| `usuarioModificacion` | object | Usuario CSJN |
| `inicializacion` | string | Flag interno |

## Resumen de acciones

| Prioridad | Accion | Campo | Impacto |
|-----------|--------|-------|---------|
| **CRITICA** | Corregir CaseNumber | `identificacionExpediente`, `claveRecurso` | 8,300 rulings con NULL |
| **CRITICA** | Corregir Summary | `falloDestacado.titulo`/`cabecilla`/`resumen` | Summary siempre null |
| **ALTA** | Corregir RulingDate fallback | `falloDestacado.fecha` | Fecha incorrecta sin hint |
| **ALTA** | Extraer votosAnalisisDocumental | `votosAnalisisDocumental[]` | Reemplaza LLM judges |
| **ALTA** | Extraer tipoAccion | `tipoAccion.valor` | Clasificacion procesal |
| **MEDIA** | Extraer referenciasNormativas | `referenciasNormativas[]` | Suplementa LLM statutes |
| **MEDIA** | Extraer stringMayoria/Disidencia | `stringMayoria`, `stringDisidencia` | Complementa votos |
| **MEDIA** | Extraer observaciones | `observaciones` | Contexto doctrinal |
| **MEDIA** | Extraer cuestionesFederales | `cuestionesFederales[]` | Clasificacion |
| **BAJA** | Extraer materia | `materia.valor` | Clasificacion interna |
| **BAJA** | Extraer formulas | `formulas[]` | Tags procesales |
