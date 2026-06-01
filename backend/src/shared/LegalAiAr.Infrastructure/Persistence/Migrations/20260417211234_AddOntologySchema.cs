using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOntologySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EffectiveFrom",
                table: "Statutes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EffectiveTo",
                table: "Statutes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuingBody",
                table: "Statutes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalBranch",
                table: "Statutes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormType",
                table: "Statutes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormativeLevel",
                table: "Statutes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "SanctionDate",
                table: "Statutes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLeadingCase",
                table: "Rulings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlenario",
                table: "Rulings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LegalBranch",
                table: "Rulings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrecedentWeight",
                table: "Rulings",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourtCategory",
                table: "Courts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fuero",
                table: "Courts",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernmentLevel",
                table: "Courts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstanceLevel",
                table: "Courts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentCourtId",
                table: "Courts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NormRelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceStatuteId = table.Column<int>(type: "int", nullable: false),
                    TargetStatuteId = table.Column<int>(type: "int", nullable: false),
                    RelationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NormRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NormRelations_Statutes_SourceStatuteId",
                        column: x => x.SourceStatuteId,
                        principalTable: "Statutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NormRelations_Statutes_TargetStatuteId",
                        column: x => x.TargetStatuteId,
                        principalTable: "Statutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courts_ParentCourtId",
                table: "Courts",
                column: "ParentCourtId");

            migrationBuilder.CreateIndex(
                name: "IX_NormRelations_SourceStatuteId_TargetStatuteId_RelationType",
                table: "NormRelations",
                columns: new[] { "SourceStatuteId", "TargetStatuteId", "RelationType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NormRelations_TargetStatuteId",
                table: "NormRelations",
                column: "TargetStatuteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courts_Courts_ParentCourtId",
                table: "Courts",
                column: "ParentCourtId",
                principalTable: "Courts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courts_Courts_ParentCourtId",
                table: "Courts");

            migrationBuilder.DropTable(
                name: "NormRelations");

            migrationBuilder.DropIndex(
                name: "IX_Courts_ParentCourtId",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "IssuingBody",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "LegalBranch",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "NormType",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "NormativeLevel",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "SanctionDate",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "IsLeadingCase",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "IsPlenario",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "LegalBranch",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "PrecedentWeight",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "CourtCategory",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "Fuero",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "GovernmentLevel",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "InstanceLevel",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "ParentCourtId",
                table: "Courts");
        }
    }
}
