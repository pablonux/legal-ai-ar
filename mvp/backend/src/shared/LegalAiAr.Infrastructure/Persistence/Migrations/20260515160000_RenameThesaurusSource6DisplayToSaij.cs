using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public class RenameThesaurusSource6DisplayToSaij : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
UPDATE dbo.Sources
SET Name = N'SAIJ', FullName = N'SAIJ — Tesauro (API vocabularios)'
WHERE Id = 6 AND (Name = N'Tesauro SAIJ' OR FullName LIKE N'Tesauro SAIJ%');
""");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
UPDATE dbo.Sources
SET Name = N'Tesauro SAIJ', FullName = N'Tesauro SAIJ de Derecho Argentino (API)'
WHERE Id = 6 AND Name = N'SAIJ';
""");
    }
}
