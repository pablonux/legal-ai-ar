# Analisis de API CSJN — 8 Endpoints

Fecha: 2026-04-24 | Muestra: 1,811 fallos destacados (2015-2026)

## Contexto

El pipeline de ingesta descarga respuestas de la API REST de la CSJN (sjconsulta.csjn.gov.ar) y las cachea en Azure Blob Storage bajo `_cache/csjn/api/`. Este analisis examina **8 endpoints** con **1,811 muestras reales** de fallos destacados para determinar: (a) que datos estan disponibles, (b) cuales se extraen hoy, (c) cuales se extraen incorrectamente, y (d) cuales son oportunidades de mejora.

## Storage

- **Container**: `legalaiar-dev`
- **Prefijo**: `_cache/csjn/api/{endpoint}/{id}.json`
- **Key para endpoints by analysisId**: `{analysisId}`
- **Key para endpoints by documentId**: `{documentId}`

## Los 8 Endpoints

| # | Endpoint | Key | Usado por parser | Datos (de 1,811) | Doc |
|---|----------|-----|:---:|:---:|-----|
| 1 | `abrirAnalisis` | analysisId | Si | 1,811 (100%) | [abrirAnalisis.md](./abrirAnalisis.md) |
| 2 | `getAllDocumentos` | analysisId | Si | 1,811 (100%) | [getAllDocumentos.md](./getAllDocumentos.md) |
| 3 | `getSumariosAnalisis` | analysisId | Si | 1,774 (98%) | [getSumariosAnalisis.md](./getSumariosAnalisis.md) |
| 4 | `getCitas` | documentId | Si | 1,811 (100%) | [getCitas.md](./getCitas.md) |
| 5 | `getCitantes` | documentId | Si | 1,104 (61%) | [getCitantes.md](./getCitantes.md) |
| 6 | `getDictamenesAnalisis` | analysisId | **No** | 1,097 (61%) | [getDictamenesAnalisis.md](./getDictamenesAnalisis.md) |
| 7 | `getSintesisAnalisis` | analysisId | **No** | 1,414 (78%) | [getSintesisAnalisis.md](./getSintesisAnalisis.md) |
| 8 | `getEnlacesAnalisis` | analysisId | **No** | 9 (0.5%) | [getEnlacesAnalisis.md](./getEnlacesAnalisis.md) |

---

## Validacion de campos: Parser actual vs API real

Auditoria completa de cada campo que el parser (`CsjnApiParser.cs`) extrae hoy, comparado contra los datos reales de la API.

### Campos CORRECTOS

| Campo KB | Campo API | Validacion |
|----------|-----------|------------|
| `CaseTitle` | `caratula` (root en abrirAnalisis) | CORRECTO. El parser busca `tituloCausa`, `caseTitle`, `titulo`, `caratula` — `caratula` siempre existe. |
| `ResourceType` | `tipoRecurso.valor` | CORRECTO. Ej: `"RECURSO DE QUEJA"`, `"RECURSO EXTRAORDINARIO"`. |
| `RulingDirection` | `sentidoPronunciamiento.valor` | CORRECTO. Ej: `"DESESTIMA"`, `"REVOCA"`, `"DEJA SIN EFECTO"`. |
| `SubjectArea` | `materiaSecretaria.valor` | CORRECTO. Ej: `"Penal"`, `"Administrativo"`. Strip de `@` legacy funciona. |
| `IsUnconstitutional` | `inconstitucional` (bool) | CORRECTO. |
| `Keywords` | `voces[].tipoVoz.valor` + `codigoValor` | CORRECTO. Path correcto, datos extraidos bien. |

### Campos con ERRORES

| Campo KB | Status | Problema | Detalle |
|----------|--------|----------|---------|
| **CaseNumber** | INCORRECTO | 8,300/8,300 = NULL | El parser busca `numeroCausa`, `caseNumber`, `numero` que **no existen** en la API. Los campos reales son `identificacionExpediente` (root, 100%) y `recursoExpediente.claveRecurso` (nested, 100%). El `caseNumberHint` del crawler mitiga parcialmente. |
| **RulingDate** | MITIGADO | Fallback incorrecto | No hay `fecha` a nivel root. El parser cae a `reciboEntrada.fechaString` que es la **fecha de recibo/alta del registro**, no la fecha del fallo. Diferencias de 4-7 dias observadas vs la fecha real del fallo (`falloDestacado.fecha`, `getSumariosAnalisis.fechaFallo`). **Mitigado**: el crawler pasa `rulingDateHint` (fecha de acuerdo) que SI es correcta. Pero: (a) si se re-procesa desde cache sin hint, la fecha es incorrecta; (b) el parser no tiene forma de obtener la fecha del fallo solo desde la API. |
| **Summary** | INCORRECTO | `falloDestacado` es objeto, no string | El parser trata `falloDestacado` como string via `GetStringFromElement` que busca `valor`/`descripcion`/`texto`. Pero el objeto real tiene `titulo`, `cabecilla` (HTML), `resumen`. Ninguno de esos sub-campos coincide con lo que `GetStringFromElement` busca. Resultado: **siempre null** de este campo. |
| **Holding** | INCORRECTO | Campo equivocado en sumarios | El parser busca `considerando`, `holding`, `sintesis` en getSumariosAnalisis. El campo real con el texto doctrinal es **`texto`**. Peor: `holding` existe como **flag numerico** (valor `1`) que el parser convierte a string `"1"`. Resultado: la KB puede tener `Holding = "1"`. |
| **Jurisdiction** | PARCIAL | Semantica incorrecta | `competencia.valor` devuelve el **tipo de competencia procesal** (ej: `"APELACION EXTRAORDINARIA"`), no la jurisdiccion territorial. Para CSJN todos son jurisdiccion federal, pero el valor almacenado no refleja eso. |
| **Citations** | PARCIAL | Faltan campos | El parser extrae `alias` + `idSumario`, pero no extrae `idFallo` (link directo a otro fallo), `textoCita` (referencia canonica "Fallos: tomo:pagina"), ni `replacementLink`. Ademas, el analisis previo decia "getCitas siempre vacio" lo cual es **falso**: 100% de los 1,811 samples tienen datos. |
| **CitedBy** | INCORRECTO | Formato incompatible | `ParseCitedBy` espera JSON array/object pero la API devuelve un **JSON string** que contiene **HTML**. Al ser `JsonValueKind.String`, no entra en ningun branch del parser y **siempre devuelve lista vacia**. Los 1,104 fallos con citantes (61%) no se procesan. |

### Campos NO extraidos (oportunidades)

Campos disponibles en la API que no se extraen y aportarian valor a la KB. Ver doc de cada endpoint para detalles.

| Campo | Endpoint | Poblacion | Valor KB |
|-------|----------|-----------|----------|
| `identificacionExpediente` | abrirAnalisis | 100% | Numero de expediente (fix CaseNumber) |
| `recursoExpediente.claveRecurso` | abrirAnalisis | 100% | ID moderno de expediente |
| `tipoAccion.valor` | abrirAnalisis | ~75% | Tipo de accion (AMPARO, PENAL, etc.) |
| `falloDestacado` (objeto) | abrirAnalisis | 100% (destacados) | titulo, cabecilla, resumen del fallo |
| `observaciones` | abrirAnalisis | ~50% | Notas del caso, contexto doctrinal |
| `votosAnalisisDocumental[]` | abrirAnalisis | 100% | Votos estructurados con jueces — **reemplaza LLM judges** |
| `stringMayoria` / `stringDisidencia` | abrirAnalisis | ~75% / ~12% | Jueces por tipo de voto |
| `referenciasNormativas[]` | abrirAnalisis | ~37% | Normas citadas — **suplementa LLM statutes** |
| `cuestionesFederales[]` | abrirAnalisis | ~50% | Tipo de cuestion federal |
| `formulas[]` | abrirAnalisis | ~12% | Formula procesal (ej: "ART. 280 CPCCN") |
| `materia.valor` | abrirAnalisis | 100% | Clasificacion interna CSJN |
| `tomo` / `pagina` | sumarios / getAllDocumentos | ~98% | Referencia oficial "Fallos: tomo:pagina" |
| `texto` | getSumariosAnalisis | 98% | **Headnote doctrinal** — mejor texto para Holding |
| `vocesSumario[]` | getSumariosAnalisis | 98% | Keywords estructuradas de sumarios |
| `idFallo` / `textoCita` | getCitas | 100% | Links directos + referencia canonica |
| Dictamen MPF (presencia) | getDictamenesAnalisis | 61% | Detecta dictamen **sin LLM** |
| Enlaces MPF (URLs) | getEnlacesAnalisis | 0.5% | URLs a PDFs de dictamenes |
| Sintesis/Resena | getSintesisAnalisis | 78% | Documentos asociados tipo RESENA |

## Samples

- **Fuente**: Fallos destacados CSJN 2015-2026, descargados con `scripts/download-fallos-destacados.ps1`
- **Indice**: `samples/fallosDestacados/index.json` (1,811 registros)
- **Por endpoint**: `samples/{endpoint}/{id}.json`

## Archivos de analisis

- [abrirAnalisis.md](./abrirAnalisis.md) — Metadata principal del fallo
- [getAllDocumentos.md](./getAllDocumentos.md) — Documentos PDF + metadata
- [getSumariosAnalisis.md](./getSumariosAnalisis.md) — Sumarios juridicos
- [getCitas.md](./getCitas.md) — Citas salientes (otros fallos citados)
- [getCitantes.md](./getCitantes.md) — Citas entrantes (fallos que citan a este)
- [getDictamenesAnalisis.md](./getDictamenesAnalisis.md) — Dictamenes del MPF
- [getSintesisAnalisis.md](./getSintesisAnalisis.md) — Sintesis / Resenas
- [getEnlacesAnalisis.md](./getEnlacesAnalisis.md) — Enlaces externos
- [DECISIONES.md](./DECISIONES.md) — Decisiones tomadas
