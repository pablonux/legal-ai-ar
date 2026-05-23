using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThesaurusTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThesaurusTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsPreferred = table.Column<bool>(type: "bit", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Depth = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesaurusTerms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThesaurusRelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceTermId = table.Column<int>(type: "int", nullable: false),
                    TargetTermId = table.Column<int>(type: "int", nullable: false),
                    RelationType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesaurusRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThesaurusRelations_ThesaurusTerms_SourceTermId",
                        column: x => x.SourceTermId,
                        principalTable: "ThesaurusTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThesaurusRelations_ThesaurusTerms_TargetTermId",
                        column: x => x.TargetTermId,
                        principalTable: "ThesaurusTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusRelations_RelationType",
                table: "ThesaurusRelations",
                column: "RelationType");

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusRelations_SourceTermId_TargetTermId_RelationType",
                table: "ThesaurusRelations",
                columns: new[] { "SourceTermId", "TargetTermId", "RelationType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusRelations_TargetTermId",
                table: "ThesaurusRelations",
                column: "TargetTermId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusTerms_ExternalId",
                table: "ThesaurusTerms",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusTerms_IsPreferred",
                table: "ThesaurusTerms",
                column: "IsPreferred");

            migrationBuilder.CreateIndex(
                name: "IX_ThesaurusTerms_Label",
                table: "ThesaurusTerms",
                column: "Label");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThesaurusRelations");

            migrationBuilder.DropTable(
                name: "ThesaurusTerms");
        }
    }
}
