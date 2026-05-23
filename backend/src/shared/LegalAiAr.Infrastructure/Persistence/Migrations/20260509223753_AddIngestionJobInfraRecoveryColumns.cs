using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIngestionJobInfraRecoveryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DegradedReason",
                table: "IngestionJobs",
                type: "nvarchar(max)",
                maxLength: -1,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DegradedSinceUtc",
                table: "IngestionJobs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DiscoveryBatchPublished",
                table: "IngestionJobs",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "InfrastructureDegraded",
                table: "IngestionJobs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegradedReason",
                table: "IngestionJobs");

            migrationBuilder.DropColumn(
                name: "DegradedSinceUtc",
                table: "IngestionJobs");

            migrationBuilder.DropColumn(
                name: "DiscoveryBatchPublished",
                table: "IngestionJobs");

            migrationBuilder.DropColumn(
                name: "InfrastructureDegraded",
                table: "IngestionJobs");
        }
    }
}
