using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PipelineRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: IngestionJobs EntityType (may have been applied in partial run)
            migrationBuilder.Sql(@"
                IF COL_LENGTH('IngestionJobs','EntityType') IS NULL
                BEGIN
                    ALTER TABLE [IngestionJobs] ADD [EntityType] nvarchar(20) NOT NULL DEFAULT N'Ruling';
                END");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('IngestionJobs','DocumentType') IS NOT NULL
                BEGIN
                    UPDATE IngestionJobs SET EntityType = CASE WHEN DocumentType = 'ruling' THEN 'Ruling' WHEN DocumentType = 'statute' THEN 'Statute' ELSE 'Ruling' END;
                    DECLARE @df NVARCHAR(256);
                    SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON d.parent_column_id = c.column_id AND d.parent_object_id = c.object_id WHERE c.object_id = OBJECT_ID('IngestionJobs') AND c.name = 'DocumentType';
                    IF @df IS NOT NULL EXEC('ALTER TABLE [IngestionJobs] DROP CONSTRAINT [' + @df + ']');
                    ALTER TABLE [IngestionJobs] DROP COLUMN [DocumentType];
                END");

            // Idempotent: Statutes columns
            migrationBuilder.Sql(@"IF COL_LENGTH('Statutes','FullText') IS NULL ALTER TABLE [Statutes] ADD [FullText] nvarchar(max) NULL;");
            migrationBuilder.Sql(@"IF COL_LENGTH('Statutes','IssuingBodyName') IS NULL ALTER TABLE [Statutes] ADD [IssuingBodyName] nvarchar(300) NULL;");
            migrationBuilder.Sql(@"IF COL_LENGTH('Statutes','IssuingOrganId') IS NULL ALTER TABLE [Statutes] ADD [IssuingOrganId] int NULL;");
            migrationBuilder.Sql(@"IF COL_LENGTH('Statutes','PublicationDate') IS NULL ALTER TABLE [Statutes] ADD [PublicationDate] date NULL;");
            migrationBuilder.Sql(@"IF COL_LENGTH('Statutes','SaijId') IS NULL ALTER TABLE [Statutes] ADD [SaijId] nvarchar(100) NULL;");
            migrationBuilder.Sql(@"IF COL_LENGTH('Statutes','Status') IS NULL ALTER TABLE [Statutes] ADD [Status] nvarchar(30) NULL;");

            // Idempotent: Citations column
            migrationBuilder.Sql(@"IF COL_LENGTH('Citations','CitedStatuteId') IS NULL ALTER TABLE [Citations] ADD [CitedStatuteId] int NULL;");

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngestionJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AnalysisId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentStage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BlobPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContentHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", maxLength: -1, nullable: true),
                    ErrorType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatuteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_IngestionJobs_IngestionJobId",
                        column: x => x.IngestionJobId,
                        principalTable: "IngestionJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(@"
                IF OBJECT_ID('LegalDoctrines','U') IS NULL
                BEGIN
                    CREATE TABLE [LegalDoctrines] (
                        [Id] int NOT NULL IDENTITY,
                        [RulingId] uniqueidentifier NOT NULL,
                        [Statement] nvarchar(max) NOT NULL,
                        [Topic] nvarchar(200) NULL,
                        [IsOverruled] bit NOT NULL,
                        [OverruledByRulingId] uniqueidentifier NULL,
                        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
                        CONSTRAINT [PK_LegalDoctrines] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_LegalDoctrines_Rulings_OverruledByRulingId] FOREIGN KEY ([OverruledByRulingId]) REFERENCES [Rulings]([Id]) ON DELETE SET NULL,
                        CONSTRAINT [FK_LegalDoctrines_Rulings_RulingId] FOREIGN KEY ([RulingId]) REFERENCES [Rulings]([Id]) ON DELETE NO ACTION
                    );
                END");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('StateOrgans','U') IS NULL
                BEGIN
                    CREATE TABLE [StateOrgans] (
                        [Id] int NOT NULL IDENTITY,
                        [Name] nvarchar(300) NOT NULL,
                        [Abbreviation] nvarchar(30) NULL,
                        [Branch] nvarchar(30) NOT NULL,
                        [GovernmentLevel] nvarchar(30) NULL,
                        [JurisdictionArea] nvarchar(100) NULL,
                        [ParentOrganId] int NULL,
                        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
                        CONSTRAINT [PK_StateOrgans] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_StateOrgans_StateOrgans_ParentOrganId] FOREIGN KEY ([ParentOrganId]) REFERENCES [StateOrgans]([Id])
                    );
                END");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('ProceduralRemedies','U') IS NULL
                BEGIN
                    CREATE TABLE [ProceduralRemedies] (
                        [Id] int NOT NULL IDENTITY,
                        [RemedyType] nvarchar(80) NOT NULL,
                        [FilingDate] date NULL,
                        [ResolutionDate] date NULL,
                        [Outcome] nvarchar(500) NULL,
                        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
                        [ResolvingRulingId] uniqueidentifier NULL,
                        [AppealedRulingId] uniqueidentifier NULL,
                        [CourtAQuoId] int NULL,
                        [CourtAdQuemId] int NULL,
                        [JudicialProceedingId] int NULL,
                        CONSTRAINT [PK_ProceduralRemedies] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_ProceduralRemedies_Courts_CourtAQuoId] FOREIGN KEY ([CourtAQuoId]) REFERENCES [Courts]([Id]) ON DELETE SET NULL,
                        CONSTRAINT [FK_ProceduralRemedies_Courts_CourtAdQuemId] FOREIGN KEY ([CourtAdQuemId]) REFERENCES [Courts]([Id]) ON DELETE NO ACTION,
                        CONSTRAINT [FK_ProceduralRemedies_JudicialProceedings_JudicialProceedingId] FOREIGN KEY ([JudicialProceedingId]) REFERENCES [JudicialProceedings]([Id]) ON DELETE SET NULL,
                        CONSTRAINT [FK_ProceduralRemedies_Rulings_AppealedRulingId] FOREIGN KEY ([AppealedRulingId]) REFERENCES [Rulings]([Id]) ON DELETE SET NULL,
                        CONSTRAINT [FK_ProceduralRemedies_Rulings_ResolvingRulingId] FOREIGN KEY ([ResolvingRulingId]) REFERENCES [Rulings]([Id]) ON DELETE NO ACTION
                    );
                END");

            // Idempotent indexes
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Statutes_IssuingOrganId') CREATE INDEX [IX_Statutes_IssuingOrganId] ON [Statutes]([IssuingOrganId]);");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Statutes_SaijId') CREATE UNIQUE INDEX [IX_Statutes_SaijId] ON [Statutes]([SaijId]) WHERE [SaijId] IS NOT NULL;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_IngestionJobs_EntityType_SourceId') CREATE INDEX [IX_IngestionJobs_EntityType_SourceId] ON [IngestionJobs]([EntityType],[SourceId]);");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Citations_CitedStatuteId') CREATE INDEX [IX_Citations_CitedStatuteId] ON [Citations]([CitedStatuteId]);");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Job_Stage_Status",
                table: "Documents",
                columns: new[] { "IngestionJobId", "CurrentStage", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SourceId_ExternalId",
                table: "Documents",
                columns: new[] { "SourceId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Stage_Status",
                table: "Documents",
                columns: new[] { "CurrentStage", "Status" });

            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_LegalDoctrines_OverruledByRulingId') CREATE INDEX [IX_LegalDoctrines_OverruledByRulingId] ON [LegalDoctrines]([OverruledByRulingId]) WHERE [OverruledByRulingId] IS NOT NULL;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_LegalDoctrines_RulingId') CREATE INDEX [IX_LegalDoctrines_RulingId] ON [LegalDoctrines]([RulingId]);");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_ProceduralRemedies_AppealedRulingId') CREATE INDEX [IX_ProceduralRemedies_AppealedRulingId] ON [ProceduralRemedies]([AppealedRulingId]);");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_ProceduralRemedies_CourtAdQuemId') CREATE INDEX [IX_ProceduralRemedies_CourtAdQuemId] ON [ProceduralRemedies]([CourtAdQuemId]);");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_ProceduralRemedies_CourtAQuoId') CREATE INDEX [IX_ProceduralRemedies_CourtAQuoId] ON [ProceduralRemedies]([CourtAQuoId]);");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_ProceduralRemedies_JudicialProceedingId') CREATE INDEX [IX_ProceduralRemedies_JudicialProceedingId] ON [ProceduralRemedies]([JudicialProceedingId]);");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_ProceduralRemedies_ResolvingRulingId') CREATE INDEX [IX_ProceduralRemedies_ResolvingRulingId] ON [ProceduralRemedies]([ResolvingRulingId]);");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_StateOrgans_Abbreviation') CREATE INDEX [IX_StateOrgans_Abbreviation] ON [StateOrgans]([Abbreviation]) WHERE [Abbreviation] IS NOT NULL;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_StateOrgans_Name') CREATE INDEX [IX_StateOrgans_Name] ON [StateOrgans]([Name]);");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_StateOrgans_ParentOrganId') CREATE INDEX [IX_StateOrgans_ParentOrganId] ON [StateOrgans]([ParentOrganId]);");

            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_Citations_Statutes_CitedStatuteId') ALTER TABLE [Citations] ADD CONSTRAINT [FK_Citations_Statutes_CitedStatuteId] FOREIGN KEY ([CitedStatuteId]) REFERENCES [Statutes]([Id]) ON DELETE SET NULL;");
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_Statutes_StateOrgans_IssuingOrganId') ALTER TABLE [Statutes] ADD CONSTRAINT [FK_Statutes_StateOrgans_IssuingOrganId] FOREIGN KEY ([IssuingOrganId]) REFERENCES [StateOrgans]([Id]) ON DELETE SET NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citations_Statutes_CitedStatuteId",
                table: "Citations");

            migrationBuilder.DropForeignKey(
                name: "FK_Statutes_StateOrgans_IssuingOrganId",
                table: "Statutes");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "LegalDoctrines");

            migrationBuilder.DropTable(
                name: "ProceduralRemedies");

            migrationBuilder.DropTable(
                name: "StateOrgans");

            migrationBuilder.DropIndex(
                name: "IX_Statutes_IssuingOrganId",
                table: "Statutes");

            migrationBuilder.DropIndex(
                name: "IX_Statutes_SaijId",
                table: "Statutes");

            migrationBuilder.DropIndex(
                name: "IX_IngestionJobs_EntityType_SourceId",
                table: "IngestionJobs");

            migrationBuilder.DropIndex(
                name: "IX_Citations_CitedStatuteId",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "FullText",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "IssuingBodyName",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "IssuingOrganId",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "PublicationDate",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "SaijId",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "IngestionJobs");

            migrationBuilder.DropColumn(
                name: "CitedStatuteId",
                table: "Citations");

            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                table: "IngestionJobs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "ruling");
        }
    }
}
