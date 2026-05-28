# Ontología del Sistema Legal Argentino
> Especificación formal para sistemas de software — Versión 1.0 | 2026

---

## Tabla de Contenidos

1. [Introducción y Alcance](#1-introducción-y-alcance)
2. [Jerarquía de Fuentes del Derecho](#2-jerarquía-de-fuentes-del-derecho)
3. [Norma Jurídica](#3-norma-jurídica)
4. [Sujetos de Derecho](#4-sujetos-de-derecho)
5. [Acto Jurídico y Hecho Jurídico](#5-acto-jurídico-y-hecho-jurídico)
6. [Relación Jurídica y Derechos Subjetivos](#6-relación-jurídica-y-derechos-subjetivos)
7. [Derecho Penal](#7-derecho-penal)
8. [Proceso Judicial](#8-proceso-judicial)
9. [Órganos del Estado y Sistema Judicial](#9-órganos-del-estado-y-sistema-judicial)
10. [Derecho Laboral y Seguridad Social](#10-derecho-laboral-y-seguridad-social)
11. [Derecho de Familia](#11-derecho-de-familia)
12. [Derecho Tributario](#12-derecho-tributario)
13. [Derecho Administrativo](#13-derecho-administrativo)
14. [Resumen de Relaciones entre Clases](#14-resumen-de-relaciones-entre-clases)
15. [Instancias Clave del Sistema Legal Argentino](#15-instancias-clave-del-sistema-legal-argentino)
16. [Lineamientos para Implementación en Software](#16-lineamientos-para-implementación-en-software)

---

## 1. Introducción y Alcance

Esta ontología define la estructura conceptual del sistema legal de la República Argentina, diseñada como base formal para el desarrollo de sistemas de software jurídicos. Establece las clases, propiedades, relaciones y restricciones que permiten representar, consultar y razonar sobre el ordenamiento jurídico argentino.

### 1.1 Propósito

- Proveer un modelo conceptual compartido y reutilizable del derecho argentino.
- Facilitar la interoperabilidad entre sistemas de información jurídica.
- Habilitar el razonamiento automatizado sobre normas, sujetos y relaciones legales.
- Servir como base para motores de búsqueda semántica de legislación y jurisprudencia.

### 1.2 Alcance

La ontología cubre la totalidad del ordenamiento jurídico argentino, incluyendo:

- Derecho Constitucional
- Derecho Civil y Comercial
- Derecho Penal
- Derecho Laboral y de la Seguridad Social
- Derecho Administrativo
- Derecho Procesal
- Derecho de Familia
- Derecho Tributario

### 1.3 Convenciones de Nomenclatura

| Elemento | Convención | Ejemplo |
|---|---|---|
| Clases | PascalCase | `PersonaHumana`, `NormaJuridica` |
| Propiedades de objeto (relaciones) | camelCase con prefijo | `tieneArticulo`, `esDictadaPor` |
| Propiedades de datos | camelCase descriptivo | `fechaSancion`, `numeroNorma` |
| Instancias | camelCase o string | `"26.994"`, `"CCCN"` |

---

## 2. Jerarquía de Fuentes del Derecho

El sistema jurídico argentino organiza sus fuentes según la **Pirámide de Kelsen**, reconocida por el artículo 31 de la Constitución Nacional.

### 2.1 Clase: `FuenteDelDerecho`

Clase raíz que representa cualquier fuente normativa reconocida por el ordenamiento argentino.

| Propiedad | Tipo | Descripción |
|---|---|---|
| `nivelJerarquico` | `Integer (1-5)` | Posición en la pirámide normativa |
| `denominacion` | `String` | Nombre oficial de la fuente |
| `obligatoriedad` | `Enum {vinculante, orientadora}` | Fuerza normativa |
| `ambitoTerritorial` | `Enum {nacional, provincial, municipal}` | Alcance geográfico |

### 2.2 Subclases de `FuenteDelDerecho`

#### 2.2.1 `NormaConstitucional` — Nivel 1

Norma suprema del ordenamiento. Incluye la Constitución Nacional (1853/60) y los Tratados con jerarquía constitucional (art. 75 inc. 22 CN).

- Constitución Nacional
- Convención Americana sobre Derechos Humanos
- Pacto Internacional de Derechos Civiles y Políticos
- Convención sobre los Derechos del Niño
- CEDAW (Convención sobre Eliminación de Discriminación contra la Mujer)

#### 2.2.2 `TratadoInternacional` — Nivel 2

Tratados internacionales sin jerarquía constitucional pero superiores a las leyes nacionales (art. 75 inc. 22 y 24 CN). Aprobados por el Congreso Nacional.

#### 2.2.3 `LeyNacional` — Nivel 3

Normas dictadas por el Congreso de la Nación.

- **Ley ordinaria**: mayoría simple de ambas cámaras.
- **Ley especial**: requiere mayorías calificadas.
- **Código**: cuerpo normativo sistemático (CCCN, Código Penal, etc.).

#### 2.2.4 `DecretoLey` — Nivel 3

Normas con fuerza de ley dictadas por el Poder Ejecutivo en circunstancias excepcionales. Incluye DNU (art. 99 inc. 3 CN).

#### 2.2.5 `NormaProvincial` — Nivel 3-4

Normas emanadas de las provincias en ejercicio de sus poderes reservados (art. 121 CN). Incluye constituciones provinciales, leyes provinciales y decretos provinciales.

#### 2.2.6 `NormaMunicipal` — Nivel 4

Normas dictadas por municipios en el marco de su autonomía (art. 123 CN).

- Ordenanza municipal
- Resolución del Concejo Deliberante
- Decreto del intendente

#### 2.2.7 `ActoAdministrativo` — Nivel 4-5

Declaración unilateral de un órgano del Estado en ejercicio de función administrativa que produce efectos jurídicos directos e individuales (Ley 19.549).

#### 2.2.8 `Jurisprudencia`

Conjunto de fallos y resoluciones de los tribunales. Los fallos plenarios son de aplicación obligatoria para los jueces inferiores del fuero.

#### 2.2.9 `Costumbre`

Práctica uniforme, constante y generalizada con convicción de obligatoriedad. Opera como fuente supletoria, especialmente en derecho comercial y de familia.

#### 2.2.10 `Doctrina`

Opiniones de juristas y académicos. Fuente orientadora sin fuerza vinculante directa.

---

## 3. Norma Jurídica

La clase `NormaJuridica` representa cualquier disposición normativa concreta dentro del ordenamiento argentino.

### 3.1 Propiedades de `NormaJuridica`

| Propiedad | Tipo de Dato | Cardinalidad | Descripción |
|---|---|---|---|
| `numeroNorma` | `String` | 1..1 | Número oficial (ej. `"26.994"`) |
| `denominacion` | `String` | 1..1 | Nombre completo de la norma |
| `nombreComun` | `String` | 0..1 | Nombre coloquial (ej. `"Código Civil y Comercial"`) |
| `fechaSancion` | `Date` | 0..1 | Fecha de sanción legislativa |
| `fechaPromulgacion` | `Date` | 0..1 | Fecha de promulgación por el Ejecutivo |
| `fechaPublicacion` | `Date` | 0..1 | Fecha en el Boletín Oficial |
| `fechaVigencia` | `Date` | 0..1 | Fecha de entrada en vigor |
| `estaVigente` | `Boolean` | 1..1 | Estado de vigencia actual |
| `textoCompleto` | `String (URL)` | 0..1 | Enlace al texto oficial |
| `ramaDelDerecho` | `Enum` | 1..N | Clasificación por rama jurídica |
| `ambitoTerritorial` | `Enum` | 1..1 | Alcance geográfico de aplicación |

### 3.2 Relaciones de `NormaJuridica`

| Relación | Dominio | Rango | Descripción |
|---|---|---|---|
| `modificaA` | `NormaJuridica` | `NormaJuridica` | Modifica parcial o totalmente a otra norma |
| `derogaA` | `NormaJuridica` | `NormaJuridica` | Deroga expresa o tácitamente a otra norma |
| `reglamentaA` | `NormaJuridica` | `NormaJuridica` | Norma inferior reglamenta a norma superior |
| `complementaA` | `NormaJuridica` | `NormaJuridica` | Complementa sin derogar a otra norma |
| `esDictadaPor` | `NormaJuridica` | `OrganoEstatal` | Órgano del Estado que dictó la norma |
| `regulaA` | `NormaJuridica` | `InstitucionJuridica` | Institución jurídica que regula |
| `esPublicadaEn` | `NormaJuridica` | `MedioOficialPublicacion` | Boletín o medio de publicación |
| `tieneArticulo` | `NormaJuridica` | `Articulo` | Artículos que componen la norma |

### 3.3 Composición Interna: `Articulo`

Un artículo es la unidad estructural básica de una norma jurídica.

| Propiedad | Tipo | Descripción |
|---|---|---|
| `numeroArticulo` | `String` | Número o identificador del artículo |
| `textoNormativo` | `String` | Contenido textual completo |
| `esVigente` | `Boolean` | Si el artículo está vigente o fue derogado |
| `tieneInciso` | `Inciso` | Subdivisiones del artículo |
| `tienePárrafo` | `Párrafo` | Párrafos que lo componen |

---

## 4. Sujetos de Derecho

Los sujetos de derecho son aquellos a quienes el ordenamiento atribuye capacidad para ser titulares de derechos y obligaciones (arts. 19-50 CCCN).

### 4.1 Clase: `SujetoDeDeRecho` (raíz)

| Propiedad | Tipo | Descripción |
|---|---|---|
| `identificador` | `String` | CUIT, CUIL, DNI o identificador único |
| `domicilio` | `Domicilio` | Domicilio legal o real |
| `tieneCapacidad` | `CapacidadJuridica` | Capacidad jurídica del sujeto |

### 4.2 `PersonaHumana`

Todo ser humano goza de la aptitud para ser titular de derechos desde la concepción (art. 19 CCCN). La existencia termina con la muerte.

| Propiedad | Tipo | Descripción |
|---|---|---|
| `apellido` | `String` | Apellido/s de la persona |
| `nombre` | `String` | Nombre/s de pila |
| `fechaNacimiento` | `Date` | Fecha de nacimiento |
| `fechaFallecimiento` | `Date?` | Fecha de defunción, si corresponde |
| `numeroDNI` | `String` | Número de Documento Nacional de Identidad |
| `CUIL` | `String` | Clave Única de Identificación Laboral |
| `estadoCivil` | `Enum` | `soltero / casado / divorciado / viudo / unidoConvivencialmente` |
| `nacionalidad` | `String` | Nacionalidad/es de la persona |
| `domicilioReal` | `Domicilio` | Lugar donde reside habitualmente |
| `domicilioLegal` | `Domicilio` | Domicilio legal establecido por la ley |
| `tieneCapacidadDeEjercicio` | `Boolean` | Si tiene capacidad de ejercicio plena |

#### 4.2.1 Capacidad de Ejercicio

La capacidad de ejercicio es plena a los 18 años (mayoría de edad). Existen restricciones:

- **Menores de 13 años**: sin capacidad de ejercicio.
- **Adolescentes (13-18)**: capacidad progresiva según el acto jurídico.
- **Personas con capacidad restringida**: según sentencia judicial (art. 32 CCCN).
- **Personas incapaces**: declaradas judicialmente.

### 4.3 `PersonaJuridica`

Entes a los cuales el ordenamiento les confiere aptitud para adquirir derechos y contraer obligaciones (art. 141 CCCN).

| Propiedad | Tipo | Descripción |
|---|---|---|
| `denominacion` | `String` | Razón social o nombre |
| `CUIT` | `String` | Clave Única de Identificación Tributaria |
| `fechaConstitucion` | `Date` | Fecha de constitución o fundación |
| `tipoPersonaJuridica` | `Enum` | `publica / privada` |
| `objetoSocial` | `String` | Actividad o fin para el que fue creada |
| `domicilioLegal` | `Domicilio` | Domicilio legal registrado |
| `tieneOrganosGobierno` | `OrganoPersonaJuridica` | Órganos de administración y fiscalización |
| `estaInscriptaEn` | `RegistroPublico` | Registro donde está inscripta |

#### 4.3.1 Personas Jurídicas Públicas

- Estado Nacional
- Provincias
- Ciudad Autónoma de Buenos Aires
- Municipios
- Entidades autárquicas (ANSES, ARCA, BCRA, etc.)
- Universidades Nacionales
- Iglesia Católica Apostólica Romana

#### 4.3.2 Personas Jurídicas Privadas

- **Sociedades** (SA, SRL, SAS, etc.) — Ley 19.550
- **Asociaciones Civiles** — art. 168 CCCN
- **Simples Asociaciones** — art. 187 CCCN
- **Fundaciones** — Ley 19.836
- **Mutuales** — Ley 20.321
- **Cooperativas** — Ley 20.337
- **Consorcios de Propiedad Horizontal**
- **Fideicomisos** con personería jurídica

---

## 5. Acto Jurídico y Hecho Jurídico

### 5.1 `HechoJuridico`

Acontecimiento que produce el nacimiento, modificación o extinción de relaciones jurídicas (art. 257 CCCN).

| Subclase | Descripción | Ejemplo |
|---|---|---|
| `HechoNatural` | Producido por la naturaleza, sin intervención humana | Nacimiento, muerte, paso del tiempo |
| `HechoHumanoVoluntario` | Acto humano con discernimiento, intención y libertad | Contratos, delitos dolosos |
| `HechoHumanoInvoluntario` | Sin discernimiento o libertad plena | Actos del demente no declarado |

### 5.2 `ActoJuridico`

Acto voluntario lícito que tiene por fin inmediato la adquisición, modificación o extinción de relaciones jurídicas (art. 259 CCCN).

| Propiedad | Tipo | Descripción |
|---|---|---|
| `fecha` | `Date` | Fecha de celebración del acto |
| `lugar` | `String` | Lugar de celebración |
| `esOneroso` | `Boolean` | Si tiene contraprestación económica |
| `esCondicional` | `Boolean` | Si está sujeto a condición |
| `esModal` | `Boolean` | Si tiene cargo o modo |
| `requiereForma` | `TipoForma` | Forma requerida por la ley |
| `esNulo` | `Boolean` | Si fue declarado nulo |
| `causaDeNulidad` | `Enum` | Vicio que genera la nulidad |

#### 5.2.1 Vicios del Acto Jurídico

| Vicio | Tipo de Nulidad | Norma |
|---|---|---|
| Error esencial | Relativa | Art. 265-270 CCCN |
| Dolo | Relativa | Art. 271-275 CCCN |
| Violencia (fuerza/intimidación) | Relativa | Art. 276-278 CCCN |
| Lesión | Relativa | Art. 332 CCCN |
| Simulación | Absoluta o relativa | Art. 333-337 CCCN |
| Fraude | Inoponibilidad | Art. 338-342 CCCN |
| Objeto ilícito | Absoluta | Art. 279 CCCN |
| Causa ilícita | Absoluta | Art. 281-282 CCCN |

### 5.3 `Contrato`

Acto jurídico mediante el cual dos o más partes manifiestan su consentimiento para crear, regular, modificar, transferir o extinguir relaciones jurídicas patrimoniales (art. 957 CCCN).

| Propiedad | Tipo | Descripción |
|---|---|---|
| `tipoContrato` | `Enum` | Tipo de contrato según clasificación legal |
| `partesContratantes` | `SujetoDeDeRecho[]` | Partes que celebran el contrato |
| `objetoContractual` | `String` | Objeto del contrato |
| `precioOContraprestacion` | `Decimal` | Valor económico pactado (si aplica) |
| `plazo` | `Duration` | Duración o plazo del contrato |
| `formaCelebracion` | `Enum` | `escrito / verbal / electrónico / escritura pública` |
| `jurisdiccionAplicable` | `Jurisdiccion` | Jurisdicción para disputas |
| `leyAplicable` | `NormaJuridica` | Ley que rige el contrato |

#### 5.3.1 Principales Tipos de Contrato (CCCN)

| Contrato | Artículos CCCN | Características |
|---|---|---|
| Compraventa | 1123-1169 | Transferencia de dominio a cambio de precio en dinero |
| Permuta | 1172-1175 | Intercambio de cosas o derechos |
| Locación | 1187-1250 | Uso y goce de cosa a cambio de precio |
| Mandato | 1319-1334 | Gestión de negocios por cuenta del mandante |
| Fianza | 1574-1598 | Garantía personal de obligación ajena |
| Donación | 1542-1573 | Transferencia gratuita de cosa o derecho |
| Mutuo | 1525-1532 | Préstamo de cosas fungibles |
| Comodato | 1533-1541 | Préstamo de uso gratuito |
| Cesión de derechos | 1614-1631 | Transmisión de derechos a un tercero |
| Contrato de trabajo | Ley 20.744 | Relación laboral subordinada y dependiente |
| Seguro | Ley 17.418 | Cobertura de riesgos a cambio de prima |
| Franquicia | 1512-1524 | Licencia de marca y sistema de negocios |

---

## 6. Relación Jurídica y Derechos Subjetivos

### 6.1 `RelacionJuridica`

Vínculo entre sujetos de derecho, reglado por el ordenamiento, que asigna a uno la posición de titular de un derecho y al otro la de obligado.

| Propiedad | Tipo | Descripción |
|---|---|---|
| `sujetoActivo` | `SujetoDeDeRecho` | Titular del derecho (acreedor, propietario) |
| `sujetosPasivos` | `SujetoDeDeRecho[]` | Obligados (deudor, terceros) |
| `objetoRelacion` | `String` | Bien, conducta o abstención sobre la que recae |
| `fuenteRelacion` | `FuenteDelDerecho` | Norma o hecho que genera la relación |
| `esPatrimonial` | `Boolean` | Si tiene contenido económico |
| `esTransmisible` | `Boolean` | Si puede transmitirse a terceros |

### 6.2 Derechos Reales

Derechos que se ejercen sobre una cosa (art. 1882 CCCN). Son de número cerrado (**numerus clausus**).

| Derecho Real | Arts. CCCN | Características |
|---|---|---|
| Dominio | 1941-1982 | Derecho real más pleno: usar, gozar y disponer |
| Condominio | 1983-2036 | Dominio de varios sobre una misma cosa |
| Propiedad Horizontal | 2037-2072 | Dominio sobre unidades funcionales en edificios |
| Conjuntos Inmobiliarios | 2073-2086 | Countries, barrios privados, parques industriales |
| Tiempo Compartido | 2087-2102 | Uso periódico de inmueble o cosa mueble |
| Cementerio Privado | 2103-2113 | Derecho sobre sepulturas |
| Superficie | 2114-2128 | Derecho de plantar, forestar o construir sobre inmueble ajeno |
| Usufructo | 2129-2153 | Uso y goce de cosa ajena sin alterar su sustancia |
| Uso | 2154-2157 | Uso y goce limitado de cosa ajena |
| Habitación | 2158-2161 | Derecho de morar en casa ajena |
| Servidumbre | 2162-2183 | Carga sobre inmueble en beneficio de otro |
| Hipoteca | 2205-2211 | Garantía real sobre inmueble sin desposesión |
| Anticresis | 2212-2218 | Garantía real con entrega del inmueble al acreedor |
| Prenda | 2219-2237 | Garantía real sobre cosas muebles |

### 6.3 Derechos Personales / Obligaciones

La obligación es una relación jurídica en virtud de la cual el acreedor tiene el derecho a exigir al deudor una prestación (art. 724 CCCN).

| Propiedad | Tipo | Descripción |
|---|---|---|
| `acreedor` | `SujetoDeDeRecho` | Sujeto activo de la obligación |
| `deudor` | `SujetoDeDeRecho[]` | Sujeto/s pasivo/s |
| `tipoPrestacion` | `Enum` | `dar / hacer / no hacer` |
| `objetoPrestacion` | `String` | Descripción de la prestación debida |
| `montoDeuda` | `Decimal` | Monto en pesos o moneda extranjera |
| `tasaInteres` | `Decimal` | Tasa de interés aplicable |
| `tipoInteres` | `Enum` | `compensatorio / moratorio / punitorio` |
| `fechaVencimiento` | `Date` | Fecha de exigibilidad |
| `estaExtinguida` | `Boolean` | Si la obligación fue cumplida o extinguida |
| `causaExtincion` | `Enum` | Causa de extinción |

#### 6.3.1 Medios de Extinción de Obligaciones

- Pago (art. 865 CCCN)
- Novación (art. 933 CCCN)
- Compensación (art. 921 CCCN)
- Confusión (art. 931 CCCN)
- Renuncia (art. 944 CCCN)
- Remisión de deuda (art. 950 CCCN)
- Imposibilidad de cumplimiento (art. 955 CCCN)
- Prescripción liberatoria (art. 2532 CCCN)
- Transacción (art. 1641 CCCN)

---

## 7. Derecho Penal

El derecho penal regula el *ius puniendi* del Estado, definiendo delitos, penas y garantías (Código Penal, Ley 11.179 y modificatorias).

### 7.1 Clase: `Delito`

| Propiedad | Tipo | Descripción |
|---|---|---|
| `tipoDelito` | `Enum` | `doloso / culposo / preterintencional` |
| `clasificacion` | `Enum` | `crimen / delito / contravención` |
| `bienJuridicoProtegido` | `String` | Bien jurídico que protege la norma penal |
| `esTentativa` | `Boolean` | Si es tentativa o delito consumado |
| `esContinuado` | `Boolean` | Si es un delito continuado |
| `articuloCodigo` | `String` | Artículo del Código Penal aplicable |
| `penaMinima` | `Integer` | Pena mínima en meses o años |
| `penaMaxima` | `Integer` | Pena máxima en meses o años |
| `tipoPena` | `Enum` | `prisión / reclusión / multa / inhabilitación` |
| `admiteEjecucionCondicional` | `Boolean` | Si puede ser dejada en suspenso |
| `prescripcion` | `Integer` | Plazo de prescripción en años |

### 7.2 Clasificación de Delitos por Bien Jurídico

| Categoría | Ejemplos | Arts. CP |
|---|---|---|
| Delitos contra las personas | Homicidio, lesiones, abandono | 79-108 |
| Delitos contra el honor | Calumnia, injuria | 109-117 bis |
| Delitos contra la integridad sexual | Abuso sexual, violación | 119-133 |
| Delitos contra la libertad | Reducción a servidumbre, trata | 140-149 ter |
| Delitos contra la propiedad | Hurto, robo, estafa, defraudación | 162-185 |
| Delitos contra la seguridad pública | Incendio, estrago, piratería aérea | 186-213 bis |
| Delitos contra el orden público | Sedición, asociación ilícita | 209-213 bis |
| Delitos contra la administración pública | Cohecho, peculado, malversación | 237-281 bis |
| Delitos contra la fe pública | Falsificación de documentos y moneda | 282-302 |
| Delitos contra el orden económico | Lavado de activos (Ley 25.246) | 303-313 |

### 7.3 Clase: `Imputado`

Persona física sobre quien recae la persecución penal.

| Propiedad | Tipo | Descripción |
|---|---|---|
| `esImputado` | `PersonaHumana` | Vinculación con la clase PersonaHumana |
| `estadoProcesalPenal` | `Enum` | `imputado / procesado / sobreseído / condenado / absuelto` |
| `medidasCautelares` | `MedidaCautelar[]` | Medidas restrictivas aplicadas |
| `defensor` | `Abogado` | Abogado defensor designado o de oficio |
| `tieneAntecedentes` | `Boolean` | Si registra condenas anteriores |
| `esReincidente` | `Boolean` | Si califica como reincidente (art. 50 CP) |

### 7.4 Clase: `Condena`

| Propiedad | Tipo | Descripción |
|---|---|---|
| `fechaCondena` | `Date` | Fecha de la sentencia condenatoria |
| `tribunal` | `OrganoJudicial` | Tribunal que dictó la condena |
| `penaImpuesta` | `String` | Descripción completa de la pena |
| `duracionAnios` | `Integer` | Duración de la pena privativa de libertad |
| `esCondicional` | `Boolean` | Si la ejecución está en suspenso |
| `estaFirme` | `Boolean` | Si la condena está firme |
| `fechaVencimiento` | `Date` | Fecha de cumplimiento de la pena |
| `establecimientoPenitenciario` | `String` | Unidad carcelaria (si aplica) |

---

## 8. Proceso Judicial

El proceso judicial es el conjunto de actos procesales ordenados que culminan en una decisión jurisdiccional. Está regulado por los Códigos Procesales de cada jurisdicción.

### 8.1 Clase: `Proceso`

| Propiedad | Tipo | Descripción |
|---|---|---|
| `numeroExpediente` | `String` | Número de expediente judicial |
| `caratula` | `String` | Denominación del proceso (partes y objeto) |
| `tipoProcesal` | `Enum` | `civil / penal / laboral / contenciosoadmin / familia` |
| `subtipoProceso` | `Enum` | `ordinario / sumarísimo / ejecutivo / amparo / etc.` |
| `fechaInicio` | `Date` | Fecha de inicio o presentación de la demanda |
| `estadoProceso` | `Enum` | `iniciado / en trámite / en sentencia / apelado / concluido` |
| `jurisdiccion` | `Jurisdiccion` | Ámbito jurisdiccional (`federal / provincial / CABA`) |
| `fuero` | `Enum` | `civil / penal / laboral / comercial / familia / contencioso` |
| `tribunal` | `OrganoJudicial` | Juzgado o Tribunal interviniente |
| `juez` | `Magistrado` | Juez a cargo del proceso |
| `instancia` | `Enum` | `primera / segunda / tercera / extraordinaria` |

### 8.2 Partes Procesales

| Clase | Descripción | Relación |
|---|---|---|
| Actor/Demandante | Quien inicia la acción judicial | `tieneActor → SujetoDeDeRecho` |
| Demandado | Contra quien se dirige la acción | `tieneDemandado → SujetoDeDeRecho` |
| Tercero | Quien interviene sin ser parte original | `tieneIntervieniente → SujetoDeDeRecho` |
| Querellante | Víctima que impulsa la acción penal | `tieneQuerellante → SujetoDeDeRecho` |
| Ministerio Público Fiscal | Acusación pública en procesos penales | `tieneRepresentante → Fiscal` |
| Defensor Oficial | Defensa en casos de incapacidad económica | `tieneDefensor → Abogado` |

### 8.3 `ActoProcesal`

| Propiedad | Tipo | Descripción |
|---|---|---|
| `tipoActo` | `Enum` | `demanda / contestación / prueba / sentencia / recurso / etc.` |
| `fechaActo` | `Date` | Fecha de realización del acto |
| `realizadoPor` | `SujetoProcesal` | Quién realizó el acto |
| `contenidoActo` | `String (URL)` | Texto o documento del acto procesal |
| `esNotificado` | `Boolean` | Si fue notificado a las partes |
| `fechaNotificacion` | `Date` | Fecha de notificación |

### 8.4 `Sentencia`

La sentencia es el acto procesal del juez que pone fin al proceso resolviendo el fondo de la controversia.

| Propiedad | Tipo | Descripción |
|---|---|---|
| `tipoSentencia` | `Enum` | `definitiva / interlocutoria / de mérito` |
| `fechaDictado` | `Date` | Fecha de la sentencia |
| `resultado` | `Enum` | `favorable / desfavorable / parcial` |
| `estaFirme` | `Boolean` | Si está firme (no fue recurrida o venció plazo) |
| `costas` | `Enum` | `al vencido / en el orden causado / sin costas` |
| `tieneFundamentos` | `String` | Fundamentos jurídicos de la decisión |
| `esApelable` | `Boolean` | Si admite recurso de apelación |
| `recursoInterpuesto` | `Recurso` | Recurso interpuesto contra la sentencia |

### 8.5 Recursos Procesales

| Recurso | Tipo | Tribunal Competente |
|---|---|---|
| Aclaratoria | Ordinario | Mismo tribunal que dictó la resolución |
| Apelación | Ordinario | Cámara de Apelaciones |
| Nulidad | Ordinario | Cámara de Apelaciones |
| Casación | Extraordinario local | Tribunal Superior de Justicia provincial |
| Inaplicabilidad de ley | Extraordinario local | Suprema Corte o TSJ provincial |
| Recurso Extraordinario Federal (REF) | Extraordinario | Corte Suprema de Justicia de la Nación |
| Queja por denegación | Extraordinario | Corte Suprema de Justicia de la Nación |

---

## 9. Órganos del Estado y Sistema Judicial

### 9.1 Clase: `OrganoEstatal`

| Propiedad | Tipo | Descripción |
|---|---|---|
| `denominacion` | `String` | Nombre oficial del órgano |
| `poderDelEstado` | `Enum` | `Legislativo / Ejecutivo / Judicial / Ministerio Público` |
| `ambitoJurisdiccional` | `Enum` | `federal / provincial / municipal / CABA` |
| `baseNormativa` | `NormaJuridica` | Norma que crea o regula el órgano |
| `competencia` | `String` | Descripción de la competencia material |

### 9.2 Sistema Judicial Federal

| Órgano | Instancia | Competencia |
|---|---|---|
| Corte Suprema de Justicia | Suprema | Control constitucional, REF, instancia originaria |
| Cámaras Federales de Apelaciones | Segunda | Recursos de apelación en materia federal |
| Juzgados Federales | Primera | Civil, penal, contencioso-administrativo federal |
| Cámara Nacional Electoral | Segunda | Materia electoral nacional |
| Cámara Federal de Casación Penal | Tercera penal | Unificación de jurisprudencia penal federal |
| Tribunales Orales Federales | Única penal | Juicio oral y público en materia penal federal |

### 9.3 Ministerio Público

| Órgano | Función |
|---|---|
| Procurador General de la Nación | Máxima autoridad del Ministerio Público Fiscal |
| Defensor General de la Nación | Máxima autoridad del Ministerio Público de la Defensa |
| Fiscales | Ejercicio de la acción penal pública |
| Defensores Oficiales | Defensa de imputados sin recursos económicos |
| Asesores de Menores e Incapaces | Protección de intereses de niños e incapaces |

### 9.4 Clase: `Magistrado`

| Propiedad | Tipo | Descripción |
|---|---|---|
| `nombreCompleto` | `String` | Nombre completo del magistrado |
| `cargo` | `Enum` | `juez / vocal / ministro / camarista / conjuez` |
| `tribunal` | `OrganoJudicial` | Tribunal al que pertenece |
| `fechaDesignacion` | `Date` | Fecha de designación |
| `formaNombramiento` | `String` | Mecanismo de designación (JEM, concurso, etc.) |
| `esVitalicio` | `Boolean` | Si ejerce con inamovilidad hasta los 75 años |
| `especialidad` | `Enum` | `civil / penal / laboral / electoral / constitucional / etc.` |

---

## 10. Derecho Laboral y Seguridad Social

El derecho laboral argentino se rige por la Ley de Contrato de Trabajo (LCT, Ley 20.744) y sus modificatorias, con principios protectorios constitucionales (art. 14 bis CN).

### 10.1 Clase: `RelacionLaboral`

| Propiedad | Tipo | Descripción |
|---|---|---|
| `empleador` | `SujetoDeDeRecho` | Persona humana o jurídica que emplea |
| `trabajador` | `PersonaHumana` | Persona humana que presta el servicio |
| `tipoContrato` | `Enum` | `indeterminado / plazo fijo / eventual / temporada / aprendizaje` |
| `fechaInicio` | `Date` | Fecha de inicio de la relación laboral |
| `fechaEgreso` | `Date?` | Fecha de extinción del vínculo |
| `causaEgreso` | `Enum` | `despido sin causa / renuncia / mutuo acuerdo / justa causa / jubilación` |
| `remuneracionBruta` | `Decimal` | Remuneración mensual bruta en pesos |
| `categoriaConvenio` | `String` | Categoría según CCT aplicable |
| `convenioColectivo` | `ConvenioColectivo` | CCT que rige la relación |
| `jornada` | `Enum` | `tiempo completo / parcial / por turnos` |
| `horasSemanales` | `Integer` | Horas semanales de trabajo |
| `estaRegistrada` | `Boolean` | Si está registrada ante ARCA (ex AFIP) |

### 10.2 Derechos del Trabajador

- **Jornada máxima**: 8 horas diarias / 48 horas semanales (Ley 11.544)
- **Descanso semanal**: 35 horas corridas a partir del sábado (Ley 11.544)
- **Vacaciones anuales pagas**: 14-35 días según antigüedad (art. 150 LCT)
- **SAC (aguinaldo)**: 2 cuotas anuales en junio y diciembre (Ley 23.041)
- **Licencia por maternidad**: 90 días (art. 177 LCT)
- **Licencia por paternidad**: 15 días corridos (Ley 27.591)
- **Indemnización por antigüedad**: 1 mes de sueldo por año trabajado (art. 245 LCT)
- **Preaviso**: 15 días a 2 meses según antigüedad (art. 231 LCT)

### 10.3 Clase: `ConvenioColectivoTrabajo` (CCT)

| Propiedad | Tipo | Descripción |
|---|---|---|
| `numeroCCT` | `String` | Número identificatorio del convenio |
| `actividadRama` | `String` | Actividad o sector que regula |
| `sindicatoFirmante` | `String` | Sindicato/s signatario/s |
| `camaraEmpresaria` | `String` | Cámara o representación empleadora |
| `ambitoAplicacion` | `Enum` | `de actividad / de empresa / de oficio` |
| `fechaHomologacion` | `Date` | Fecha de homologación por el MTEySS |
| `vigencia` | `Date` | Fecha de vencimiento pactada |

---

## 11. Derecho de Familia

El derecho de familia regula las relaciones derivadas del parentesco, matrimonio y unión convivencial (arts. 401-723 CCCN).

### 11.1 Clase: `VinculoFamiliar`

| Subclase | Descripción | Norma CCCN |
|---|---|---|
| `Matrimonio` | Unión voluntaria de dos personas con vocación de permanencia | Art. 401-445 |
| `UnionConvivencial` | Convivencia afectiva de pareja inscripta o acreditada | Art. 509-528 |
| `Parentesco` | Vínculo jurídico por consanguinidad, afinidad o adopción | Art. 529-536 |
| `Filiacion` | Vínculo jurídico entre padres e hijos | Art. 558-593 |
| `Adopcion` | Acto jurídico que crea filiación por sentencia judicial | Art. 594-637 |
| `ResponsabilidadParental` | Conjunto de derechos y deberes de los progenitores | Art. 638-704 |

### 11.2 Clase: `Matrimonio`

| Propiedad | Tipo | Descripción |
|---|---|---|
| `conyuge1` | `PersonaHumana` | Primer cónyuge |
| `conyuge2` | `PersonaHumana` | Segundo cónyuge |
| `fechaCelebracion` | `Date` | Fecha del matrimonio civil |
| `lugarCelebracion` | `String` | Registro Civil donde se celebró |
| `regimen` | `Enum` | `comunidad de ganancias / separación de bienes` |
| `estaDisuelto` | `Boolean` | Si el matrimonio fue disuelto |
| `causaDisolucion` | `Enum` | `divorcio / fallecimiento / nulidad` |
| `fechaDisolucion` | `Date?` | Fecha de la disolución |

---

## 12. Derecho Tributario

El derecho tributario regula la potestad del Estado de exigir tributos (Ley 11.683 de Procedimiento Tributario y leyes especiales).

### 12.1 Clase: `Tributo`

| Propiedad | Tipo | Descripción |
|---|---|---|
| `denominacion` | `String` | Nombre del tributo |
| `tipoTributo` | `Enum` | `impuesto / tasa / contribución especial` |
| `nivelJurisdiccional` | `Enum` | `nacional / provincial / municipal` |
| `organoRecaudador` | `OrganoEstatal` | Ente recaudador (ARCA, DGR provincial, etc.) |
| `hechoImponible` | `String` | Hecho o situación que genera la obligación |
| `sujetosPasivos` | `SujetoDeDeRecho[]` | Contribuyentes y responsables |
| `baseImponible` | `String` | Base de cálculo del tributo |
| `alicuota` | `Decimal` | Tasa o porcentaje aplicable |
| `periodicidad` | `Enum` | `mensual / anual / único / por operación` |
| `normaCreadora` | `NormaJuridica` | Ley que crea el tributo |

### 12.2 Principales Tributos Nacionales

| Tributo | Tipo | Alícuota General | Norma |
|---|---|---|---|
| Impuesto a las Ganancias | Impuesto directo | Escala progresiva | Ley 20.628 |
| IVA | Impuesto indirecto | 21% (general) | Ley 23.349 |
| Bienes Personales | Impuesto directo | Variable según patrimonio | Ley 23.966 |
| Impuesto a los Créditos y Débitos | Impuesto indirecto | 0,6% c/u | Ley 25.413 |
| Derechos de Exportación | Impuesto al comercio | Variable por producto | Código Aduanero |
| Aportes/Contrib. Seguridad Social | Contribución especial | 28% empleador + 17% empleado | Ley 24.241 |

---

## 13. Derecho Administrativo

El derecho administrativo regula la organización y actividad de la Administración Pública (Ley 19.549 y sus reglamentos).

### 13.1 Clase: `ActoAdministrativo`

Declaración unilateral de un órgano del Estado en ejercicio de función administrativa que produce efectos jurídicos directos (art. 7 Ley 19.549).

| Propiedad | Tipo | Descripción |
|---|---|---|
| `tipoActo` | `Enum` | `decreto / resolución / disposición / ordenanza / circular` |
| `organoEmisor` | `OrganoEstatal` | Órgano que dictó el acto |
| `fecha` | `Date` | Fecha de emisión |
| `numeroActo` | `String` | Número identificatorio |
| `destinatario` | `SujetoDeDeRecho` | A quien va dirigido (si es particular) |
| `esRegulatorio` | `Boolean` | Si tiene efectos generales o particulares |
| `estaMotivado` | `Boolean` | Si contiene motivación suficiente (art. 7 inc. e) |
| `estaNotificado` | `Boolean` | Si fue notificado al interesado |
| `esFirme` | `Boolean` | Si quedó firme (no fue recurrido) |
| `estaEjecutoriado` | `Boolean` | Si fue ejecutado por la Administración |
| `recursosDisponibles` | `Recurso[]` | Recursos administrativos disponibles |

### 13.2 Recursos Administrativos

| Recurso | Plazo | Ante quien | Norma |
|---|---|---|---|
| Reconsideración | 10 días hábiles | Mismo órgano | Art. 84 Decreto 1759/72 |
| Jerárquico | 15 días hábiles | Superior jerárquico | Art. 89 Decreto 1759/72 |
| De alzada | 15 días hábiles | Ministerio competente | Art. 94 Decreto 1759/72 |
| Queja | En cualquier momento | Superior por mora | Art. 71 Decreto 1759/72 |

### 13.3 Contrato Administrativo y Servicio Público

| Clase | Descripción | Norma principal |
|---|---|---|
| `ContratoAdministrativo` | Contrato celebrado por el Estado con particulares | Ley 13.064 y Ley 27.437 |
| `LicitacionPublica` | Procedimiento de selección del contratista | Ley 27.437 |
| `Concesion` | Delegación de gestión de servicio público a privado | Leyes especiales por sector |
| `PermisoPrecario` | Habilitación provisional y revocable | Reglamentos sectoriales |

---

## 14. Resumen de Relaciones entre Clases

Principales propiedades de objeto de la ontología, con dominio, rango y cardinalidad.

| Relación | Dominio | Rango | Cardinalidad |
|---|---|---|---|
| `esDictadaPor` | `NormaJuridica` | `OrganoEstatal` | N:1 |
| `modificaA` | `NormaJuridica` | `NormaJuridica` | N:N |
| `derogaA` | `NormaJuridica` | `NormaJuridica` | N:N |
| `reglamentaA` | `NormaJuridica` | `NormaJuridica` | N:N |
| `tieneArticulo` | `NormaJuridica` | `Articulo` | 1:N |
| `esParteEn` | `SujetoDeDeRecho` | `Proceso` | N:N |
| `tieneDefensor` | `Proceso` | `Abogado` | N:1 |
| `tieneJuez` | `Proceso` | `Magistrado` | N:1 |
| `dictaSentencia` | `Magistrado` | `Sentencia` | 1:N |
| `esCondenadoPor` | `Condena` | `Delito` | 1:N |
| `tieneImputado` | `Proceso` | `Imputado` | 1:N |
| `celebra` | `SujetoDeDeRecho` | `Contrato` | N:N |
| `esAcreedorDe` | `SujetoDeDeRecho` | `Obligacion` | N:N |
| `esDeudorDe` | `SujetoDeDeRecho` | `Obligacion` | N:N |
| `tieneRelacionLaboral` | `PersonaHumana` | `RelacionLaboral` | 1:N |
| `empleaA` | `SujetoDeDeRecho` | `RelacionLaboral` | 1:N |
| `esCasadoCon` | `PersonaHumana` | `PersonaHumana` | 1:1 |
| `tieneHijo` | `PersonaHumana` | `PersonaHumana` | 1:N |
| `tributaEn` | `SujetoDeDeRecho` | `Tributo` | N:N |
| `recurreContra` | `SujetoProcesal` | `ActoAdministrativo` | N:N |

---

## 15. Instancias Clave del Sistema Legal Argentino

### 15.1 Normas Fundamentales

| Norma | Número | Fecha Vigencia | Descripción |
|---|---|---|---|
| Constitución Nacional | — | 01/05/1853 (reforma 1994) | Ley suprema de la Nación |
| Código Civil y Comercial | 26.994 | 01/08/2015 | Régimen general de derecho privado |
| Código Penal | 11.179 | 29/04/1922 (con reformas) | Delitos y penas |
| Ley de Contrato de Trabajo | 20.744 | 20/09/1974 (con reformas) | Régimen laboral general |
| Ley de Proc. Tributario | 11.683 | 12/01/1933 (con reformas) | Procedimiento ante ARCA/AFIP |
| Ley de Proc. Administrativo | 19.549 | 03/04/1972 | Actos y recursos administrativos |
| Ley General de Sociedades | 19.550 | 25/04/1972 (con reformas) | Tipos societarios |
| Código Procesal Civil y Com. | 17.454 | 07/11/1967 (Fed./CABA) | Proceso civil federal |
| Código Procesal Penal Federal | 27.063 | 10/12/2014 (impl. gradual) | Proceso penal acusatorio |

### 15.2 Organismos Clave

| Organismo | Tipo | Función principal |
|---|---|---|
| ARCA (ex AFIP) | Ente autárquico nacional | Recaudación impositiva, aduanera y de seguridad social |
| BCRA | Ente autárquico nacional | Regulación y supervisión del sistema financiero |
| IGJ | Organismo desconcentrado | Registro y control de personas jurídicas |
| INDEC | Organismo técnico | Estadísticas y censo |
| ANSES | Ente autárquico nacional | Administración de jubilaciones, pensiones y asignaciones |
| CNV | Ente autárquico nacional | Regulación del mercado de capitales |
| Defensoría del Pueblo | Órgano independiente | Control de la administración pública, protección de derechos |
| Ministerio Público Fiscal | Órgano extra-poder | Acción penal pública y defensa del interés general |

---

## 16. Lineamientos para Implementación en Software

### 16.1 Modelo de Datos Recomendado

| Caso de Uso | Tecnología Recomendada | Justificación |
|---|---|---|
| Base de datos relacional | PostgreSQL | Integridad referencial, consultas complejas, soporte JSON |
| Grafo de conocimiento | Neo4j / Apache Jena | Consultas de relaciones complejas (SPARQL/Cypher) |
| Web Semántica | OWL 2 + RDF + SPARQL | Interoperabilidad y razonamiento automático |
| Motor de búsqueda | Elasticsearch | Búsqueda de texto en normas y jurisprudencia |
| API REST | FastAPI / Django REST | Exposición de la ontología a otros sistemas |

### 16.2 Identificadores Únicos Recomendados

- **Normas**: `{tipo}-{número}-{año}` — Ej: `LEY-26994-2014`
- **Personas físicas**: DNI o CUIL — Ej: `20-12345678-4`
- **Personas jurídicas**: CUIT — Ej: `30-70012345-9`
- **Procesos judiciales**: `{jurisdicción}-{año}-{número}` — Ej: `CABA-2024-00123`
- **Expedientes administrativos**: GDE o sistema de cada organismo

### 16.3 Consideraciones de Seguridad y Privacidad

- Los datos personales están protegidos por la **Ley 25.326** de Protección de Datos Personales.
- El tratamiento de datos sensibles (salud, filiación política, religión) requiere consentimiento expreso del titular.
- Los sistemas deben registrarse ante la **Agencia de Acceso a la Información Pública (AAIP)**.
- Los datos de procesos judiciales tienen acceso regulado por acordadas de la CSJN y legislación procesal.
- Aplicar cifrado en reposo y en tránsito para datos personales y sensibles.

### 16.4 Actualización de la Ontología

El ordenamiento jurídico argentino es dinámico. Se recomienda:

- Monitorear el **Boletín Oficial** de la República Argentina (`boletinoficial.gob.ar`) para nuevas normas.
- Suscribirse a las alertas de **InfoLEG** (`infoleg.gob.ar`) para modificaciones a leyes vigentes.
- Implementar **versionado semántico** para la ontología (`MAJOR.MINOR.PATCH`).
- Mantener un **registro de cambios** (changelog) con la norma que motivó cada actualización.
- Definir un comité de actualización con abogados especialistas en cada rama del derecho.

---

*Ontología del Sistema Legal Argentino — v1.0 — 2026*
