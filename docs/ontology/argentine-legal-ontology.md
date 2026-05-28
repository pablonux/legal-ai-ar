# Argentine Legal System Ontology
> Formal specification for software systems — Version 1.0 | 2026

---

## Table of Contents

1. [Introduction and Scope](#1-introduction-and-scope)
2. [Hierarchy of Legal Sources](#2-hierarchy-of-legal-sources)
3. [Legal Norm](#3-legal-norm)
4. [Legal Subjects](#4-legal-subjects)
5. [Legal Act and Legal Fact](#5-legal-act-and-legal-fact)
6. [Legal Relationship and Subjective Rights](#6-legal-relationship-and-subjective-rights)
7. [Criminal Law](#7-criminal-law)
8. [Judicial Proceeding](#8-judicial-proceeding)
9. [State Bodies and Judicial System](#9-state-bodies-and-judicial-system)
10. [Labor Law and Social Security](#10-labor-law-and-social-security)
11. [Family Law](#11-family-law)
12. [Tax Law](#12-tax-law)
13. [Administrative Law](#13-administrative-law)
14. [Summary of Relationships Between Classes](#14-summary-of-relationships-between-classes)
15. [Key Instances of the Argentine Legal System](#15-key-instances-of-the-argentine-legal-system)
16. [Guidelines for Software Implementation](#16-guidelines-for-software-implementation)

---

## 1. Introduction and Scope

This ontology defines the conceptual structure of the legal system of the Argentine Republic, designed as a formal foundation for the development of legal software systems. It establishes the classes, properties, relationships, and constraints that allow representing, querying, and reasoning over the Argentine legal order.

### 1.1 Purpose

- Provide a shared, reusable conceptual model of Argentine law.
- Facilitate interoperability between legal information systems.
- Enable automated reasoning over norms, subjects, and legal relationships.
- Serve as a foundation for semantic search engines over legislation and case law.

### 1.2 Scope

The ontology covers the entire Argentine legal order, including:

- Constitutional Law
- Civil and Commercial Law
- Criminal Law
- Labor and Social Security Law
- Administrative Law
- Procedural Law
- Family Law
- Tax Law

### 1.3 Naming Conventions

| Element | Convention | Example |
|---|---|---|
| Classes | PascalCase | `NaturalPerson`, `LegalNorm` |
| Object properties (relationships) | camelCase with prefix | `hasArticle`, `isEnactedBy` |
| Data properties | descriptive camelCase | `enactmentDate`, `normNumber` |
| Instances | camelCase or string | `"26.994"`, `"CCCN"` |

---

## 2. Hierarchy of Legal Sources

The Argentine legal system organizes its sources according to the **Kelsen Pyramid**, recognized by article 31 of the National Constitution.

### 2.1 Class: `LegalSource`

Root class representing any normative source recognized by the Argentine legal order.

| Property | Type | Description |
|---|---|---|
| `hierarchyLevel` | `Integer (1-5)` | Position in the normative pyramid |
| `name` | `String` | Official name of the source |
| `bindingForce` | `Enum {binding, persuasive}` | Normative force |
| `territorialScope` | `Enum {national, provincial, municipal}` | Geographic reach |

### 2.2 Subclasses of `LegalSource`

#### 2.2.1 `ConstitutionalNorm` — Level 1

Supreme norm of the legal order. Includes the National Constitution (1853/60) and treaties with constitutional rank (art. 75 inc. 22 CN).

- National Constitution
- American Convention on Human Rights
- International Covenant on Civil and Political Rights
- Convention on the Rights of the Child
- CEDAW (Convention on the Elimination of Discrimination against Women)

#### 2.2.2 `InternationalTreaty` — Level 2

International treaties without constitutional rank but superior to national laws (art. 75 inc. 22 and 24 CN). Approved by the National Congress.

#### 2.2.3 `NationalLaw` — Level 3

Norms enacted by the National Congress.

- **Ordinary law**: simple majority of both chambers.
- **Special law**: requires qualified majorities.
- **Code**: a systematic body of norms (CCCN, Criminal Code, etc.).

#### 2.2.4 `DecreeLaw` — Level 3

Norms with the force of law issued by the Executive Branch in exceptional circumstances. Includes DNU (art. 99 inc. 3 CN).

#### 2.2.5 `ProvincialNorm` — Level 3-4

Norms issued by the provinces in the exercise of their reserved powers (art. 121 CN). Includes provincial constitutions, provincial laws, and provincial decrees.

#### 2.2.6 `MunicipalNorm` — Level 4

Norms issued by municipalities within their autonomy (art. 123 CN).

- Municipal ordinance
- Resolution of the Deliberative Council
- Mayor's decree

#### 2.2.7 `AdministrativeAct` — Level 4-5

Unilateral declaration of a State body in the exercise of an administrative function that produces direct and individual legal effects (Ley 19.549).

#### 2.2.8 `CaseLaw`

The body of rulings and decisions of the courts. En banc (plenary) rulings are mandatory for the lower judges of the jurisdiction.

#### 2.2.9 `Custom`

Uniform, constant, and generalized practice with a conviction of obligatoriness. Operates as a supplementary source, especially in commercial and family law.

#### 2.2.10 `Doctrine`

Opinions of jurists and academics. A persuasive source without direct binding force.

---

## 3. Legal Norm

The `LegalNorm` class represents any concrete normative provision within the Argentine legal order.

### 3.1 Properties of `LegalNorm`

| Property | Data Type | Cardinality | Description |
|---|---|---|---|
| `normNumber` | `String` | 1..1 | Official number (e.g. `"26.994"`) |
| `name` | `String` | 1..1 | Full name of the norm |
| `commonName` | `String` | 0..1 | Colloquial name (e.g. `"Código Civil y Comercial"`) |
| `enactmentDate` | `Date` | 0..1 | Legislative enactment date |
| `promulgationDate` | `Date` | 0..1 | Date of promulgation by the Executive |
| `publicationDate` | `Date` | 0..1 | Date in the Official Gazette |
| `effectiveDate` | `Date` | 0..1 | Date of entry into force |
| `isInForce` | `Boolean` | 1..1 | Current validity status |
| `fullText` | `String (URL)` | 0..1 | Link to the official text |
| `lawBranch` | `Enum` | 1..N | Classification by legal branch |
| `territorialScope` | `Enum` | 1..1 | Geographic scope of application |

### 3.2 Relationships of `LegalNorm`

| Relationship | Domain | Range | Description |
|---|---|---|---|
| `amends` | `LegalNorm` | `LegalNorm` | Partially or fully amends another norm |
| `repeals` | `LegalNorm` | `LegalNorm` | Expressly or tacitly repeals another norm |
| `regulates` | `LegalNorm` | `LegalNorm` | A lower norm regulates a higher norm |
| `complements` | `LegalNorm` | `LegalNorm` | Complements another norm without repealing it |
| `isEnactedBy` | `LegalNorm` | `StateBody` | State body that enacted the norm |
| `governs` | `LegalNorm` | `LegalInstitution` | Legal institution it regulates |
| `isPublishedIn` | `LegalNorm` | `OfficialPublicationMedium` | Gazette or publication medium |
| `hasArticle` | `LegalNorm` | `Article` | Articles that make up the norm |

### 3.3 Internal Composition: `Article`

An article is the basic structural unit of a legal norm.

| Property | Type | Description |
|---|---|---|
| `articleNumber` | `String` | Number or identifier of the article |
| `normativeText` | `String` | Full textual content |
| `isInForce` | `Boolean` | Whether the article is in force or was repealed |
| `hasClause` | `Clause` | Subdivisions of the article |
| `hasParagraph` | `Paragraph` | Paragraphs that compose it |

---

## 4. Legal Subjects

Legal subjects are those to whom the legal order attributes the capacity to hold rights and obligations (arts. 19-50 CCCN).

### 4.1 Class: `LegalSubject` (root)

| Property | Type | Description |
|---|---|---|
| `identifier` | `String` | CUIT, CUIL, DNI, or unique identifier |
| `domicile` | `Domicile` | Legal or actual domicile |
| `hasCapacity` | `LegalCapacity` | Legal capacity of the subject |

### 4.2 `NaturalPerson`

Every human being has the capacity to hold rights from conception (art. 19 CCCN). Existence ends with death.

| Property | Type | Description |
|---|---|---|
| `lastName` | `String` | Last name(s) of the person |
| `firstName` | `String` | First/given name(s) |
| `birthDate` | `Date` | Date of birth |
| `deathDate` | `Date?` | Date of death, if applicable |
| `nationalIdNumber` | `String` | National Identity Document (DNI) number |
| `CUIL` | `String` | Unique Labor Identification Code |
| `maritalStatus` | `Enum` | `single / married / divorced / widowed / inCivilUnion` |
| `nationality` | `String` | Nationality/ies of the person |
| `actualDomicile` | `Domicile` | Place of habitual residence |
| `legalDomicile` | `Domicile` | Legal domicile established by law |
| `hasFullLegalCapacity` | `Boolean` | Whether they have full capacity to exercise rights |

#### 4.2.1 Capacity to Exercise Rights

Full capacity to exercise rights is reached at 18 (age of majority). There are restrictions:

- **Under 13**: no capacity to exercise rights.
- **Adolescents (13-18)**: progressive capacity depending on the legal act.
- **Persons with restricted capacity**: per a court judgment (art. 32 CCCN).
- **Incapable persons**: declared by a court.

### 4.3 `LegalEntity`

Entities to which the legal order grants the capacity to acquire rights and incur obligations (art. 141 CCCN).

| Property | Type | Description |
|---|---|---|
| `name` | `String` | Corporate name |
| `CUIT` | `String` | Unique Tax Identification Code |
| `incorporationDate` | `Date` | Date of incorporation or founding |
| `legalEntityType` | `Enum` | `public / private` |
| `corporatePurpose` | `String` | Activity or purpose for which it was created |
| `legalDomicile` | `Domicile` | Registered legal domicile |
| `hasGoverningBodies` | `LegalEntityBody` | Administration and oversight bodies |
| `isRegisteredIn` | `PublicRegistry` | Registry where it is registered |

#### 4.3.1 Public Legal Entities

- National State
- Provinces
- Autonomous City of Buenos Aires
- Municipalities
- Autarkic entities (ANSES, ARCA, BCRA, etc.)
- National Universities
- Roman Catholic Apostolic Church

#### 4.3.2 Private Legal Entities

- **Companies** (SA, SRL, SAS, etc.) — Ley 19.550
- **Civil Associations** — art. 168 CCCN
- **Simple Associations** — art. 187 CCCN
- **Foundations** — Ley 19.836
- **Mutual Societies** — Ley 20.321
- **Cooperatives** — Ley 20.337
- **Horizontal Property Consortia**
- **Trusts** with legal personality

---

## 5. Legal Act and Legal Fact

### 5.1 `LegalFact`

An event that produces the creation, modification, or extinction of legal relationships (art. 257 CCCN).

| Subclass | Description | Example |
|---|---|---|
| `NaturalFact` | Produced by nature, without human intervention | Birth, death, passage of time |
| `VoluntaryHumanFact` | A human act with discernment, intention, and freedom | Contracts, intentional crimes |
| `InvoluntaryHumanFact` | Without discernment or full freedom | Acts of an undeclared mentally incapacitated person |

### 5.2 `LegalAct`

A lawful voluntary act whose immediate purpose is the acquisition, modification, or extinction of legal relationships (art. 259 CCCN).

| Property | Type | Description |
|---|---|---|
| `date` | `Date` | Date the act was executed |
| `place` | `String` | Place of execution |
| `isOnerous` | `Boolean` | Whether it has economic consideration |
| `isConditional` | `Boolean` | Whether it is subject to a condition |
| `isModal` | `Boolean` | Whether it has a charge or mode |
| `requiredForm` | `FormType` | Form required by law |
| `isVoid` | `Boolean` | Whether it was declared void |
| `nullityCause` | `Enum` | Defect that generates the nullity |

#### 5.2.1 Defects of the Legal Act

| Defect | Type of Nullity | Norm |
|---|---|---|
| Essential error | Relative | Art. 265-270 CCCN |
| Fraud (dolo) | Relative | Art. 271-275 CCCN |
| Violence (force/intimidation) | Relative | Art. 276-278 CCCN |
| Injury (lesión) | Relative | Art. 332 CCCN |
| Simulation | Absolute or relative | Art. 333-337 CCCN |
| Fraud (against creditors) | Unenforceability | Art. 338-342 CCCN |
| Unlawful object | Absolute | Art. 279 CCCN |
| Unlawful cause | Absolute | Art. 281-282 CCCN |

### 5.3 `Contract`

A legal act by which two or more parties express their consent to create, regulate, modify, transfer, or extinguish patrimonial legal relationships (art. 957 CCCN).

| Property | Type | Description |
|---|---|---|
| `contractType` | `Enum` | Type of contract per the legal classification |
| `contractingParties` | `LegalSubject[]` | Parties that enter into the contract |
| `contractObject` | `String` | Object of the contract |
| `priceOrConsideration` | `Decimal` | Agreed economic value (if applicable) |
| `term` | `Duration` | Duration or term of the contract |
| `executionForm` | `Enum` | `written / verbal / electronic / public deed` |
| `applicableJurisdiction` | `Jurisdiction` | Jurisdiction for disputes |
| `applicableLaw` | `LegalNorm` | Law governing the contract |

#### 5.3.1 Main Contract Types (CCCN)

| Contract | CCCN Articles | Characteristics |
|---|---|---|
| Sale | 1123-1169 | Transfer of ownership in exchange for a price in money |
| Barter | 1172-1175 | Exchange of things or rights |
| Lease | 1187-1250 | Use and enjoyment of a thing in exchange for a price |
| Mandate | 1319-1334 | Management of affairs on behalf of the principal |
| Surety | 1574-1598 | Personal guarantee of another's obligation |
| Donation | 1542-1573 | Free transfer of a thing or right |
| Loan for consumption (Mutuo) | 1525-1532 | Loan of fungible things |
| Loan for use (Comodato) | 1533-1541 | Free loan for use |
| Assignment of rights | 1614-1631 | Transfer of rights to a third party |
| Employment contract | Ley 20.744 | Subordinate, dependent employment relationship |
| Insurance | Ley 17.418 | Risk coverage in exchange for a premium |
| Franchise | 1512-1524 | License of a brand and business system |

---

## 6. Legal Relationship and Subjective Rights

### 6.1 `LegalRelationship`

A bond between legal subjects, governed by the legal order, that assigns one the position of holder of a right and the other that of the obligor.

| Property | Type | Description |
|---|---|---|
| `activeSubject` | `LegalSubject` | Holder of the right (creditor, owner) |
| `passiveSubjects` | `LegalSubject[]` | Obligors (debtor, third parties) |
| `relationshipObject` | `String` | Good, conduct, or abstention it falls upon |
| `relationshipSource` | `LegalSource` | Norm or fact that generates the relationship |
| `isPatrimonial` | `Boolean` | Whether it has economic content |
| `isTransferable` | `Boolean` | Whether it can be transferred to third parties |

### 6.2 Real Rights (Rights in rem)

Rights exercised over a thing (art. 1882 CCCN). They are a closed set (**numerus clausus**).

| Real Right | CCCN Arts. | Characteristics |
|---|---|---|
| Ownership | 1941-1982 | The fullest real right: use, enjoy, and dispose |
| Co-ownership | 1983-2036 | Ownership by several over the same thing |
| Horizontal Property | 2037-2072 | Ownership over functional units in buildings |
| Real Estate Developments | 2073-2086 | Gated communities, private neighborhoods, industrial parks |
| Timeshare | 2087-2102 | Periodic use of real estate or a movable thing |
| Private Cemetery | 2103-2113 | Right over burial plots |
| Surface | 2114-2128 | Right to plant, forest, or build on another's property |
| Usufruct | 2129-2153 | Use and enjoyment of another's thing without altering its substance |
| Use | 2154-2157 | Limited use and enjoyment of another's thing |
| Habitation | 2158-2161 | Right to dwell in another's house |
| Easement | 2162-2183 | Charge on real estate for the benefit of another |
| Mortgage | 2205-2211 | Real guarantee over real estate without dispossession |
| Antichresis | 2212-2218 | Real guarantee with delivery of the property to the creditor |
| Pledge | 2219-2237 | Real guarantee over movable things |

### 6.3 Personal Rights / Obligations

An obligation is a legal relationship by virtue of which the creditor has the right to demand a performance from the debtor (art. 724 CCCN).

| Property | Type | Description |
|---|---|---|
| `creditor` | `LegalSubject` | Active subject of the obligation |
| `debtor` | `LegalSubject[]` | Passive subject(s) |
| `performanceType` | `Enum` | `give / do / not do` |
| `performanceObject` | `String` | Description of the owed performance |
| `debtAmount` | `Decimal` | Amount in pesos or foreign currency |
| `interestRate` | `Decimal` | Applicable interest rate |
| `interestType` | `Enum` | `compensatory / default / punitive` |
| `dueDate` | `Date` | Date of enforceability |
| `isExtinguished` | `Boolean` | Whether the obligation was fulfilled or extinguished |
| `extinctionCause` | `Enum` | Cause of extinction |

#### 6.3.1 Means of Extinction of Obligations

- Payment (art. 865 CCCN)
- Novation (art. 933 CCCN)
- Set-off (art. 921 CCCN)
- Merger (art. 931 CCCN)
- Waiver (art. 944 CCCN)
- Debt remission (art. 950 CCCN)
- Impossibility of performance (art. 955 CCCN)
- Liberative prescription (art. 2532 CCCN)
- Settlement (art. 1641 CCCN)

---

## 7. Criminal Law

Criminal law governs the State's *ius puniendi*, defining crimes, penalties, and guarantees (Criminal Code, Ley 11.179 and amendments).

### 7.1 Class: `Crime`

| Property | Type | Description |
|---|---|---|
| `crimeType` | `Enum` | `intentional / negligent / preterintentional` |
| `classification` | `Enum` | `felony / misdemeanor / infraction` |
| `protectedLegalInterest` | `String` | Legal interest the criminal norm protects |
| `isAttempt` | `Boolean` | Whether it is an attempt or a completed crime |
| `isContinuing` | `Boolean` | Whether it is a continuing crime |
| `codeArticle` | `String` | Applicable Criminal Code article |
| `minPenalty` | `Integer` | Minimum penalty in months or years |
| `maxPenalty` | `Integer` | Maximum penalty in months or years |
| `penaltyType` | `Enum` | `prison / reclusion / fine / disqualification` |
| `allowsSuspendedSentence` | `Boolean` | Whether it can be suspended |
| `prescription` | `Integer` | Prescription period in years |

### 7.2 Classification of Crimes by Legal Interest

| Category | Examples | CP Arts. |
|---|---|---|
| Crimes against persons | Homicide, injury, abandonment | 79-108 |
| Crimes against honor | Slander, libel | 109-117 bis |
| Crimes against sexual integrity | Sexual abuse, rape | 119-133 |
| Crimes against freedom | Reduction to servitude, trafficking | 140-149 ter |
| Crimes against property | Theft, robbery, fraud, embezzlement | 162-185 |
| Crimes against public safety | Arson, devastation, air piracy | 186-213 bis |
| Crimes against public order | Sedition, unlawful association | 209-213 bis |
| Crimes against public administration | Bribery, peculation, malversation | 237-281 bis |
| Crimes against public faith | Forgery of documents and currency | 282-302 |
| Crimes against the economic order | Money laundering (Ley 25.246) | 303-313 |

### 7.3 Class: `Defendant`

A natural person against whom criminal prosecution is directed.

| Property | Type | Description |
|---|---|---|
| `isDefendant` | `NaturalPerson` | Link to the NaturalPerson class |
| `criminalProcessStatus` | `Enum` | `accused / indicted / dismissed / convicted / acquitted` |
| `precautionaryMeasures` | `PrecautionaryMeasure[]` | Restrictive measures applied |
| `defenseCounsel` | `Lawyer` | Appointed or public defense counsel |
| `hasRecord` | `Boolean` | Whether they have prior convictions |
| `isRepeatOffender` | `Boolean` | Whether they qualify as a repeat offender (art. 50 CP) |

### 7.4 Class: `Conviction`

| Property | Type | Description |
|---|---|---|
| `convictionDate` | `Date` | Date of the convicting judgment |
| `court` | `JudicialBody` | Court that issued the conviction |
| `imposedPenalty` | `String` | Full description of the penalty |
| `durationYears` | `Integer` | Duration of the custodial penalty |
| `isSuspended` | `Boolean` | Whether enforcement is suspended |
| `isFinal` | `Boolean` | Whether the conviction is final |
| `endDate` | `Date` | Date of completion of the penalty |
| `penitentiaryFacility` | `String` | Prison unit (if applicable) |

---

## 8. Judicial Proceeding

The judicial proceeding is the set of ordered procedural acts that culminate in a jurisdictional decision. It is governed by the Procedural Codes of each jurisdiction.

### 8.1 Class: `Proceeding`

| Property | Type | Description |
|---|---|---|
| `caseFileNumber` | `String` | Court case file number |
| `caption` | `String` | Designation of the proceeding (parties and object) |
| `procedureType` | `Enum` | `civil / criminal / labor / administrativeLitigation / family` |
| `procedureSubtype` | `Enum` | `ordinary / summary / enforcement / amparo / etc.` |
| `startDate` | `Date` | Start date or filing of the complaint |
| `proceedingStatus` | `Enum` | `initiated / in progress / awaiting judgment / appealed / concluded` |
| `jurisdiction` | `Jurisdiction` | Jurisdictional scope (`federal / provincial / CABA`) |
| `court venue` | `Enum` | `civil / criminal / labor / commercial / family / litigation` |
| `court` | `JudicialBody` | Intervening court |
| `judge` | `Judge` | Judge in charge of the proceeding |
| `instance` | `Enum` | `first / second / third / extraordinary` |

### 8.2 Procedural Parties

| Class | Description | Relationship |
|---|---|---|
| Plaintiff/Claimant | Who initiates the judicial action | `hasPlaintiff → LegalSubject` |
| Defendant | Against whom the action is directed | `hasDefendant → LegalSubject` |
| Third party | Who intervenes without being an original party | `hasIntervenor → LegalSubject` |
| Complainant | Victim who drives the criminal action | `hasComplainant → LegalSubject` |
| Public Prosecutor's Office | Public accusation in criminal proceedings | `hasRepresentative → Prosecutor` |
| Public Defender | Defense in cases of financial incapacity | `hasDefenseCounsel → Lawyer` |

### 8.3 `ProceduralAct`

| Property | Type | Description |
|---|---|---|
| `actType` | `Enum` | `complaint / answer / evidence / judgment / appeal / etc.` |
| `actDate` | `Date` | Date the act was performed |
| `performedBy` | `ProceduralSubject` | Who performed the act |
| `actContent` | `String (URL)` | Text or document of the procedural act |
| `isNotified` | `Boolean` | Whether it was notified to the parties |
| `notificationDate` | `Date` | Date of notification |

### 8.4 `Judgment`

The judgment is the procedural act of the judge that ends the proceeding by resolving the merits of the controversy.

| Property | Type | Description |
|---|---|---|
| `judgmentType` | `Enum` | `final / interlocutory / on the merits` |
| `issueDate` | `Date` | Date of the judgment |
| `outcome` | `Enum` | `favorable / unfavorable / partial` |
| `isFinal` | `Boolean` | Whether it is final (not appealed or the term expired) |
| `costs` | `Enum` | `on the loser / split / no costs` |
| `hasReasoning` | `String` | Legal grounds of the decision |
| `isAppealable` | `Boolean` | Whether it admits an appeal |
| `filedAppeal` | `Appeal` | Appeal filed against the judgment |

### 8.5 Procedural Remedies

| Remedy | Type | Competent Court |
|---|---|---|
| Clarification | Ordinary | Same court that issued the decision |
| Appeal | Ordinary | Court of Appeals |
| Nullity | Ordinary | Court of Appeals |
| Cassation | Local extraordinary | Provincial Superior Court of Justice |
| Inapplicability of law | Local extraordinary | Provincial Supreme Court or TSJ |
| Federal Extraordinary Appeal (REF) | Extraordinary | Supreme Court of Justice of the Nation |
| Complaint for denial | Extraordinary | Supreme Court of Justice of the Nation |

---

## 9. State Bodies and Judicial System

### 9.1 Class: `StateBody`

| Property | Type | Description |
|---|---|---|
| `name` | `String` | Official name of the body |
| `branchOfGovernment` | `Enum` | `Legislative / Executive / Judicial / Public Prosecution` |
| `jurisdictionalScope` | `Enum` | `federal / provincial / municipal / CABA` |
| `normativeBasis` | `LegalNorm` | Norm that creates or regulates the body |
| `competence` | `String` | Description of the material competence |

### 9.2 Federal Judicial System

| Body | Instance | Competence |
|---|---|---|
| Supreme Court of Justice | Supreme | Constitutional review, REF, original jurisdiction |
| Federal Courts of Appeals | Second | Appeals in federal matters |
| Federal Courts | First | Civil, criminal, federal administrative litigation |
| National Electoral Court | Second | National electoral matters |
| Federal Criminal Cassation Court | Third criminal | Unification of federal criminal case law |
| Federal Oral Courts | Single criminal | Oral and public trial in federal criminal matters |

### 9.3 Public Prosecution

| Body | Function |
|---|---|
| Attorney General of the Nation | Highest authority of the Public Prosecutor's Office |
| General Public Defender of the Nation | Highest authority of the Public Defense |
| Prosecutors | Exercise of the public criminal action |
| Public Defenders | Defense of defendants without financial resources |
| Advisors for Minors and Incapacitated Persons | Protection of the interests of children and incapacitated persons |

### 9.4 Class: `Judge`

| Property | Type | Description |
|---|---|---|
| `fullName` | `String` | Full name of the judge |
| `position` | `Enum` | `judge / associate judge / justice / appellate judge / substitute judge` |
| `court` | `JudicialBody` | Court they belong to |
| `appointmentDate` | `Date` | Date of appointment |
| `appointmentMethod` | `String` | Appointment mechanism (impeachment council, competition, etc.) |
| `hasLifeTenure` | `Boolean` | Whether they serve with irremovability until age 75 |
| `specialty` | `Enum` | `civil / criminal / labor / electoral / constitutional / etc.` |

---

## 10. Labor Law and Social Security

Argentine labor law is governed by the Employment Contract Law (LCT, Ley 20.744) and its amendments, with constitutional protective principles (art. 14 bis CN).

### 10.1 Class: `EmploymentRelationship`

| Property | Type | Description |
|---|---|---|
| `employer` | `LegalSubject` | Natural or legal person who employs |
| `employee` | `NaturalPerson` | Natural person who provides the service |
| `contractType` | `Enum` | `indefinite / fixed-term / occasional / seasonal / apprenticeship` |
| `startDate` | `Date` | Start date of the employment relationship |
| `endDate` | `Date?` | Date of termination of the bond |
| `terminationCause` | `Enum` | `dismissal without cause / resignation / mutual agreement / just cause / retirement` |
| `grossSalary` | `Decimal` | Gross monthly salary in pesos |
| `agreementCategory` | `String` | Category per the applicable CCT |
| `collectiveAgreement` | `CollectiveBargainingAgreement` | CCT governing the relationship |
| `workSchedule` | `Enum` | `full time / part time / shift` |
| `weeklyHours` | `Integer` | Weekly working hours |
| `isRegistered` | `Boolean` | Whether it is registered with ARCA (formerly AFIP) |

### 10.2 Worker's Rights

- **Maximum working day**: 8 hours daily / 48 hours weekly (Ley 11.544)
- **Weekly rest**: 35 continuous hours starting Saturday (Ley 11.544)
- **Paid annual vacation**: 14-35 days depending on seniority (art. 150 LCT)
- **SAC (annual bonus)**: 2 annual installments in June and December (Ley 23.041)
- **Maternity leave**: 90 days (art. 177 LCT)
- **Paternity leave**: 15 continuous days (Ley 27.591)
- **Seniority severance**: 1 month's salary per year worked (art. 245 LCT)
- **Prior notice**: 15 days to 2 months depending on seniority (art. 231 LCT)

### 10.3 Class: `CollectiveBargainingAgreement` (CCT)

| Property | Type | Description |
|---|---|---|
| `cctNumber` | `String` | Identifying number of the agreement |
| `activitySector` | `String` | Activity or sector it regulates |
| `signatoryUnion` | `String` | Signatory union(s) |
| `employerChamber` | `String` | Employer chamber or representation |
| `applicationScope` | `Enum` | `by activity / by company / by trade` |
| `ratificationDate` | `Date` | Date of ratification by the MTEySS |
| `validity` | `Date` | Agreed expiration date |

---

## 11. Family Law

Family law governs relationships arising from kinship, marriage, and civil union (arts. 401-723 CCCN).

### 11.1 Class: `FamilyBond`

| Subclass | Description | CCCN Norm |
|---|---|---|
| `Marriage` | Voluntary union of two persons with a vocation of permanence | Art. 401-445 |
| `CivilUnion` | Registered or proven affective cohabitation of a couple | Art. 509-528 |
| `Kinship` | Legal bond by consanguinity, affinity, or adoption | Art. 529-536 |
| `Filiation` | Legal bond between parents and children | Art. 558-593 |
| `Adoption` | Legal act that creates filiation by court judgment | Art. 594-637 |
| `ParentalResponsibility` | Set of rights and duties of the parents | Art. 638-704 |

### 11.2 Class: `Marriage`

| Property | Type | Description |
|---|---|---|
| `spouse1` | `NaturalPerson` | First spouse |
| `spouse2` | `NaturalPerson` | Second spouse |
| `celebrationDate` | `Date` | Date of the civil marriage |
| `celebrationPlace` | `String` | Civil Registry where it was celebrated |
| `regime` | `Enum` | `community of gains / separation of property` |
| `isDissolved` | `Boolean` | Whether the marriage was dissolved |
| `dissolutionCause` | `Enum` | `divorce / death / annulment` |
| `dissolutionDate` | `Date?` | Date of dissolution |

---

## 12. Tax Law

Tax law governs the State's power to demand taxes (Ley 11.683 on Tax Procedure and special laws).

### 12.1 Class: `Tax`

| Property | Type | Description |
|---|---|---|
| `name` | `String` | Name of the tax |
| `taxType` | `Enum` | `tax / fee / special contribution` |
| `jurisdictionalLevel` | `Enum` | `national / provincial / municipal` |
| `collectingBody` | `StateBody` | Collecting entity (ARCA, provincial DGR, etc.) |
| `taxableEvent` | `String` | Event or situation that generates the obligation |
| `passiveSubjects` | `LegalSubject[]` | Taxpayers and responsible parties |
| `taxableBase` | `String` | Calculation base of the tax |
| `rate` | `Decimal` | Applicable rate or percentage |
| `periodicity` | `Enum` | `monthly / annual / one-time / per transaction` |
| `creatingNorm` | `LegalNorm` | Law that creates the tax |

### 12.2 Main National Taxes

| Tax | Type | General Rate | Norm |
|---|---|---|---|
| Income Tax | Direct tax | Progressive scale | Ley 20.628 |
| VAT | Indirect tax | 21% (general) | Ley 23.349 |
| Personal Assets Tax | Direct tax | Variable by net worth | Ley 23.966 |
| Tax on Credits and Debits | Indirect tax | 0.6% each | Ley 25.413 |
| Export Duties | Trade tax | Variable by product | Customs Code |
| Social Security Contributions | Special contribution | 28% employer + 17% employee | Ley 24.241 |

---

## 13. Administrative Law

Administrative law governs the organization and activity of the Public Administration (Ley 19.549 and its regulations).

### 13.1 Class: `AdministrativeAct`

A unilateral declaration of a State body in the exercise of an administrative function that produces direct legal effects (art. 7 Ley 19.549).

| Property | Type | Description |
|---|---|---|
| `actType` | `Enum` | `decree / resolution / disposition / ordinance / circular` |
| `issuingBody` | `StateBody` | Body that issued the act |
| `date` | `Date` | Date of issuance |
| `actNumber` | `String` | Identifying number |
| `recipient` | `LegalSubject` | To whom it is addressed (if an individual) |
| `isRegulatory` | `Boolean` | Whether it has general or individual effects |
| `isReasoned` | `Boolean` | Whether it contains sufficient reasoning (art. 7 inc. e) |
| `isNotified` | `Boolean` | Whether it was notified to the interested party |
| `isFinal` | `Boolean` | Whether it became final (not appealed) |
| `isEnforced` | `Boolean` | Whether it was executed by the Administration |
| `availableRemedies` | `Remedy[]` | Available administrative remedies |

### 13.2 Administrative Remedies

| Remedy | Term | Before whom | Norm |
|---|---|---|---|
| Reconsideration | 10 business days | Same body | Art. 84 Decreto 1759/72 |
| Hierarchical | 15 business days | Hierarchical superior | Art. 89 Decreto 1759/72 |
| Appeal (alzada) | 15 business days | Competent Ministry | Art. 94 Decreto 1759/72 |
| Complaint | At any time | Superior, for delay | Art. 71 Decreto 1759/72 |

### 13.3 Administrative Contract and Public Service

| Class | Description | Main norm |
|---|---|---|
| `AdministrativeContract` | Contract entered into by the State with private parties | Ley 13.064 and Ley 27.437 |
| `PublicTender` | Procedure for selecting the contractor | Ley 27.437 |
| `Concession` | Delegation of public service management to a private party | Special laws by sector |
| `PrecariousPermit` | Provisional and revocable authorization | Sector regulations |

---

## 14. Summary of Relationships Between Classes

Main object properties of the ontology, with domain, range, and cardinality.

| Relationship | Domain | Range | Cardinality |
|---|---|---|---|
| `isEnactedBy` | `LegalNorm` | `StateBody` | N:1 |
| `amends` | `LegalNorm` | `LegalNorm` | N:N |
| `repeals` | `LegalNorm` | `LegalNorm` | N:N |
| `regulates` | `LegalNorm` | `LegalNorm` | N:N |
| `hasArticle` | `LegalNorm` | `Article` | 1:N |
| `isPartyIn` | `LegalSubject` | `Proceeding` | N:N |
| `hasDefenseCounsel` | `Proceeding` | `Lawyer` | N:1 |
| `hasJudge` | `Proceeding` | `Judge` | N:1 |
| `issuesJudgment` | `Judge` | `Judgment` | 1:N |
| `isConvictedOf` | `Conviction` | `Crime` | 1:N |
| `hasDefendant` | `Proceeding` | `Defendant` | 1:N |
| `enters` | `LegalSubject` | `Contract` | N:N |
| `isCreditorOf` | `LegalSubject` | `Obligation` | N:N |
| `isDebtorOf` | `LegalSubject` | `Obligation` | N:N |
| `hasEmploymentRelationship` | `NaturalPerson` | `EmploymentRelationship` | 1:N |
| `employs` | `LegalSubject` | `EmploymentRelationship` | 1:N |
| `isMarriedTo` | `NaturalPerson` | `NaturalPerson` | 1:1 |
| `hasChild` | `NaturalPerson` | `NaturalPerson` | 1:N |
| `paysTaxIn` | `LegalSubject` | `Tax` | N:N |
| `appealsAgainst` | `ProceduralSubject` | `AdministrativeAct` | N:N |

---

## 15. Key Instances of the Argentine Legal System

### 15.1 Fundamental Norms

| Norm | Number | Effective Date | Description |
|---|---|---|---|
| National Constitution | — | 01/05/1853 (1994 reform) | Supreme law of the Nation |
| Civil and Commercial Code | 26.994 | 01/08/2015 | General private law regime |
| Criminal Code | 11.179 | 29/04/1922 (with reforms) | Crimes and penalties |
| Employment Contract Law | 20.744 | 20/09/1974 (with reforms) | General labor regime |
| Tax Procedure Law | 11.683 | 12/01/1933 (with reforms) | Procedure before ARCA/AFIP |
| Administrative Procedure Law | 19.549 | 03/04/1972 | Administrative acts and remedies |
| General Companies Law | 19.550 | 25/04/1972 (with reforms) | Company types |
| Civil and Commercial Procedure Code | 17.454 | 07/11/1967 (Fed./CABA) | Federal civil procedure |
| Federal Criminal Procedure Code | 27.063 | 10/12/2014 (gradual impl.) | Adversarial criminal procedure |

### 15.2 Key Organizations

| Organization | Type | Main function |
|---|---|---|
| ARCA (formerly AFIP) | National autarkic entity | Tax, customs, and social security collection |
| BCRA | National autarkic entity | Regulation and supervision of the financial system |
| IGJ | Deconcentrated body | Registration and control of legal entities |
| INDEC | Technical body | Statistics and census |
| ANSES | National autarkic entity | Administration of pensions and allowances |
| CNV | National autarkic entity | Regulation of the capital markets |
| Ombudsman's Office | Independent body | Oversight of public administration, protection of rights |
| Public Prosecutor's Office | Extra-branch body | Public criminal action and defense of the general interest |

---

## 16. Guidelines for Software Implementation

### 16.1 Recommended Data Model

| Use Case | Recommended Technology | Rationale |
|---|---|---|
| Relational database | PostgreSQL | Referential integrity, complex queries, JSON support |
| Knowledge graph | Neo4j / Apache Jena | Complex relationship queries (SPARQL/Cypher) |
| Semantic Web | OWL 2 + RDF + SPARQL | Interoperability and automatic reasoning |
| Search engine | Elasticsearch | Full-text search over norms and case law |
| REST API | FastAPI / Django REST | Exposing the ontology to other systems |

### 16.2 Recommended Unique Identifiers

- **Norms**: `{type}-{number}-{year}` — E.g.: `LEY-26994-2014`
- **Natural persons**: DNI or CUIL — E.g.: `20-12345678-4`
- **Legal entities**: CUIT — E.g.: `30-70012345-9`
- **Judicial proceedings**: `{jurisdiction}-{year}-{number}` — E.g.: `CABA-2024-00123`
- **Administrative files**: GDE or each organization's system

### 16.3 Security and Privacy Considerations

- Personal data is protected by **Ley 25.326** on Personal Data Protection.
- The processing of sensitive data (health, political affiliation, religion) requires the data subject's express consent.
- Systems must be registered with the **Agency for Access to Public Information (AAIP)**.
- Judicial proceeding data has access regulated by CSJN resolutions and procedural legislation.
- Apply encryption at rest and in transit for personal and sensitive data.

### 16.4 Ontology Updates

The Argentine legal order is dynamic. It is recommended to:

- Monitor the **Official Gazette** of the Argentine Republic (`boletinoficial.gob.ar`) for new norms.
- Subscribe to **InfoLEG** alerts (`infoleg.gob.ar`) for amendments to current laws.
- Implement **semantic versioning** for the ontology (`MAJOR.MINOR.PATCH`).
- Maintain a **changelog** with the norm that motivated each update.
- Define an update committee with lawyers specialized in each law branch.

---

*Argentine Legal System Ontology — v1.0 — 2026*
