# Persister — análisis de corridas de benchmark (logs)

**Estado:** referencia de mediciones tomadas desde logs del worker.  
**Fecha de referencia:** 2026-05-13 (corrida 4); **2026-05-14** (corridas 5–6 y referencia post–cambios 1+2); **2026-05-15** (corrida 7).  
**Instrumentación:** fases y tiempos descritos en `persister-worker-analysis-deferred.md` y código en `LegalAiAr.Worker.Persister` (worker + estrategias).

**Archivos de log analizados**

| Corrida | Archivo (raíz repo) |
|---------|----------------------|
| 1 (~15) | Log de la primera prueba (archivo inicial de la serie; puede haber sido sobrescrito). |
| 2 (~200) | `persister-job-run-second-200.log` |
| 3 (~700) | `persister-job-run-tercera-700.log` |
| 4 (~2100) | **`persister-job-run.log`** — cuarta corrida (reutiliza el nombre del archivo; ~40 MB). Conviene renombrar a `persister-job-run-cuarta-2100.log` en futuras mediciones para no pisar históricos. |
| Post 1+2, sin tesauro en BD (~576) | `persister-job-run-after-1-and-2.log` — warm-up con **0** términos tesauro / **0** UF; útil para comparar **batch keywords** sin efecto caché tesauro. |
| 5 (~371, **con tesauro**) | **`persister-job-run-with-thesaurus.log`** — tesauro ingerido; warm-up carga términos + UF. |
| 6 (~758–759, **con tesauro**) | **`persister-job-run-with-thesaurus-769.log`** — validación en muestra ~2× la corrida 5. |
| 7 (~1052, **con tesauro**) | **`persister-job-run-with-thesaurus-1052.log`** — muestra ~1,4× la corrida 6; warm-up corto (~6,4 s); caché de etiquetas tesauro normalizada (acentos/espacios) y cited-by por lote en SQL. |

## Contexto

- Los tiempos provienen de logs en consola con nivel **Debug** en `LegalAiAr.Worker.Persister` y redirección a archivo (nombres arriba).
- **`ProcessMessageAsync`**: tiempo total por mensaje de cola (deserialización, `SetProcessing`, `PersistAsync`, avance de etapa, publicación a Indexer, borrado de mensaje, `LogStage`).
- **`PersistAsync`**: solo la estrategia (p. ej. `RulingPersistStrategy`).
- La diferencia **`ProcessMessageAsync` − `PersistAsync`** acota el coste del “tubo” posterior al grafo (SQL de documento, cola saliente, etc.).
- Si el **job** declara *N* documentos y el log muestra *M* &lt; *N* mensajes completos, la brecha no se explica desde el archivo: otra instancia del Persister, mensajes aún no encolados, o cierre del log antes de vaciar la cola. Para alinear IDs conviene cruzar con **`DocumentStageLogs`** y/o `GET /api/admin/jobs/{jobId}/metrics`.

---

## Corrida 1 — prueba corta (~15 documentos job)

### Hechos observados en el log

| Concepto | Valor |
|----------|--------|
| Mensajes con `Processing PersisterMessage` / `ProcessMessageAsync` completos | **14** |
| Omisiones explícitas en log (`not at Persister`, `skipping`, etc.) | **0** |
| Conclusión operativa | El **15.º** no apareció en este proceso (no encolado a Persister visto por este worker, u otro consumidor). |

### Arranque

| Fase | Tiempo aprox. |
|------|----------------|
| `EntityCacheWarmUp` | **~8,7 s** |
| Caché de tesauro (términos cargados) | **0** (mensaje de warm-up coherente con resoluciones vía otras rutas) |

### Tiempos globales (*n* = 14)

| Métrica | `PersistAsync` | `ProcessMessageAsync` |
|---------|----------------|------------------------|
| Mín | 2733 ms | 3728 ms |
| Máx | 12 611 ms | 13 592 ms |
| Media | ~4423 ms | ~5429 ms |
| Mediana (~p50) | — | **~4810 ms** |

Con *n* = 14, el percentil 95 coincide prácticamente con el **máximo** (un solo outlier fuerte).

### Outlier principal (corrida 1)

**`DocId=b5d79ef2-af59-4e70-84a0-4f6fc75082d9`** (HashPrefix `44b6b538`):

| Fase | Tiempo aprox. | Nota |
|------|----------------|------|
| `Statutes` | **~2470 ms** (`StatuteCount=2`) | Frente a un doc rápido con **~159 ms** y `StatuteCount=1` |
| `RulingRepositoryAdd` | **~7486 ms** | Dominante frente a **~647 ms** en un doc “rápido” |
| `BlobJsonPayload` / `ContentHashLookup` / `CourtResolve` | ~160 ms c/u | No explican el pico |

**Primer mensaje** del lote (`DocId=f7f7f950-…`): **`BlobJsonPayload` ~742 ms** vs **~160 ms** en el resto — posible efecto de **primera lectura** (blob/red).

### Fases con lectura “0 ms” en muestras

`CitationsFirstSave` y `ArticlesOpinionSave` aparecieron en **0 ms** en varios ejemplos: no implican coste cero real; indican que **no fueron el cuello** frente a `RulingRepositoryAdd` / `Statutes` en ese subconjunto.

### Lectura corrida 1

- El lote es **demasiado pequeño** para percentiles estables; sirve para **detectar forma** de costes (normas + primer `AddAsync` del fallo).
- El cuello del outlier está en **grafo EF / insert del ruling** y **persistencia de normas**, no en blob ni lookup por hash en ese caso.

---

## Corrida 2 — ~200 documentos job (incluye los 15 anteriores; omitidos en esta corrida respecto al job)

### Hechos observados en el log

| Concepto | Valor |
|----------|--------|
| `Processing PersisterMessage` / `ProcessMessageAsync` / `PersistAsync` | **184** cada uno |
| Omisiones / duplicados / fallos / DLQ en texto buscado | **0** |
| Brecha vs ~200 documentos | **~16** sin traza en **este** log (ver contexto arriba). |

### Arranque

| Fase | Tiempo aprox. |
|------|----------------|
| `StartupRecovery` | ~230 ms (`ResetRows=0`) |
| `EntityCacheWarmUp` | **~6625 ms** |

### Tiempos globales — `ProcessMessageAsync` (*n* = 184)

| Métrica | Valor |
|---------|--------|
| Mín | 3484 ms |
| **p50** | **4953 ms** |
| **p95** | **7113 ms** |
| **p99** | **7776 ms** |
| Máx | **9748 ms** |
| Media | **~5205 ms** |

### Histograma aproximado (`ProcessMessageAsync`)

| Ventana | Cantidad |
|---------|----------|
| 3–4 s | 10 |
| 4–5 s | **86** |
| 5–6 s | 56 |
| 6–7 s | 19 |
| 7–8 s | 12 |
| 8–10 s | **1** |

La mayoría del tráfico cae entre **4 y 6 s**; cola larga reducida.

### `PersistAsync` (*n* = 184)

| Métrica | Valor |
|---------|--------|
| Mín / máx | 2480 / **8745 ms** |
| Media | **~4163 ms** |
| **p95** | **~6083 ms** |

### “Tubo” post-`PersistAsync`

**`ProcessMessageAsync` − `PersistAsync`** (media **~1042 ms**, rango aprox. **959–3214 ms**): coherente con `SetProcessing` + `PostPersistSql` + `PublishIndexer` + `DeleteQueueMessage` + `LogStageAsync`.

### Agregado de fases internas (184 rulings, parseadas del log)

| Fase | min | p50 | p95 | max | media aprox. |
|------|-----|-----|-----|-----|----------------|
| `BlobJsonPayload` | 156 | 163 | 233 | 705 | **181** |
| `ContentHashLookup` | 155 | 160 | 177 | **4919** | **194** |
| `Statutes` | 0 | 161 | 488 | 904 | **196** |
| `RulingRepositoryAdd` | 630 | 656 | 820 | 1432 | **677** |
| `Keywords` | 0 | 940 | 2426 | 3719 | **1016** |
| `LinkProceeding` | — | ~1294 | ~1490 | **2720** | **~1127** |

- **`RulingRepositoryAdd`:** homogéneo (p95 ~820 ms); **no** explica el máximo global en esta corrida.
- **`Keywords`:** alta variabilidad → seguir mirando **tesauro / get-or-create** cuando crezca el payload de keywords.
- **`LinkProceeding`:** contribución estable (**~1,1 s** media).
- **Cited-by:** solo **2** documentos con `CitedByCount > 0` en este lote; poco peso agregado de citas inversas.
- **Tesauro:** suma de `ThesaurusResolveCount` en el log **385** (~**2,1** por documento); **183** documentos con al menos una resolución; máximo **7** en un solo documento.

### Outlier principal (corrida 2)

**`DocId=e043d718-9c22-4f88-99a2-1ec0dfcf278c`** — máximo `ProcessMessageAsync` (**~9748 ms**):

| Fase | Tiempo aprox. |
|------|----------------|
| **`ContentHashLookup`** | **~4919 ms** |
| `PersistAsync` total | **~8745 ms** |
| Resto de fases del ruling | acotadas (p. ej. `RulingRepositoryAdd` ~658 ms) |

**Distribución `ContentHashLookup`:** solo **1** medición **&gt; 2 s**; **3** mediciones **&gt; 500 ms**. El resto ~**155–177 ms** (p50 ~160 ms).

**Hipótesis de trabajo:** revisar **`GetByContentHashAsync`** (índice en `ContentHash`, plan de ejecución, contención, estado del buffer pool) para el pico aislado.

### Lectura corrida 2

- Muestra **suficiente** para p50/p95/p99 fiables en el total por mensaje.
- El cuello típico ya **no** es el mismo que en corrida 1: aquí **`ContentHashLookup`** puede dominar un outlier puntual; **`RulingRepositoryAdd`** queda **controlado** en el agregado.
- **`Keywords`** y **`LinkProceeding`** son las fases **más variables** del “cuerpo” del lote.

---

## Corrida 3 — job ~700 (`persister-job-run-tercera-700.log`)

Incluye los **200** de la corrida 2; en este worker se observaron **493** mensajes completos (~**500** planificados: la brecha puede deberse a otra réplica del Persister, mensajes aún en cola al cortar el log, o conteo de job distinto; validar con API/SQL).

### Hechos observados en el log

| Concepto | Valor |
|----------|--------|
| `Processing PersisterMessage` / `ProcessMessageAsync` / `PersistAsync` | **493** cada uno |
| Omisiones / duplicados / fallos / DLQ (búsqueda en texto) | **0** |

### Arranque

| Fase | Tiempo aprox. |
|------|----------------|
| `StartupRecovery` | ~281 ms (`ResetRows=0`) |
| `EntityCacheWarmUp` | **~8739 ms** |

### Tiempos globales — `ProcessMessageAsync` (*n* = 493)

| Métrica | Valor |
|---------|--------|
| Mín | **1643 ms** |
| **p50** | **5199 ms** |
| **p95** | **8192 ms** |
| **p99** | **9925 ms** |
| Máx | **10 973 ms** |
| Media | **~5549 ms** |

### Histograma aproximado (`ProcessMessageAsync`, ms)

| Ventana | Cantidad |
|---------|----------|
| &lt; 3000 | **1** |
| 3–4 s | 19 |
| 4–5 s | **171** |
| 5–6 s | **165** |
| 6–7 s | 74 |
| 7–8 s | 36 |
| 8–10 s | 23 |
| &gt; 10 s | **4** |

### `PersistAsync` (*n* = 493)

| Métrica | Valor |
|---------|--------|
| Mín / máx | **655** / **10 004 ms** |
| Media | **~4502 ms** |
| **p95** | **~7097 ms** |

### “Tubo” post-`PersistAsync`

**`ProcessMessageAsync` − `PersistAsync`:** media **~1047 ms** (rango **964–3151 ms**), coherente con corridas anteriores (~1 s fijos de pipeline post-persist).

### Agregado de fases internas (493 rulings)

| Fase | min | p50 | p95 | max | media aprox. |
|------|-----|-----|-----|-----|----------------|
| `BlobJsonPayload` | 156 | 160 | 238 | 1135 | **183** |
| `ContentHashLookup` | 156 | 160 | 187 | **1049** | **176** |
| `Statutes` | 0 | 159 | 479 | 1589 | **192** |
| `RulingRepositoryAdd` | 168 | 650 | 893 | **1995** | **701** |
| `Keywords` | 0 | 951 | 3322 | **5511** | **1218** |
| `LinkProceeding` | 0 | 1305 | 1497 | 2885 | **1081** |

**Cola de `ContentHashLookup`:** **10** mediciones **&gt; 500 ms**; **2** **&gt; 1 s**. No se repite el pico de **~4,9 s** de la corrida 2; el máximo global pasa a estar en otras fases.

**Tesauro:** suma de `ThesaurusResolveCount` **1185** (~**2,4** por documento); máximo **10** resoluciones en un solo doc.

**Cited-by:** **5** líneas con `CitedByCount` &gt; 0 (lote aún con poca carga inversa).

### Outlier principal (corrida 3)

**`DocId=01412771-3774-416c-acf9-146a04439095`** — máximo `ProcessMessageAsync` **~10 973 ms**, `PersistAsync` **~10 004 ms**:

| Fase | Tiempo aprox. | Nota |
|------|----------------|------|
| **`Keywords`** | **~5511 ms** | `KeywordCount=9`, `ThesaurusResolveCount=9` |
| **`Statutes`** | **~1116 ms** | `StatuteCount=7` |
| `Persons` | ~629 ms | `PersonCount=4` |
| `RulingRepositoryAdd` | ~961 ms | Acotado vs outlier de corrida 1 |
| `LinkProceeding` | ~1142 ms | En línea con p50 agregado |
| `ContentHashLookup` | ~157 ms | Normal |

En esta corrida el **cuello extremo** vuelve a alinearse con la **corrida 1**: carga de **keywords + normas + grafo**, no con `GetByContentHashAsync`.

### Lectura corrida 3

- **p50** sube levemente respecto a corrida 2 (**5199** vs **4953 ms**); **p95/p99** suben más (**8192 / 9925** vs **7113 / 7776 ms**), coherente con mezcla más pesada (más keywords/normas y cuatro docs &gt; 10 s).
- **`ContentHashLookup`** queda **controlado** en agregado (p95 ~187 ms; pocos &gt; 500 ms); el monstruo de **~4,9 s** de la corrida 2 **no reaparece** en este archivo (ese documento no está en este lote o el efecto fue puntual).
- **`RulingRepositoryAdd`** sigue con p95 modesto (**~893 ms**); el máximo **~2 s** aporta a la cola pero no domina como en el outlier de 14 docs.
- **`Keywords`** es de nuevo el principal candidato a **multiplicar** tiempo cuando sube `KeywordCount` y resoluciones de tesauro.

---

## Comparación resumida (corridas 1–4)

| Tema | C1 (*n*≈14) | C2 (*n*=184) | C3 (*n*=493) | C4 (*n*=1643 Persist / 1646 PM*) |
|------|-------------|--------------|--------------|-------------------------------------|
| `ProcessMessageAsync` p50 | ~4810 ms | 4953 ms | 5199 ms | **5500 ms** (≈5498 ms excl. 3 fallos) |
| `ProcessMessageAsync` p95 | ≈ máx | 7113 ms | 8192 ms | **8559 ms** |
| `ProcessMessageAsync` p99 | — | 7776 ms | 9925 ms | **10 574 ms** |
| `PersistAsync` p95 | — | ~6083 ms | ~7097 ms | **~7439 ms** |
| Outlier dominante | `Statutes` + `RulingRepositoryAdd` | `ContentHashLookup` (~4,9 s) | `Keywords` + `Statutes` | **`Keywords` + citas + cited-by** (doc 12 kw, 43 citas, 10 cited-by) |
| `ContentHashLookup` &gt; 500 ms | — | 3 / 184 | 10 / 493 | **29 / 1643** (~1,8 %) |
| `ContentHashLookup` &gt; 2 s | — | 1 | 2 | **1** |
| Fallos / duplicados | 1 sin traza | 0 / brecha cola | 0 | **3** truncamiento `Persons.LastName`; **1** hash duplicado (`already exists`); **3** `DocId` con **2** líneas `ProcessMessageAsync` (fallo + registro) |
| Warm-up | ~8,7 s | ~6,6 s | ~8,7 s | **~7,0 s** |

\*En corrida 4: **1643** `PersistAsync` exitosos; **1646** líneas `ProcessMessageAsync` porque los **3** documentos fallidos generan **línea duplicada** en el flujo de error. Estadísticas de percentiles sobre **1646** líneas (o **1640** excluyendo todas las apariciones de esos `DocId`); la diferencia en p50 es mínima.

---

## Corrida 4 — job ~2100 (`persister-job-run.log`)

Muestra grande (~**1643** persistencias completas en este worker). Incluye trabajo nuevo respecto a la corrida 3 según el plan de job (~**1400** documentos nuevos + reencolados / solape; validar conteos con API/SQL).

### Hechos observados en el log

| Concepto | Valor |
|----------|--------|
| `Processing PersisterMessage` (inicios) | **1643** |
| `PersistAsync` completados | **1643** |
| `ProcessMessageAsync` (líneas totales) | **1646** (tres `DocId` duplicados por **fallo**; ver tabla comparativa) |
| `already exists, skipping` (mismo hash) | **2** líneas de log (**1** caso de idempotencia / reprocess) |
| `Failed to persist` (truncamiento SQL) | **3** (`Persons.LastName` — título de causa demasiado largo para la columna) |

### Arranque

| Fase | Tiempo aprox. |
|------|----------------|
| `StartupRecovery` | ~250 ms (`ResetRows=0`) |
| `EntityCacheWarmUp` | **~7014 ms** |

### Tiempos globales — `ProcessMessageAsync` (líneas *n* = **1646**)

| Métrica | Valor |
|---------|--------|
| Mín | **1317 ms** |
| **p50** | **5500 ms** |
| **p95** | **8559 ms** |
| **p99** | **10 574 ms** |
| Máx | **16 407 ms** |
| Media | **~5850 ms** |

Excluyendo las **6** líneas correspondientes a los **3** `DocId` fallidos: *n* = **1640**, p50 **~5498 ms**, p95/p99/máx **iguales** en la práctica al conjunto completo.

### Histograma aproximado (`ProcessMessageAsync`, *n* = 1646)

| Ventana | Cantidad |
|---------|----------|
| &lt; 3 s | **1** |
| 3–4 s | 32 |
| 4–5 s | **423** |
| 5–6 s | **605** |
| 6–7 s | 330 |
| 7–8 s | 141 |
| 8–10 s | 87 |
| 10–12 s | 24 |
| &gt; 12 s | **3** |

### `PersistAsync` (*n* = **1643**)

| Métrica | Valor |
|---------|--------|
| Mín / máx | **320** / **15 409 ms** |
| Media | **~4764 ms** |
| **p95** | **~7439 ms** |

### “Tubo” post-`PersistAsync`

Emparejando por `DocId` donde existen ambas fases: media **~1085 ms** (rango **964–3225 ms**), en línea con corridas 2–3 (~**1040–1047 ms**).

### Agregado de fases internas (líneas *n* ≈ 1642–1643 según fase)

| Fase | min | p50 | p95 | max | media aprox. |
|------|-----|-----|-----|-----|----------------|
| `BlobJsonPayload` | 156 | 165 | 482 | **3007** | **197** |
| `ContentHashLookup` | 156 | 166 | 204 | **2537** | **179** |
| `Statutes` | 0 | 164 | 508 | 2169 | **197** |
| `RulingRepositoryAdd` | 505 | 675 | 902 | **2430** | **728** |
| `Keywords` | 0 | 990 | 3317 | **7600** | **1284** |
| `LinkProceeding` | 482 | 1309 | 1552 | **4908** | **1140** |

**`ContentHashLookup`:** **29** mediciones **&gt; 500 ms**; **1** **&gt; 2 s** (~**2537 ms**) — reaparece cola leve respecto a corrida 3, sin el pico de **~4,9 s** de la corrida 2.

**Tesauro:** suma `ThesaurusResolveCount` **4124** sobre **1642** líneas (~**2,51** por doc); máximo **12** resoluciones en un documento.

**Cited-by:** **31** documentos con `CitedByCount` &gt; 0 (más carga inversa que en corridas 2–3).

### Outlier principal (corrida 4)

**`DocId=1c7120f7-f340-4e4a-8ad1-3a7191025fb0`** — `ProcessMessageAsync` **~16 407 ms**, `PersistAsync` **~15 409 ms**:

| Fase | Tiempo aprox. | Nota |
|------|----------------|------|
| **`Keywords`** | **~7600 ms** | `KeywordCount=12`, `ThesaurusResolveCount=12` |
| **`CitationsFirstSave`** | **~1142 ms** | `OutboundCitationCount=43` |
| **`CitedBy`** | **~1988 ms** | `CitedByCount=10` |
| `RulingRepositoryAdd` | ~2354 ms | |
| `RelatedGraph` | ~408 ms | `SumarioCount=7`, `VoteCount=2` |
| `ContentHashLookup` | ~158 ms | Normal |

Confirma que con **payloads densos** (muchas keywords, muchas citas y cited-by), el tiempo deja de estar dominado solo por `GetByContentHash` o por `Add` “vacío”, y pasa a **tesauro + grafo de citas + persistencia inversa**.

### Incidencias de datos (acción recomendada)

- **Truncamiento `Persons.LastName`:** tres documentos fallaron al persistir participaciones / personas con texto de causa judicial que excede el tamaño de columna. Mitigación típica: truncar/normalizar en código, ampliar columna, o usar campo `nvarchar(max)` / tabla de texto completo según diseño.
- **Hash duplicado:** un mensaje resuelto por rama “ya existe” (`already exists, skipping`) — comportamiento esperado con idempotencia por `ContentHash`.

### Lectura corrida 4

- La muestra confirma la **tendencia** de corrida 3: **p50 ~5,5 s**, **p95 ~8,6 s**, con cola más pesada que la corrida 2.
- **`Keywords`** sigue siendo el **principal multiplicador** cuando crecen `KeywordCount` y `ThesaurusResolveCount`.
- Con más **`CitedBy`** en el lote, **`CitedBy`** pasa a ser **relevante** en outliers (junto a **citas salientes**).
- **`ContentHashLookup`** tiene más eventos &gt; 500 ms que en corrida 3, pero sigue siendo **minoría**; conviene seguir monitoreando índice y plan en SQL.

---

## Corrida 5 — post cambios 1+2, con tesauro en BD (`persister-job-run-with-thesaurus.log`)

**Contexto:** después de cargar el tesauro en SQL (`ThesaurusTerms` + relaciones UF). El worker usa warm-up ampliado (términos + mapa sinónimo → preferido en memoria) y batch de keywords.

### Hechos observados en el log

| Concepto | Valor |
|----------|--------|
| `ProcessMessageAsync` / `PersistAsync` / `ContentHashLookup` / fase `Keywords` | **371** líneas cada una (mismo conjunto de mensajes) |
| Warm-up (línea `Entity cache warm-up finished`) | **5378** keywords, **4674** statutes, **2** courts, **41849** persons, **22467** thesaurus terms, **6778** UF synonym links |
| `EntityCacheWarmUp` (fase registrada) | **~99 785 ms (~100 s)** — coste de arranque; persistencia por mensaje se beneficia después. |
| Suma `ThesaurusResolveCount` (todas las líneas `Keywords`) | **365** (≈**0,98** por mensaje en promedio) |
| Mensajes con al menos una resolución (`ThesaurusResolveCount` &gt; 0) | **268 / 371** |
| Máximo `ThesaurusResolveCount` en un solo documento | **5** |

### Tiempos globales — `ProcessMessageAsync` (*n* = **371**)

| Métrica | Valor |
|---------|--------|
| Mín | **3324 ms** |
| **p50** | **4852 ms** |
| **p95** | **6254 ms** |
| **p99** | **7624 ms** |
| Máx | **8303 ms** |
| Media | **~4911 ms** |

### `PersistAsync` (*n* = **371**)

| Métrica | Valor |
|---------|--------|
| Mín / máx | **2261 / 7214 ms** |
| Media | **~3793 ms** |
| **p95** | **~5028 ms** |

### Agregado de fases internas (*n* = **371**)

| Fase | min | p50 | p95 | max | media aprox. |
|------|-----|-----|-----|-----|----------------|
| `ContentHashLookup` | 169 | 172 | 192 | 554 | **177** |
| `Keywords` | 0 | 345 | 1030 | 2757 | **354** |

- **`ContentHashLookup`:** **2** mediciones **&gt; 500 ms**; **0** **&gt; 2 s**.
- **`Keywords`:** **0** documentos con fase **&gt; 3000 ms** (frente a 12 en `persister-job-run-after-1-and-2.log` con *n* distinto y **sin** tesauro en caché).

### Lectura corrida 5

- Con tesauro en warm-up, **`Keywords`** baja fuerte en mediana y cola (**p50 ~345 ms**, **p95 ~1030 ms**, media **~354 ms**) respecto a corridas 3–4 y respecto a `after-1-and-2` sin términos en caché.
- **`ThesaurusResolveCount`** agregado cae (muchas coincidencias resueltas sin SQL de tres pasos por término).
- **Trade-off operativo:** warm-up **~100 s** al levantar el Persister; valorar en despliegue (aceptar, ventana de mantenimiento, o evolucionar a carga diferida si molesta).

---

## Corrida 6 — con tesauro, muestra ampliada (`persister-job-run-with-thesaurus-769.log`)

**Contexto:** misma base con tesauro que la corrida 5; job orientado a **~769** rulings. En este worker aparecen **758** líneas `ProcessMessageAsync` y **759** `PersistAsync` (diferencia de **1** mensaje: típico de doble línea en rama error/reintento o conteo desalineado; validar con `DocumentStageLogs` / job metrics).

### Hechos observados en el log

| Concepto | Valor |
|----------|--------|
| `ProcessMessageAsync` | **758** |
| `PersistAsync` | **759** |
| `ContentHashLookup` / fase `Keywords` | **759** / **758** |
| Warm-up (última línea `Entity cache warm-up finished`) | **5379** keywords, **4695** statutes, **2** courts, **42357** persons, **22467** thesaurus terms, **6778** UF synonym links |
| `EntityCacheWarmUp` (fase registrada) | **~4406 ms** — mucho menor que los **~100 s** de la corrida 5 en el mismo entorno; coherente con **buffer pool / planes ya calientes** u otro arranque (no implica menos datos cargados: el conteo de términos UF coincide). |
| Suma `ThesaurusResolveCount` | **753** (≈**0,99** por mensaje con `Keywords` en promedio) |
| Mensajes con `ThesaurusResolveCount` &gt; 0 | **535 / 758** |
| Máximo `ThesaurusResolveCount` en un documento | **4** |

### Tiempos globales — `ProcessMessageAsync` (*n* = **758**)

| Métrica | Valor |
|---------|--------|
| Mín | **1759 ms** |
| **p50** | **4947 ms** |
| **p95** | **6505 ms** |
| **p99** | **7698 ms** |
| Máx | **25 527 ms** |
| Media | **~5035 ms** |

### `PersistAsync` (*n* = **759**)

| Métrica | Valor |
|---------|--------|
| Mín / máx | **698 / 26 508 ms** |
| Media | **~3916 ms** |
| **p95** | **~5280 ms** |

### Agregado de fases internas (*n* ≈ **758–759**)

| Fase | min | p50 | p95 | max | media aprox. |
|------|-----|-----|-----|-----|----------------|
| `ContentHashLookup` | 169 | 172 | 200 | 843 | **181** |
| `Keywords` | 0 | 344 | 1032 | 2139 | **350** |

- **`ContentHashLookup`:** **7** mediciones **&gt; 500 ms**; **0** **&gt; 2 s**.
- **`Keywords`:** **0** documentos con fase **&gt; 3000 ms** (misma conclusión cualitativa que corrida 5).

### Comparación corrida 5 vs 6 (ambas con tesauro)

| Métrica | Corrida 5 (*n*=371) | Corrida 6 (*n*≈758) |
|---------|---------------------|----------------------|
| `ProcessMessageAsync` p50 | 4852 ms | **4947 ms** |
| `ProcessMessageAsync` p95 | 6254 ms | **6505 ms** |
| `ProcessMessageAsync` p99 | 7624 ms | **7698 ms** |
| `Keywords` p50 / p95 / media | 345 / 1030 / **354** ms | **344 / 1032 / 350** ms |
| `ThesaurusResolveCount` suma | 365 | **753** (escala ~2× con *n*) |

La distribución central (**p50/p95** de totales y de **`Keywords`**) **se mantiene** al duplicar la muestra: refuerza que la corrida 5 no era un artefacto de *n* pequeño.

### Outlier principal (corrida 6)

**`DocId=f973a6d9-3c69-4e43-9a4d-864b4b4afdee`** — `ProcessMessageAsync` **~25 527 ms**, `PersistAsync` **~24 395 ms**:

| Fase | Tiempo aprox. | Nota |
|------|----------------|------|
| **`LinkProceeding`** | **~8300 ms** | Expediente + partes |
| **`RulingRepositoryAdd`** | **~7420 ms** | Inserción grafo (incluye `Executed DbCommand` **~3549 ms** en un solo batch) |
| **`CitationsFirstSave`** | **~3978 ms** | `OutboundCitationCount=5` |
| **`Keywords`** | **~2139 ms** | `KeywordCount=3`, `ThesaurusResolveCount=1` — el log muestra **dos** `SELECT` sobre `ThesaurusTerms` por etiqueta **~1 s** cada uno (fallback SQL); posible **desajuste de etiqueta** vs. claves del mapa en memoria (p. ej. acentos / normalización `PRISION PREVENTIVA`). |
| `Statutes` | ~1053 ms | `StatuteCount=1` |
| `Persons` | ~983 ms | `PersonCount=3` (un `SELECT` a `Persons` **~608 ms**) |
| `ContentHashLookup` | ~171 ms | Normal |

El máximo global **no** viene del tesauro en agregado sino de **grafo + expediente + citas** en un documento pesado; el tesauro aporta solo una parte (queries lentas puntuales por *cache miss* de etiqueta).

### Lectura corrida 6

- **Replica** el patrón de la corrida 5 en **`Keywords`** y percentiles centrales de **`ProcessMessageAsync`** con el doble de mensajes.
- **`EntityCacheWarmUp`** puede variar mucho entre arranques (**~4,4 s** vs **~100 s**); conviene anotar siempre la línea de warm-up y el contexto (primera subida del día vs. reinicio corto).
- El outlier **~25 s** prioriza **trabajo futuro** en expediente/citas/`RulingRepositoryAdd`, no en invalidar el punto 2.

---

## Corrida 7 — con tesauro, muestra ~1052 (`persister-job-run-with-thesaurus-1052.log`)

**Contexto:** misma línea de producto que corridas 5–6 (tesauro en warm-up + batch keywords). Incluye mejoras posteriores: **normalización de etiquetas** en caché (coincidencia con variantes de acento/espacios) y **resolución cited-by en un solo `SELECT`** por mensaje. Job orientado a **~1052** documentos; en este worker aparecen **1032** líneas `ProcessMessageAsync` y **`PersistAsync`** (alineadas). La fase **`Keywords`** del strategy aparece **1031** veces (**−1** respecto al total de mensajes: un mensaje sin esa línea de fase o conteo desalineado; validar con `DocumentStageLogs` / métricas del job).

### Hechos observados en el log

| Concepto | Valor |
|----------|--------|
| `ProcessMessageAsync` | **1032** |
| `PersistAsync` | **1032** |
| `ContentHashLookup` / fase `Keywords` (strategy) | **1032** / **1031** |
| Warm-up (línea `Entity cache warm-up finished`) | **5384** keywords, **4721** statutes, **2** courts, **43 376** persons, **22 467** thesaurus terms, **6778** UF synonym links |
| `EntityCacheWarmUp` (fase registrada) | **~6390 ms (~6,4 s)** — sin el outlier de **~100 s** de la corrida 5; coherente con **SQL ya caliente** u otro contexto de arranque. |
| Suma `ThesaurusResolveCount` (líneas `Keywords`) | **594** (≈**0,58** por mensaje con fase `Keywords`, denominador **1031**) |
| Mensajes con `ThesaurusResolveCount` &gt; 0 | **474 / 1031** |
| Máximo `ThesaurusResolveCount` en un documento | **3** |
| `KeywordCount` (media / máx. en fase `Keywords`) | **~2,26** / **11** |
| Fase `Keywords` en **0 ms** | **553 / 1031** mensajes (caché + batch muy baratos en esos casos) |

### Tiempos globales — `ProcessMessageAsync` (*n* = **1032**)

| Métrica | Valor |
|---------|--------|
| Mín | **1399 ms** |
| **p50** | **4818 ms** |
| **p95** | **6526 ms** |
| **p99** | **7554 ms** |
| Máx | **9943 ms** |
| Media | **~4939 ms** |

### `PersistAsync` (*n* = **1032**)

| Métrica | Valor |
|---------|--------|
| Mín / máx | **344 / 8884 ms** |
| Media | **~3811 ms** |
| **p95** | **~5279 ms** |

### Agregado de fases internas (*n* ≈ **1031** para `Keywords`; **1032** para `ContentHashLookup`)

| Fase | min | p50 | p95 | max | media aprox. |
|------|-----|-----|-----|-----|----------------|
| `ContentHashLookup` | 169 | 171 | 200 | 761 | **179** |
| `Keywords` | 0 | 0 | 850 | 2025 | **211** |

- **`ContentHashLookup`:** **6** mediciones **&gt; 500 ms**; **0** **&gt; 2 s**.
- **`Keywords`:** **0** documentos con fase **&gt; 3000 ms**. En documentos con tiempo **&gt; 0** en `Keywords` (*n* = **478**), **p50 ≈ 344 ms** y **p95 ≈ 850 ms** (la mediana global **0 ms** refleja que **&gt; 50 %** de mensajes registran **0 ms** en esa fase).

### Lectura corrida 7

- **Escala:** *n* **~40 %** mayor que la corrida 6; **`ProcessMessageAsync` p50/p95** siguen en el **mismo corredor** que C5/C6 (**~4,8–4,9 s** y **~6,25–6,5 s**), lo que refuerza estabilidad con muestra más grande.
- **Carga de tesauro:** **~0,58** resoluciones por mensaje (`Keywords`) frente a **~0,98–0,99** en C5/C6 — menor presión SQL de tesauro en este lote (mezcla de documentos + coincidencias normalizadas en caché + keywords ya enlazados en BD).
- **Warm-up ~6,4 s** añade otro datapoint **favorable** frente a **~100 s** (corrida 5) para planificar despliegue.
- **Cuellos residuales** (como en C6): **`LinkProceeding`** y **`RulingRepositoryAdd`** siguen dominando parte del tiempo por documento cuando el expediente o el grafo son pesados; no invalidan el trabajo en keywords/tesauro.

---

## Evolución de tiempos (C2 → C4, post–cambios 1+2, corridas 5–7)

Tabla **solo con fines comparativos**: cambian el **tamaño del job** (*n*), la **mezcla** de fallos y a veces el **estado de la base** (tesauro en warm-up o no). Los totales **`ProcessMessageAsync`** no son un A/B limpio del código; lo más comparable entre corridas es **`ContentHashLookup`**, **`Keywords`** y la **carga de tesauro** (suma `ThesaurusResolveCount` / mensajes con fase `Keywords`). En **C7**, el **p50 global** de `Keywords` es **0 ms** porque más de la mitad de mensajes registran **0 ms** en esa fase; el **p50 condicional** (solo mensajes con `Keywords` &gt; 0 ms) es **~344 ms**, alineado con C5/C6.

| Corrida | *n* (`ProcessMessageAsync`) | PM p50 | PM p95 | PM media | `PersistAsync` p95 | `Keywords` p50 | `Keywords` media | Σ `ThesaurusResolveCount` / *n*† | `ContentHashLookup` p50 | `ContentHashLookup` p95 |
|---------|-----------------------------|--------|--------|----------|-------------------|----------------|------------------|----------------------------------|---------------------------|---------------------------|
| C2 | 184 | 4953 ms | 7113 ms | ~5205 ms | ~6083 ms | ~940 ms | ~1016 ms | **385 / 184 ≈ 2,09** | ~160 ms | ~177 ms |
| C3 | 493 | 5199 ms | 8192 ms | ~5549 ms | ~7097 ms | ~951 ms | ~1218 ms | **1185 / 493 ≈ 2,40** | ~160 ms | ~187 ms |
| C4 | 1646 | 5500 ms | 8559 ms | ~5850 ms | ~7439 ms | ~990 ms | ~1284 ms | **4124 / 1642 ≈ 2,51** | ~166 ms | ~204 ms |
| Post 1+2 (sin tesauro en warm-up) | 576 | 6036 ms | 8219 ms | ~6350 ms | ~6947 ms | ~744 ms | ~1028 ms | **1471 / 576 ≈ 2,55** | ~185 ms | ~216 ms |
| C5 (con tesauro) | 371 | 4852 ms | 6254 ms | ~4911 ms | ~5028 ms | ~345 ms | ~354 ms | **365 / 371 ≈ 0,98** | ~172 ms | ~192 ms |
| C6 (con tesauro) | 758 | 4947 ms | 6505 ms | ~5035 ms | ~5280 ms | ~344 ms | ~350 ms | **753 / 758 ≈ 0,99** | ~172 ms | ~200 ms |
| C7 (con tesauro) | 1032 | 4818 ms | 6526 ms | ~4939 ms | ~5279 ms | **0 ms** (p50 ≈ **344 ms** si dur. &gt; 0) | ~211 ms | **594 / 1031 ≈ 0,58** | ~171 ms | ~200 ms |

†Denominador: líneas de fase `Keywords` del strategy; en **C7** es **1031** (ver sección corrida 7).

**Lectura rápida**

- **C2 → C4:** sube el **p50** de `ProcessMessageAsync` (lotes más grandes y más densos), no implica por sí regresión de código.
- **Post 1+2 sin tesauro en RAM:** **`Keywords`** ya mejora vs C3/C4 (batch); los **totales** del mensaje en ese log son **peores** por mezcla de lote / sin tesauro en caché.
- **C5/C6/C7:** **`Keywords`** y **resoluciones de tesauro por mensaje** caen fuerte vs C3/C4; **`PersistAsync` p95** muy por debajo de C3/C4; **`ContentHashLookup`** estable en **~170–185 ms** en p50. **C7** confirma el patrón con **más mensajes** y menor **Σ `ThesaurusResolveCount` / mensaje** en este lote.

---

## Otras mejoras pensadas (Persister / pipeline)

Resumen de lo ya listado en `persister-worker-analysis-deferred.md` (tabla de propuestas) y de la **hoja “qué sigue”** del análisis de benchmark, **después** de cerrar puntos 1–2 (hash + keywords/tesauro):

| Prioridad razonable | Mejora | Notas |
|--------------------|--------|--------|
| **Alta (siguiente foco)** | **Cited-by + muchas citas salientes** — lecturas en **batch** donde hoy hay bucle + query por ítem (`FindByAnalysisIdAsync`, etc.). | Corrida 4 y outlier C6 muestran **`CitationsFirstSave`**, **`CitedBy`** y **`LinkProceeding`** como multiplicadores cuando el payload es denso. |
| **Alta** | **Normalización de etiquetas de tesauro** (acentos, mayúsculas, espacios) para que el mapa en memoria coincida con el payload y bajen los *fallback* SQL puntuales. | Ejemplo en log C6: `PRISION PREVENTIVA` con `SELECT` ~1 s al no matchear caché. |
| **Media** | **Batch / menos round-trips** en otras dimensiones: **personas**, **normas** (mismo patrón que keywords por mensaje). | Reutilizar idea de `GetOrCreateBatchAsync` donde el grafo lo permita. |
| **Media–alta** | **Fusionar `SaveChanges`** en tramos del ruling donde las FK lo permitan. | Alto impacto potencial; **alto riesgo** de regresiones; requiere pruebas fuertes. |
| **Media** | **Tuning operativo:** `PollIntervalSeconds`, `EmptyPollIntervalSeconds`, visibilidad de cola, `BatchSize` (solo recepción; no paraleliza persist). | Bajo esfuerzo; impacto en latencia entre mensajes. |
| **Media** | **Warm-up:** carga diferida, en segundo plano, o snapshot si **~100 s** de arranque no es aceptable en producción. | Trade-off ya observado entre C5 y C6 según SQL caliente. |
| **Media** | **Contrato de payload** / reducción de tamaño (menos redundancia, compresión upstream). | Menos bytes blob + JSON → menos CPU y I/O por mensaje. |
| **Baja–media (observabilidad)** | **Dashboard / alertas** p95, throughput, profundidad de cola; seguir **`GET .../jobs/{id}/metrics`**. | Ya alineado con filas 1–2 de la tabla en `persister-worker-analysis-deferred.md`. |
| **Arquitectura (más tarde)** | **Diferir parte del grafo** a post-indexer; **escalar** varias instancias Persister con partición de cola. | Muy alto esfuerzo o riesgo de carreras. |

**Hecho o en gran parte hecho respecto a esa lista:** índice único / proyección en **`ContentHashLookup`** (punto 1); **batch keywords + tesauro en warm-up** (punto 2); parte de “aprovechar warm-up / reducir round-trips” en dimensiones ya tocadas.

---

## Conclusiones globales (corridas 1–4)

Las siguientes conclusiones resumen **corridas 1–4**. Las **corridas 5, 6 y 7** (tesauro en warm-up; ver secciones anteriores) confirman el impacto del punto 2 en `Keywords` y el trade-off de warm-up (duración **variable** entre arranques: **~4 s** a **~100 s** según contexto). La **corrida 6** valida la 5 con muestra **~2×** (*n* ≈ 758); la **corrida 7** (~1032 mensajes) refuerza estabilidad y añade contexto post–mejoras de caché/cited-by. Para una **tabla única de evolución** C2 → C4 → post–1+2 → C5–C7 y la lista de **mejoras siguientes**, ver las secciones **Evolución de tiempos** y **Otras mejoras pensadas** más arriba.

1. **Orden de magnitud estable:** el “tubo” fijo post-`PersistAsync` ronda **~1,0–1,1 s** por mensaje en todas las corridas con muestra grande; el coste variable está **dentro de `PersistAsync`**.
2. **Cuellos variables (por documento):** en orden de importancia según los logs agregados y outliers:
   - **`Keywords` + tesauro** (`ResolveAsync`): p95 alto y máximos extremos cuando hay muchas keywords.
   - **`Statutes`** y **`RulingRepositoryAdd`**: picos en cargas pesadas o grafos grandes; en la muestra de 1643+ suelen tener **p95 acotado** salvo outliers.
   - **`LinkProceeding`:** contribución estable del orden de **~1,1–1,3 s** en mediana.
   - **`ContentHashLookup`:** mayormente **~160 ms**; **picos aislados** (segundos) posibles — revisar **índice**, **plan** y **contención** en SQL; frecuencia baja pero impacto alto cuando ocurre.
   - **`CitedBy` + citas:** pasan a importar cuando el lote incluye muchos **cited-by** y muchas **citas salientes** (corrida 4).
3. **Calidad de datos:** aparecieron **fallos reales** (truncamiento en `Persons.LastName`) que el Persister no puede “asumir”; hay que **corregir esquema o normalización** para no perder documentos en producción.
4. **Idempotencia:** mensajes con ruling ya existente por hash se manejan con **skip** corto; no distorsionan mucho las métricas de tiempo.
5. **Metodología:** reutilizar siempre **un archivo de log por corrida** (nombre distinto) para no sobrescribir históricos; alinear *n* del log con **métricas del job** y **`DocumentStageLogs`**.

---

## Referencias de código

- Worker: `backend/src/workers/LegalAiAr.Worker.Persister/PersisterWorkerService.cs`
- Estrategia ruling: `backend/src/workers/LegalAiAr.Worker.Persister/Strategies/RulingPersistStrategy.cs`
- Métricas por job (API): `GET /api/admin/jobs/{id}/metrics` — agregados desde `DocumentStageLogs`
- Diseño / lista de fases: `docs/design/persister-worker-analysis-deferred.md`
