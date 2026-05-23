# Legal AI AR

**Development Specifications**
v1.0 — March 2026 · *Phase 1 — MVP*
Table of contents

- [1. Introduction](#1-introduction)
- [2. Repository structure](#2-repository-structure)
- [3. Clean Architecture layers](#3-clean-architecture-layers)
- [4. CQRS with MediatR](#4-cqrs-with-mediatr)
- [5. Data model](#5-data-model)
- [6. Ingestion pipeline](#6-ingestion-pipeline)
- [7. API — ASP.NET Core](#7-api--aspnet-core)
- [8. Frontend — Angular SPA](#8-frontend--angular-spa)
- [9. Configuration and infrastructure](#9-configuration-and-infrastructure)
- [10. Testing](#10-testing)
- [11. Architecture decisions](#11-architecture-decisions)
- [12. Open technical risks](#12-open-technical-risks)

## 1. Introduction
This document describes the complete technical specifications for the
development of Legal AI AR: an internal legal AI platform that
automates the ingestion, indexing and querying of Argentine judicial
rulings.

### 1.1 Scope
This document covers the complete implementation of Phase 1 (MVP),
including:

-   Ingestion pipeline for CSJN

-   Hybrid Knowledge Base (Azure SQL, Azure AI Search, Azure Blob
    Storage, Neo4j)

-   REST API with CQRS/MediatR

-   Angular SPA with integrated admin panel

-   4 workers as Azure Container Apps

### 1.2 Development principles

  **Principle**          **Application**

  **Clean Architecture** Separation in layers: Domain → Application →
                         Infrastructure → Presentation

  **CQRS with MediatR**   Commands and Queries separated. Handlers in the
                         Application layer

  **Code in English**    All identifiers, classes and schemas in
                         English. UI in Spanish

  **Monorepo**           One .NET solution with shared projects. One
                         Angular repo

  **Immutability**       Indexed rulings are not modified.
                         Deduplication by SHA-256

  **Idempotency**        Each worker can reprocess the same message
                         without side effects

## 2. Repository structure
The project is organized as a monorepo with two roots: /backend for
the .NET solution and /frontend for the Angular project.

### 2.1 Repository root

**legal-ai-ar/ ← repository root**
```
├── backend/ ← .NET solution
├── frontend/ ← Angular project
├── infra/ ← Deploy scripts (optional; existing Azure services)
├── README.md ← general description (root)
├── docs/ ← documentation
│   ├── architecture/ ← architecture and specs
│   ├── roadmap/ ← development roadmap
│   ├── prompts/ ← agent prompts (Cursor skills in .cursor/skills/)
│   └── design/ ← roadmap design deliverables
├── .gitignore
├── .editorconfig
└── docker-compose.yml ← Neo4j + SQL Server local
```

### 2.2 Backend — .NET solution

📌 *Naming convention: LegalAiAr.{Component}.{Layer}*
```
backend/
├── LegalAiAr.sln
│
├── src/
│ ├── shared/ ← shared libraries
│ │ ├── LegalAiAr.Core/ ← domain and contracts
│ │ │ ├── LegalAiAr.Core.csproj
│ │ │ ├── Entities/ ← domain entities
│ │ │ │ ├── Ruling.cs
│ │ │ │ ├── Court.cs
│ │ │ │ ├── Judge.cs
│ │ │ │ ├── Keyword.cs
│ │ │ │ ├── Statute.cs
│ │ │ │ ├── Citation.cs
│ │ │ │ ├── Source.cs
│ │ │ │ └── CrawlerConfig.cs
│ │ │ ├── Enums/
│ │ │ │ ├── CitationType.cs ← UPHOLDS\|OVERRULES\|DISTINGUISHES\|CITES
│ │ │ │ ├── ParticipationType.cs ← SIGNATORY\|DISSENT\|MAJORITY
│ │ │ │ ├── RulingStatus.cs ← indexed\|error\|pending
│ │ │ │ └── IngestionType.cs ← incremental\|by-range
│ │ │ ├── Interfaces/
│ │ │ │ ├── Repositories/
│ │ │ │ │ ├── IRulingRepository.cs
│ │ │ │ │ ├── ICourtRepository.cs
│ │ │ │ │ ├── IJudgeRepository.cs
│ │ │ │ │ ├── IKeywordRepository.cs
│ │ │ │ │ ├── IStatuteRepository.cs
│ │ │ │ │ ├── ICitationRepository.cs
│ │ │ │ │ └── ICrawlerConfigRepository.cs
│ │ │ │ ├── Services/
│ │ │ │ │ ├── IEmbeddingService.cs
│ │ │ │ │ ├── ISearchService.cs
│ │ │ │ │ ├── IBlobStorageService.cs
│ │ │ │ │ ├── IGraphService.cs
│ │ │ │ │ └── IQueuePublisher.cs
│ │ │ │ └── Pipeline/
│ │ │ │ ├── IDocumentParser.cs
│ │ │ │ └── ITextNormalizer.cs
│ │ │ ├── Messages/ ← Service Bus message contracts
│ │ │ │ ├── CrawlerMessage.cs
│ │ │ │ ├── ParserMessage.cs
│ │ │ │ ├── EnrichmentMessage.cs
│ │ │ │ └── IndexerMessage.cs
│ │ │ └── Exceptions/
│ │ │ ├── DomainException.cs
│ │ │ └── DuplicateRulingException.cs
│ │ │
│ │ └── LegalAiAr.Infrastructure/ ← shared implementations
│ │ ├── LegalAiAr.Infrastructure.csproj
│ │ ├── Persistence/
│ │ │ ├── AppDbContext.cs ← EF Core DbContext
│ │ │ ├── Configurations/ ← IEntityTypeConfiguration\<T\>
│ │ │ │ ├── RulingConfiguration.cs
│ │ │ │ ├── CourtConfiguration.cs
│ │ │ │ ├── JudgeConfiguration.cs
│ │ │ │ ├── KeywordConfiguration.cs
│ │ │ │ ├── StatuteConfiguration.cs
│ │ │ │ ├── CitationConfiguration.cs
│ │ │ │ └── CrawlerConfigConfiguration.cs
│ │ │ ├── Repositories/
│ │ │ │ ├── RulingRepository.cs
│ │ │ │ ├── CourtRepository.cs
│ │ │ │ ├── JudgeRepository.cs
│ │ │ │ ├── KeywordRepository.cs
│ │ │ │ ├── StatuteRepository.cs
│ │ │ │ ├── CitationRepository.cs
│ │ │ │ └── CrawlerConfigRepository.cs
│ │ │ └── Migrations/ ← EF Core migrations
│ │ ├── Search/
│ │ │ ├── AzureSearchService.cs
│ │ │ └── Models/
│ │ │ ├── RulingSearchDocument.cs
│ │ │ └── ChunkSearchDocument.cs
│ │ ├── Blob/
│ │ │ └── AzureBlobStorageService.cs
│ │ ├── Graph/
│ │ │ └── Neo4jGraphService.cs
│ │ ├── Messaging/
│ │ │ └── ServiceBusQueuePublisher.cs
│ │ ├── Ai/
│ │ │ ├── AzureOpenAiEmbeddingService.cs
│ │ │ └── AzureOpenAiEnrichmentService.cs
│ │ └── Extensions/
│ │ └── InfrastructureServiceExtensions.cs
│ ├── api/ ← ASP.NET Core Web API
│ │ ├── LegalAiAr.Api/ ← presentation layer
│ │ │ ├── LegalAiAr.Api.csproj
│ │ │ ├── Program.cs
│ │ │ ├── appsettings.json
│ │ │ ├── appsettings.Development.json
│ │ │ ├── Controllers/
│ │ │ │ ├── RulingsController.cs ← /api/rulings/\*
│ │ │ │ ├── ChatController.cs ← /api/chat (SSE)
│ │ │ │ ├── Admin/
│ │ │ │ │ ├── CrawlersAdminController.cs
│ │ │ │ │ ├── JobsAdminController.cs
│ │ │ │ │ ├── DlqAdminController.cs
│ │ │ │ │ └── UsersAdminController.cs
│ │ │ │ └── HealthController.cs
│ │ │ ├── Middleware/
│ │ │ │ ├── ExceptionHandlingMiddleware.cs
│ │ │ │ └── RequestLoggingMiddleware.cs
│ │ │ └── Extensions/
│ │ │ ├── ServiceCollectionExtensions.cs
│ │ │ └── WebApplicationExtensions.cs
│ │ │
│ │ └── LegalAiAr.Application/ ← application layer (CQRS)
│ │ ├── LegalAiAr.Application.csproj
│ │ ├── Rulings/
│ │ │ ├── Queries/
│ │ │ │ ├── SearchRulings/
│ │ │ │ │ ├── SearchRulingsQuery.cs
│ │ │ │ │ ├── SearchRulingsHandler.cs
│ │ │ │ │ └── SearchRulingsResult.cs
│ │ │ │ ├── GetRulingById/
│ │ │ │ │ ├── GetRulingByIdQuery.cs
│ │ │ │ │ ├── GetRulingByIdHandler.cs
│ │ │ │ │ └── GetRulingByIdResult.cs
│ │ │ │ └── GetRelatedRulings/
│ │ │ │ ├── GetRelatedRulingsQuery.cs
│ │ │ │ ├── GetRelatedRulingsHandler.cs
│ │ │ │ └── GetRelatedRulingsResult.cs
│ │ │ └── DTOs/
│ │ │ ├── RulingDto.cs
│ │ │ ├── RulingSearchResultDto.cs
│ │ │ └── RelatedRulingDto.cs
│ │ ├── Chat/
│ │ │ ├── Commands/
│ │ │ │ ├── ChatQuery/
│ │ │ │ │ ├── ChatQueryCommand.cs
│ │ │ │ │ └── ChatQueryHandler.cs ← RAG pipeline
│ │ │ │ └── DTOs/
│ │ │ │ └── ChatResponseDto.cs
│ │ ├── Admin/
│ │ │ ├── Crawlers/
│ │ │ │ ├── Commands/
│ │ │ │ │ ├── RunCrawler/
│ │ │ │ │ │ ├── RunCrawlerCommand.cs
│ │ │ │ │ │ └── RunCrawlerHandler.cs
│ │ │ │ │ └── UpdateCrawlerConfig/
│ │ │ │ │ ├── UpdateCrawlerConfigCommand.cs
│ │ │ │ │ └── UpdateCrawlerConfigHandler.cs
│ │ │ │ └── Queries/
│ │ │ │ ├── GetCrawlers/
│ │ │ │ │ ├── GetCrawlersQuery.cs
│ │ │ │ │ └── GetCrawlersHandler.cs
│ │ │ │ └── DTOs/
│ │ │ │ └── CrawlerConfigDto.cs
│ │ │ ├── Jobs/
│ │ │ │ ├── Queries/
│ │ │ │ │ ├── GetPipelineStatus/
│ │ │ │ │ │ ├── GetPipelineStatusQuery.cs
│ │ │ │ │ │ └── GetPipelineStatusHandler.cs
│ │ │ │ │ └── GetJobs/
│ │ │ │ │   ├── GetJobsQuery.cs
│ │ │ │ │   └── GetJobsHandler.cs
│ │ │ ├── Dlq/
│ │ │ │ ├── Queries/
│ │ │ │ │ └── GetDlqMessages/
│ │ │ │ │ ├── GetDlqMessagesQuery.cs
│ │ │ │ │ └── GetDlqMessagesHandler.cs
│ │ │ │ └── Commands/
│ │ │ │ └── RequeueMessage/
│ │ │ │ ├── RequeueMessageCommand.cs
│ │ │ │ └── RequeueMessageHandler.cs
│ │ │ └── Users/
│ │ │ ├── Commands/
│ │ │ │ ├── CreateUser/
│ │ │ │ ├── UpdateUser/
│ │ │ │ └── DeleteUser/
│ │ │ └── Queries/
│ │ │ └── GetUsers/
│ │ ├── Common/
│ │ │ ├── Behaviors/
│ │ │ │ ├── ValidationBehavior.cs ← FluentValidation pipeline
│ │ │ │ └── LoggingBehavior.cs
│ │ │ └── Mappings/
│ │ │ └── RulingMappingProfile.cs ← AutoMapper
│ │ └── Extensions/
│ │ └── ApplicationServiceExtensions.cs
│ └── workers/
│ ├── LegalAiAr.Worker.Crawler/
│ │ ├── LegalAiAr.Worker.Crawler.csproj
│ │ ├── Program.cs
│ │ ├── appsettings.json
│ │ ├── Dockerfile
│ │ ├── CrawlerWorkerService.cs ← IHostedService principal
│ │ ├── Sources/
│ │ │ ├── ICrawlerSource.cs ← interface per source
│ │ │ ├── CsjnCrawlerSource.cs ← CSJN (Selenium discovery; see 6.0)
│ │ │ └── SaijCrawlerSource.cs ← SAIJ implementation
│ │ └── Extensions/
│ │ └── CrawlerServiceExtensions.cs
│ │
│ ├── LegalAiAr.Worker.Parser/
│ │ ├── LegalAiAr.Worker.Parser.csproj
│ │ ├── Program.cs
│ │ ├── appsettings.json
│ │ ├── Dockerfile
│ │ ├── ParserWorkerService.cs ← IHostedService + Service Bus consumer
│ │ ├── Parsers/
│ │ │ ├── CsjnApiParser.cs ← consumes CSJN REST endpoints
│ │ │ ├── HtmlParser.cs ← HTML scraping for generic sources
│ │ │ └── PdfTextExtractor.cs ← PdfPig + normalization
│ │ └── Extensions/
│ │ └── ParserServiceExtensions.cs
│ │
│ ├── LegalAiAr.Worker.Enrichment/
│ │ ├── LegalAiAr.Worker.Enrichment.csproj
│ │ ├── Program.cs
│ │ ├── appsettings.json
│ │ ├── Dockerfile
│ │ ├── EnrichmentWorkerService.cs
│ │ ├── Strategies/
│ │ │ ├── IEnrichmentStrategy.cs
│ │ │ ├── CsjnEnrichmentStrategy.cs ← gap-filling only (judges,
```
statutes)
```
│ │ │ └── FullEnrichmentStrategy.cs ← full enrichment for
```
SAIJ/PJN/SCBA
```
│ │ ├── Prompts/
│ │ │ ├── JudgesExtractionPrompt.cs
│ │ │ ├── StatutesExtractionPrompt.cs
│ │ │ └── FullEnrichmentPrompt.cs
│ │ └── Extensions/
│ │ └── EnrichmentServiceExtensions.cs
│ │
│ └── LegalAiAr.Worker.Indexer/
│ ├── LegalAiAr.Worker.Indexer.csproj
│ ├── Program.cs
│ ├── appsettings.json
│ ├── Dockerfile
│ ├── IndexerWorkerService.cs
│ ├── Steps/ ← indexing pipeline by step
│ │ ├── PersistRulingStep.cs ← Azure SQL
│ │ ├── UploadBlobStep.cs ← Azure Blob Storage
│ │ ├── GenerateEmbeddingsStep.cs ← text-embedding-3-large
│ │ ├── IndexSearchStep.cs ← Azure AI Search
│ │ ├── IndexGraphStep.cs ← Neo4j
│ │ └── ResolveCitationsStep.cs ← retroactive citation resolution
│ └── Extensions/
│ └── IndexerServiceExtensions.cs
│
└── tests/
├── LegalAiAr.Core.Tests/
│ ├── LegalAiAr.Core.Tests.csproj
│ └── Entities/
│ ├── RulingTests.cs
│ └── CitationTests.cs
├── LegalAiAr.Application.Tests/
│ ├── LegalAiAr.Application.Tests.csproj
│ ├── Rulings/
│ │ ├── Queries/
│ │ │ └── SearchRulingsHandlerTests.cs
│ │ └── Commands/
│ └── Admin/
│     └── Crawlers/
│         └── Commands/
│             └── RunCrawlerHandlerTests.cs
├── LegalAiAr.Worker.Crawler.Tests/
│ ├── LegalAiAr.Worker.Crawler.Tests.csproj
│ └── Sources/
│ └── CsjnCrawlerSourceTests.cs
├── LegalAiAr.Worker.Parser.Tests/
│   ├── LegalAiAr.Worker.Parser.Tests.csproj
│   └── Parsers/
│       ├── PdfTextExtractorTests.cs
│       └── CsjnApiParserTests.cs
├── LegalAiAr.Worker.Enrichment.Tests/
│   ├── LegalAiAr.Worker.Enrichment.Tests.csproj
│   └── Strategies/
│       └── CsjnEnrichmentStrategyTests.cs
└── LegalAiAr.Worker.Indexer.Tests/
│   ├── LegalAiAr.Worker.Indexer.Tests.csproj
│   └── Steps/
│       └── ResolveCitationsStepTests.cs
```

### 2.3 Frontend — Angular SPA
```
frontend/
├── angular.json
├── package.json
├── tsconfig.json
├── tsconfig.app.json
├── .env.example
│
└── src/
├── main.ts
├── index.html
├── styles.scss ← global styles
│
└── app/
├── app.config.ts ← standalone app config, providers
├── app.routes.ts ← root routes
│
├── core/ ← singleton services, guards, interceptors
│ ├── auth/
│ │ ├── auth.guard.ts ← AuthGuard (authenticated vs anonymous)
│ │ └── msal.config.ts ← MSAL Angular configuration
│ ├── interceptors/
│ │ ├── auth.interceptor.ts ← adds Bearer token to requests
│ │ └── error.interceptor.ts ← global HTTP error handling
│ ├── services/
│ │ ├── ruling.service.ts ← /api/rulings/\*
│ │ ├── chat.service.ts ← /api/chat (SSE)
│ │ ├── crawler.service.ts ← /api/admin/crawlers/\*
│ │ ├── dlq.service.ts ← /api/admin/dlq/\*
│ │ ├── jobs.service.ts ← /api/admin/pipeline/status, /api/admin/jobs
│ │ └── user.service.ts ← /api/admin/users/\*
│ └── models/
│ ├── ruling.model.ts
│ ├── search-result.model.ts
│ ├── chat.model.ts
│ ├── crawler-config.model.ts
│ └── user.model.ts
│
├── features/
│ ├── search/
│ │ ├── search.routes.ts
│ │ ├── search-home/
│ │ │ ├── search-home.component.ts
│ │ │ ├── search-home.component.html
│ │ │ └── search-home.component.scss
│ │ └── search-results/
│ │ ├── search-results.component.ts
│ │ ├── search-results.component.html
│ │ └── search-results.component.scss
│ ├── rulings/
│ │ ├── rulings.routes.ts
│ │ └── ruling-detail/
│ │ ├── ruling-detail.component.ts
│ │ ├── ruling-detail.component.html
│ │ └── ruling-detail.component.scss
│ ├── chat/
│ │ ├── chat.routes.ts
│ │ └── chat-view/
│ │ ├── chat-view.component.ts
│ │ ├── chat-view.component.html
│ │ └── chat-view.component.scss
│ └── admin/
│ ├── admin.routes.ts
│ ├── dashboard/
│ │ ├── dashboard.component.ts
│ │ ├── dashboard.component.html
│ │ └── dashboard.component.scss
│ ├── crawlers/
│ │ ├── crawlers.component.ts
│ │ ├── crawlers.component.html
│ │ └── crawlers.component.scss
│ ├── jobs/
│ │ ├── jobs.component.ts
│ │ └── jobs.component.html
│ ├── dlq/ ← DeadLetterQueueComponent
│ │ ├── dlq.component.ts
│ │ └── dlq.component.html
│ └── users/
│ ├── users.component.ts
│ └── users.component.html
│
└── shared/
├── components/
│ ├── ruling-card/
│ │ ├── ruling-card.component.ts
│ │ ├── ruling-card.component.html
│ │ └── ruling-card.component.scss
│ ├── search-bar/
│ │ ├── search-bar.component.ts
│ │ └── search-bar.component.html
│ ├── citation-badge/
│ │ └── citation-badge.component.ts
│ └── loading-spinner/
│ └── loading-spinner.component.ts
└── pipes/
├── ruling-date.pipe.ts
└── citation-type-label.pipe.ts
```

## 3. Clean Architecture layers
The backend follows the Clean Architecture dependency model. Inner
layers do not know about outer layers.

  **Layer**            **Project**                **Responsibility**

  **Domain**           LegalAiAr.Core             Entities, enums, repository and
                                                  service interfaces,
                                                  Service Bus messages

  **Application**      LegalAiAr.Application      CQRS handlers, DTOs, validations
                                                  (FluentValidation), mappings
                                                  (AutoMapper)

  **Infrastructure**   LegalAiAr.Infrastructure   EF Core, Azure SQL, Azure AI
                                                  Search, Blob Storage, Neo4j,
                                                  Service Bus, Azure OpenAI

  **Presentation**     LegalAiAr.Api             Controllers, middleware,
                                                  service configuration,
                                                  Program.cs

### 3.1 Dependency flow
The dependency rule is applied strictly: outer layer projects reference
inner layer projects, never the reverse.

> LegalAiAr.Api → LegalAiAr.Application → LegalAiAr.Core
>
> LegalAiAr.Api → LegalAiAr.Infrastructure
>
> LegalAiAr.Infrastructure → LegalAiAr.Core
>
> LegalAiAr.Worker.\* → LegalAiAr.Infrastructure → LegalAiAr.Core

📌 *Workers share LegalAiAr.Core (entities, interfaces,
messages) and LegalAiAr.Infrastructure (repositories, external services).
Each worker has its own internal business logic.*

## 4. CQRS with MediatR

### 4.1 Command structure
Each write operation is implemented as a Command + Handler.
Example: triggering a manual crawl.

> // RunCrawlerCommand.cs
>
> public record RunCrawlerCommand(int SourceId, string Type, DateOnly?
> Since, DateOnly? DateFrom, DateOnly? DateTo) : IRequest\<RunCrawlerResult\>;
>
> // RunCrawlerHandler.cs
>
> public class RunCrawlerHandler : IRequestHandler\<RunCrawlerCommand,
> RunCrawlerResult\>
>
> {
>
> private readonly IQueuePublisher \_publisher;
>
> private readonly ICrawlerConfigRepository \_configs;
>
> public RunCrawlerHandler(IQueuePublisher publisher,
> ICrawlerConfigRepository configs) { \... }
>
> public async Task\<RunCrawlerResult\> Handle(RunCrawlerCommand cmd,
> CancellationToken ct)
>
> {
>
> var config = await \_configs.GetBySourceIdAsync(cmd.SourceId, ct);
>
> if (config is null \|\| !config.IsEnabled) throw new
> DomainException(\"Source not enabled\");
>
> await \_publisher.PublishAsync(\"queue-crawler\", new CrawlerMessage {
> \... }, ct);
>
> return new RunCrawlerResult(Success: true);
>
> }
>
> }

### 4.2 Query structure
Queries do not modify state. Example: semantic search of rulings.

> // SearchRulingsQuery.cs
>
> public record SearchRulingsQuery(
>
> string Query,
>
> SearchFilters? Filters,
>
> int Page = 1,
>
> int PageSize = 10
>
> ) : IRequest\<SearchRulingsResult\>;
>
> // SearchRulingsHandler.cs
>
> public class SearchRulingsHandler :
> IRequestHandler\<SearchRulingsQuery, SearchRulingsResult\>
>
> {
>
> private readonly ISearchService \_search;
>
> private readonly IEmbeddingService \_embeddings;
>
> public async Task\<SearchRulingsResult\> Handle(SearchRulingsQuery q,
> CancellationToken ct)
>
> {
>
> var embedding = await \_embeddings.GenerateAsync(q.Query, ct);
>
> var results = await \_search.SearchAsync(embedding, q.Filters, q.Page,
> q.PageSize, ct);
>
> return new SearchRulingsResult(results);
>
> }
>
> }

### 4.3 Pipeline behaviors

  **Behavior**                      **Function**

  **ValidationBehavior\<TRequest,   Runs FluentValidation before each
  TResponse\>**                     handler. Throws ValidationException if
                                    it fails

  **LoggingBehavior\<TRequest,     Logs input and output of each
  TResponse\>**                     command/query with duration

## 5. Data model

### 5.1 Rulings (Azure SQL)

`Ruling` entity mapped to `Rulings` table in Azure SQL.

**Rulings**

  **Field**          **Type**              **Notes**

  Id                 UNIQUEIDENTIFIER PK   UUID generated when indexing

  SourceId           INT FK → Sources      CSJN=1, SAIJ=2, PJN=3, SCBA=4

  ExternalId         VARCHAR(50)           ID in source (e.g.: 8048522)

  AnalysisId         VARCHAR(50)           CSJN only. Null for other
                                           sources

  ContentHash        CHAR(64)              SHA-256 of PDF. Deduplication
                                           key

  CaseTitle          NVARCHAR(500)         Official case title

  CaseNumber         VARCHAR(100)          E.g.: CAF 9548/2021/CA1-CS1

  RulingDate         DATE                  Date of the ruling

  CourtId            INT FK → Courts       

  JurisdictionArea   VARCHAR(100)          E.g.: CONTENCIOSO ADMINISTRATIVO
                                           FEDERAL

  Instance           VARCHAR(50)           E.g.: CSJN, Cámara, Primera Instancia

  Jurisdiction       VARCHAR(100)          E.g.: APELACION EXTRAORDINARIA. Null
                                           for non-CSJN sources

  ResourceType       VARCHAR(100)          E.g.: RECURSO EXTRAORDINARIO FEDERAL
                                           (CSJN)

  RulingDirection    VARCHAR(50)           UPHOLDS \| OVERRULES \| GRANTS

  SubjectArea        VARCHAR(100)          E.g.: Tributario - Bancario

  IsUnconstitutional BIT                   Declares unconstitutionality

  Summary            NVARCHAR(MAX)         Ruling summary

  Holding            NVARCHAR(MAX)         Main holding

  FullText           NVARCHAR(MAX)         Extracted and normalized PDF text

  BlobPath           VARCHAR(500)          Path in Azure Blob Storage

  IndexedAt          DATETIME              Indexing timestamp

  Status             VARCHAR(20)           indexed \| error \| pending

### 5.2 Sources (Azure SQL)

Catalog of judicial sources. Referenced as FK from `Rulings`, `CrawlerConfigs` and `IngestionJobs`. Initial seed is defined in F0-2.

| Field | Type | Description |
|---|---|---|
| Id | INT PK | CSJN=1, SAIJ=2, PJN=3, SCBA=4 |
| Name | VARCHAR(50) | Short name (e.g.: CSJN) |
| FullName | NVARCHAR(200) | Full name |
| BaseUrl | VARCHAR(500) | Base URL of the source |
| Strategy | VARCHAR(20) | api-first, html-pdf |
| IsActive | BIT | Whether the source is active |

### 5.3 Citations (Azure SQL)

  **Field**         **Type**              **Notes**

  Id               INT PK                

  SourceRulingId   UNIQUEIDENTIFIER FK   Ruling that cites

  TargetRulingId   UNIQUEIDENTIFIER FK   Null if the cited ruling is not yet
                   nullable              indexed

  ExternalAlias    VARCHAR(100)          E.g.: Fallos: 328:1883

  CsjnSummaryId    INT nullable          For future resolution

  CitationType     VARCHAR(50)           UPHOLDS \| OVERRULES \|
                                         DISTINGUISHES \| CITES

### 5.4 Azure AI Search indexes

  **Index**              **Key fields**

  **rulings-by-ruling**   id, rulingId, caseTitle (searchable), summary
                          (searchable), holding (searchable), rulingDate
                          (filterable), jurisdictionArea (facetable), instance
                          (facetable), court (filterable, facetable),
                          rulingDirection (filterable), keywords (facetable),
                          embedding (vector 3072 dims)

  **rulings-by-chunk**    id, rulingId (filterable), chunkIndex, text
                          (searchable), embedding (vector 3072 dims)

### 5.5 Neo4j graph

> // Nodes
>
> (:Ruling {id, caseTitle, rulingDate, jurisdictionArea, instance,
> rulingDirection})
>
> (:Judge {id, firstName, lastName})
>
> (:Court {id, name, jurisdictionArea, territory})
>
> (:Keyword {id, description})
>
> (:Statute {number, name})
>
> // Relationships
>
> (:Ruling)-\[:CITES {citationType}\]-\>(:Ruling)
>
> (:Ruling)-\[:SIGNED_BY {participationType}\]-\>(:Judge)
>
> (:Ruling)-\[:ISSUED_BY\]-\>(:Court)
>
> (:Ruling)-\[:HAS_KEYWORD\]-\>(:Keyword)
>
> (:Ruling)-\[:CITES_STATUTE\]-\>(:Statute)
>
> (:Judge)-\[:MEMBER_OF\]-\>(:Court)

### 5.6 CrawlerConfigs (Azure SQL)

  **Field**             **Type**              **Description**

  Id                    INT PK                

  SourceId              INT FK                 FK to source (CSJN=1, SAIJ=2, PJN=3, SCBA=4)

  IsEnabled             BIT                   Whether the source is active for crawling

  CronExpression        VARCHAR(100)          **Phase 2** — cron expression for automatic schedule. Null in Phase 1.

  LastCrawledAt         DATETIME nullable     Timestamp of last completed crawl

  LastCrawledStatus     VARCHAR(20)           success, partial, failed. Null if never executed.

  LastDocumentCount     INT nullable          Number of documents processed in last crawl

  CreatedAt             DATETIME              

  UpdatedAt             DATETIME

### 5.7 IngestionJobs (Azure SQL, Phase 2)

The IngestionJobs table and Rulings.IngestionJobId column are defined in the architecture (section 4.15). The complete schema and migrations are implemented in F2-5.

| Field | Type | Description |
|---|---|---|
| Id | UNIQUEIDENTIFIER PK | UUID generated when job starts |
| SourceId | INT FK | Crawled source |
| Type | VARCHAR(20) | incremental, by-range |
| TriggeredBy | VARCHAR(100) | Admin user who triggered it, or scheduler |
| StartedAt | DATETIME | Start timestamp |
| CompletedAt | DATETIME nullable | Completion timestamp. Null if still in progress. |
| Status | VARCHAR(20) | running, completed, partial, failed |
| DocumentsDiscovered | INT | New documents detected by CrawlerWorker |
| DocumentsIndexed | INT | Documents actually persisted in the KB |
| DocumentsFailed | INT | Documents that failed at some pipeline stage |
| ErrorSummary | NVARCHAR(MAX) nullable | Error summary if Status is partial or failed |

*Source of truth: architecture (section 4.15).*

## 6. Ingestion pipeline

### 6.0 CSJN discovery (CrawlerWorker)

CSJN discovery requires **Selenium** (headless Chrome/Chromium). Pure HTTP search does not work with the current `sjconsulta.csjn.gov.ar` portal. The CrawlerWorker:

1. Navigates to `fallos/consulta.html`
2. Sets `fechaDesde` and `fechaHasta` (dd/MM/yyyy) in the search form
3. Clicks "Buscar"
4. Paginates via `fallos/paginarFallos.html?jtStartIndex={page*10}` (page size: 10)
5. Parses response (XML or JSON) — each Record yields `idAnalisis` and `Codigo` (document ID)

For incremental crawls, use `LastCrawledAt` as `fechaDesde`. The CrawlerWorker project must include Selenium.WebDriver (or equivalent). See `docs/architecture/legal-ai-ar-architecture.md` section 3.2.1.

### 6.1 Service Bus messages
CrawlerMessage (queue-crawler)

> public record CrawlerMessage
>
> {
>
> public int SourceId { get; init; }
>
> public string Type { get; init; } // \"incremental\" \| \"by-range\"
>
> public DateOnly? Since { get; init; } // only for incremental
>
> public DateOnly? DateFrom { get; init; } // only for by-range
>
> public DateOnly? DateTo { get; init; } // only for by-range
>
> }
ParserMessage (queue-parser)

The CrawlerWorker downloads the PDF, uploads it to Blob Storage and publishes the path in `BlobPathPdf`. The ParserWorker reads the PDF from Blob using that path.

> public record ParserMessage
>
> {
>
> public int SourceId { get; init; }
>
> public string DocumentId { get; init; }
>
> public string? AnalysisId { get; init; }
>
> public string BlobPathPdf { get; init; }
>
> public string ContentHash { get; init; }
>
> public CsjnApiMetadata? ApiMetadata { get; init; } // null for
> non-CSJN
>
> }
EnrichmentMessage (queue-enrichment)

> public record EnrichmentMessage
>
> {
>
> public string DocumentId { get; init; }
>
> public int SourceId { get; init; }
>
> public string NormalizedText { get; init; }
>
> public ExtractedMetadata ExtractedMetadata { get; init; }
>
> public string\[\] MissingFields { get; init; } //
> ["judges","cited_statutes","citation_types"]
>
> }
IndexerMessage (queue-indexer)

> public record IndexerMessage
>
> {
>
> public string DocumentId { get; init; }
>
> public RulingData Ruling { get; init; }
>
> public JudgeData\[\] Judges { get; init; }
>
> public KeywordData\[\] Keywords { get; init; }
>
> public StatuteData\[\] Statutes { get; init; }
>
> public CitationData\[\] Citations { get; init; }
>
> public ChunkData\[\] Chunks { get; init; }
>
> }

### 6.2 Retry and Dead Letter Queue

  **Configuration**       **Value**

  **Max delivery count** 3 attempts

  **Lock duration**      5 minutes

  **Message TTL**        7 days

  **After 3 failures**   Message → DLQ automatically

  **DLQ per queue**      queue-{name}/\$deadletterqueue

### 6.3 Idempotency
Before processing any document, the IndexerWorker checks if
ContentHash already exists in the Rulings table. If it exists, it discards the
message without error. This ensures that a message reprocessed from the
DLQ does not create duplicates.

## 7. API — ASP.NET Core

### 7.1 Endpoints
Rulings

  **Method + Route**         **Handler**                **Description**

  POST /api/rulings/search   SearchRulingsHandler       Hybrid semantic search
                                                         with filters

  GET /api/rulings/{id}      GetRulingByIdHandler       Full ruling details

  GET                        GetRelatedRulingsHandler   Related rulings by
  /api/rulings/{id}/related                             similarity

  POST /api/chat             ChatQueryHandler           RAG chat with SSE
                                                         streaming
Admin

  **Method + Route**                    **Handler**                  **Description**

  GET /api/admin/pipeline/status        GetPipelineStatusHandler     Pipeline status per source

  GET /api/admin/jobs                  GetJobsHandler               Active, completed and failed jobs

  *Note*: Phase 1: data is obtained from CrawlerConfigs (LastCrawledAt, LastCrawledStatus, LastDocumentCount per source) and/or queue metrics; the IngestionJobs table is introduced in F2-5.

  GET /api/admin/crawlers              GetCrawlersHandler            List with status of all
                                                                     crawlers

  GET /api/admin/crawlers/{sourceId}    GetCrawlersHandler            Status of a specific
                                                                     crawler

  POST                                 RunCrawlerHandler            Triggers manual crawl
  /api/admin/crawlers/{sourceId}/run                                 

  PATCH /api/admin/crawlers/{sourceId} UpdateCrawlerConfigHandler    Enable/disable source

  GET /api/admin/dlq                   GetDlqMessagesHandler        DLQ per queue (query param:
                                                                     queue)

  POST                                 RequeueMessageHandler        Requeues failed message
  /api/admin/dlq/{queue}/{id}/requeue                                

  GET /api/admin/users                 GetUsersHandler              User list

  POST /api/admin/users                CreateUserHandler            Create user

  PUT /api/admin/users/{id}            UpdateUserHandler            Update user

  DELETE /api/admin/users/{id}         DeleteUserHandler            Deactivate user

  GET /api/health                      (inline)                     Health check

  *Note*: GetCrawlersHandler receives optional `sourceId`: if null/absent returns the full list; if present returns the specific crawler.

### 7.2 Chat RAG — internal flow

  **Step**  **Component**             **Detail**

  1         ChatQueryHandler          Receives user query

  2         IEmbeddingService         Generates query embedding
                                      (text-embedding-3-large)

  3         ISearchService            Search rulings-by-chunk: top-K=10

  4         ISearchService            Search rulings-by-ruling: top-K=5

  5         ChatQueryHandler          Builds context with chunks + metadata

  6         AzureOpenAiEnrichmentService   Call to GPT-4o with legal RAG prompt

  7         ChatController            SSE stream of response to client

## 8. Frontend — Angular SPA

### 8.1 Routes

  **Route**                **Component**

  **/buscar**              SearchHomeComponent

  **/buscar/resultados**   SearchResultsComponent

  **/fallos/:id**          RulingDetailComponent

  **/chat**                ChatViewComponent

  **/admin**               DashboardComponent

  **/admin/crawlers**      CrawlersComponent

  **/admin/jobs**          JobsComponent

  **/admin/dlq**           DeadLetterQueueComponent

  **/admin/users**         UsersComponent

  **/login**               MsalRedirectComponent (MSAL built-in)

### 8.2 Authentication — MSAL Angular
All routes (including /admin/*) are protected by AuthGuard.
In Phase 1 there is no role distinction: any authenticated user
has access to everything.

> // app.routes.ts
>
> export const routes: Routes = \[
>
> { path: \"\", redirectTo: \"/buscar\", pathMatch: \"full\" },
>
> { path: \"buscar\", component: SearchHomeComponent, canActivate:
> \[AuthGuard\] },
>
> { path: \"buscar/resultados\", component: SearchResultsComponent,
> canActivate: \[AuthGuard\] },
>
> { path: \"fallos/:id\", component: RulingDetailComponent, canActivate:
> \[AuthGuard\] },
>
> { path: \"chat\", component: ChatViewComponent, canActivate:
> \[AuthGuard\] },
>
> { path: \"admin\", component: DashboardComponent, canActivate:
> \[AuthGuard\] },
>
> // ... rest of admin routes
>
> { path: \"login\", component: MsalRedirectComponent },
>
> \];

📌 *Phase 2: replace AuthGuard with a combination of AuthGuard +
RoleGuard. Introduce admin, lawyer and viewer roles from Entra
ID groups via JWT claims.*

### 8.3 HTTP services

  **Service**          **Main methods**

  **RulingService**    search(query, filters, page):
                       Observable\<SearchResult\> · getById(id):
                       Observable\<Ruling\> · getRelated(id):
                       Observable\<Ruling\[\]\>

  **ChatService**      sendMessage(query): Observable\<string\> (SSE stream)

  **CrawlerService**   getAll(): Observable\<CrawlerConfig\[\]\> ·
                       run(sourceId, type): Observable\<void\> ·
                       update(sourceId, config): Observable\<void\>

  **DlqService**       getMessages(queue): Observable\<DlqMessage\[\]\> ·
                       requeue(queue, id): Observable\<void\>

  **JobsService**      getPipelineStatus(): Observable\<PipelineStatus\> ·
                       getJobs(): Observable\<Job\[\]\> — consumes GET
                       /api/admin/pipeline/status and GET /api/admin/jobs

  **UserService**      getAll(): Observable\<User\[\]\> · create(user):
                       Observable\<User\> · update(id, user):
                       Observable\<User\> · delete(id): Observable\<void\>

## 9. Configuration and infrastructure

### 9.1 Environment variables — API and Workers
Shared (all components)

  **Variable**                              **Description**

  **AzureSql__ConnectionString**          Connection string for Azure SQL Database

  **AzureServiceBus__ConnectionString**   Connection string for the Service Bus namespace

  **AzureBlob__ConnectionString**         Connection string for Azure Blob Storage

  **AzureBlob__ContainerName**            Container name for PDFs (e.g.: rulings-pdfs)

  **Neo4j__Uri**                          Neo4j URI (e.g.: bolt://neo4j-vm:7687)

  **Neo4j__Username**                     Neo4j username

  **Neo4j__Password**                     Neo4j password

  **AzureOpenAI__Endpoint**               Endpoint of the Azure OpenAI resource (e.g.: https://legal-ai-openai.openai.azure.com/)

  **AzureOpenAI__ApiKey**                 API key of the Azure OpenAI resource

  **AzureOpenAI__ChatDeploymentName**    Deployment name for gpt-4o
  **AzureOpenAI__EmbeddingDeploymentName** Deployment name for text-embedding-3-large
API and IndexerWorker

  **Variable**                         **Description**

  **AzureSearch__Endpoint**          Endpoint for Azure AI Search (API and IndexerWorker for IndexSearchStep)

  **AzureSearch__ApiKey**            API key for Azure AI Search

  **AzureSearch__RulingIndexName**   rulings-by-ruling

  **AzureSearch__ChunkIndexName**    rulings-by-chunk

API only

  **AzureAd__TenantId**              Tenant ID for Azure Entra ID

  **AzureAd__ClientId**              Client ID of the registered app

  **AzureAd__Audience**              JWT token audience

### 9.2 Local infrastructure (development)
The docker-compose.yml file in the root starts the services
needed for local development:

> services:
>
> sqlserver:
>
> image: mcr.microsoft.com/mssql/server:2022-latest
>
> environment: { SA_PASSWORD: \"Dev_Password123!\", ACCEPT_EULA: \"Y\" }
>
> ports: \[\"1433:1433\"\]
>
> neo4j:
>
> image: neo4j:5-community
>
> environment: { NEO4J_AUTH: \"neo4j/dev_password\" }
>
> ports: \[\"7474:7474\", \"7687:7687\"\]

📌 *Azure Service Bus, Azure AI Search and Azure Blob Storage are used
directly from Azure in development (free / basic tier). They are not
emulated locally.*

### 9.3 Container Apps — configuration per worker

  **Worker**            **Resources**    **Scale**

  crawler-worker        0.5 vCPU / 1GB   min 0, max 1
                        RAM              

  parser-worker         0.5 vCPU / 1GB   min 0, max 2 (KEDA)
                        RAM              

  enrichment-worker     1 vCPU / 2GB RAM min 0, max 2 (KEDA)

  indexer-worker        1 vCPU / 2GB RAM min 0, max 2 (KEDA)

## 10. Testing

### 10.1 Strategy
Phase 1 implements only unit tests with xUnit. Mocks are
implemented with NSubstitute. Integration and E2E tests are deferred
to Phase 2.

📌 *Infrastructure tests (repositories, external services) are
deferred to Phase 2 (integration). LegalAiAr.Infrastructure.Tests
is not required in Phase 1.*

### 10.2 Coverage per project

  **Test project**                    **What to test**       **Examples**

  LegalAiAr.Core.Tests                Pure domain logic:     Ruling cannot have
                                      constructors,          future RulingDate · Citation
                                      business rules,        without ExternalAlias is invalid
                                      entity validations     

  LegalAiAr.Application.Tests        CQRS handlers with     SearchRulingsHandler returns
                                      mocked repositories.  correct results ·
                                      Validation behaviors. RunCrawlerHandler fails if
                                                             source not enabled

  LegalAiAr.Worker.Crawler.Tests     Incremental discovery  CsjnCrawlerSource does not enqueue
                                      logic. Duplicate       already indexed documents
                                      detection by           
                                      SHA-256.               

  LegalAiAr.Worker.Parser.Tests      PDF text normalization. PdfTextExtractor collapses
                                      Parsing of CSJN API    multiple spaces ·
                                      responses.             CsjnApiParser maps fields
                                                             correctly

  LegalAiAr.Worker.Enrichment.Tests  Strategy selection by   CsjnEnrichmentStrategy only
                                      source. Prompt         calls GPT-4o for
                                      construction.          missingFields

  LegalAiAr.Worker.Indexer.Tests     Retroactive citation    ResolveCitationsStep updates
                                      resolution. Hash-based TargetRulingId when
                                      idempotency.           match found

### 10.3 Conventions

-   Test name: {Method}_{Scenario}_{ExpectedResult}

-   One test file per class under test

-   Arrange / Act / Assert separated by blank line

-   No magic strings: use constants in TestData class or
    Theory/InlineData

> // Example of correct name
>
> \[Fact\]
>
> public async Task Handle_WhenSourceDisabled_ThrowsDomainException()

## 11. Architecture decisions

| ID | Decision | Detail |
|---|---|---|
| ADR-001 | Stack cloud | Azure (Blob, SQL, AI Search, Entra ID, Service Bus, Azure OpenAI) |
| ADR-002 | Chunking | 512 tokens, overlap 50. Two index levels in Azure AI Search |
| ADR-003 | Authentication | Entra ID. Phase 1: no roles. Phase 2: roles from Entra ID groups |
| ADR-004 | Graph | Neo4j Community Edition on Azure VM (without managed service) |
| ADR-005 | Immutability | Total. Deduplication by SHA-256 of PDF content |
| ADR-006 | Single-tenant | Final |
| ADR-007 | PDF parsing | PdfPig without Azure Document Intelligence in Phase 1 |
| ADR-008 | CSJN pipeline | API-first. GPT-4o only for gap-filling |
| ADR-009 | Messaging | Service Bus Standard. 4 separate queues per worker |
| ADR-010 | Workers | Azure Container Apps consumption plan with KEDA |
| ADR-011 | Crawler triggers | Manual in Phase 1. Cron in Phase 2 |
| ADR-012 | Admin UI | Integrated in Angular SPA under `/admin/*` |
| ADR-013 | Phase 1 roles | All authenticated users are admin |
| ADR-014 | LLM and embeddings | Azure OpenAI Service: `gpt-4o` (enrichment, agentic chat), `gpt-4o-mini` (guardrail, query enricher), `text-embedding-3-large` (embeddings) |

## 12. Open technical risks

| ID | Description | Severity | Status |
|---|---|---|---|
| R-001 | Breaking changes in CSJN API without notice | Medium | Open |
| R-002 | Undocumented rate limiting on CSJN endpoints | Medium | Open |
| R-003 | PDF quality in PJN and SCBA not validated | High | Open — Phase 2 |
| R-004 | Neo4j CE without clustering → SPOF | Low | Accepted |
| R-005 | Worker hosting undefined (ADR-010) | Medium | ✅ Closed — Container Apps |
