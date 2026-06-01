using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Thesaurus;

/// <summary>
/// Applies the same conditional inserts as migration AddThesaurusIngestSource when the DB was not migrated.
/// </summary>
public sealed class ThesaurusIngestCatalogBootstrap : IThesaurusIngestCatalogBootstrap
{
    private const string BootstrapSql = """
IF NOT EXISTS (SELECT 1 FROM dbo.Sources WHERE Id = 6)
BEGIN
    SET IDENTITY_INSERT dbo.Sources ON;
    INSERT INTO dbo.Sources (Id, Name, FullName, BaseUrl, Strategy, IsActive)
    VALUES (6, N'SAIJ', N'SAIJ — Tesauro (API vocabularios)', N'http://vocabularios.saij.gob.ar/', N'thesaurus', 1);
    SET IDENTITY_INSERT dbo.Sources OFF;
END
IF NOT EXISTS (SELECT 1 FROM dbo.CrawlerConfigs WHERE SourceId = 6)
BEGIN
    INSERT INTO dbo.CrawlerConfigs (CreatedAt, CronExpression, IsEnabled, LastCrawledAt, LastCrawledStatus, LastDocumentCount, SourceId, UpdatedAt)
    VALUES (GETUTCDATE(), NULL, 1, NULL, NULL, NULL, 6, GETUTCDATE());
END
""";

    private readonly AppDbContext _db;
    private readonly ILogger<ThesaurusIngestCatalogBootstrap> _logger;

    public ThesaurusIngestCatalogBootstrap(AppDbContext db, ILogger<ThesaurusIngestCatalogBootstrap> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task EnsureAsync(CancellationToken cancellationToken = default)
    {
        await _db.Database.ExecuteSqlRawAsync(BootstrapSql, cancellationToken);
        _logger.LogDebug("Thesaurus ingest catalog bootstrap executed (Sources/CrawlerConfigs for SourceId 6).");
    }
}
