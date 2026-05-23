/*
 * KB Model V2 Migration Script
 * ============================================================
 * Transforms the existing schema into the KB Model V2 design.
 *
 * Sections:
 *   1. Rename tables and columns (Judges→Persons, RulingJudges→RulingParticipations, JudgeTenures→JudicialOffices)
 *   2. Modify existing tables (RulingStatutes PK change)
 *   3. Create new tables (doctrinal, contextual, identifiers, GraphRAG, retrieval, audit)
 *   4. Migrate data
 *   5. Drop deprecated columns/constraints
 *
 * Prerequisites: backup the database before running.
 * Target: Azure SQL Database (SQL Server 2019+)
 */

SET XACT_ABORT ON;
BEGIN TRANSACTION;

PRINT '=== KB Model V2 Migration START ===';

-- ============================================================
-- SECTION 1: Rename tables and add new columns
-- ============================================================

-- 1a. Judges → Persons
PRINT '1a. Renaming Judges → Persons...';

-- Drop FK from Judges to Courts (CurrentCourtId) — replaced by JudicialOffices
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Judges_Courts_CurrentCourtId')
    ALTER TABLE [Judges] DROP CONSTRAINT [FK_Judges_Courts_CurrentCourtId];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Judges_CurrentCourtId' AND object_id = OBJECT_ID('Judges'))
    DROP INDEX [IX_Judges_CurrentCourtId] ON [Judges];

-- Add new columns before rename
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Judges') AND name = 'DisplayName')
    ALTER TABLE [Judges] ADD [DisplayName] NVARCHAR(400) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Judges') AND name = 'PersonType')
    ALTER TABLE [Judges] ADD [PersonType] NVARCHAR(20) NOT NULL CONSTRAINT DF_Judges_PersonType DEFAULT 'Physical';

-- Populate DisplayName from existing data
UPDATE [Judges]
SET [DisplayName] = CASE
    WHEN [FirstName] IS NOT NULL AND [LastName] IS NOT NULL AND LEN([FirstName]) > 0
        THEN [LastName] + ', ' + [FirstName]
    WHEN [LastName] IS NOT NULL AND LEN([LastName]) > 0
        THEN [LastName]
    WHEN [FirstName] IS NOT NULL AND LEN([FirstName]) > 0
        THEN [FirstName]
    ELSE 'Unknown'
END
WHERE [DisplayName] IS NULL;

ALTER TABLE [Judges] ALTER COLUMN [DisplayName] NVARCHAR(400) NOT NULL;
ALTER TABLE [Judges] ALTER COLUMN [FirstName] NVARCHAR(200) NULL;
ALTER TABLE [Judges] ALTER COLUMN [LastName] NVARCHAR(200) NULL;

-- Drop Role column (replaced by JudicialPosition in JudicialOffice)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Judges') AND name = 'Role')
    ALTER TABLE [Judges] DROP COLUMN [Role];

-- Drop CurrentCourtId column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Judges') AND name = 'CurrentCourtId')
    ALTER TABLE [Judges] DROP COLUMN [CurrentCourtId];

-- Rename table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Judges') AND NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Persons')
    EXEC sp_rename 'Judges', 'Persons';

-- Rename PK
IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_Judges')
    EXEC sp_rename 'PK_Judges', 'PK_Persons', 'OBJECT';

-- Drop default constraint for PersonType
IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_Judges_PersonType')
    ALTER TABLE [Persons] DROP CONSTRAINT [DF_Judges_PersonType];

ALTER TABLE [Persons] ADD CONSTRAINT [DF_Persons_PersonType] DEFAULT 'Physical' FOR [PersonType];

-- Add index on DisplayName
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Persons_DisplayName' AND object_id = OBJECT_ID('Persons'))
    CREATE NONCLUSTERED INDEX [IX_Persons_DisplayName] ON [Persons] ([DisplayName]);

-- Rename CsjnMinistroId unique index
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Judges_CsjnMinistroId' AND object_id = OBJECT_ID('Persons'))
    EXEC sp_rename 'Persons.IX_Judges_CsjnMinistroId', 'IX_Persons_CsjnMinistroId', 'INDEX';

PRINT '  Persons table ready.';


-- 1b. RulingJudges → RulingParticipations
PRINT '1b. Renaming RulingJudges → RulingParticipations...';

-- Drop existing FKs
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_RulingJudges_Judges_JudgeId')
    ALTER TABLE [RulingJudges] DROP CONSTRAINT [FK_RulingJudges_Judges_JudgeId];
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_RulingJudges_Rulings_RulingId')
    ALTER TABLE [RulingJudges] DROP CONSTRAINT [FK_RulingJudges_Rulings_RulingId];

-- Drop old PK (composite)
IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_RulingJudges')
    ALTER TABLE [RulingJudges] DROP CONSTRAINT [PK_RulingJudges];

-- Drop old index
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RulingJudges_JudgeId' AND object_id = OBJECT_ID('RulingJudges'))
    DROP INDEX [IX_RulingJudges_JudgeId] ON [RulingJudges];

-- Add Id column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('RulingJudges') AND name = 'Id')
    ALTER TABLE [RulingJudges] ADD [Id] INT IDENTITY(1,1) NOT NULL;

-- Add VoteId column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('RulingJudges') AND name = 'VoteId')
    ALTER TABLE [RulingJudges] ADD [VoteId] INT NULL;

-- Rename JudgeId → PersonId
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('RulingJudges') AND name = 'JudgeId')
    EXEC sp_rename 'RulingJudges.JudgeId', 'PersonId', 'COLUMN';

-- Rename ParticipationType → Role
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('RulingJudges') AND name = 'ParticipationType')
    EXEC sp_rename 'RulingJudges.ParticipationType', 'Role', 'COLUMN';

-- Rename table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RulingJudges') AND NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RulingParticipations')
    EXEC sp_rename 'RulingJudges', 'RulingParticipations';

-- Add new PK on Id
ALTER TABLE [RulingParticipations] ADD CONSTRAINT [PK_RulingParticipations] PRIMARY KEY ([Id]);

-- Re-create FKs
ALTER TABLE [RulingParticipations] ADD CONSTRAINT [FK_RulingParticipations_Rulings_RulingId]
    FOREIGN KEY ([RulingId]) REFERENCES [Rulings] ([Id]) ON DELETE CASCADE;
ALTER TABLE [RulingParticipations] ADD CONSTRAINT [FK_RulingParticipations_Persons_PersonId]
    FOREIGN KEY ([PersonId]) REFERENCES [Persons] ([Id]) ON DELETE NO ACTION;

-- Unique index on (RulingId, PersonId, Role)
CREATE UNIQUE NONCLUSTERED INDEX [IX_RulingParticipations_RulingId_PersonId_Role]
    ON [RulingParticipations] ([RulingId], [PersonId], [Role]);

PRINT '  RulingParticipations table ready.';


-- 1c. JudgeTenures → JudicialOffices
PRINT '1c. Renaming JudgeTenures → JudicialOffices...';

-- Drop existing FKs
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_JudgeTenures_Judges_JudgeId')
    ALTER TABLE [JudgeTenures] DROP CONSTRAINT [FK_JudgeTenures_Judges_JudgeId];
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_JudgeTenures_Courts_CourtId')
    ALTER TABLE [JudgeTenures] DROP CONSTRAINT [FK_JudgeTenures_Courts_CourtId];

-- Drop old indexes
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_JudgeTenures_CourtId' AND object_id = OBJECT_ID('JudgeTenures'))
    DROP INDEX [IX_JudgeTenures_CourtId] ON [JudgeTenures];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_JudgeTenures_JudgeId_CourtId_StartDate' AND object_id = OBJECT_ID('JudgeTenures'))
    DROP INDEX [IX_JudgeTenures_JudgeId_CourtId_StartDate] ON [JudgeTenures];

-- Add DesignationAuthority
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('JudgeTenures') AND name = 'DesignationAuthority')
    ALTER TABLE [JudgeTenures] ADD [DesignationAuthority] NVARCHAR(300) NULL;

-- Rename columns
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('JudgeTenures') AND name = 'JudgeId')
    EXEC sp_rename 'JudgeTenures.JudgeId', 'PersonId', 'COLUMN';
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('JudgeTenures') AND name = 'Role')
    EXEC sp_rename 'JudgeTenures.Role', 'Position', 'COLUMN';

-- Rename PK
IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_JudgeTenures')
    EXEC sp_rename 'PK_JudgeTenures', 'PK_JudicialOffices', 'OBJECT';

-- Rename table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'JudgeTenures') AND NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'JudicialOffices')
    EXEC sp_rename 'JudgeTenures', 'JudicialOffices';

-- Re-create FKs
ALTER TABLE [JudicialOffices] ADD CONSTRAINT [FK_JudicialOffices_Persons_PersonId]
    FOREIGN KEY ([PersonId]) REFERENCES [Persons] ([Id]) ON DELETE CASCADE;
ALTER TABLE [JudicialOffices] ADD CONSTRAINT [FK_JudicialOffices_Courts_CourtId]
    FOREIGN KEY ([CourtId]) REFERENCES [Courts] ([Id]) ON DELETE NO ACTION;

-- Re-create indexes
CREATE NONCLUSTERED INDEX [IX_JudicialOffices_CourtId] ON [JudicialOffices] ([CourtId]);
CREATE UNIQUE NONCLUSTERED INDEX [IX_JudicialOffices_PersonId_CourtId_StartDate]
    ON [JudicialOffices] ([PersonId], [CourtId], [StartDate])
    WHERE [StartDate] IS NOT NULL;

PRINT '  JudicialOffices table ready.';


-- ============================================================
-- SECTION 2: Modify existing tables
-- ============================================================

-- 2a. RulingStatutes: composite PK → Id PK
PRINT '2a. Migrating RulingStatutes PK...';

-- Drop FKs referencing RulingStatutes
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_RulingStatutes_Rulings_RulingId')
    ALTER TABLE [RulingStatutes] DROP CONSTRAINT [FK_RulingStatutes_Rulings_RulingId];
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_RulingStatutes_Statutes_StatuteId')
    ALTER TABLE [RulingStatutes] DROP CONSTRAINT [FK_RulingStatutes_Statutes_StatuteId];

-- Drop composite PK
IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_RulingStatutes')
    ALTER TABLE [RulingStatutes] DROP CONSTRAINT [PK_RulingStatutes];

-- Add Id column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('RulingStatutes') AND name = 'Id')
    ALTER TABLE [RulingStatutes] ADD [Id] INT IDENTITY(1,1) NOT NULL;

-- New PK on Id
ALTER TABLE [RulingStatutes] ADD CONSTRAINT [PK_RulingStatutes] PRIMARY KEY ([Id]);

-- Unique index on original composite key
CREATE UNIQUE NONCLUSTERED INDEX [IX_RulingStatutes_RulingId_StatuteId]
    ON [RulingStatutes] ([RulingId], [StatuteId]);

-- Widen Articles from 200 to 500
ALTER TABLE [RulingStatutes] ALTER COLUMN [Articles] NVARCHAR(500) NULL;

-- Re-create FKs
ALTER TABLE [RulingStatutes] ADD CONSTRAINT [FK_RulingStatutes_Rulings_RulingId]
    FOREIGN KEY ([RulingId]) REFERENCES [Rulings] ([Id]) ON DELETE CASCADE;
ALTER TABLE [RulingStatutes] ADD CONSTRAINT [FK_RulingStatutes_Statutes_StatuteId]
    FOREIGN KEY ([StatuteId]) REFERENCES [Statutes] ([Id]) ON DELETE NO ACTION;

PRINT '  RulingStatutes PK migrated.';


-- ============================================================
-- SECTION 3: Create new tables
-- ============================================================

-- 3a. Votes
PRINT '3a. Creating Votes...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Votes')
BEGIN
    CREATE TABLE [Votes] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [RulingId]  UNIQUEIDENTIFIER NOT NULL,
        [VoteType]  NVARCHAR(20) NOT NULL,
        [Pages]     NVARCHAR(100) NULL,
        [Summary]   NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_Votes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Votes_Rulings_RulingId] FOREIGN KEY ([RulingId])
            REFERENCES [Rulings] ([Id]) ON DELETE CASCADE
    );
END

-- FK from RulingParticipations.VoteId → Votes.Id
ALTER TABLE [RulingParticipations] ADD CONSTRAINT [FK_RulingParticipations_Votes_VoteId]
    FOREIGN KEY ([VoteId]) REFERENCES [Votes] ([Id]) ON DELETE SET NULL;


-- 3b. Sumarios
PRINT '3b. Creating Sumarios...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Sumarios')
BEGIN
    CREATE TABLE [Sumarios] (
        [Id]         INT IDENTITY(1,1) NOT NULL,
        [RulingId]   UNIQUEIDENTIFIER NOT NULL,
        [ExternalId] INT NULL,
        [Text]       NVARCHAR(MAX) NOT NULL,
        [Volume]     NVARCHAR(50) NULL,
        [Page]       NVARCHAR(50) NULL,
        [SortOrder]  INT NOT NULL DEFAULT 0,
        [CreatedAt]  DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Sumarios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sumarios_Rulings_RulingId] FOREIGN KEY ([RulingId])
            REFERENCES [Rulings] ([Id]) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_Sumarios_RulingId_ExternalId] ON [Sumarios] ([RulingId], [ExternalId]);
END


-- 3c. SumarioKeywords
PRINT '3c. Creating SumarioKeywords...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SumarioKeywords')
BEGIN
    CREATE TABLE [SumarioKeywords] (
        [SumarioId] INT NOT NULL,
        [KeywordId] INT NOT NULL,
        [SortOrder] INT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_SumarioKeywords] PRIMARY KEY ([SumarioId], [KeywordId]),
        CONSTRAINT [FK_SumarioKeywords_Sumarios] FOREIGN KEY ([SumarioId])
            REFERENCES [Sumarios] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_SumarioKeywords_Keywords] FOREIGN KEY ([KeywordId])
            REFERENCES [Keywords] ([Id]) ON DELETE NO ACTION
    );
END


-- 3d. RulingSyntheses
PRINT '3d. Creating RulingSyntheses...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RulingSyntheses')
BEGIN
    CREATE TABLE [RulingSyntheses] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [RulingId]  UNIQUEIDENTIFIER NOT NULL,
        [Text]      NVARCHAR(MAX) NOT NULL,
        [SortOrder] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_RulingSyntheses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RulingSyntheses_Rulings_RulingId] FOREIGN KEY ([RulingId])
            REFERENCES [Rulings] ([Id]) ON DELETE CASCADE
    );
END


-- 3e. RulingLinks
PRINT '3e. Creating RulingLinks...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RulingLinks')
BEGIN
    CREATE TABLE [RulingLinks] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [RulingId]  UNIQUEIDENTIFIER NOT NULL,
        [Url]       NVARCHAR(2000) NOT NULL,
        [Title]     NVARCHAR(500) NULL,
        [LinkType]  NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_RulingLinks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RulingLinks_Rulings_RulingId] FOREIGN KEY ([RulingId])
            REFERENCES [Rulings] ([Id]) ON DELETE CASCADE
    );
END


-- 3f. RulingStatuteArticles
PRINT '3f. Creating RulingStatuteArticles...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RulingStatuteArticles')
BEGIN
    CREATE TABLE [RulingStatuteArticles] (
        [Id]              INT IDENTITY(1,1) NOT NULL,
        [RulingStatuteId] INT NOT NULL,
        [Article]         NVARCHAR(50) NOT NULL,
        [Subsection]      NVARCHAR(100) NULL,
        CONSTRAINT [PK_RulingStatuteArticles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RulingStatuteArticles_RulingStatutes] FOREIGN KEY ([RulingStatuteId])
            REFERENCES [RulingStatutes] ([Id]) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_RulingStatuteArticles_RulingStatuteId_Article]
        ON [RulingStatuteArticles] ([RulingStatuteId], [Article]);
END


-- 3g. ProceedingParties
PRINT '3g. Creating ProceedingParties...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ProceedingParties')
BEGIN
    CREATE TABLE [ProceedingParties] (
        [Id]                     INT IDENTITY(1,1) NOT NULL,
        [JudicialProceedingId]   INT NOT NULL,
        [PersonId]               INT NOT NULL,
        [Role]                   NVARCHAR(20) NOT NULL,
        [CreatedAt]              DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_ProceedingParties] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProceedingParties_JudicialProceedings] FOREIGN KEY ([JudicialProceedingId])
            REFERENCES [JudicialProceedings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProceedingParties_Persons] FOREIGN KEY ([PersonId])
            REFERENCES [Persons] ([Id]) ON DELETE NO ACTION
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_ProceedingParties_ProceedingId_PersonId_Role]
        ON [ProceedingParties] ([JudicialProceedingId], [PersonId], [Role]);
END


-- 3h. LegalRepresentations
PRINT '3h. Creating LegalRepresentations...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LegalRepresentations')
BEGIN
    CREATE TABLE [LegalRepresentations] (
        [Id]                     INT IDENTITY(1,1) NOT NULL,
        [LawyerPersonId]         INT NOT NULL,
        [PartyPersonId]          INT NOT NULL,
        [JudicialProceedingId]   INT NOT NULL,
        [CreatedAt]              DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_LegalRepresentations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LegalRepresentations_LawyerPerson] FOREIGN KEY ([LawyerPersonId])
            REFERENCES [Persons] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LegalRepresentations_PartyPerson] FOREIGN KEY ([PartyPersonId])
            REFERENCES [Persons] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LegalRepresentations_JudicialProceedings] FOREIGN KEY ([JudicialProceedingId])
            REFERENCES [JudicialProceedings] ([Id]) ON DELETE CASCADE
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_LegalRepresentations_Lawyer_Party_Proceeding]
        ON [LegalRepresentations] ([LawyerPersonId], [PartyPersonId], [JudicialProceedingId]);
END


-- 3i. ExternalIdentifiers
PRINT '3i. Creating ExternalIdentifiers...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ExternalIdentifiers')
BEGIN
    CREATE TABLE [ExternalIdentifiers] (
        [Id]           INT IDENTITY(1,1) NOT NULL,
        [EntityType]   NVARCHAR(50) NOT NULL,
        [EntityId]     NVARCHAR(50) NOT NULL,
        [SourceId]     INT NOT NULL,
        [ExternalCode] NVARCHAR(200) NOT NULL,
        [Label]        NVARCHAR(200) NULL,
        [CreatedAt]    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_ExternalIdentifiers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExternalIdentifiers_Sources] FOREIGN KEY ([SourceId])
            REFERENCES [Sources] ([Id]) ON DELETE NO ACTION
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_ExternalIdentifiers_Entity_Source_Code]
        ON [ExternalIdentifiers] ([EntityType], [EntityId], [SourceId], [ExternalCode]);
    CREATE NONCLUSTERED INDEX [IX_ExternalIdentifiers_Source_Code]
        ON [ExternalIdentifiers] ([SourceId], [ExternalCode]);
END


-- 3j. RulingSourceMetadata
PRINT '3j. Creating RulingSourceMetadata...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RulingSourceMetadata')
BEGIN
    CREATE TABLE [RulingSourceMetadata] (
        [Id]        INT IDENTITY(1,1) NOT NULL,
        [RulingId]  UNIQUEIDENTIFIER NOT NULL,
        [SourceId]  INT NOT NULL,
        [Key]       NVARCHAR(100) NOT NULL,
        [Value]     NVARCHAR(4000) NULL,
        CONSTRAINT [PK_RulingSourceMetadata] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RulingSourceMetadata_Rulings] FOREIGN KEY ([RulingId])
            REFERENCES [Rulings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RulingSourceMetadata_Sources] FOREIGN KEY ([SourceId])
            REFERENCES [Sources] ([Id]) ON DELETE NO ACTION
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_RulingSourceMetadata_Ruling_Source_Key]
        ON [RulingSourceMetadata] ([RulingId], [SourceId], [Key]);
END


-- 3k. EmbeddingConfigs (created before GraphCommunities which references it)
PRINT '3k. Creating EmbeddingConfigs...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EmbeddingConfigs')
BEGIN
    CREATE TABLE [EmbeddingConfigs] (
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        [Version]                  NVARCHAR(50) NOT NULL,
        [EmbeddingModel]           NVARCHAR(100) NOT NULL,
        [EmbeddingDimensions]      INT NOT NULL,
        [ContextualizationMethod]  NVARCHAR(100) NOT NULL,
        [ChunkingStrategy]         NVARCHAR(100) NOT NULL,
        [ChunkSize]                INT NOT NULL,
        [ChunkOverlap]             INT NOT NULL,
        [IsActive]                 BIT NOT NULL DEFAULT 0,
        [CreatedAt]                DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [Notes]                    NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_EmbeddingConfigs] PRIMARY KEY ([Id])
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_EmbeddingConfigs_Version]
        ON [EmbeddingConfigs] ([Version]);
    CREATE NONCLUSTERED INDEX [IX_EmbeddingConfigs_IsActive]
        ON [EmbeddingConfigs] ([IsActive]) WHERE [IsActive] = 1;
END


-- 3l. GraphCommunities
PRINT '3l. Creating GraphCommunities...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GraphCommunities')
BEGIN
    CREATE TABLE [GraphCommunities] (
        [Id]                INT IDENTITY(1,1) NOT NULL,
        [Level]             INT NOT NULL,
        [ParentCommunityId] INT NULL,
        [Title]             NVARCHAR(500) NOT NULL,
        [Summary]           NVARCHAR(MAX) NOT NULL,
        [KeyFindings]       NVARCHAR(MAX) NULL,
        [EntityCount]       INT NOT NULL DEFAULT 0,
        [EmbeddingConfigId] INT NULL,
        [CreatedAt]         DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_GraphCommunities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GraphCommunities_Parent] FOREIGN KEY ([ParentCommunityId])
            REFERENCES [GraphCommunities] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GraphCommunities_EmbeddingConfig] FOREIGN KEY ([EmbeddingConfigId])
            REFERENCES [EmbeddingConfigs] ([Id]) ON DELETE SET NULL
    );
    CREATE NONCLUSTERED INDEX [IX_GraphCommunities_Level] ON [GraphCommunities] ([Level]);
END


-- 3m. CommunityMemberships
PRINT '3m. Creating CommunityMemberships...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CommunityMemberships')
BEGIN
    CREATE TABLE [CommunityMemberships] (
        [Id]          INT IDENTITY(1,1) NOT NULL,
        [CommunityId] INT NOT NULL,
        [EntityType]  NVARCHAR(50) NOT NULL,
        [EntityId]    NVARCHAR(50) NOT NULL,
        [Relevance]   REAL NULL,
        CONSTRAINT [PK_CommunityMemberships] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CommunityMemberships_Community] FOREIGN KEY ([CommunityId])
            REFERENCES [GraphCommunities] ([Id]) ON DELETE CASCADE
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_CommunityMemberships_Community_Entity]
        ON [CommunityMemberships] ([CommunityId], [EntityType], [EntityId]);
    CREATE NONCLUSTERED INDEX [IX_CommunityMemberships_Entity]
        ON [CommunityMemberships] ([EntityType], [EntityId]);
END


-- 3n. ChunkEntityMentions
PRINT '3n. Creating ChunkEntityMentions...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ChunkEntityMentions')
BEGIN
    CREATE TABLE [ChunkEntityMentions] (
        [Id]          BIGINT IDENTITY(1,1) NOT NULL,
        [RulingId]    UNIQUEIDENTIFIER NOT NULL,
        [ChunkIndex]  INT NOT NULL,
        [EntityType]  NVARCHAR(50) NOT NULL,
        [EntityId]    NVARCHAR(50) NOT NULL,
        [MentionType] NVARCHAR(20) NOT NULL,
        [Confidence]  REAL NULL,
        CONSTRAINT [PK_ChunkEntityMentions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChunkEntityMentions_Rulings] FOREIGN KEY ([RulingId])
            REFERENCES [Rulings] ([Id]) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_ChunkEntityMentions_Ruling_Chunk]
        ON [ChunkEntityMentions] ([RulingId], [ChunkIndex]);
    CREATE NONCLUSTERED INDEX [IX_ChunkEntityMentions_Entity]
        ON [ChunkEntityMentions] ([EntityType], [EntityId]);
END


-- 3o. RulingEmbeddingStates
PRINT '3o. Creating RulingEmbeddingStates...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RulingEmbeddingStates')
BEGIN
    CREATE TABLE [RulingEmbeddingStates] (
        [RulingId]           UNIQUEIDENTIFIER NOT NULL,
        [EmbeddingConfigId]  INT NOT NULL,
        [EmbeddedAt]         DATETIME2 NOT NULL,
        [ChunkCount]         INT NOT NULL DEFAULT 0,
        [NeedsReembedding]   BIT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_RulingEmbeddingStates] PRIMARY KEY ([RulingId]),
        CONSTRAINT [FK_RulingEmbeddingStates_Rulings] FOREIGN KEY ([RulingId])
            REFERENCES [Rulings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RulingEmbeddingStates_EmbeddingConfig] FOREIGN KEY ([EmbeddingConfigId])
            REFERENCES [EmbeddingConfigs] ([Id]) ON DELETE NO ACTION
    );
    CREATE NONCLUSTERED INDEX [IX_RulingEmbeddingStates_NeedsReembedding]
        ON [RulingEmbeddingStates] ([NeedsReembedding]) WHERE [NeedsReembedding] = 1;
END


-- 3p. FieldProvenance
PRINT '3p. Creating FieldProvenance...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'FieldProvenance')
BEGIN
    CREATE TABLE [FieldProvenance] (
        [Id]              BIGINT IDENTITY(1,1) NOT NULL,
        [EntityType]      NVARCHAR(50) NOT NULL,
        [EntityId]        NVARCHAR(50) NOT NULL,
        [FieldName]       NVARCHAR(100) NOT NULL,
        [Value]           NVARCHAR(4000) NULL,
        [PreviousValue]   NVARCHAR(4000) NULL,
        [SourceId]        INT NOT NULL,
        [SourceEndpoint]  NVARCHAR(100) NULL,
        [SourceField]     NVARCHAR(100) NULL,
        [InferenceMethod] NVARCHAR(20) NOT NULL,
        [AiModel]         NVARCHAR(100) NULL,
        [AiPrompt]        NVARCHAR(200) NULL,
        [AiConfidence]    REAL NULL,
        [IngestionJobId]  UNIQUEIDENTIFIER NOT NULL,
        [ChangeType]      NVARCHAR(10) NOT NULL,
        [RecordedAt]      DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsCurrent]       BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_FieldProvenance] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FieldProvenance_Sources] FOREIGN KEY ([SourceId])
            REFERENCES [Sources] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FieldProvenance_IngestionJobs] FOREIGN KEY ([IngestionJobId])
            REFERENCES [IngestionJobs] ([Id]) ON DELETE NO ACTION
    );
    CREATE NONCLUSTERED INDEX [IX_FieldProvenance_Entity_Field_Current]
        ON [FieldProvenance] ([EntityType], [EntityId], [FieldName], [IsCurrent])
        WHERE [IsCurrent] = 1;
    CREATE NONCLUSTERED INDEX [IX_FieldProvenance_IngestionJobId]
        ON [FieldProvenance] ([IngestionJobId]);
END


-- 3q. EntityAuditLogs
PRINT '3q. Creating EntityAuditLogs...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EntityAuditLogs')
BEGIN
    CREATE TABLE [EntityAuditLogs] (
        [Id]              BIGINT IDENTITY(1,1) NOT NULL,
        [EntityType]      NVARCHAR(50) NOT NULL,
        [EntityId]        NVARCHAR(50) NOT NULL,
        [Operation]       NVARCHAR(20) NOT NULL,
        [IngestionJobId]  UNIQUEIDENTIFIER NULL,
        [PerformedBy]     NVARCHAR(200) NOT NULL,
        [ChangeSummary]   NVARCHAR(2000) NULL,
        [FieldsChanged]   NVARCHAR(MAX) NULL,
        [Timestamp]       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_EntityAuditLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EntityAuditLogs_IngestionJobs] FOREIGN KEY ([IngestionJobId])
            REFERENCES [IngestionJobs] ([Id]) ON DELETE SET NULL
    );
    CREATE NONCLUSTERED INDEX [IX_EntityAuditLogs_Entity]
        ON [EntityAuditLogs] ([EntityType], [EntityId]);
    CREATE NONCLUSTERED INDEX [IX_EntityAuditLogs_IngestionJobId]
        ON [EntityAuditLogs] ([IngestionJobId]);
    CREATE NONCLUSTERED INDEX [IX_EntityAuditLogs_Timestamp]
        ON [EntityAuditLogs] ([Timestamp]);
END


-- 3r. IngestionJobDetails
PRINT '3r. Creating IngestionJobDetails...';
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'IngestionJobDetails')
BEGIN
    CREATE TABLE [IngestionJobDetails] (
        [Id]              INT IDENTITY(1,1) NOT NULL,
        [IngestionJobId]  UNIQUEIDENTIFIER NOT NULL,
        [EntityType]      NVARCHAR(50) NOT NULL,
        [EntitiesCreated] INT NOT NULL DEFAULT 0,
        [EntitiesUpdated] INT NOT NULL DEFAULT 0,
        [EntitiesDeleted] INT NOT NULL DEFAULT 0,
        [FieldsUpdated]   INT NOT NULL DEFAULT 0,
        [Timestamp]       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_IngestionJobDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_IngestionJobDetails_IngestionJobs] FOREIGN KEY ([IngestionJobId])
            REFERENCES [IngestionJobs] ([Id]) ON DELETE CASCADE
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_IngestionJobDetails_Job_EntityType]
        ON [IngestionJobDetails] ([IngestionJobId], [EntityType]);
END


-- ============================================================
-- SECTION 4: Enable SQL Server temporal tables on domain tables
-- ============================================================
-- Temporal tables automatically track full row history.
-- Combined with FieldProvenance (field-level) gives complete audit trail.
-- Requires Azure SQL or SQL Server 2016+.

PRINT '4. Enabling temporal tables on domain tables...';

-- 4a. Rulings
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Rulings') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [Rulings]
    ADD [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL
            CONSTRAINT DF_Rulings_SysStart DEFAULT GETUTCDATE(),
        [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL
            CONSTRAINT DF_Rulings_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    ALTER TABLE [Rulings] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[Rulings_History]));
    PRINT '  Rulings temporal table enabled.';
END

-- 4b. Persons
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Persons') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [Persons]
    ADD [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL
            CONSTRAINT DF_Persons_SysStart DEFAULT GETUTCDATE(),
        [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL
            CONSTRAINT DF_Persons_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    ALTER TABLE [Persons] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[Persons_History]));
    PRINT '  Persons temporal table enabled.';
END

-- 4c. Courts
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Courts') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [Courts]
    ADD [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL
            CONSTRAINT DF_Courts_SysStart DEFAULT GETUTCDATE(),
        [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL
            CONSTRAINT DF_Courts_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    ALTER TABLE [Courts] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[Courts_History]));
    PRINT '  Courts temporal table enabled.';
END

-- 4d. Statutes
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Statutes') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [Statutes]
    ADD [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL
            CONSTRAINT DF_Statutes_SysStart DEFAULT GETUTCDATE(),
        [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL
            CONSTRAINT DF_Statutes_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    ALTER TABLE [Statutes] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[Statutes_History]));
    PRINT '  Statutes temporal table enabled.';
END

-- 4e. Keywords
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Keywords') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [Keywords]
    ADD [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL
            CONSTRAINT DF_Keywords_SysStart DEFAULT GETUTCDATE(),
        [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL
            CONSTRAINT DF_Keywords_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    ALTER TABLE [Keywords] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[Keywords_History]));
    PRINT '  Keywords temporal table enabled.';
END

-- 4f. Citations
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Citations') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [Citations]
    ADD [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL
            CONSTRAINT DF_Citations_SysStart DEFAULT GETUTCDATE(),
        [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL
            CONSTRAINT DF_Citations_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    ALTER TABLE [Citations] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[Citations_History]));
    PRINT '  Citations temporal table enabled.';
END

-- 4g. JudicialProceedings
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('JudicialProceedings') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [JudicialProceedings]
    ADD [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL
            CONSTRAINT DF_JudicialProceedings_SysStart DEFAULT GETUTCDATE(),
        [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL
            CONSTRAINT DF_JudicialProceedings_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    ALTER TABLE [JudicialProceedings] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[JudicialProceedings_History]));
    PRINT '  JudicialProceedings temporal table enabled.';
END

-- 4h. ProsecutorOpinions
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('ProsecutorOpinions') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE [ProsecutorOpinions]
    ADD [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL
            CONSTRAINT DF_ProsecutorOpinions_SysStart DEFAULT GETUTCDATE(),
        [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL
            CONSTRAINT DF_ProsecutorOpinions_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
    ALTER TABLE [ProsecutorOpinions] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ProsecutorOpinions_History]));
    PRINT '  ProsecutorOpinions temporal table enabled.';
END

PRINT '  Temporal tables enabled on 8 domain tables.';


-- ============================================================
-- SECTION 5: Update EF Migrations history
-- ============================================================

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('20260428190000_KbModelV2Migration', '8.0.0');


PRINT '=== KB Model V2 Migration COMPLETE ===';

COMMIT TRANSACTION;
GO
