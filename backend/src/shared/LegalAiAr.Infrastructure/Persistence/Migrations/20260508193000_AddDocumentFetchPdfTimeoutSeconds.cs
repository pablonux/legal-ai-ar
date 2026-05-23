using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddDocumentFetchPdfTimeoutSeconds : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Documents') AND name = 'FetchPdfTimeoutSeconds')
                ALTER TABLE [Documents] ADD [FetchPdfTimeoutSeconds] int NULL;
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FetchPdfTimeoutSeconds",
            table: "Documents");
    }
}
