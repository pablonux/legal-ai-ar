# Fuentes de Datos por Clase — Ontología Legal Argentina

> Para cada clase se indican las fuentes primarias (oficiales) y secundarias (agregadores/APIs) donde obtener los datos para poblar sus instancias y propiedades.

---

## 1. Fuentes del Derecho y Normas

### `NormaJuridica` / `LeyNacional` / `DecretoLey`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `numeroNorma`, `denominacion`, `fechaSancion`, `fechaPromulgacion`, `fechaPublicacion`, `textoCompleto` | **InfoLEG** (base oficial del Ministerio de Justicia) | https://www.infoleg.gob.ar |
| `fechaPublicacion`, texto oficial | **Boletín Oficial de la República Argentina** | https://www.boletinoficial.gob.ar |
| Texto consolidado con modificaciones | **Sistema Argentino de Información Jurídica (SAIJ)** | https://www.saij.gob.ar |
| `estaVigente`, historial de modificaciones | **InfoLEG** → sección "Normas que modifican" | https://www.infoleg.gob.ar |
| Leyes aprobadas por el Congreso | **Honorable Cámara de Diputados** | https://www.diputados.gov.ar |
| Leyes aprobadas por el Senado | **Honorable Senado de la Nación** | https://www.senado.gob.ar |
| DNU y decretos del PEN | **Casa Rosada — Decretos** | https://www.casarosada.gob.ar |

### `NormaProvincial` / `NormaMunicipal`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Leyes y decretos provinciales | **Boletines Oficiales provinciales** (cada provincia tiene el suyo) | Ej: https://boletinoficial.ba.gov.ar (Bs. As.) |
| Texto unificado por provincia | **SAIJ — Legislación provincial** | https://www.saij.gob.ar |
| Ordenanzas municipales | **Sitios web de cada municipio** / Concejos Deliberantes | Variable por municipio |
| Ordenanzas CABA | **Legislatura CABA** | https://www.legislatura.gov.ar |

### `NormaConstitucional` / `TratadoInternacional`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Texto Constitución Nacional | **InfoLEG**, **SAIJ**, sitio oficial del Congreso | https://www.infoleg.gob.ar |
| Tratados con jerarquía constitucional | **Cancillería Argentina** | https://www.cancilleria.gob.ar |
| Base de tratados internacionales | **ONU Treaty Collection** | https://treaties.un.org |
| Tratados del sistema interamericano | **OEA** | https://www.oas.org/es/sla/ddi/tratados_multilaterales_interamericanos.asp |

### `Articulo` / `Inciso`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Texto de artículos | **InfoLEG** (versión con articulado) | https://www.infoleg.gob.ar |
| Articulado con anotaciones | **SAIJ** | https://www.saij.gob.ar |
| Artículos con historial de modificaciones | **Microjuris** (pago) | https://www.microjuris.com.ar |

### `Jurisprudencia`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Fallos de la CSJN | **Centro de Información Judicial (CIJ)** | https://www.cij.gov.ar |
| Fallos de la CSJN históricos | **CSJN — Fallos** | https://sjconsulta.csjn.gov.ar |
| Jurisprudencia provincial y nacional | **SAIJ — Jurisprudencia** | https://www.saij.gob.ar |
| Bases de jurisprudencia comercial | **El Dial** (pago) | https://www.eldial.com |
| Jurisprudencia laboral | **DT Online / La Ley** (pago) | https://laleyonline.com.ar |
| Plenarios de Cámaras Nacionales | **Poder Judicial de la Nación** | https://www.pjn.gov.ar |

---

## 2. Sujetos de Derecho

### `PersonaHumana`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `numeroDNI`, `apellido`, `nombre`, `fechaNacimiento`, `nacionalidad` | **RENAPER** (Registro Nacional de las Personas) | API oficial para organismos: https://www.argentina.gob.ar/renaper |
| `CUIL` | **AFIP/ARCA — Consulta CUIL** | https://www.cuitonline.com / API ARCA |
| `estadoCivil`, `domicilio` | **Registro Civil** (nacional o provincial) | Sistemas de cada provincia |
| `fechaFallecimiento` | **RENAPER** / Registro Civil | https://www.argentina.gob.ar/renaper |
| `tieneCapacidadDeEjercicio` (restricciones) | **Registro Nacional de Interdicciones** (RENACI) | Consulta judicial |
| Datos de extranjeros | **Dirección Nacional de Migraciones** | https://www.migraciones.gov.ar |

### `PersonaJuridica` (Privada)

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `denominacion`, `CUIT`, `fechaConstitucion`, `objetoSocial`, `estaInscriptaEn` | **IGJ** (Inspección General de Justicia) — para CABA y sociedades nacionales | https://www.igj.gov.ar |
| Sociedades en provincias | **Registros Públicos de Comercio provinciales** | Variable por provincia |
| `CUIT`, datos fiscales | **ARCA (ex AFIP) — Consulta pública** | https://www.afip.gob.ar/cuitverificacion |
| Cooperativas | **INAES** (Instituto Nacional de Asociativismo) | https://www.inaes.gob.ar |
| Mutuales | **INAES** | https://www.inaes.gob.ar |
| Fundaciones | **IGJ** | https://www.igj.gov.ar |
| Personas jurídicas extranjeras | **IGJ** — Sección sociedades extranjeras | https://www.igj.gov.ar |

### `PersonaJuridica` (Pública)

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Organismos nacionales | **Argentina.gob.ar — Organismos** | https://www.argentina.gob.ar/organismos |
| Empresas y entes del Estado | **FONAFE / Secretaría de Hacienda** | https://www.argentina.gob.ar |
| Universidades nacionales | **SPU** (Secretaría de Políticas Universitarias) | https://www.argentina.gob.ar/educacion/universidades |
| Municipios | **INDEC — Municipios** | https://www.indec.gob.ar |

### `Domicilio`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Nomenclador de calles y localidades | **INDEC — Unidades Geoestadísticas** | https://www.indec.gob.ar |
| Geolocalización y validación | **API de Normalización de Direcciones (georef)** | https://apis.datos.gob.ar/georef |
| Códigos postales | **Correo Argentino** | https://www.correoargentino.com.ar/codigo-postal |

---

## 3. Contratos y Actos Jurídicos

### `Contrato` / `ActoJuridico`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Contratos con el Estado | **Portal de Compras del Estado (COMPR.AR)** | https://www.argentinacompra.gov.ar |
| Contratos registrados (escrituras) | **Registro de la Propiedad Inmueble** (cada provincia) | Variable |
| Contratos laborales | **AFIP/ARCA — Sistema de Altas Tempranas (SIAT)** | https://serviciosweb.afip.gob.ar |
| Contratos de seguro | **SSN** (Superintendencia de Seguros de la Nación) | https://www.ssn.gob.ar |
| Contratos de locación (declarados) | **AFIP/ARCA — RELI** (Registro de Locaciones de Inmuebles) | https://www.afip.gob.ar |
| Escrituras y actos notariales | **Colegios de Escribanos** (cada provincia) | Ej: https://www.colegio-escribanos.org.ar (CABA) |

### `DerechoReal` / `Hipoteca` / `Prenda`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Dominio e hipotecas sobre inmuebles | **Registro de la Propiedad Inmueble** (por provincia) | Variable por provincia |
| Prendas sobre automotores | **Registro Nacional de la Propiedad del Automotor (RNPA)** | https://www.dnrpa.gov.ar |
| Prenda con registro | **Registro Nacional de Créditos Prendarios** | https://www.dnrpa.gov.ar |
| Buques | **Prefectura Naval Argentina** | https://www.prefecturanaval.gob.ar |
| Aeronaves | **ANAC** (Administración Nacional de Aviación Civil) | https://www.anac.gob.ar |

---

## 4. Derecho Penal

### `Delito`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `articuloCodigo`, `tipoPena`, `penaMinima`, `penaMaxima` | **Código Penal** (InfoLEG) | https://www.infoleg.gob.ar/infolegInternet/anexos/15000-19999/16546/texact.htm |
| Leyes penales especiales | **InfoLEG** | https://www.infoleg.gob.ar |
| Estadísticas de delitos | **SNIC** (Sistema Nacional de Información Criminal) | https://www.argentina.gob.ar/seguridad/snic |

### `Imputado` / `Condena`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `estadoProcesalPenal`, datos del proceso | **Sistema de Gestión Judicial** (cada jurisdicción) | Variable (MEJ, Lex100, etc.) |
| Antecedentes penales | **Registro Nacional de Reincidencia** | https://www.argentina.gob.ar/justicia/reincidencia |
| Personas privadas de libertad | **Servicio Penitenciario Federal** | https://www.spf.gob.ar |
| Servicios penitenciarios provinciales | Cada Servicio Penitenciario provincial | Variable |
| Condenas firmes | **CIJ — Fallos** / sistema de gestión judicial | https://www.cij.gov.ar |

### `MedidaCautelarPenal`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Órdenes de restricción y detenciones | **Sistema de Gestión Judicial** | Variable por jurisdicción |
| Inhibiciones e interdicciones | **RENACI** (Registro Nacional de Interdicciones) | Consulta judicial |

---

## 5. Proceso Judicial

### `Proceso` / `ActoProcesal` / `Sentencia`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `numeroExpediente`, `caratula`, `estadoProceso`, `fechaInicio` | **Sistema de Consulta de Causas (PJN)** | https://www.pjn.gov.ar |
| Causas de la CSJN | **CSJN — Sistema de Consulta** | https://sjconsulta.csjn.gov.ar |
| Causas en CABA | **Poder Judicial CABA** | https://www.jusbaires.gov.ar |
| Causas provinciales | Sistemas de consulta de cada provincia | Variable |
| Expedientes digitales (GDE) | **GDE** (Gestión Documental Electrónica) | Para organismos habilitados |
| Notificaciones electrónicas | **Sistema de Notificaciones Electrónicas (SINOE)** | https://notificaciones.scba.gov.ar (Bs. As.) |

### `Recurso`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Recursos ante la CSJN | **CSJN — Mesa de Entradas** / sistema de consulta | https://sjconsulta.csjn.gov.ar |
| Recursos provinciales | Sistema de gestión de cada jurisdicción | Variable |

### `Magistrado` / `Abogado`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `nombreCompleto`, `cargo`, `tribunal`, `fechaDesignacion` | **CIJ — Magistrados** | https://www.cij.gov.ar |
| Designaciones y concursos | **Consejo de la Magistratura de la Nación** | https://www.consejomagistratura.gov.ar |
| Magistrados provinciales | Consejos de la Magistratura provinciales | Variable |
| `matricula` del abogado | **Colegio de Abogados** (por jurisdicción) | Ej: https://www.cabiabilogados.org.ar (CABA) |

---

## 6. Órganos del Estado

### `OrganoEstatal` / `OrganoJudicial` / `OrganoLegislativo` / `OrganoEjecutivo`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Organismos nacionales | **Argentina.gob.ar** | https://www.argentina.gob.ar/organismos |
| Estructura orgánica del PEN | **Jefatura de Gabinete de Ministros** | https://www.argentina.gob.ar/jefatura |
| Estructura del Poder Judicial | **CIJ — Mapa Judicial** | https://www.cij.gov.ar |
| Estructura del Congreso | **Cámara de Diputados / Senado** | https://www.diputados.gov.ar / https://www.senado.gob.ar |
| Organismos provinciales | Sitios oficiales de cada provincia | Variable |

### `MinisterioPublico`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Fiscalías y fiscales | **Ministerio Público Fiscal** | https://www.mpf.gov.ar |
| Defensorías | **Ministerio Público de la Defensa** | https://www.mpd.gov.ar |
| Asesorías de menores | **Ministerio Público de la Defensa** | https://www.mpd.gov.ar |

---

## 7. Derecho Laboral

### `RelacionLaboral`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `fechaInicio`, `remuneracionBruta`, `estaRegistrada`, `tipoContrato` | **AFIP/ARCA — Mi Simplificación (SIAT)** | https://serviciosweb.afip.gob.ar |
| Altas y bajas de empleados | **AFIP/ARCA — Relaciones Laborales** | https://www.afip.gob.ar |
| Denuncias laborales y fiscalización | **AFIP/ARCA — Blanqueo Laboral** | https://www.afip.gob.ar |
| Disputas y causas laborales | **Cámara Nacional de Apelaciones del Trabajo (CNAT)** | https://www.pjn.gov.ar |
| Accidentes de trabajo | **SRT** (Superintendencia de Riesgos del Trabajo) | https://www.srt.gob.ar |

### `ConvenioColectivoTrabajo` (CCT)

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `numeroCCT`, `actividadRama`, `sindicatoFirmante`, `fechaHomologacion` | **MTEySS — Convenios Colectivos** | https://www.argentina.gob.ar/trabajo/relacioneslaborales/convenioscolectivos |
| Texto completo de CCTs | **InfoLEG** (CCTs homologados) | https://www.infoleg.gob.ar |
| Paritarias y actualizaciones salariales | **MTEySS** | https://www.argentina.gob.ar/trabajo |

### `Sindicato`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `denominacion`, `personeriaGremial`, `ambitoRepresentacion` | **MTEySS — Registro Sindical** | https://www.argentina.gob.ar/trabajo/sindicatos |

---

## 8. Derecho de Familia

### `Matrimonio` / `UnionConvivencial`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `fechaCelebracion`, `conyuge1`, `conyuge2`, `regimen`, `estaDisuelto` | **Registro Civil** (nacional o provincial) | Variable por provincia |
| Actas de matrimonio | **RENAPER** (para consultas con habilitación) | https://www.argentina.gob.ar/renaper |
| Uniones convivenciales inscriptas | **Registro Civil** de cada jurisdicción | Variable |
| Divorcio y disolución | **Sistema de Gestión Judicial** | Variable |

### `Filiacion` / `Adopcion`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Reconocimiento de hijos | **Registro Civil** | Variable por provincia |
| Partidas de nacimiento | **RENAPER** | https://www.argentina.gob.ar/renaper |
| Sentencias de adopción | **Sistema de Gestión Judicial** | Variable |
| Registro de adoptantes | **RUAGA** (Registro Único de Aspirantes a Guarda con Fines Adoptivos) | https://www.argentina.gob.ar/justicia/ruaga |

### `ResponsabilidadParental`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Acuerdos de cuota alimentaria | **Sistema de Gestión Judicial** | Variable |
| Registro de deudores alimentarios | **Registro de Deudores Alimentarios** (por provincia) | Ej: https://www.scba.gov.ar (Bs. As.) |

---

## 9. Derecho Tributario

### `Tributo` / `ObligacionTributaria`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| `hechoImponible`, `alicuota`, `periodicidad`, `normaCreadora` | **ARCA (ex AFIP) — Normativa** | https://www.afip.gob.ar/normativa |
| Alícuotas de IVA, Ganancias, etc. | **InfoLEG** + **ARCA** | https://www.infoleg.gob.ar |
| `montoDeclarado`, `montoPagado` | **ARCA — Cuentas Tributarias** (con clave fiscal) | https://www.afip.gob.ar |
| Deuda tributaria y planes de pago | **ARCA — Mis Facilidades** | https://www.afip.gob.ar |
| Tributos provinciales | **AGIP** (CABA), **ARBA** (Bs. As.) y organismos provinciales | Variable |
| Estadísticas de recaudación | **ARCA — Estadísticas** | https://www.afip.gob.ar/institucional/estudios |

---

## 10. Derecho Administrativo

### `ActoAdministrativo`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Decretos y resoluciones nacionales | **Boletín Oficial** | https://www.boletinoficial.gob.ar |
| Expedientes administrativos nacionales | **GDE** (Gestión Documental Electrónica) | Para organismos habilitados |
| Consulta pública de expedientes | **TrackiAR / Seguimiento de trámites** | https://www.argentina.gob.ar/tramites |
| Actos provinciales | Boletines Oficiales provinciales | Variable |

### `ContratoAdministrativo` / `LicitacionPublica`

| Propiedad | Fuente | URL / Sistema |
|---|---|---|
| Licitaciones y contrataciones nacionales | **COMPR.AR** (Portal de Compras del Estado) | https://www.argentinacompra.gov.ar |
| Contratos de obra pública | **Dirección Nacional de Vialidad / MOP** | https://www.argentina.gob.ar/obras-publicas |
| Concesiones de servicios públicos | **ENRE, ENARGAS, ORSNA** (según sector) | Variable por regulador |
| Contrataciones provinciales | Portales de compras provinciales | Variable |
| Declaraciones juradas de funcionarios | **OA** (Oficina Anticorrupción) | https://www.argentina.gob.ar/anticorrupcion |

---

## Fuentes Transversales y Agregadores

Estas fuentes son útiles para múltiples clases de la ontología.

| Fuente | Clases que nutre | URL |
|---|---|---|
| **InfoLEG** | NormaJuridica, Articulo, CCT, Tributo | https://www.infoleg.gob.ar |
| **SAIJ** | NormaJuridica, Jurisprudencia, Doctrina | https://www.saij.gob.ar |
| **Boletín Oficial** | NormaJuridica, ActoAdministrativo | https://www.boletinoficial.gob.ar |
| **CIJ** | OrganoJudicial, Magistrado, Sentencia, Proceso | https://www.cij.gov.ar |
| **ARCA (ex AFIP)** | PersonaHumana, PersonaJuridica, RelacionLaboral, Tributo | https://www.afip.gob.ar |
| **RENAPER** | PersonaHumana, Domicilio, Matrimonio, Filiacion | https://www.argentina.gob.ar/renaper |
| **APIs datos.gob.ar** | Domicilio, OrganoEstatal, estadísticas | https://datos.gob.ar |
| **Microjuris** (pago) | Jurisprudencia, NormaJuridica | https://www.microjuris.com.ar |
| **La Ley Online** (pago) | Jurisprudencia, Doctrina, NormaJuridica | https://laleyonline.com.ar |
| **El Dial** (pago) | Jurisprudencia, NormaJuridica | https://www.eldial.com |

---

*Ontología del Sistema Legal Argentino — Fuentes de datos — v1.0 — 2026*
