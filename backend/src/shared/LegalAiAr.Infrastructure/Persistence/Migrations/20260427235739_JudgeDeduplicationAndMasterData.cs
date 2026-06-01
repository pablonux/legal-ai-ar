using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class JudgeDeduplicationAndMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Statutes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "CsjnMinistroId",
                table: "Judges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Judges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Judges",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Unknown");

            migrationBuilder.AddColumn<string>(
                name: "ExternalCode",
                table: "Courts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Courts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ExternalAlias",
                table: "Citations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "JudgeTenures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JudgeId = table.Column<int>(type: "int", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JudgeTenures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JudgeTenures_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JudgeTenures_Judges_JudgeId",
                        column: x => x.JudgeId,
                        principalTable: "Judges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Judges_CsjnMinistroId",
                table: "Judges",
                column: "CsjnMinistroId",
                unique: true,
                filter: "[CsjnMinistroId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JudgeTenures_CourtId",
                table: "JudgeTenures",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_JudgeTenures_JudgeId_CourtId_StartDate",
                table: "JudgeTenures",
                columns: new[] { "JudgeId", "CourtId", "StartDate" },
                unique: true,
                filter: "[StartDate] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JudgeTenures");

            migrationBuilder.DropIndex(
                name: "IX_Judges_CsjnMinistroId",
                table: "Judges");

            migrationBuilder.DropColumn(
                name: "CsjnMinistroId",
                table: "Judges");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Judges");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Judges");

            migrationBuilder.DropColumn(
                name: "ExternalCode",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Courts");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Statutes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "ExternalAlias",
                table: "Citations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
