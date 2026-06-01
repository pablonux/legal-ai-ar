namespace LegalAiAr.Core.Models;

/// <summary>
/// A node in a multi-hop citation chain. Depth 0 = the queried ruling.
/// </summary>
public record CitationChainNode(
    Guid RulingId,
    string CaseTitle,
    DateOnly RulingDate,
    string? CourtName,
    string? LegalBranch,
    string CitationType,
    int Depth,
    Guid? ParentRulingId);

/// <summary>
/// Full graph neighborhood around a ruling: its entities + cited/citing rulings with their entities.
/// </summary>
public record RulingGraphNeighborhood(
    Guid CenterRulingId,
    string CaseTitle,
    DateOnly RulingDate,
    string? CourtName,
    IReadOnlyList<NeighborhoodPerson> Persons,
    IReadOnlyList<NeighborhoodKeyword> Keywords,
    IReadOnlyList<NeighborhoodStatute> Statutes,
    IReadOnlyList<NeighborhoodCitation> OutboundCitations,
    IReadOnlyList<NeighborhoodCitation> InboundCitations);

public record NeighborhoodPerson(int PersonId, string FullName, string RulingRole);
public record NeighborhoodKeyword(int KeywordId, string Description);
public record NeighborhoodStatute(int StatuteId, string Number, string Name, string? Articles);
public record NeighborhoodCitation(
    Guid RulingId,
    string CaseTitle,
    DateOnly RulingDate,
    string? CourtName,
    string CitationType);

/// <summary>
/// Entities shared across a set of rulings (intersection).
/// </summary>
public record SharedEntitiesResult(
    IReadOnlyList<SharedEntity> Persons,
    IReadOnlyList<SharedEntity> Keywords,
    IReadOnlyList<SharedEntity> Statutes);

public record SharedEntity(int EntityId, string Name, int SharedCount);

/// <summary>
/// A step in the shortest citation path between two rulings.
/// </summary>
public record CitationPathStep(
    Guid RulingId,
    string CaseTitle,
    DateOnly RulingDate,
    string? CourtName,
    string? CitationType,
    int StepIndex);

/// <summary>
/// Person ruling network: rulings grouped by legal branch and ruling role.
/// </summary>
public record PersonRulingNetwork(
    int PersonId,
    string FullName,
    int TotalRulings,
    IReadOnlyList<PersonRulingGroup> Groups);

public record PersonRulingGroup(
    string? LegalBranch,
    string RulingRole,
    int Count,
    IReadOnlyList<PersonRulingEntry> Rulings);

public record PersonRulingEntry(
    Guid RulingId,
    string CaseTitle,
    DateOnly RulingDate,
    string? CourtName);
