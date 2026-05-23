# Developer — Code Standards Reference

## C# / .NET

- **Namespaces**: Aligned with folder structure. E.g. `LegalAiAr.Worker.Crawler.Sources`
- **Dependency injection**: Constructor only, never property
- **async/await**: All I/O async. Use `CancellationToken` throughout
- **Records**: For Service Bus messages, immutable DTOs, query results
- **Logging**: `ILogger<T>` with structured logging. Never `Console.WriteLine`
- **Configuration**: `IOptions<T>` with typed option classes. Never `IConfiguration` in business classes
- **Exceptions**: `DomainException` for business errors
- **Tests**: Arrange / Act / Assert with blank line. One test per behavior. NSubstitute for mocks

## Angular

- **Standalone components**: Always, no NgModules
- **Signals**: For component local state where appropriate
- **Observables**: For data streams and HTTP
- **Typing**: No `any`. Interfaces in `core/models/`
- **HTTP errors**: Centralized in `ErrorInterceptor`
- **Routes**: Slugs in Spanish for URLs; component/file names in English

## Configuration

- `appsettings.json`: No secrets — use env vars or Key Vault
- `.env.example`: Update when new variables added
- Dockerfiles: Multi-stage (build + runtime)
