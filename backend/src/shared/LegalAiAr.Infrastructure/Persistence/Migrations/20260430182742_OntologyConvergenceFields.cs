using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAiAr.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OntologyConvergenceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JudgeTenures");

            migrationBuilder.DropTable(
                name: "RulingJudges");

            migrationBuilder.DropTable(
                name: "Judges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RulingStatutes",
                table: "RulingStatutes");

            migrationBuilder.AddColumn<string>(
                name: "OfficialUrl",
                table: "Statutes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Articles",
                table: "RulingStatutes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RulingStatutes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "DoctrinaLegal",
                table: "Rulings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RatioDecidendi",
                table: "Rulings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentBlobPath",
                table: "ProsecutorOpinions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "ProsecutorOpinions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourtId",
                table: "JudicialProceedings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalBranch",
                table: "JudicialProceedings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessSubtype",
                table: "JudicialProceedings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessType",
                table: "JudicialProceedings",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "JudicialProceedings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RulingStatutes",
                table: "RulingStatutes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ChunkEntityMentions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChunkIndex = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MentionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Confidence = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChunkEntityMentions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChunkEntityMentions_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmbeddingConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmbeddingModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmbeddingDimensions = table.Column<int>(type: "int", nullable: false),
                    ContextualizationMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChunkingStrategy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChunkSize = table.Column<int>(type: "int", nullable: false),
                    ChunkOverlap = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbeddingConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IngestionJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChangeSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FieldsChanged = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityAuditLogs_IngestionJobs_IngestionJobId",
                        column: x => x.IngestionJobId,
                        principalTable: "IngestionJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ExternalIdentifiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    ExternalCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalIdentifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalIdentifiers_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FieldProvenance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PreviousValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    SourceEndpoint = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceField = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InferenceMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AiModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AiPrompt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AiConfidence = table.Column<float>(type: "real", nullable: true),
                    IngestionJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldProvenance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldProvenance_IngestionJobs_IngestionJobId",
                        column: x => x.IngestionJobId,
                        principalTable: "IngestionJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldProvenance_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngestionJobDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IngestionJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntitiesCreated = table.Column<int>(type: "int", nullable: false),
                    EntitiesUpdated = table.Column<int>(type: "int", nullable: false),
                    EntitiesDeleted = table.Column<int>(type: "int", nullable: false),
                    FieldsUpdated = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngestionJobDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngestionJobDetails_IngestionJobs_IngestionJobId",
                        column: x => x.IngestionJobId,
                        principalTable: "IngestionJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PersonType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Physical"),
                    LegalEntityType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CsjnMinistroId = table.Column<int>(type: "int", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RulingLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LinkType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulingLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RulingLinks_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RulingSourceMetadata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulingSourceMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RulingSourceMetadata_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RulingSourceMetadata_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RulingStatuteArticles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RulingStatuteId = table.Column<int>(type: "int", nullable: false),
                    Article = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subsection = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulingStatuteArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RulingStatuteArticles_RulingStatutes_RulingStatuteId",
                        column: x => x.RulingStatuteId,
                        principalTable: "RulingStatutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RulingSyntheses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulingSyntheses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RulingSyntheses_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sumarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Volume = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Page = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sumarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sumarios_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoteType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Pages = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GraphCommunities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ParentCommunityId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KeyFindings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityCount = table.Column<int>(type: "int", nullable: false),
                    EmbeddingConfigId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphCommunities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphCommunities_EmbeddingConfigs_EmbeddingConfigId",
                        column: x => x.EmbeddingConfigId,
                        principalTable: "EmbeddingConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GraphCommunities_GraphCommunities_ParentCommunityId",
                        column: x => x.ParentCommunityId,
                        principalTable: "GraphCommunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RulingEmbeddingStates",
                columns: table => new
                {
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmbeddingConfigId = table.Column<int>(type: "int", nullable: false),
                    EmbeddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChunkCount = table.Column<int>(type: "int", nullable: false),
                    NeedsReembedding = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulingEmbeddingStates", x => x.RulingId);
                    table.ForeignKey(
                        name: "FK_RulingEmbeddingStates_EmbeddingConfigs_EmbeddingConfigId",
                        column: x => x.EmbeddingConfigId,
                        principalTable: "EmbeddingConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RulingEmbeddingStates_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JudicialOffices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    DesignationAuthority = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JudicialOffices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JudicialOffices_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JudicialOffices_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegalRepresentations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LawyerPersonId = table.Column<int>(type: "int", nullable: false),
                    PartyPersonId = table.Column<int>(type: "int", nullable: false),
                    JudicialProceedingId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalRepresentations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalRepresentations_JudicialProceedings_JudicialProceedingId",
                        column: x => x.JudicialProceedingId,
                        principalTable: "JudicialProceedings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalRepresentations_Persons_LawyerPersonId",
                        column: x => x.LawyerPersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LegalRepresentations_Persons_PartyPersonId",
                        column: x => x.PartyPersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProceedingParties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JudicialProceedingId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProceedingParties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProceedingParties_JudicialProceedings_JudicialProceedingId",
                        column: x => x.JudicialProceedingId,
                        principalTable: "JudicialProceedings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProceedingParties_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SumarioKeywords",
                columns: table => new
                {
                    SumarioId = table.Column<int>(type: "int", nullable: false),
                    KeywordId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SumarioKeywords", x => new { x.SumarioId, x.KeywordId });
                    table.ForeignKey(
                        name: "FK_SumarioKeywords_Keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SumarioKeywords_Sumarios_SumarioId",
                        column: x => x.SumarioId,
                        principalTable: "Sumarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RulingParticipations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    VoteId = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulingParticipations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RulingParticipations_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RulingParticipations_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RulingParticipations_Votes_VoteId",
                        column: x => x.VoteId,
                        principalTable: "Votes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CommunityMemberships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommunityId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Relevance = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityMemberships_GraphCommunities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "GraphCommunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RulingStatutes_RulingId_StatuteId",
                table: "RulingStatutes",
                columns: new[] { "RulingId", "StatuteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProsecutorOpinions_PersonId",
                table: "ProsecutorOpinions",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_JudicialProceedings_CourtId",
                table: "JudicialProceedings",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_ChunkEntityMentions_EntityType_EntityId",
                table: "ChunkEntityMentions",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChunkEntityMentions_RulingId_ChunkIndex",
                table: "ChunkEntityMentions",
                columns: new[] { "RulingId", "ChunkIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_CommunityId_EntityType_EntityId",
                table: "CommunityMemberships",
                columns: new[] { "CommunityId", "EntityType", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_EntityType_EntityId",
                table: "CommunityMemberships",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingConfigs_IsActive",
                table: "EmbeddingConfigs",
                column: "IsActive",
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingConfigs_Version",
                table: "EmbeddingConfigs",
                column: "Version",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityAuditLogs_EntityType_EntityId",
                table: "EntityAuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityAuditLogs_IngestionJobId",
                table: "EntityAuditLogs",
                column: "IngestionJobId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityAuditLogs_Timestamp",
                table: "EntityAuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIdentifiers_EntityType_EntityId_SourceId_ExternalCode",
                table: "ExternalIdentifiers",
                columns: new[] { "EntityType", "EntityId", "SourceId", "ExternalCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIdentifiers_SourceId_ExternalCode",
                table: "ExternalIdentifiers",
                columns: new[] { "SourceId", "ExternalCode" });

            migrationBuilder.CreateIndex(
                name: "IX_FieldProvenance_EntityType_EntityId_FieldName_IsCurrent",
                table: "FieldProvenance",
                columns: new[] { "EntityType", "EntityId", "FieldName", "IsCurrent" },
                filter: "[IsCurrent] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FieldProvenance_IngestionJobId",
                table: "FieldProvenance",
                column: "IngestionJobId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldProvenance_SourceId",
                table: "FieldProvenance",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_GraphCommunities_EmbeddingConfigId",
                table: "GraphCommunities",
                column: "EmbeddingConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GraphCommunities_Level",
                table: "GraphCommunities",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_GraphCommunities_ParentCommunityId",
                table: "GraphCommunities",
                column: "ParentCommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_IngestionJobDetails_IngestionJobId_EntityType",
                table: "IngestionJobDetails",
                columns: new[] { "IngestionJobId", "EntityType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JudicialOffices_CourtId",
                table: "JudicialOffices",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_JudicialOffices_PersonId_CourtId_StartDate",
                table: "JudicialOffices",
                columns: new[] { "PersonId", "CourtId", "StartDate" },
                unique: true,
                filter: "[StartDate] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LegalRepresentations_JudicialProceedingId",
                table: "LegalRepresentations",
                column: "JudicialProceedingId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalRepresentations_LawyerPersonId_PartyPersonId_JudicialProceedingId",
                table: "LegalRepresentations",
                columns: new[] { "LawyerPersonId", "PartyPersonId", "JudicialProceedingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LegalRepresentations_PartyPersonId",
                table: "LegalRepresentations",
                column: "PartyPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CsjnMinistroId",
                table: "Persons",
                column: "CsjnMinistroId",
                unique: true,
                filter: "[CsjnMinistroId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_DisplayName",
                table: "Persons",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_ProceedingParties_JudicialProceedingId_PersonId_Role",
                table: "ProceedingParties",
                columns: new[] { "JudicialProceedingId", "PersonId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProceedingParties_PersonId",
                table: "ProceedingParties",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_RulingEmbeddingStates_EmbeddingConfigId",
                table: "RulingEmbeddingStates",
                column: "EmbeddingConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_RulingEmbeddingStates_NeedsReembedding",
                table: "RulingEmbeddingStates",
                column: "NeedsReembedding",
                filter: "[NeedsReembedding] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_RulingLinks_RulingId",
                table: "RulingLinks",
                column: "RulingId");

            migrationBuilder.CreateIndex(
                name: "IX_RulingParticipations_PersonId",
                table: "RulingParticipations",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_RulingParticipations_RulingId_PersonId_Role",
                table: "RulingParticipations",
                columns: new[] { "RulingId", "PersonId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RulingParticipations_VoteId",
                table: "RulingParticipations",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_RulingSourceMetadata_RulingId_SourceId_Key",
                table: "RulingSourceMetadata",
                columns: new[] { "RulingId", "SourceId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RulingSourceMetadata_SourceId",
                table: "RulingSourceMetadata",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_RulingStatuteArticles_RulingStatuteId_Article",
                table: "RulingStatuteArticles",
                columns: new[] { "RulingStatuteId", "Article" });

            migrationBuilder.CreateIndex(
                name: "IX_RulingSyntheses_RulingId",
                table: "RulingSyntheses",
                column: "RulingId");

            migrationBuilder.CreateIndex(
                name: "IX_SumarioKeywords_KeywordId",
                table: "SumarioKeywords",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_Sumarios_RulingId_ExternalId",
                table: "Sumarios",
                columns: new[] { "RulingId", "ExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_RulingId",
                table: "Votes",
                column: "RulingId");

            migrationBuilder.AddForeignKey(
                name: "FK_JudicialProceedings_Courts_CourtId",
                table: "JudicialProceedings",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProsecutorOpinions_Persons_PersonId",
                table: "ProsecutorOpinions",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JudicialProceedings_Courts_CourtId",
                table: "JudicialProceedings");

            migrationBuilder.DropForeignKey(
                name: "FK_ProsecutorOpinions_Persons_PersonId",
                table: "ProsecutorOpinions");

            migrationBuilder.DropTable(
                name: "ChunkEntityMentions");

            migrationBuilder.DropTable(
                name: "CommunityMemberships");

            migrationBuilder.DropTable(
                name: "EntityAuditLogs");

            migrationBuilder.DropTable(
                name: "ExternalIdentifiers");

            migrationBuilder.DropTable(
                name: "FieldProvenance");

            migrationBuilder.DropTable(
                name: "IngestionJobDetails");

            migrationBuilder.DropTable(
                name: "JudicialOffices");

            migrationBuilder.DropTable(
                name: "LegalRepresentations");

            migrationBuilder.DropTable(
                name: "ProceedingParties");

            migrationBuilder.DropTable(
                name: "RulingEmbeddingStates");

            migrationBuilder.DropTable(
                name: "RulingLinks");

            migrationBuilder.DropTable(
                name: "RulingParticipations");

            migrationBuilder.DropTable(
                name: "RulingSourceMetadata");

            migrationBuilder.DropTable(
                name: "RulingStatuteArticles");

            migrationBuilder.DropTable(
                name: "RulingSyntheses");

            migrationBuilder.DropTable(
                name: "SumarioKeywords");

            migrationBuilder.DropTable(
                name: "GraphCommunities");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "Sumarios");

            migrationBuilder.DropTable(
                name: "EmbeddingConfigs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RulingStatutes",
                table: "RulingStatutes");

            migrationBuilder.DropIndex(
                name: "IX_RulingStatutes_RulingId_StatuteId",
                table: "RulingStatutes");

            migrationBuilder.DropIndex(
                name: "IX_ProsecutorOpinions_PersonId",
                table: "ProsecutorOpinions");

            migrationBuilder.DropIndex(
                name: "IX_JudicialProceedings_CourtId",
                table: "JudicialProceedings");

            migrationBuilder.DropColumn(
                name: "OfficialUrl",
                table: "Statutes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RulingStatutes");

            migrationBuilder.DropColumn(
                name: "DoctrinaLegal",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "RatioDecidendi",
                table: "Rulings");

            migrationBuilder.DropColumn(
                name: "DocumentBlobPath",
                table: "ProsecutorOpinions");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "ProsecutorOpinions");

            migrationBuilder.DropColumn(
                name: "CourtId",
                table: "JudicialProceedings");

            migrationBuilder.DropColumn(
                name: "LegalBranch",
                table: "JudicialProceedings");

            migrationBuilder.DropColumn(
                name: "ProcessSubtype",
                table: "JudicialProceedings");

            migrationBuilder.DropColumn(
                name: "ProcessType",
                table: "JudicialProceedings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "JudicialProceedings");

            migrationBuilder.AlterColumn<string>(
                name: "Articles",
                table: "RulingStatutes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RulingStatutes",
                table: "RulingStatutes",
                columns: new[] { "RulingId", "StatuteId" });

            migrationBuilder.CreateTable(
                name: "Judges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentCourtId = table.Column<int>(type: "int", nullable: true),
                    CsjnMinistroId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Unknown")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Judges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Judges_Courts_CurrentCourtId",
                        column: x => x.CurrentCourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "JudgeTenures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    JudgeId = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "RulingJudges",
                columns: table => new
                {
                    RulingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JudgeId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ParticipationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulingJudges", x => new { x.RulingId, x.JudgeId });
                    table.ForeignKey(
                        name: "FK_RulingJudges_Judges_JudgeId",
                        column: x => x.JudgeId,
                        principalTable: "Judges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RulingJudges_Rulings_RulingId",
                        column: x => x.RulingId,
                        principalTable: "Rulings",
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
                name: "IX_Judges_CurrentCourtId",
                table: "Judges",
                column: "CurrentCourtId");

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

            migrationBuilder.CreateIndex(
                name: "IX_RulingJudges_JudgeId",
                table: "RulingJudges",
                column: "JudgeId");
        }
    }
}
