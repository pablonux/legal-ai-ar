using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRulingReprocessRequestsAndIngestionJobSentinel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RulingReprocessRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    UseCache = table.Column<bool>(type: "bit", nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulingReprocessRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RulingReprocessRequests_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RulingReprocessRequests_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Seed may already exist (manual / prior DB). Inserts are conditional.
            migrationBuilder.Sql(
                """
                IF NOT EXISTS (SELECT 1 FROM [Sources] WHERE [Id] = 6)
                BEGIN
                    SET IDENTITY_INSERT [Sources] ON;
                    INSERT INTO [Sources] ([Id], [BaseUrl], [FullName], [IsActive], [Name], [Strategy])
                    VALUES (6, N'http://vocabularios.saij.gob.ar/', N'SAIJ — Tesauro (API vocabularios)', CAST(1 AS bit), N'SAIJ', N'thesaurus');
                    SET IDENTITY_INSERT [Sources] OFF;
                END
                """);

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (SELECT 1 FROM [CrawlerConfigs] WHERE [Id] = 5)
                BEGIN
                    SET IDENTITY_INSERT [CrawlerConfigs] ON;
                    INSERT INTO [CrawlerConfigs] ([Id], [CreatedAt], [CronExpression], [IsEnabled], [LastCrawledAt], [LastCrawledStatus], [LastDocumentCount], [SourceId], [UpdatedAt])
                    VALUES (5, '2026-03-10T00:00:00.0000000', NULL, CAST(1 AS bit), NULL, NULL, NULL, 6, '2026-03-10T00:00:00.0000000');
                    SET IDENTITY_INSERT [CrawlerConfigs] OFF;
                END
                """);

            migrationBuilder.CreateIndex(
                name: "IX_RulingReprocessRequests_DocumentId",
                table: "RulingReprocessRequests",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RulingReprocessRequests_RequestedAt",
                table: "RulingReprocessRequests",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RulingReprocessRequests_RulingId_Status",
                table: "RulingReprocessRequests",
                columns: new[] { "RulingId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RulingReprocessRequests");

            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM [CrawlerConfigs] WHERE [Id] = 5 AND [SourceId] = 6)
                    DELETE FROM [CrawlerConfigs] WHERE [Id] = 5;
                IF EXISTS (
                    SELECT 1 FROM [Sources]
                    WHERE [Id] = 6
                      AND [BaseUrl] = N'http://vocabularios.saij.gob.ar/'
                      AND [Strategy] = N'thesaurus')
                    DELETE FROM [Sources] WHERE [Id] = 6;
                """);
        }
    }
}
