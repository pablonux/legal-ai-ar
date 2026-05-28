using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public class AddThesaurusIngestSource : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM dbo.Sources WHERE Id = 6)
BEGIN
    SET IDENTITY_INSERT dbo.Sources ON;
    INSERT INTO dbo.Sources (Id, Name, FullName, BaseUrl, Strategy, IsActive)
    VALUES (6, N'SAIJ', N'SAIJ — Tesauro (API vocabularios)', N'http://vocabularios.saij.gob.ar/', N'thesaurus', 1);
    SET IDENTITY_INSERT dbo.Sources OFF;
END
IF NOT EXISTS (SELECT 1 FROM dbo.CrawlerConfigs WHERE SourceId = 6)
BEGIN
    SET IDENTITY_INSERT dbo.CrawlerConfigs ON;
    INSERT INTO dbo.CrawlerConfigs (Id, CreatedAt, CronExpression, IsEnabled, LastCrawledAt, LastCrawledStatus, LastDocumentCount, SourceId, UpdatedAt)
    VALUES (5, GETUTCDATE(), NULL, 1, NULL, NULL, NULL, 6, GETUTCDATE());
    SET IDENTITY_INSERT dbo.CrawlerConfigs OFF;
END
""");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
DELETE FROM dbo.IngestionJobs WHERE SourceId = 6;
DELETE FROM dbo.CrawlerConfigs WHERE SourceId = 6;
DELETE FROM dbo.Sources WHERE Id = 6;
""");
    }
}
