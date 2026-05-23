SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Rulings_CaseNumber')
    CREATE NONCLUSTERED INDEX IX_Rulings_CaseNumber ON Rulings(CaseNumber) WHERE CaseNumber IS NOT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Citations_Pending_ExternalAlias')
    CREATE NONCLUSTERED INDEX IX_Citations_Pending_ExternalAlias ON Citations(ExternalAlias) INCLUDE(SourceRulingId) WHERE TargetRulingId IS NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Rulings_AnalysisId')
    CREATE NONCLUSTERED INDEX IX_Rulings_AnalysisId ON Rulings(AnalysisId) WHERE AnalysisId IS NOT NULL;

PRINT 'Indexes created successfully';
