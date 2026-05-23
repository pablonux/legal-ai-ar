using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for Legal AI AR. Maps to Azure SQL Database.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Core domain
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<Ruling> Rulings => Set<Ruling>();
    public DbSet<JudicialProceeding> JudicialProceedings => Set<JudicialProceeding>();
    public DbSet<Statute> Statutes => Set<Statute>();
    public DbSet<Keyword> Keywords => Set<Keyword>();
    public DbSet<ThesaurusTerm> ThesaurusTerms => Set<ThesaurusTerm>();

    // Doctrinal content
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<Sumario> Sumarios => Set<Sumario>();
    public DbSet<RulingSynthesis> RulingSyntheses => Set<RulingSynthesis>();
    public DbSet<RulingLink> RulingLinks => Set<RulingLink>();

    // Contextual roles
    public DbSet<JudicialOffice> JudicialOffices => Set<JudicialOffice>();
    public DbSet<RulingParticipation> RulingParticipations => Set<RulingParticipation>();
    public DbSet<ProceedingParty> ProceedingParties => Set<ProceedingParty>();
    public DbSet<LegalRepresentation> LegalRepresentations => Set<LegalRepresentation>();
    public DbSet<ProsecutorOpinion> ProsecutorOpinions => Set<ProsecutorOpinion>();

    // Entity relationships
    public DbSet<Citation> Citations => Set<Citation>();
    public DbSet<RulingStatute> RulingStatutes => Set<RulingStatute>();
    public DbSet<RulingStatuteArticle> RulingStatuteArticles => Set<RulingStatuteArticle>();
    public DbSet<RulingKeyword> RulingKeywords => Set<RulingKeyword>();
    public DbSet<SumarioKeyword> SumarioKeywords => Set<SumarioKeyword>();
    public DbSet<StateOrgan> StateOrgans => Set<StateOrgan>();
    public DbSet<LegalDoctrine> LegalDoctrines => Set<LegalDoctrine>();
    public DbSet<ProceduralRemedy> ProceduralRemedies => Set<ProceduralRemedy>();
    public DbSet<NormRelation> NormRelations => Set<NormRelation>();
    public DbSet<ThesaurusRelation> ThesaurusRelations => Set<ThesaurusRelation>();

    // Identifiers and metadata
    public DbSet<ExternalIdentifier> ExternalIdentifiers => Set<ExternalIdentifier>();
    public DbSet<RulingSourceMetadata> RulingSourceMetadata => Set<RulingSourceMetadata>();

    // GraphRAG
    public DbSet<GraphCommunity> GraphCommunities => Set<GraphCommunity>();
    public DbSet<CommunityMembership> CommunityMemberships => Set<CommunityMembership>();
    public DbSet<ChunkEntityMention> ChunkEntityMentions => Set<ChunkEntityMention>();

    // Contextual Retrieval
    public DbSet<EmbeddingConfig> EmbeddingConfigs => Set<EmbeddingConfig>();
    public DbSet<RulingEmbeddingState> RulingEmbeddingStates => Set<RulingEmbeddingState>();

    // Pipeline tracking
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentStageLog> DocumentStageLogs => Set<DocumentStageLog>();

    // Operations
    public DbSet<Source> Sources => Set<Source>();
    public DbSet<CrawlerConfig> CrawlerConfigs => Set<CrawlerConfig>();
    public DbSet<IngestionJob> IngestionJobs => Set<IngestionJob>();
    public DbSet<User> Users => Set<User>();

    // Audit and traceability
    public DbSet<FieldProvenance> FieldProvenance => Set<FieldProvenance>();
    public DbSet<EntityAuditLog> EntityAuditLogs => Set<EntityAuditLog>();
    public DbSet<IngestionJobDetail> IngestionJobDetails => Set<IngestionJobDetail>();

    // Worker control
    public DbSet<WorkerPauseState> WorkerPauseStates => Set<WorkerPauseState>();

    // Admin ruling reprocess queue
    public DbSet<RulingReprocessRequest> RulingReprocessRequests => Set<RulingReprocessRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
