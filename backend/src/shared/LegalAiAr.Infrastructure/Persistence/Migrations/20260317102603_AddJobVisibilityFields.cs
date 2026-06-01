using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJobVisibilityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentsEnriched",
                table: "IngestionJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DocumentsParsed",
                table: "IngestionJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentJobId",
                table: "IngestionJobs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartitionIndex",
                table: "IngestionJobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartitionTotal",
                table: "IngestionJobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalSearchResults",
                table: "IngestionJobs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngestionJobs_ParentJobId",
                table: "IngestionJobs",
                column: "ParentJobId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngestionJobs_IngestionJobs_ParentJobId",
                table: "IngestionJobs",
                column: "ParentJobId",
                principalTable: "IngestionJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngestionJobs_IngestionJobs_ParentJobId",
                table: "IngestionJobs");

            migrationBuilder.DropIndex(
                name: "IX_IngestionJobs_ParentJobId",
                table: "IngestionJobs");

            migrationBuilder.DropColumn(
                name: "DocumentsEnriched",
                table: "IngestionJobs");

            migrationBuilder.DropColumn(
                name: "DocumentsParsed",
                table: "IngestionJobs");

            migrationBuilder.DropColumn(
                name: "ParentJobId",
                table: "IngestionJobs");

            migrationBuilder.DropColumn(
                name: "PartitionIndex",
                table: "IngestionJobs");

            migrationBuilder.DropColumn(
                name: "PartitionTotal",
                table: "IngestionJobs");

            migrationBuilder.DropColumn(
                name: "TotalSearchResults",
                table: "IngestionJobs");
        }
    }
}
