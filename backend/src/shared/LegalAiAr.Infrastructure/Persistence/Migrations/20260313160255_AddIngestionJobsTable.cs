using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIngestionJobsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IngestionJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "ruling"),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    DateTo = table.Column<DateOnly>(type: "date", nullable: true),
                    TriggeredBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DocumentsDiscovered = table.Column<int>(type: "int", nullable: false),
                    DocumentsIndexed = table.Column<int>(type: "int", nullable: false),
                    DocumentsFailed = table.Column<int>(type: "int", nullable: false),
                    ErrorSummary = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngestionJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngestionJobs_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngestionJobs_SourceId",
                table: "IngestionJobs",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IngestionJobs_StartedAt",
                table: "IngestionJobs",
                column: "StartedAt");

            migrationBuilder.AddColumn<Guid>(
                name: "IngestionJobId",
                table: "Rulings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rulings_IngestionJobId",
                table: "Rulings",
                column: "IngestionJobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rulings_IngestionJobs_IngestionJobId",
                table: "Rulings",
                column: "IngestionJobId",
                principalTable: "IngestionJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rulings_IngestionJobs_IngestionJobId",
                table: "Rulings");

            migrationBuilder.DropIndex(
                name: "IX_Rulings_IngestionJobId",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "IngestionJobId",
                table: "Rulings");

            migrationBuilder.DropTable(
                name: "IngestionJobs");
        }
    }
}
