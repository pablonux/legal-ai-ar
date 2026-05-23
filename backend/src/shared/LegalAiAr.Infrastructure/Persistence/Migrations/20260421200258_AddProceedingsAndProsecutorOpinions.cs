using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProceedingsAndProsecutorOpinions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JudicialProceedingId",
                table: "Rulings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JudicialProceedings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    JurisdictionArea = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RulingCount = table.Column<int>(type: "int", nullable: false),
                    FirstRulingDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LastRulingDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JudicialProceedings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProsecutorOpinions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProsecutorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RecommendedDirection = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AgreedWithCourt = table.Column<bool>(type: "bit", nullable: false),
                    ExtractedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProsecutorOpinions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProsecutorOpinions_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rulings_JudicialProceedingId",
                table: "Rulings",
                column: "JudicialProceedingId");

            migrationBuilder.CreateIndex(
                name: "IX_JudicialProceedings_CaseNumber_JurisdictionArea",
                table: "JudicialProceedings",
                columns: new[] { "CaseNumber", "JurisdictionArea" });

            migrationBuilder.CreateIndex(
                name: "IX_ProsecutorOpinions_RulingId",
                table: "ProsecutorOpinions",
                column: "RulingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rulings_JudicialProceedings_JudicialProceedingId",
                table: "Rulings",
                column: "JudicialProceedingId",
                principalTable: "JudicialProceedings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rulings_JudicialProceedings_JudicialProceedingId",
                table: "Rulings");

            migrationBuilder.DropTable(
                name: "JudicialProceedings");

            migrationBuilder.DropTable(
                name: "ProsecutorOpinions");

            migrationBuilder.DropIndex(
                name: "IX_Rulings_JudicialProceedingId",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "JudicialProceedingId",
                table: "Rulings");
        }
    }
}
