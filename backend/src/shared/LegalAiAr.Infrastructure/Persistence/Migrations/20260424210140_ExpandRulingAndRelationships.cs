using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExpandRulingAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ThesaurusRelations",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RulingStatutes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "Rulings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FederalQuestion",
                table: "Rulings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasDictamen",
                table: "Rulings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InternalSubject",
                table: "Rulings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observations",
                table: "Rulings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialReference",
                table: "Rulings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProceduralFormula",
                table: "Rulings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RulingKeywords",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RulingJudges",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "NormRelations",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EffectiveDate",
                table: "NormRelations",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CitationText",
                table: "Citations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Citations",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "CsjnFalloId",
                table: "Citations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Citations_CsjnFalloId",
                table: "Citations",
                column: "CsjnFalloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Citations_CsjnFalloId",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ThesaurusRelations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RulingStatutes");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "FederalQuestion",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "HasDictamen",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "InternalSubject",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "Observations",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "OfficialReference",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "ProceduralFormula",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RulingKeywords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RulingJudges");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "NormRelations");

            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "NormRelations");

            migrationBuilder.DropColumn(
                name: "CitationText",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "CsjnFalloId",
                table: "Citations");
        }
    }
}
