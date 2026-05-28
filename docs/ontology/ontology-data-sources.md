# Data Sources by Class — Argentine Legal Ontology

> For each class, the primary (official) and secondary (aggregators/APIs) sources are indicated, from which to obtain the data to populate its instances and properties.

---

## 1. Legal Sources and Norms

### `LegalNorm` / `NationalLaw` / `DecreeLaw`

| Property | Source | URL / System |
|---|---|---|
| `normNumber`, `name`, `enactmentDate`, `promulgationDate`, `publicationDate`, `fullText` | **InfoLEG** (official database of the Ministry of Justice) | https://www.infoleg.gob.ar |
| `publicationDate`, official text | **Official Gazette of the Argentine Republic** | https://www.boletinoficial.gob.ar |
| Consolidated text with amendments | **Sistema Argentino de Información Jurídica (SAIJ)** | https://www.saij.gob.ar |
| `isInForce`, amendment history | **InfoLEG** → "Normas que modifican" section | https://www.infoleg.gob.ar |
| Laws approved by Congress | **Chamber of Deputies** | https://www.diputados.gov.ar |
| Laws approved by the Senate | **Senate of the Nation** | https://www.senado.gob.ar |
| DNU and Executive decrees | **Casa Rosada — Decrees** | https://www.casarosada.gob.ar |

### `ProvincialNorm` / `MunicipalNorm`

| Property | Source | URL / System |
|---|---|---|
| Provincial laws and decrees | **Provincial Official Gazettes** (each province has its own) | E.g.: https://boletinoficial.ba.gov.ar (Bs. As.) |
| Unified text by province | **SAIJ — Provincial legislation** | https://www.saij.gob.ar |
| Municipal ordinances | **Each municipality's websites** / Deliberative Councils | Varies by municipality |
| CABA ordinances | **CABA Legislature** | https://www.legislatura.gov.ar |

### `ConstitutionalNorm` / `InternationalTreaty`

| Property | Source | URL / System |
|---|---|---|
| National Constitution text | **InfoLEG**, **SAIJ**, official Congress site | https://www.infoleg.gob.ar |
| Treaties with constitutional rank | **Argentine Foreign Ministry** | https://www.cancilleria.gob.ar |
| International treaties database | **UN Treaty Collection** | https://treaties.un.org |
| Inter-American system treaties | **OAS** | https://www.oas.org/es/sla/ddi/tratados_multilaterales_interamericanos.asp |

### `Article` / `Clause`

| Property | Source | URL / System |
|---|---|---|
| Article text | **InfoLEG** (version with articles) | https://www.infoleg.gob.ar |
| Articles with annotations | **SAIJ** | https://www.saij.gob.ar |
| Articles with amendment history | **Microjuris** (paid) | https://www.microjuris.com.ar |

### `CaseLaw`

| Property | Source | URL / System |
|---|---|---|
| CSJN rulings | **Centro de Información Judicial (CIJ)** | https://www.cij.gov.ar |
| Historical CSJN rulings | **CSJN — Fallos** | https://sjconsulta.csjn.gov.ar |
| Provincial and national case law | **SAIJ — Case law** | https://www.saij.gob.ar |
| Commercial case law databases | **El Dial** (paid) | https://www.eldial.com |
| Labor case law | **DT Online / La Ley** (paid) | https://laleyonline.com.ar |
| En banc rulings of National Courts | **Judiciary of the Nation** | https://www.pjn.gov.ar |

---

## 2. Legal Subjects

### `NaturalPerson`

| Property | Source | URL / System |
|---|---|---|
| `nationalIdNumber`, `lastName`, `firstName`, `birthDate`, `nationality` | **RENAPER** (National Registry of Persons) | Official API for agencies: https://www.argentina.gob.ar/renaper |
| `CUIL` | **AFIP/ARCA — CUIL lookup** | https://www.cuitonline.com / ARCA API |
| `maritalStatus`, `domicile` | **Civil Registry** (national or provincial) | Each province's systems |
| `deathDate` | **RENAPER** / Civil Registry | https://www.argentina.gob.ar/renaper |
| `hasFullLegalCapacity` (restrictions) | **National Registry of Interdictions** (RENACI) | Judicial lookup |
| Foreigners' data | **National Directorate of Migration** | https://www.migraciones.gov.ar |

### `LegalEntity` (Private)

| Property | Source | URL / System |
|---|---|---|
| `name`, `CUIT`, `incorporationDate`, `corporatePurpose`, `isRegisteredIn` | **IGJ** (Inspección General de Justicia) — for CABA and national companies | https://www.igj.gov.ar |
| Companies in provinces | **Provincial Public Trade Registries** | Varies by province |
| `CUIT`, tax data | **ARCA (formerly AFIP) — Public lookup** | https://www.afip.gob.ar/cuitverificacion |
| Cooperatives | **INAES** (National Institute of Associativism) | https://www.inaes.gob.ar |
| Mutual societies | **INAES** | https://www.inaes.gob.ar |
| Foundations | **IGJ** | https://www.igj.gov.ar |
| Foreign legal entities | **IGJ** — Foreign companies section | https://www.igj.gov.ar |

### `LegalEntity` (Public)

| Property | Source | URL / System |
|---|---|---|
| National agencies | **Argentina.gob.ar — Organizations** | https://www.argentina.gob.ar/organismos |
| State companies and entities | **FONAFE / Treasury Secretariat** | https://www.argentina.gob.ar |
| National universities | **SPU** (Secretariat of University Policies) | https://www.argentina.gob.ar/educacion/universidades |
| Municipalities | **INDEC — Municipalities** | https://www.indec.gob.ar |

### `Domicile`

| Property | Source | URL / System |
|---|---|---|
| Street and locality gazetteer | **INDEC — Geostatistical Units** | https://www.indec.gob.ar |
| Geolocation and validation | **Address Normalization API (georef)** | https://apis.datos.gob.ar/georef |
| Postal codes | **Correo Argentino** | https://www.correoargentino.com.ar/codigo-postal |

---

## 3. Contracts and Legal Acts

### `Contract` / `LegalAct`

| Property | Source | URL / System |
|---|---|---|
| Contracts with the State | **State Procurement Portal (COMPR.AR)** | https://www.argentinacompra.gov.ar |
| Registered contracts (deeds) | **Real Estate Registry** (each province) | Varies |
| Employment contracts | **AFIP/ARCA — Early Registration System (SIAT)** | https://serviciosweb.afip.gob.ar |
| Insurance contracts | **SSN** (Superintendence of Insurance) | https://www.ssn.gob.ar |
| Lease contracts (declared) | **AFIP/ARCA — RELI** (Registry of Real Estate Leases) | https://www.afip.gob.ar |
| Deeds and notarial acts | **Notary Associations** (each province) | E.g.: https://www.colegio-escribanos.org.ar (CABA) |

### `RealRight` / `Mortgage` / `Pledge`

| Property | Source | URL / System |
|---|---|---|
| Ownership and mortgages on real estate | **Real Estate Registry** (by province) | Varies by province |
| Pledges on vehicles | **National Registry of Vehicle Ownership (RNPA)** | https://www.dnrpa.gov.ar |
| Registered pledge | **National Registry of Pledge Credits** | https://www.dnrpa.gov.ar |
| Vessels | **Argentine Naval Prefecture** | https://www.prefecturanaval.gob.ar |
| Aircraft | **ANAC** (National Civil Aviation Administration) | https://www.anac.gob.ar |

---

## 4. Criminal Law

### `Crime`

| Property | Source | URL / System |
|---|---|---|
| `codeArticle`, `penaltyType`, `minPenalty`, `maxPenalty` | **Criminal Code** (InfoLEG) | https://www.infoleg.gob.ar/infolegInternet/anexos/15000-19999/16546/texact.htm |
| Special criminal laws | **InfoLEG** | https://www.infoleg.gob.ar |
| Crime statistics | **SNIC** (National Criminal Information System) | https://www.argentina.gob.ar/seguridad/snic |

### `Defendant` / `Conviction`

| Property | Source | URL / System |
|---|---|---|
| `criminalProcessStatus`, proceeding data | **Judicial Case Management System** (each jurisdiction) | Varies (MEJ, Lex100, etc.) |
| Criminal record | **National Recidivism Registry** | https://www.argentina.gob.ar/justicia/reincidencia |
| Persons deprived of liberty | **Federal Penitentiary Service** | https://www.spf.gob.ar |
| Provincial penitentiary services | Each provincial Penitentiary Service | Varies |
| Final convictions | **CIJ — Fallos** / judicial case management system | https://www.cij.gov.ar |

### `CriminalPrecautionaryMeasure`

| Property | Source | URL / System |
|---|---|---|
| Restraining orders and detentions | **Judicial Case Management System** | Varies by jurisdiction |
| Inhibitions and interdictions | **RENACI** (National Registry of Interdictions) | Judicial lookup |

---

## 5. Judicial Proceeding

### `Proceeding` / `ProceduralAct` / `Judgment`

| Property | Source | URL / System |
|---|---|---|
| `caseFileNumber`, `caption`, `proceedingStatus`, `startDate` | **Case Lookup System (PJN)** | https://www.pjn.gov.ar |
| CSJN cases | **CSJN — Lookup System** | https://sjconsulta.csjn.gov.ar |
| CABA cases | **CABA Judiciary** | https://www.jusbaires.gov.ar |
| Provincial cases | Each province's lookup systems | Varies |
| Digital case files (GDE) | **GDE** (Electronic Document Management) | For authorized agencies |
| Electronic notifications | **Electronic Notification System (SINOE)** | https://notificaciones.scba.gov.ar (Bs. As.) |

### `Remedy`

| Property | Source | URL / System |
|---|---|---|
| Remedies before the CSJN | **CSJN — Filing Desk** / lookup system | https://sjconsulta.csjn.gov.ar |
| Provincial remedies | Each jurisdiction's management system | Varies |

### `Judge` / `Lawyer`

| Property | Source | URL / System |
|---|---|---|
| `fullName`, `position`, `court`, `appointmentDate` | **CIJ — Magistrates** | https://www.cij.gov.ar |
| Appointments and competitions | **National Council of the Magistracy** | https://www.consejomagistratura.gov.ar |
| Provincial magistrates | Provincial Councils of the Magistracy | Varies |
| Lawyer's `barLicense` | **Bar Association** (by jurisdiction) | E.g.: https://www.cabiabilogados.org.ar (CABA) |

---

## 6. State Bodies

### `StateBody` / `JudicialBody` / `LegislativeBody` / `ExecutiveBody`

| Property | Source | URL / System |
|---|---|---|
| National agencies | **Argentina.gob.ar** | https://www.argentina.gob.ar/organismos |
| Executive Branch organizational structure | **Chief of the Cabinet of Ministers** | https://www.argentina.gob.ar/jefatura |
| Judiciary structure | **CIJ — Judicial Map** | https://www.cij.gov.ar |
| Congress structure | **Chamber of Deputies / Senate** | https://www.diputados.gov.ar / https://www.senado.gob.ar |
| Provincial agencies | Official sites of each province | Varies |

### `PublicProsecution`

| Property | Source | URL / System |
|---|---|---|
| Prosecutor's offices and prosecutors | **Public Prosecutor's Office** | https://www.mpf.gov.ar |
| Public defender's offices | **Public Defense** | https://www.mpd.gov.ar |
| Advisors for minors | **Public Defense** | https://www.mpd.gov.ar |

---

## 7. Labor Law

### `EmploymentRelationship`

| Property | Source | URL / System |
|---|---|---|
| `startDate`, `grossSalary`, `isRegistered`, `contractType` | **AFIP/ARCA — Mi Simplificación (SIAT)** | https://serviciosweb.afip.gob.ar |
| Employee hirings and terminations | **AFIP/ARCA — Labor Relations** | https://www.afip.gob.ar |
| Labor complaints and oversight | **AFIP/ARCA — Labor Formalization** | https://www.afip.gob.ar |
| Labor disputes and cases | **National Labor Court of Appeals (CNAT)** | https://www.pjn.gov.ar |
| Workplace accidents | **SRT** (Superintendence of Work Risks) | https://www.srt.gob.ar |

### `CollectiveBargainingAgreement` (CCT)

| Property | Source | URL / System |
|---|---|---|
| `cctNumber`, `activitySector`, `signatoryUnion`, `ratificationDate` | **MTEySS — Collective Agreements** | https://www.argentina.gob.ar/trabajo/relacioneslaborales/convenioscolectivos |
| Full text of CCTs | **InfoLEG** (ratified CCTs) | https://www.infoleg.gob.ar |
| Wage negotiations and salary updates | **MTEySS** | https://www.argentina.gob.ar/trabajo |

### `Union`

| Property | Source | URL / System |
|---|---|---|
| `name`, `unionStatus`, `representationScope` | **MTEySS — Union Registry** | https://www.argentina.gob.ar/trabajo/sindicatos |

---

## 8. Family Law

### `Marriage` / `CivilUnion`

| Property | Source | URL / System |
|---|---|---|
| `celebrationDate`, `spouse1`, `spouse2`, `regime`, `isDissolved` | **Civil Registry** (national or provincial) | Varies by province |
| Marriage certificates | **RENAPER** (for authorized lookups) | https://www.argentina.gob.ar/renaper |
| Registered civil unions | **Civil Registry** of each jurisdiction | Varies |
| Divorce and dissolution | **Judicial Case Management System** | Varies |

### `Filiation` / `Adoption`

| Property | Source | URL / System |
|---|---|---|
| Acknowledgment of children | **Civil Registry** | Varies by province |
| Birth certificates | **RENAPER** | https://www.argentina.gob.ar/renaper |
| Adoption judgments | **Judicial Case Management System** | Varies |
| Registry of adopters | **RUAGA** (Single Registry of Adoption Guardianship Applicants) | https://www.argentina.gob.ar/justicia/ruaga |

### `ParentalResponsibility`

| Property | Source | URL / System |
|---|---|---|
| Child support agreements | **Judicial Case Management System** | Varies |
| Registry of support debtors | **Registry of Support Debtors** (by province) | E.g.: https://www.scba.gov.ar (Bs. As.) |

---

## 9. Tax Law

### `Tax` / `TaxObligation`

| Property | Source | URL / System |
|---|---|---|
| `taxableEvent`, `rate`, `periodicity`, `creatingNorm` | **ARCA (formerly AFIP) — Regulations** | https://www.afip.gob.ar/normativa |
| VAT, Income Tax rates, etc. | **InfoLEG** + **ARCA** | https://www.infoleg.gob.ar |
| `declaredAmount`, `paidAmount` | **ARCA — Tax Accounts** (with tax key) | https://www.afip.gob.ar |
| Tax debt and payment plans | **ARCA — Mis Facilidades** | https://www.afip.gob.ar |
| Provincial taxes | **AGIP** (CABA), **ARBA** (Bs. As.) and provincial agencies | Varies |
| Collection statistics | **ARCA — Statistics** | https://www.afip.gob.ar/institucional/estudios |

---

## 10. Administrative Law

### `AdministrativeAct`

| Property | Source | URL / System |
|---|---|---|
| National decrees and resolutions | **Official Gazette** | https://www.boletinoficial.gob.ar |
| National administrative files | **GDE** (Electronic Document Management) | For authorized agencies |
| Public file lookup | **TrackiAR / Procedure tracking** | https://www.argentina.gob.ar/tramites |
| Provincial acts | Provincial Official Gazettes | Varies |

### `AdministrativeContract` / `PublicTender`

| Property | Source | URL / System |
|---|---|---|
| National tenders and procurements | **COMPR.AR** (State Procurement Portal) | https://www.argentinacompra.gov.ar |
| Public works contracts | **National Highway Directorate / MOP** | https://www.argentina.gob.ar/obras-publicas |
| Public service concessions | **ENRE, ENARGAS, ORSNA** (by sector) | Varies by regulator |
| Provincial procurements | Provincial procurement portals | Varies |
| Officials' sworn statements | **OA** (Anti-Corruption Office) | https://www.argentina.gob.ar/anticorrupcion |

---

## Cross-Cutting Sources and Aggregators

These sources are useful for multiple ontology classes.

| Source | Classes it feeds | URL |
|---|---|---|
| **InfoLEG** | LegalNorm, Article, CCT, Tax | https://www.infoleg.gob.ar |
| **SAIJ** | LegalNorm, CaseLaw, Doctrine | https://www.saij.gob.ar |
| **Official Gazette** | LegalNorm, AdministrativeAct | https://www.boletinoficial.gob.ar |
| **CIJ** | JudicialBody, Judge, Judgment, Proceeding | https://www.cij.gov.ar |
| **ARCA (formerly AFIP)** | NaturalPerson, LegalEntity, EmploymentRelationship, Tax | https://www.afip.gob.ar |
| **RENAPER** | NaturalPerson, Domicile, Marriage, Filiation | https://www.argentina.gob.ar/renaper |
| **datos.gob.ar APIs** | Domicile, StateBody, statistics | https://datos.gob.ar |
| **Microjuris** (paid) | CaseLaw, LegalNorm | https://www.microjuris.com.ar |
| **La Ley Online** (paid) | CaseLaw, Doctrine, LegalNorm | https://laleyonline.com.ar |
| **El Dial** (paid) | CaseLaw, LegalNorm | https://www.eldial.com |

---

*Argentine Legal System Ontology — Data sources — v1.0 — 2026*
