using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    JurisdictionArea = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Territory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Instance = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalCode = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Strategy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statutes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Judges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CurrentCourtId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Judges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Judges_Courts_CurrentCourtId",
                        column: x => x.CurrentCourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CrawlerConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CronExpression = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastCrawledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastCrawledStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LastDocumentCount = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrawlerConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrawlerConfigs_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rulings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnalysisId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContentHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CaseTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CaseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RulingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    JurisdictionArea = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Instance = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Jurisdiction = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ResourceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RulingDirection = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SubjectArea = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsUnconstitutional = table.Column<bool>(type: "bit", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Holding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlobPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IndexedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rulings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rulings_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rulings_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Citations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceRulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetRulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalAlias = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CsjnSummaryId = table.Column<int>(type: "int", nullable: true),
                    CitationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Citations_Rulings_SourceRulingId",
                        column: x => x.SourceRulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citations_Rulings_TargetRulingId",
                        column: x => x.TargetRulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RulingJudges",
                columns: table => new
                {
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JudgeId = table.Column<int>(type: "int", nullable: false),
                    ParticipationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulingJudges", x => new { x.RulingId, x.JudgeId });
                    table.ForeignKey(
                        name: "FK_RulingJudges_Judges_JudgeId",
                        column: x => x.JudgeId,
                        principalTable: "Judges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RulingJudges_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RulingKeywords",
                columns: table => new
                {
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeywordId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulingKeywords", x => new { x.RulingId, x.KeywordId });
                    table.ForeignKey(
                        name: "FK_RulingKeywords_Keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RulingKeywords_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RulingStatutes",
                columns: table => new
                {
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatuteId = table.Column<int>(type: "int", nullable: false),
                    Articles = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulingStatutes", x => new { x.RulingId, x.StatuteId });
                    table.ForeignKey(
                        name: "FK_RulingStatutes_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RulingStatutes_Statutes_StatuteId",
                        column: x => x.StatuteId,
                        principalTable: "Statutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citations_ExternalAlias",
                table: "Citations",
                column: "ExternalAlias");

            migrationBuilder.CreateIndex(
                name: "IX_Citations_SourceRulingId",
                table: "Citations",
                column: "SourceRulingId");

            migrationBuilder.CreateIndex(
                name: "IX_Citations_TargetRulingId",
                table: "Citations",
                column: "TargetRulingId");

            migrationBuilder.CreateIndex(
                name: "IX_CrawlerConfigs_SourceId",
                table: "CrawlerConfigs",
                column: "SourceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Judges_CurrentCourtId",
                table: "Judges",
                column: "CurrentCourtId");

            migrationBuilder.CreateIndex(
                name: "IX_RulingJudges_JudgeId",
                table: "RulingJudges",
                column: "JudgeId");

            migrationBuilder.CreateIndex(
                name: "IX_RulingKeywords_KeywordId",
                table: "RulingKeywords",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_Rulings_ContentHash",
                table: "Rulings",
                column: "ContentHash");

            migrationBuilder.CreateIndex(
                name: "IX_Rulings_CourtId",
                table: "Rulings",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_Rulings_SourceId_ExternalId",
                table: "Rulings",
                columns: new[] { "SourceId", "ExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_RulingStatutes_StatuteId",
                table: "RulingStatutes",
                column: "StatuteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Citations");

            migrationBuilder.DropTable(
                name: "CrawlerConfigs");

            migrationBuilder.DropTable(
                name: "RulingJudges");

            migrationBuilder.DropTable(
                name: "RulingKeywords");

            migrationBuilder.DropTable(
                name: "RulingStatutes");

            migrationBuilder.DropTable(
                name: "Judges");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropTable(
                name: "Rulings");

            migrationBuilder.DropTable(
                name: "Statutes");

            migrationBuilder.DropTable(
                name: "Courts");

            migrationBuilder.DropTable(
                name: "Sources");
        }
    }
}
