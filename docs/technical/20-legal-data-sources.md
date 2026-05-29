# Legal Data Sources Catalog

> The catalog of known Argentine legal data sources — jurisprudence portals, administrative *dictamen*
> repositories, and legislation databases — with their URLs and **integration status**. Source/portal
> names and URLs are kept verbatim (proper nouns).
>
> This complements [Data Sources by Class](../ontology/ontology-data-sources.md) (which maps sources to
> ontology classes) and the ingestion docs ([13](13-saij-thesaurus-ingestion.md) ·
> [14](14-csjn-ruling-ingestion.md) · [15](15-saij-web-ingestion.md)).

---

## Status legend

| Status | Meaning |
|--------|---------|
| ✅ **Integrated** | Ingested today via a pipeline source |
| 🔵 **Candidate** | Public search/portal exists; not yet ingested |
| ⚪ **No search** | Institution online but no jurisprudence search |
| ⚠️ **Not working / limited** | Portal down, or only newsletters/PDF |

---

## 1. Integrated sources

| Source | `SourceId` | Type | Access | Doc |
|--------|:----------:|------|--------|-----|
| **CSJN** (Corte Suprema de Justicia de la Nación) | 1 | Jurisprudence | sjconsulta JSON API + PDF (`sjconsulta.csjn.gov.ar`) | [14](14-csjn-ruling-ingestion.md) |
| **SAIJ — Legislation** | 2 | Legislation | SAIJ API (`api.saij.gob.ar`) | [15](15-saij-web-ingestion.md) |
| **SAIJ — Jurisprudence** | 3 | Jurisprudence | SAIJ API (`api.saij.gob.ar`) | [15](15-saij-web-ingestion.md) |
| **SAIJ — Thesaurus** | 6 | Controlled vocabulary | TemaTres API (`vocabularios.saij.gob.ar`) | [13](13-saij-thesaurus-ingestion.md) |

---

## 2. National jurisprudence

| Source | URL | Status |
|--------|-----|--------|
| CSJN — Fallos por tomo | https://sjservicios.csjn.gov.ar/sj/tomosFallos.do?method=iniciar | ✅ (via sjconsulta) |
| CSJN — Tribunales Federales y Nacionales | https://www.csjn.gov.ar/tribunales-federales-nacionales/inicio.html | 🔵 |
| Tribunal Fiscal de la Nación (buscador con IA) | https://jurisprudenciatfn.mecon.gob.ar/ | 🔵 |
| SAIJ — Jurisprudencia nacional | https://www.saij.gob.ar/buscador/jurisprudencia-nacional | ✅ (via SAIJ API) |

---

## 3. Provincial jurisprudence

| Jurisdiction | Source | URL | Status |
|--------------|--------|-----|--------|
| Buenos Aires | SCBA — JUBA | https://juba.scba.gov.ar/Busquedas.aspx | 🔵 |
| Buenos Aires | Tribunal Fiscal de Apelación (TFABA) | http://www.tfaba.gov.ar/Apartados/busqueda.asp | 🔵 |
| CABA | Tribunal Superior de Justicia (TSJ) | http://jurisprudencia.tsjbaires.gob.ar/jurisprudencia/ | 🔵 |
| CABA | JURISTECA — Cámara Cont. Adm. y Trib. | https://juristeca.jusbaires.gob.ar/busqueda-avanzada-de-jurisprudencia/ | 🔵 |
| Corrientes | Superior Tribunal de Justicia | https://www.juscorrientes.gov.ar/seccion/jurisprudenciaybiblioteca/ | 🔵 |
| Corrientes | Cámara Civil y Comercial | https://www.juscorrientes.gov.ar/seccion/jurisprudencia/fallos-camara-civycom/ | 🔵 |
| Corrientes | Cámara Cont. Adm. y Electoral | https://www.juscorrientes.gov.ar/seccion/jurisprudencia/fallos-camara-contenciosa/ | 🔵 |
| Corrientes | Cámara Laboral | https://www.juscorrientes.gov.ar/seccion/jurisprudencia/fallos-camara-laboral/ | 🔵 |
| Chubut | Poder Judicial | http://www.juschubut.gov.ar | ⚪ Sin buscador |
| Entre Ríos | Poder Judicial | https://jur.jusentrerios.gov.ar/jur/?ai=jur\|\|newpublica | 🔵 |
| Formosa | Poder Judicial | https://jusformosa.gob.ar/decisiones-judiciales/jurisprudencia | ⚠️ No funciona |
| Jujuy | Poder Judicial | https://jurisprudencia.justiciajujuy.gov.ar/public/buscador?index=0 | 🔵 |
| La Pampa | Poder Judicial | https://justicia.lapampa.gob.ar/newsletters-2025.html | ⚠️ Newsletters |
| La Rioja | Poder Judicial | http://www.justicialarioja.gob.ar | ⚪ Sin buscador |
| Mendoza | Biblioteca Judicial — Jurisprudencia | https://jusmendoza.gob.ar/biblioteca-judicial/jurisprudencia/ | 🔵 |
| Misiones | Poder Judicial | http://www.jusmisiones.gov.ar | ⚪ Sin buscador |
| Neuquén | Fueros Civil y Comercial | http://juriscivil.jusneuquen.gov.ar/ | 🔵 |
| Río Negro | Poder Judicial | https://fallos.jusrionegro.gov.ar/protocoloweb/protocolo/busqueda?stj=0 | 🔵 |
| Salta | Corte Suprema | https://appweb.justiciasalta.gov.ar:8091/juriscorte/com.juriscargacorte.modbusqueda.busqueda | 🔵 |
| Salta | Tribunales 2.ª Instancia | https://appweb2.justiciasalta.gov.ar:9091/juriscamext/com.juris.busqueda | 🔵 |
| Salta | Cámara Civil en Pleno | https://juriscccp.justiciasalta.gov.ar/wescrito09.aspx | 🔵 |
| San Juan | Corte de Justicia — Fallos | https://jurisprudencia.jussanjuan.gob.ar/corte/index.php?t=fallos | 🔵 |
| San Juan | Segunda Instancia | https://jurisprudencia.jussanjuan.gob.ar/buscador/v3/camara/ | 🔵 |
| San Juan | Dictámenes Fiscal General | https://jurisprudencia.jussanjuan.gob.ar/ministeriopublico/ | 🔵 |
| San Luis | Superior Tribunal de Justicia | https://busca.justiciasanluis.gov.ar/publico/buscador_list.php?page=list | 🔵 |
| Santa Cruz | Poder Judicial | https://www.jussantacruz.gob.ar/index.php/consulta-de-jurisprudencia | 🔵 |
| Santa Fe | Corte Suprema de Justicia | https://bdj.justiciasantafe.gov.ar/ | 🔵 |
| Santa Fe | Cámaras de Apelación y Cont. Adm. | https://bdjcamara.justiciasantafe.gov.ar/ | 🔵 |
| Santiago del Estero | Poder Judicial | http://fallos-sumarios.jussantiago.gov.ar/ | 🔵 |
| Tierra del Fuego | Poder Judicial | https://juris.justierradelfuego.gov.ar/ | 🔵 |
| Tucumán | Poder Judicial | https://juris.justucuman.gov.ar/busca_juris_internet_new.php | 🔵 |

---

## 4. Administrative *dictámenes* (tax & public administration)

| Source | URL | Status |
|--------|-----|--------|
| Dictámenes — Dirección Nacional de Impuestos (DNI) | https://www.argentina.gob.ar/economia/ingresospublicos/dni/dictamenes | 🔵 |
| Comisión Arbitral — Resoluciones CA / Comisión Plenaria / casos concretos | https://www.ca.gob.ar/ | 🔵 |
| ARCA — dictámenes | https://biblioteca.arca.gob.ar/search/query/index.aspx | 🔵 |
| ARBA — informes y otros documentos tributarios | https://web.arba.gov.ar/biblioteca-legal | 🔵 |
| ARBA — sistematizados por año/tema | https://app.arba.gov.ar/codFiscal/sinsso/inicializarArbolTemasConLink.do | 🔵 |
| AGIP — dictámenes (remite a Comisión Arbitral) | — | 🔵 |
| Córdoba — Rentas, consultas vinculantes | https://www.rentascordoba.gob.ar/cms/ca/consulta-vinculante/ | 🔵 |
| Santa Fe — informes y dictámenes de Técnica y Jurídica | https://www.santafe.gov.ar/index.php/web/content/view/full/128224 | 🔵 |

---

## 5. Legislation & official norms

| Source | URL | Notes |
|--------|-----|-------|
| **SAIJ** | https://www.saij.gob.ar | ✅ Integrated (API) — consolidated text |
| **InfoLEG** (Ministerio de Justicia) | https://www.infoleg.gob.ar | 🔵 Official norm database + amendment history |
| **Boletín Oficial de la República Argentina** | https://www.boletinoficial.gob.ar | 🔵 Official publication / `publicationDate` |
| Cámara de Diputados | https://www.diputados.gov.ar | 🔵 Laws approved by the Chamber |
| Senado de la Nación | https://www.senado.gob.ar | 🔵 Laws approved by the Senate |
| Casa Rosada — Decretos | https://www.casarosada.gob.ar | 🔵 DNU and Executive decrees |

---

## 6. Aggregators & commercial sources

Cross-cutting sources that feed multiple entity types (see
[Data Sources by Class](../ontology/ontology-data-sources.md) for the per-class detail):

| Source | URL | Notes |
|--------|-----|-------|
| CIJ — Centro de Información Judicial | https://www.cij.gov.ar | Judicial bodies, judges, judgments |
| ARCA (ex-AFIP) | https://www.afip.gob.ar | Persons, entities, tax |
| RENAPER | https://www.argentina.gob.ar/renaper | Natural persons, domicile, civil status |
| datos.gob.ar (open-data APIs) | https://datos.gob.ar | Domicile, state bodies, statistics |
| Microjuris | https://www.microjuris.com.ar | 💲 Paid — case law, norms |
| La Ley Online | https://laleyonline.com.ar | 💲 Paid — case law, doctrine, norms |
| El Dial | https://www.eldial.com | 💲 Paid — case law, norms |

---

## 7. Judicial actors registries (MJN)

Roster and appointment data for national/federal judges, prosecutors, and public defenders, published
as **open data** by the **Ministerio de Justicia** (Secretaría de Justicia — Dirección Nacional de
Relaciones con el Poder Judicial). CSV, UTF-8, coverage from **1976** to date.

| Dataset | URL | Content | Feeds (ontology) | Status |
|---------|-----|---------|------------------|--------|
| **Magistrados** de la Justicia Federal y Nacional | [datos.gob.ar](https://datos.gob.ar/ar/dataset/justicia-magistrados-justicia-federal-justicia-nacional) · [datos.jus.gob.ar](https://datos.jus.gob.ar/ar/dataset/magistrados-justicia-federal-y-de-la-justicia-nacional) | Roster and seats: jurisdiction, *cámara*, organ, `cargo`, name, DNI, gender, titular/subrogante, oath date, vacancy/leave, *concurso*, appointment norm, province | `Person`, `Court`, `JudicialOffice` | 🔵 Candidate |
| **Designaciones** de magistrados de la Justicia Federal y Nacional | [datos.gob.ar](https://datos.gob.ar/ar/dataset/justicia-designaciones-magistrados-justicia-federal-justicia-nacional) · [datos.jus.gob.ar](https://datos.jus.gob.ar/dataset/designaciones-de-magistrados-de-la-justicia-federal-y-la-justicia-nacional) | Appointment history (since 1976): jurisdiction, organ, `cargo_detalle`, magistrate, appointment date, decree (number/date, signing president & minister), province/locality | `Person`, `RulingParticipation` | 🔵 Candidate |

> Both datasets should be **ingested** from these open-data endpoints and integrated into the ontology
> (rather than maintained as static CSVs).

---

## 8. Notes

- **Integration priority** follows the roadmap: CSJN and SAIJ are integrated; the provincial portals
  are candidates for incremental expansion (each typically needs its own discovery/parse strategy
  given heterogeneous portals — see the strategy pattern in [14 §2](14-csjn-ruling-ingestion.md)).
- Several provincial portals have **no machine-friendly search** (or are down); these require PDF/HTML
  scraping or are deferred.
- For the source-to-ontology-class mapping (which source populates which field), see
  [Data Sources by Class](../ontology/ontology-data-sources.md).

---

*Legal Data Sources Catalog — Legal Ai Ar*
