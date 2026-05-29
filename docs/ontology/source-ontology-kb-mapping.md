> 📦 **Ported reference.** Preserved from the previous documentation set as a reference for how
> semi-structured sources are transformed through the ontology into the Knowledge Base. Spanish
> original retained; diagrams are the sibling `source-ontology-kb-*.mermaid` files.

# Fuentes → Ontología → Knowledge Base — Diagramas de Transformación

| Campo | Valor |
|---|---|
| **Fecha** | 2026-04-29 |
| **Propósito** | Documentar el proceso de pasaje de fuentes semi-estructuradas diversas a través del modelo ontológico hacia la Knowledge Base |

---

## 1. Visión general: Pipeline de conocimiento

```mermaid
flowchart LR
    subgraph sources [Fuentes Semi-Estructuradas]
        CSJN["CSJN API\n8 endpoints REST\n+ PDF"]
        SAIJ_J["SAIJ\nJurisprudencia\nHTML + PDF"]
        SAIJ_L["SAIJ\nLegislación\nAPI REST"]
        PJN["PJN\nHTML + PDF"]
        SCBA["SCBA\nHTML + PDF"]
    end

    subgraph pipeline [Pipeline de Ingesta]
        Crawler --> Parser
        Parser --> Enrichment
        Enrichment --> Indexer
    end

    subgraph ontology [Modelo Ontológico]
        direction TB
        ONT["10 Clases Fundamentales\nlegar:*"]
    end

    subgraph kb [Knowledge Base]
        SQL["Azure SQL\n37+ entidades"]
        Search["Azure AI Search\n2 índices"]
        Graph["Grafo relacional\nSQL recursive CTEs"]
    end

    sources --> Crawler
    ONT -.->|"guía el\nmapeo"| pipeline
    Indexer --> kb
```

---

## 2. Descomposición de fuentes por clase ontológica

Cada fuente semi-estructurada aporta datos que se mapean a una o más clases del modelo ontológico. Este diagrama muestra qué provee cada fuente.

```mermaid
flowchart TB
    subgraph csjn [CSJN API — 8 Endpoints]
        E1["abrirAnalisis\ncarátula, competencia,\ntipoAccion, inconstitucional,\nvotos, referenciasNormativas"]
        E2["getSumariosAnalisis\ntexto doctrinal, tomo/página,\nfechaFallo, vocesSumario"]
        E3["getCitas\nalias, idFallo,\ntextoCita"]
        E4["getCitantes\nHTML con idAnalisis\nde fallos citantes"]
        E5["getAllDocumentos\nPDF del fallo"]
        E6["getDictamenesAnalisis\ndictámenes fiscales"]
        E7["getSintesisAnalisis\nsíntesis editoriales"]
        E8["getEnlacesAnalisis\nenlaces externos"]
    end

    subgraph ont [Clases Ontológicas]
        Sentencia["Sentencia\nlegar:Jurisprudencia"]
        Norma["NormaJuridica"]
        Sujeto["SujetoDeDerecho"]
        Organo["OrganoEstatal\nTribunal"]
        Proceso["ProcesoJudicial"]
        Fuente["FuenteDelDerecho"]
    end

    E1 -->|"caratula, competencia\ntipoAccion, votos"| Sentencia
    E1 -->|"votosAnalisisDocumental\nstringMayoria/Disidencia"| Sujeto
    E1 -->|"competencia.valor\ninstancia fija = CSJN"| Organo
    E1 -->|"referenciasNormativas"| Norma
    E1 -->|"identificacionExpediente"| Proceso

    E2 -->|"texto doctrinal\ntomo/pagina"| Sentencia
    E2 -->|"vocesSumario"| Fuente

    E3 -->|"alias, idFallo\ntextoCita"| Sentencia
    E4 -->|"idAnalisis citantes"| Sentencia

    E5 -->|"PDF → FullText"| Sentencia
    E6 -->|"dictamen fiscal"| Sujeto
    E7 -->|"síntesis"| Sentencia
```

---

## 3. Ontología como eje de mapeo

El modelo ontológico define **qué conceptos existen** en el dominio jurídico. La KB implementa un subconjunto de estos conceptos como entidades persistentes. Este diagrama muestra la correspondencia.

```mermaid
flowchart LR
    subgraph ontClasses [Modelo Ontológico — 10 Clases]
        NJ["NormaJuridica\nConstitución, Ley, Decreto,\nTratado, Resolución, Acordada"]
        SD["SujetoDeDerecho\nPersonaHumana, PersonaJurídica\nJuez, Fiscal, Abogado"]
        OE["OrganoEstatal\nTribunal, MinisterioPúblico\nLegislativo, Ejecutivo"]
        HJ["HechoJuridico\nActoJurídico, Delito\nContrato"]
        RJ["RelacionJuridica\nDerechos, Obligaciones"]
        PJ["ProcesoJudicial\nCivil, Penal, Laboral\nConstitucional"]
        FD["FuenteDelDerecho\nJurisprudencia, Doctrina\nCostumbre"]
        JU["Jurisdiccion\nFederal, Provincial\nCABA"]
        SE["Sentencia\n(via Jurisprudencia)"]
        RE["Recurso\n(via ProcesoJudicial)"]
    end

    subgraph kbEntities [KB — Entidades Persistentes]
        Statute["Statute\nNumber, NormType,\nNormativeLevel, LegalBranch"]
        Person["Person\nDisplayName, PersonType,\nFirstName, LastName"]
        Court["Court\nName, JurisdictionArea,\nInstance, CourtCategory"]
        Ruling["Ruling\nCaseTitle, Summary,\nHolding, FullText"]
        JP["JudicialProceeding\nCaseNumber, DisplayName"]
        Vote["Vote\nVoteType, Summary"]
        Sumario["Sumario\nText, Volume, Page"]
        Citation["Citation\nExternalAlias, CitationType"]
        Keyword["Keyword\nDescriptor, ThesaurusId"]
    end

    NJ -->|"implementada"| Statute
    SD -->|"implementada"| Person
    OE -->|"parcial: Tribunal"| Court
    PJ -->|"implementada"| JP
    SE -->|"implementada"| Ruling
    FD -->|"parcial: voces"| Keyword

    HJ -.->|"no implementada"| HJ
    RJ -.->|"no implementada"| RJ
    RE -.->|"futura"| RE
```

---

## 4. Pipeline de transformación por etapas

```mermaid
flowchart TB
    subgraph stage0 [Crawler — Descubrimiento]
        CrawlerIn["CrawlerMessage\nSourceId, Type,\nDateFrom/DateTo"]
        CrawlerOut["ParserMessage\nDocumentId, AnalysisId,\nBlobPathPdf, ContentHash,\nRulingDateHint"]
        CrawlerIn --> Dedup["Deduplicación\nSHA-256 ContentHash"]
        Dedup --> CrawlerOut
    end

    subgraph stage1 [Parser — Extracción Estructurada]
        PM["ParserMessage"]
        API["8 CSJN API calls\nabrirAnalisis\ngetSumariosAnalisis\ngetCitas / getCitantes\ngetAllDocumentos\ngetDictamenesAnalisis\ngetSintesisAnalisis\ngetEnlacesAnalisis"]
        PDF["PDF → PdfPig\nNormalización de texto"]
        Meta["CsjnApiMetadata\nCampos escalares +\nKeywords, Citations, Votes,\nStatutes, Sumarios"]
        Missing["missingFields[]\njudges, cited_statutes,\ncitation_types,\nprosecutor_opinion"]
        EM["EnrichmentMessage\nExtractedMetadata +\nNormalizedText +\nMissingFields"]

        PM --> API
        PM --> PDF
        API --> Meta
        Meta --> Missing
        PDF --> EM
        Missing --> EM
        Meta --> EM
    end

    subgraph stage2 [Enrichment — LLM Gap-Fill]
        EMIn["EnrichmentMessage"]
        Strategy["CsjnEnrichmentStrategy\nSolo campos faltantes"]
        LLM_J["GPT-4o-mini\njudges extraction\n(si no hay API votes)"]
        LLM_S["GPT-4o-mini\ncited_statutes\n(si no hay API normas)"]
        LLM_C["GPT-4o-mini\ncitation_types\nclasificación por contexto"]
        LLM_P["GPT-4o-mini\nprosecutor_opinion\n(si HasDictamen)"]
        Merge["Merge: API data +\nLLM data → RulingData"]
        Chunk["Chunking\n512 tokens, overlap 50"]
        IM["IndexerMessage\nRulingData + PersonData[] +\nKeywordData[] + StatuteData[] +\nCitationData[] + ChunkData[] +\nVoteData[] + SumarioData[]"]

        EMIn --> Strategy
        Strategy -->|"si falta"| LLM_J
        Strategy -->|"si falta"| LLM_S
        Strategy -->|"siempre CSJN"| LLM_C
        Strategy -->|"si dictamen"| LLM_P
        LLM_J --> Merge
        LLM_S --> Merge
        LLM_C --> Merge
        LLM_P --> Merge
        Strategy -->|"API data"| Merge
        Merge --> Chunk
        Chunk --> IM
    end

    subgraph stage3 [Indexer — Persistencia y Grafos]
        IMIn["IndexerMessage"]
        Persist["PersistRulingStep\nUpsert en Azure SQL"]
        Embed["GenerateEmbeddingsStep\ntext-embedding-3-large"]
        Index["IndexSearchStep\nAzure AI Search"]
        Resolve["ResolveCitationsStep\nResolver TargetRulingId"]

        IMIn --> Persist
        Persist --> Embed
        Embed --> Index
        Index --> Resolve
    end

    CrawlerOut --> PM
    EM --> EMIn
    IM --> IMIn

    subgraph kbStores [Knowledge Base]
        SQLDb["Azure SQL\nRuling, Person, Court,\nVote, Sumario, Citation,\nKeyword, Statute, ...\n+ Grafo relacional"]
        AISearch["Azure AI Search\nrulings-by-ruling\nrulings-by-chunk"]
    end

    Persist --> SQLDb
    Index --> AISearch
    Resolve --> SQLDb
```

---

## 5. Mapeo detallado: Campo de fuente → DTO → Entidad KB

### 5.1 Ruling (Sentencia)

```mermaid
flowchart LR
    subgraph raw [Fuente CSJN]
        r1["caratula"]
        r2["identificacionExpediente\n+ claveRecurso"]
        r3["falloDestacado.fecha\n+ fechaFallo sumarios"]
        r4["competencia.valor"]
        r5["tipoRecurso.valor"]
        r6["sentidoPronunciamiento"]
        r7["materiaSecretaria"]
        r8["tipoAccion.valor"]
        r9["inconstitucional"]
        r10["falloDestacado.resumen\n/ cabecilla / titulo"]
        r11["sumarios[].texto"]
        r12["PDF via getAllDocumentos"]
        r13["tomo + pagina"]
    end

    subgraph dto [ExtractedMetadata / RulingData]
        d1["CaseTitle"]
        d2["CaseNumber"]
        d3["RulingDate"]
        d4["Jurisdiction +\nJurisdictionArea"]
        d5["ResourceType"]
        d6["RulingDirection"]
        d7["SubjectArea"]
        d8["ActionType"]
        d9["IsUnconstitutional"]
        d10["Summary"]
        d11["Holding"]
        d12["NormalizedText\n→ FullText"]
        d13["OfficialReference"]
    end

    subgraph entity [Ruling Entity]
        e1["Ruling.CaseTitle"]
        e2["Ruling.CaseNumber"]
        e3["Ruling.RulingDate"]
        e4["Ruling.JurisdictionArea"]
        e5["Ruling.ResourceType"]
        e6["Ruling.RulingDirection"]
        e7["Ruling.SubjectArea"]
        e8["Ruling.ActionType"]
        e9["Ruling.IsUnconstitutional"]
        e10["Ruling.Summary"]
        e11["Ruling.Holding"]
        e12["Ruling.FullText"]
        e13["Ruling.OfficialReference"]
    end

    r1 --> d1 --> e1
    r2 --> d2 --> e2
    r3 --> d3 --> e3
    r4 --> d4 --> e4
    r5 --> d5 --> e5
    r6 --> d6 --> e6
    r7 --> d7 --> e7
    r8 --> d8 --> e8
    r9 --> d9 --> e9
    r10 --> d10 --> e10
    r11 --> d11 --> e11
    r12 --> d12 --> e12
    r13 --> d13 --> e13
```

### 5.2 Personas y roles (SujetoDeDerecho)

```mermaid
flowchart LR
    subgraph raw [Fuente CSJN]
        v1["votosAnalisisDocumental[]\nministroId, nombre,\ntipoVoto"]
        v2["stringMayoria /\nstringDisidencia"]
        v3["dictamen fiscal\n(getDictamenesAnalisis)"]
        v4["LLM: judges extraction\n(fallback si no hay API votes)"]
    end

    subgraph dto [Pipeline DTOs]
        d1["CsjnVoteDto\n→ ApiPersons[]"]
        d2["ExtractedJudgeDto\n(LLM fallback)"]
        d3["ProsecutorOpinionData"]
        d4["VoteData\nVoteType, Pages"]
        d5["PersonData\nDisplayName, PersonType,\nCsjnMinistroId"]
    end

    subgraph entity [KB Entities]
        Person["Person\nDisplayName, PersonType,\nCsjnMinistroId, IsVerified"]
        RP["RulingParticipation\nPersonId, RulingId,\nVoteId, ParticipationType"]
        Vote["Vote\nRulingId, VoteType,\nSummary"]
        PO["ProsecutorOpinion\nRulingId, AuthorName,\nPosition"]
    end

    v1 -->|"API"| d1
    v2 -->|"API"| d1
    v4 -->|"LLM"| d2
    v3 -->|"API + LLM"| d3

    d1 --> d4
    d1 --> d5
    d2 --> d5

    d5 --> Person
    d4 --> Vote
    d5 --> RP
    d3 --> PO
```

### 5.3 Normas y citas (NormaJuridica + Citas)

```mermaid
flowchart LR
    subgraph raw [Fuente CSJN]
        n1["referenciasNormativas[]\nnombre, tipo, articulos"]
        n2["LLM: cited_statutes\n(fallback si API vacío)"]
        c1["getCitas[]\nalias, idFallo,\ntextoCita, idSumario"]
        c2["getCitantes HTML\nidAnalisis citantes"]
        c3["LLM: citation_types\nclasificación contextual"]
    end

    subgraph dto [Pipeline DTOs]
        sd["StatuteData\nNumber, Name, NormType,\nArticles[]"]
        cd["CitationData\nExternalAlias, CsjnFalloId,\nCitationText, CitationType"]
        cbd["CitedByData\nAnalysisId, LinkText"]
    end

    subgraph entity [KB Entities]
        Statute["Statute\nNumber, Name, NormType,\nNormativeLevel, LegalBranch"]
        RS["RulingStatute\nRulingId, StatuteId,\nArticles"]
        Citation["Citation\nExternalAlias, CitationType,\nTargetRulingId, CsjnFalloId"]
    end

    n1 -->|"API"| sd
    n2 -->|"LLM"| sd
    c1 -->|"API"| cd
    c2 -->|"API parse"| cbd
    c3 -->|"LLM"| cd

    sd --> Statute
    sd --> RS
    cd --> Citation
    cbd -->|"resolución\nretrospectiva"| Citation
```

---

## 6. Origen de cada campo: API vs LLM vs Computado

```mermaid
pie title Origen de campos KB (CSJN Phase 1)
    "API directa" : 42
    "LLM gap-fill" : 4
    "PDF extracción" : 2
    "Computado" : 8
    "Constante" : 3
```

| Categoría | Campos / artefactos |
|-----------|-------------------|
| **API directa** | CaseTitle, CaseNumber, RulingDate, Jurisdiction, ResourceType, RulingDirection, SubjectArea, ActionType, IsUnconstitutional, Summary, Holding, Keywords, Citations (alias, ids, texto), CitedBy, OfficialReference, Votes, Statutes (API), Sumarios, Syntheses, Links, Dictamen, Observations, FederalQuestion, ProceduralFormula, HasDictamen |
| **LLM gap-fill** | `judges` (si no hay API votes), `cited_statutes` (si no hay referenciasNormativas), `citation_types` (siempre para CSJN), `prosecutor_opinion` (si HasDictamen) |
| **PDF extracción** | FullText (PDF → PdfPig → normalización), TextBlobPath |
| **Computado** | Chunks (512 tokens), Embeddings (text-embedding-3-large), TargetRulingId (resolución de citas), ContentHash (SHA-256), Ruling.Id (DB), CourtId (resolución por nombre) |
| **Constante** | Court = "Corte Suprema de Justicia de la Nación", Instance = "CSJN", SourceId = 1 |

---

## 7. Múltiples fuentes → Estrategias diferenciadas

Cada fuente tiene diferente riqueza de datos estructurados, lo que determina cuánto depende del LLM.

```mermaid
flowchart TB
    subgraph strategy [Estrategia por Fuente]
        direction LR
        CSJN_S["CSJN\n8 endpoints REST\nAlta estructura\nLLM: gap-fill mínimo"]
        SAIJ_JS["SAIJ Jurisprudencia\nHTML + PDF\nEstructura media\nLLM: enrichment parcial"]
        SAIJ_LS["SAIJ Legislación\nAPI REST\nAlta estructura\nLLM: mínimo"]
        PJN_S["PJN / SCBA\nHTML + PDF\nBaja estructura\nLLM: full enrichment"]
    end

    subgraph coverage [Cobertura Ontológica por Fuente]
        direction TB
        CSJN_C["CSJN → Sentencia, Sujeto,\nTribunal, Norma, Proceso"]
        SAIJ_JC["SAIJ Juris → Sentencia,\nSujeto, Tribunal"]
        SAIJ_LC["SAIJ Leg → NormaJuridica\n(texto completo, vigencia)"]
        PJN_C["PJN/SCBA → Sentencia,\nSujeto, Tribunal"]
    end

    CSJN_S --> CSJN_C
    SAIJ_JS --> SAIJ_JC
    SAIJ_LS --> SAIJ_LC
    PJN_S --> PJN_C

    subgraph enrichment [Intensidad LLM]
        Low["Baja\n1-2 calls/doc"]
        Med["Media\n3-4 calls/doc"]
        High["Alta\n4-5 calls/doc"]
    end

    CSJN_S -.-> Low
    SAIJ_JS -.-> Med
    SAIJ_LS -.-> Low
    PJN_S -.-> High
```

---

## 8. Grafo de conocimiento emergente

Las entidades de la KB y sus relaciones forman un grafo de conocimiento implícito en Azure SQL, navegable via recursive CTEs.

```mermaid
erDiagram
    Ruling ||--o{ RulingParticipation : "firmada por"
    Ruling ||--o{ Vote : "contiene"
    Ruling ||--o{ Sumario : "documenta"
    Ruling ||--o{ RulingKeyword : "clasificada por"
    Ruling ||--o{ RulingStatute : "aplica"
    Ruling ||--o{ Citation : "cita a"
    Ruling }o--|| Court : "emitida por"
    Ruling }o--o| JudicialProceeding : "parte de"

    Person ||--o{ RulingParticipation : "participa en"
    Person ||--o{ JudicialOffice : "ocupa cargo"

    Vote ||--o{ RulingParticipation : "incluye a"

    Court ||--o{ JudicialOffice : "tiene cargos"

    Statute ||--o{ RulingStatute : "aplicada en"
    Statute ||--o{ NormRelation : "relacionada con"

    Keyword ||--o{ RulingKeyword : "usada en"
    Keyword ||--o{ SumarioKeyword : "describe"

    Citation }o--o| Ruling : "resuelve a"

    JudicialProceeding ||--o{ ProceedingParty : "tiene partes"
    ProceedingParty }o--|| Person : "es"
```
