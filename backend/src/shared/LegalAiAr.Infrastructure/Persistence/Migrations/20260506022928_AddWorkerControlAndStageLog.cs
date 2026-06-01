using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerControlAndStageLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentStageLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMs = table.Column<int>(type: "int", nullable: false),
                    WorkerInstanceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentStageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentStageLogs_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerPauseStates",
                columns: table => new
                {
                    WorkerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPaused = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerPauseStates", x => x.WorkerType);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentStageLogs_DocumentId",
                table: "DocumentStageLogs",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentStageLogs_DocumentId_Stage",
                table: "DocumentStageLogs",
                columns: new[] { "DocumentId", "Stage" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentStageLogs_StartedAt",
                table: "DocumentStageLogs",
                column: "StartedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentStageLogs");

            migrationBuilder.DropTable(
                name: "WorkerPauseStates");
        }
    }
}
