using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Messages;

/// <summary>
/// Message for queue-indexer. Contains the complete entity payload for persistence and indexing.
/// New fields (PipelineDocumentId, EntityType, EntityId) support the 6-stage pipeline.
/// In Phase 7, this will be simplified to a lightweight message (entity already in SQL).
/// </summary>
public record IndexerMessage(
    string DocumentId,
    string ContentHash,
    int SourceId,
    RulingData Ruling,
    IReadOnlyList<PersonData> Persons,
    IReadOnlyList<KeywordData> Keywords,
    IReadOnlyList<StatuteData> Statutes,
    IReadOnlyList<CitationData> Citations,
    IReadOnlyList<ChunkData> Chunks,
    string? TextBlobPath = null,
    Guid? IngestionJobId = null,
    string? AnalysisId = null,
    IReadOnlyList<CitedByData>? CitedBy = null,
    bool ForceReindex = false,
    ProsecutorOpinionData? ProsecutorOpinion = null,
    string? PayloadBlobPath = null,
    IReadOnlyList<VoteData>? Votes = null,
    IReadOnlyList<SumarioData>? Sumarios = null,
    IReadOnlyList<SynthesisData>? Syntheses = null,
    IReadOnlyList<LinkData>? Links = null,
    IReadOnlyList<PartyData>? Parties = null,
    IReadOnlyList<LegalRepresentationData>? LegalRepresentations = null,
    bool Reprocess = false,
    Guid? PipelineDocumentId = null,
    EntityType EntityType = EntityType.Ruling,
    Guid? EntityId = null);
