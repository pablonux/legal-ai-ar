using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddKeywordThesaurusLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThesaurusTermId",
                table: "Keywords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_ThesaurusTermId",
                table: "Keywords",
                column: "ThesaurusTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_Keywords_ThesaurusTerms_ThesaurusTermId",
                table: "Keywords",
                column: "ThesaurusTermId",
                principalTable: "ThesaurusTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Keywords_ThesaurusTerms_ThesaurusTermId",
                table: "Keywords");

            migrationBuilder.DropIndex(
                name: "IX_Keywords_ThesaurusTermId",
                table: "Keywords");

            migrationBuilder.DropColumn(
                name: "ThesaurusTermId",
                table: "Keywords");
        }
    }
}
