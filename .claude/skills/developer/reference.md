# Developer — Code Standards Reference

## C# / .NET 10

- **Namespaces**: aligned with folders. E.g., `LegalAiAr.Infrastructure.Services.Search`
- **Dependency injection**: constructor only, never property
- **async/await**: all I/O async. Always use `CancellationToken`
- **Records**: for queue messages, immutable DTOs, query results
- **Logging**: `ILogger<T>` with structured logging. Never `Console.WriteLine`
- **Configuration**: `IOptions<T>` with typed classes. Never `IConfiguration` in business classes
- **Exceptions**: `DomainException` for business errors
- **Tests**: Arrange / Act / Assert with a blank line. One test per behavior. NSubstitute for mocks
- **Minimal API**: endpoints grouped by feature with extension methods (`MapLegalNormEndpoints()`)
- **CQRS**: Commands and Queries separated in Application, handlers with MediatR
- **Validation**: FluentValidation in Application, MediatR pipeline validation

## Angular 19

- **Standalone components**: always, no NgModules
- **Signals**: for local component state
- **Observables**: for data and HTTP streams
- **Typing**: no `any`. Interfaces in `core/models/`
- **HTTP errors**: centralized in `ErrorInterceptor`
- **Routes and URLs**: in English (kebab-case); component/file names in English. Only user-visible text is in Spanish
- **Styles**: Tailwind CSS 4 utility classes
- **SSE**: `EventSource` for agent chat streaming

## Semantic Kernel (LegalAiAr.Agents)

- Plugins in `Plugins/` with the `[KernelFunction]` attribute
- Versioned prompts in `Prompts/` as .yaml or .txt files
- Orchestration in `Orchestration/` with the ReAct pattern
- Each plugin receives dependencies via constructor (kernel DI)

## Configuration

- `appsettings.json`: no secrets — use env vars or Azure Key Vault
- `.env.example`: update when adding new variables
- `Directory.Packages.props`: centralized NuGet versions
- Docker: multi-stage builds (build + runtime)

## Naming

- Projects: `LegalAiAr.{Layer}` (never LegalKB)
- Azure resources: `{service}-legal-ai-ar-{environment}`
- Database: `LegalAiAr`
- Storage containers: kebab-case
- **Language rule (strict)**: everything in English (code, variables, methods, classes, file names, tables, indexes, storage, URLs, endpoints, DTOs, domain entities, commits, technical docs, work items, code comments). Spanish only for end-user facing content (UI labels, error messages, tooltips). Agent prompt YAML content stays Spanish (user-facing agent layer).
