using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedSourcesAndCrawlerConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "BaseUrl", "FullName", "IsActive", "Name", "Strategy" },
                values: new object[,]
                {
                    { 1, "https://sjconsulta.csjn.gov.ar/sjconsulta/", "Corte Suprema de Justicia de la NaciÃ³n", true, "CSJN", "api-first" },
                    { 2, "https://www.saij.gob.ar/", "Sistema Argentino de InformaciÃ³n JurÃ­dica", true, "SAIJ", "html-pdf" },
                    { 3, "https://www.pjn.gov.ar/", "Poder Judicial de la NaciÃ³n", true, "PJN", "html-pdf" },
                    { 4, "https://www.scba.gov.ar/", "Suprema Corte de Justicia de la Provincia de Buenos Aires", true, "SCBA", "html-pdf" }
                });

            migrationBuilder.InsertData(
                table: "CrawlerConfigs",
                columns: new[] { "Id", "CreatedAt", "CronExpression", "IsEnabled", "LastCrawledAt", "LastCrawledStatus", "LastDocumentCount", "SourceId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, null, null, 1, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, null, null, 2, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, null, null, 3, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, null, null, 4, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CrawlerConfigs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CrawlerConfigs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CrawlerConfigs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CrawlerConfigs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
