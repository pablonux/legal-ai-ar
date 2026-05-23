using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RulingContentHashUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rulings_ContentHash",
                table: "Rulings");

            migrationBuilder.CreateIndex(
                name: "IX_Rulings_ContentHash",
                table: "Rulings",
                column: "ContentHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rulings_ContentHash",
                table: "Rulings");

            migrationBuilder.CreateIndex(
                name: "IX_Rulings_ContentHash",
                table: "Rulings",
                column: "ContentHash");
        }
    }
}
