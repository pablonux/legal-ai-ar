# Endpoint: getEnlacesAnalisis (NUEVO — no usado por parser)

**URL**: `{baseUrl}/enlaces/getEnlacesAnalisis.html?idAnalisis={analysisId}`
**Cache key**: `_cache/csjn/api/getEnlacesAnalisis/{analysisId}.json`
**Tamano**: 2 bytes (vacio) a ~2 KB
**Formato**: JSON array `[]` cuando vacio, JSON array de objetos cuando tiene datos
**Poblacion**: 9/1,811 (0.5%) tienen datos — 1,802 vacios

## Descripcion

Contiene enlaces externos asociados al analisis. En la practica, casi exclusivamente son URLs a dictamenes del Ministerio Publico Fiscal (MPF) publicados en mpf.gob.ar. La poblacion extremadamente baja (0.5%) sugiere que solo se cargan para un subconjunto muy reducido de fallos.

## Estructura (cuando hay datos)

Cada item del array:

| Campo | Tipo | Ejemplo | Valor KB |
|-------|------|---------|----------|
| `interno` | number | `2` | Orden del enlace |
| `descripcion` | string | `"DICTAMEN DE LA PROCURACION"` | Tipo de enlace |
| `enlace` | string | `"http://www.mpf.gob.ar/dictamenes/2013/IGarcia/diciembre/Consumidores_Fin_C_434_L_XLVII.pdf"` | **URL directa al PDF** |
| `idAnalisis` | number | `718919` | ID del analisis asociado |
| `usuarioCreacion` | string | `"JURC5F"` | Audit |
| `usuarioModificacion` | string | `"JURC5F"` | Audit |
| `fechaCreacion` | number | `201502111321` | Audit (yyyyMMddHHmm) |
| `fechaModificacion` | number | `201502111321` | Audit |

## Ejemplo real (718919.json)

```json
[
  {
    "interno": 2,
    "descripcion": "DICTAMEN DE LA PROCURACION",
    "enlace": "http://www.mpf.gob.ar/dictamenes/2013/IGarcia/diciembre/Consumidores_Fin_C_434_L_XLVII.pdf",
    "idAnalisis": 718919
  },
  {
    "interno": 1,
    "descripcion": "DICTAMEN DE LA PROCURACION",
    "enlace": "http://www.mpf.gob.ar/dictamenes/2014/IGarcia/febrero/Asoc_Prot_Cons_A_566_L_XLVIII.pdf",
    "idAnalisis": 718919
  }
]
```

## Valor para la KB

### Valor cuando presente (alto, pero muy raro)

Los URLs son enlaces directos a PDFs de dictamenes del MPF. En un flujo futuro, estos PDFs podrian:
1. Descargarse automaticamente
2. Extraerse el texto
3. Procesarse con LLM para obtener el contenido del dictamen (opinion, recomendacion, concordancia con la Corte)

### Limitacion: poblacion 0.5%

Con solo 9 de 1,811 fallos teniendo datos, el impacto directo es minimo. El endpoint es util como complemento de `getDictamenesAnalisis` (61%), pero no puede ser la fuente principal de deteccion de dictamenes.

### Patron de `descripcion`

En todas las muestras con datos, `descripcion` es siempre `"DICTAMEN DE LA PROCURACION"`. No se observaron otros tipos de enlaces.

## Resumen de acciones

| Prioridad | Accion | Impacto |
|-----------|--------|---------|
| **BAJA** | Agregar endpoint al parser | Solo 0.5% de fallos tienen datos |
| **BAJA** | Almacenar URLs de dictamenes | Referencia para descarga futura de PDFs MPF |
| **FUTURA** | Descargar y procesar PDFs MPF | Extraccion de texto de dictamenes (scope futuro) |
