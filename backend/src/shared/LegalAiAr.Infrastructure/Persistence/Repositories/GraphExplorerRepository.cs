using LegalAiAr.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

public class GraphExplorerRepository : IGraphExplorerRepository
{
    private const int MaxNeighbors = 20;
    private const int MaxSearchResults = 20;
    private readonly AppDbContext _db;

    public GraphExplorerRepository(AppDbContext db) => _db = db;

    public async Task<GraphNeighborhood> GetNeighborhoodAsync(string entityType, string entityId, CancellationToken ct = default)
    {
        return entityType.ToLowerInvariant() switch
        {
            "ruling" => await GetRulingNeighborhood(Guid.Parse(entityId), ct),
            "court" => await GetCourtNeighborhood(int.Parse(entityId), ct),
            "person" => await GetPersonNeighborhood(int.Parse(entityId), ct),
            "judge" => await GetPersonNeighborhood(int.Parse(entityId), ct),
            "statute" => await GetStatuteNeighborhood(int.Parse(entityId), ct),
            "keyword" => await GetKeywordNeighborhood(int.Parse(entityId), ct),
            "proceeding" => await GetProceedingNeighborhood(int.Parse(entityId), ct),
            _ => throw new ArgumentException($"Unknown entity type: {entityType}")
        };
    }

    public async Task<IReadOnlyList<GraphSearchHit>> SearchEntitiesAsync(string query, string? types, CancellationToken ct = default)
    {
        var typeSet = types?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(t => t.ToLowerInvariant()).ToHashSet();

        var results = new List<GraphSearchHit>();

        if (typeSet is null || typeSet.Contains("ruling"))
        {
            var rulings = await _db.Rulings
                .Where(r => r.CaseTitle.Contains(query))
                .OrderByDescending(r => r.RulingDate)
                .Take(5)
                .Select(r => new { r.Id, r.CaseTitle, r.RulingDate, CourtName = r.Court.Name })
                .ToListAsync(ct);
            results.AddRange(rulings.Select(r => new GraphSearchHit(
                $"ruling:{r.Id}", "ruling", r.CaseTitle,
                $"{r.RulingDate:yyyy-MM-dd} · {r.CourtName}")));
        }

        if (typeSet is null || typeSet.Contains("court"))
        {
            var courts = await _db.Courts
                .Where(c => c.Name.Contains(query))
                .Take(5)
                .Select(c => new { c.Id, c.Name, c.JurisdictionArea })
                .ToListAsync(ct);
            results.AddRange(courts.Select(c => new GraphSearchHit(
                $"court:{c.Id}", "court", c.Name, c.JurisdictionArea)));
        }

        if (typeSet is null || typeSet.Contains("person") || typeSet.Contains("judge"))
        {
            var persons = await _db.Persons
                .Where(p => p.DisplayName.Contains(query))
                .Take(5)
                .Select(p => new
                {
                    p.Id,
                    p.DisplayName,
                    CourtName = p.JudicialOffices
                        .Where(jo => jo.IsCurrent)
                        .Select(jo => jo.Court.Name)
                        .FirstOrDefault()
                })
                .ToListAsync(ct);
            results.AddRange(persons.Select(p => new GraphSearchHit(
                $"person:{p.Id}", "person", p.DisplayName, p.CourtName)));
        }

        if (typeSet is null || typeSet.Contains("statute"))
        {
            var statutes = await _db.Statutes
                .Where(s => s.Number.Contains(query) || s.Name.Contains(query))
                .Take(5)
                .Select(s => new { s.Id, s.Number, s.Name })
                .ToListAsync(ct);
            results.AddRange(statutes.Select(s => new GraphSearchHit(
                $"statute:{s.Id}", "statute", $"Ley {s.Number}", s.Name)));
        }

        if (typeSet is null || typeSet.Contains("proceeding"))
        {
            var proceedings = await _db.JudicialProceedings
                .Where(p => p.CaseNumber.Contains(query)
                    || (p.DisplayName != null && p.DisplayName.Contains(query)))
                .Take(5)
                .Select(p => new { p.Id, p.CaseNumber, p.DisplayName, CourtName = p.Court != null ? p.Court.Name : null })
                .ToListAsync(ct);
            results.AddRange(proceedings.Select(p => new GraphSearchHit(
                $"proceeding:{p.Id}", "proceeding", p.DisplayName ?? p.CaseNumber, p.CourtName)));
        }

        return results.Take(MaxSearchResults).ToList();
    }

    private async Task<GraphNeighborhood> GetRulingNeighborhood(Guid rulingId, CancellationToken ct)
    {
        var ruling = await _db.Rulings
            .Include(r => r.Court)
            .FirstOrDefaultAsync(r => r.Id == rulingId, ct)
            ?? throw new ArgumentException("Ruling not found");

        var center = RulingNode(ruling.Id, ruling.CaseTitle, ruling.RulingDate, ruling.Court.Name,
            ruling.LegalBranch?.ToString(), ruling.PrecedentWeight?.ToString());

        var nodes = new List<GraphNodeRaw>();
        var edges = new List<GraphEdgeRaw>();

        // Court
        var courtId = $"court:{ruling.CourtId}";
        nodes.Add(new GraphNodeRaw(courtId, "court", ruling.Court.Name, ruling.Court.JurisdictionArea, null));
        edges.Add(new GraphEdgeRaw($"e-ib-{ruling.Id}-{ruling.CourtId}", center.Id, courtId, "issuedBy", null));

        // Persons (participants)
        var participants = await _db.RulingParticipations
            .Include(rp => rp.Person)
            .Where(rp => rp.RulingId == rulingId)
            .ToListAsync(ct);
        foreach (var rp in participants)
        {
            var pId = $"person:{rp.PersonId}";
            nodes.Add(new GraphNodeRaw(pId, "person", rp.Person.DisplayName, ruling.Court.Name, null));
            edges.Add(new GraphEdgeRaw($"e-sb-{ruling.Id}-{rp.PersonId}", center.Id, pId, "signedBy", rp.Role.ToString()));
        }

        // Outbound citations
        var outCitations = await _db.Citations
            .Include(c => c.TargetRuling).ThenInclude(r => r!.Court)
            .Where(c => c.SourceRulingId == rulingId && c.TargetRulingId != null)
            .ToListAsync(ct);
        foreach (var c in outCitations)
        {
            var tId = $"ruling:{c.TargetRulingId}";
            nodes.Add(RulingNode(c.TargetRulingId!.Value, c.TargetRuling!.CaseTitle, c.TargetRuling.RulingDate,
                c.TargetRuling.Court.Name, null, null));
            edges.Add(new GraphEdgeRaw($"e-ct-{c.Id}", center.Id, tId, "cites", c.CitationType.ToString()));
        }

        // Inbound citations
        var inCitations = await _db.Citations
            .Include(c => c.SourceRuling).ThenInclude(r => r.Court)
            .Where(c => c.TargetRulingId == rulingId)
            .OrderByDescending(c => c.SourceRuling.RulingDate)
            .Take(MaxNeighbors)
            .ToListAsync(ct);
        foreach (var c in inCitations)
        {
            var sId = $"ruling:{c.SourceRulingId}";
            nodes.Add(RulingNode(c.SourceRulingId, c.SourceRuling.CaseTitle, c.SourceRuling.RulingDate,
                c.SourceRuling.Court.Name, null, null));
            edges.Add(new GraphEdgeRaw($"e-cb-{c.Id}", sId, center.Id, "citedBy", c.CitationType.ToString()));
        }

        // Statutes
        var statutes = await _db.RulingStatutes
            .Include(rs => rs.Statute)
            .Where(rs => rs.RulingId == rulingId)
            .ToListAsync(ct);
        foreach (var rs in statutes)
        {
            var sId = $"statute:{rs.StatuteId}";
            nodes.Add(new GraphNodeRaw(sId, "statute", $"Ley {rs.Statute.Number}", rs.Statute.Name, null));
            edges.Add(new GraphEdgeRaw($"e-cs-{ruling.Id}-{rs.StatuteId}", center.Id, sId, "citesStatute", rs.Articles));
        }

        // Proceeding
        if (ruling.JudicialProceedingId.HasValue)
        {
            var proc = await _db.JudicialProceedings
                .FirstOrDefaultAsync(p => p.Id == ruling.JudicialProceedingId.Value, ct);
            if (proc is not null)
            {
                var procId = $"proceeding:{proc.Id}";
                nodes.Add(new GraphNodeRaw(procId, "proceeding", proc.DisplayName ?? proc.CaseNumber, null, null));
                edges.Add(new GraphEdgeRaw($"e-bt-{ruling.Id}-{proc.Id}", center.Id, procId, "belongsTo", null));
            }
        }

        // Keywords
        var keywords = await _db.RulingKeywords
            .Include(rk => rk.Keyword)
            .Where(rk => rk.RulingId == rulingId)
            .OrderBy(rk => rk.SortOrder)
            .Take(MaxNeighbors)
            .ToListAsync(ct);
        foreach (var rk in keywords)
        {
            var kId = $"keyword:{rk.KeywordId}";
            nodes.Add(new GraphNodeRaw(kId, "keyword", rk.Keyword.Description, null, null));
            edges.Add(new GraphEdgeRaw($"e-hk-{ruling.Id}-{rk.KeywordId}", center.Id, kId, "hasKeyword", null));
        }

        return new GraphNeighborhood(center, DeduplicateNodes(nodes), edges);
    }

    private async Task<GraphNeighborhood> GetCourtNeighborhood(int courtId, CancellationToken ct)
    {
        var court = await _db.Courts.FindAsync([courtId], ct)
            ?? throw new ArgumentException("Court not found");

        var center = new GraphNodeRaw($"court:{courtId}", "court", court.Name, court.JurisdictionArea, null);
        var nodes = new List<GraphNodeRaw>();
        var edges = new List<GraphEdgeRaw>();

        // Recent rulings
        var rulings = await _db.Rulings
            .Where(r => r.CourtId == courtId)
            .OrderByDescending(r => r.RulingDate)
            .Take(MaxNeighbors)
            .Select(r => new { r.Id, r.CaseTitle, r.RulingDate })
            .ToListAsync(ct);
        foreach (var r in rulings)
        {
            var rId = $"ruling:{r.Id}";
            nodes.Add(RulingNode(r.Id, r.CaseTitle, r.RulingDate, court.Name, null, null));
            edges.Add(new GraphEdgeRaw($"e-ib-{r.Id}-{courtId}", rId, center.Id, "issuedBy", null));
        }

        // Persons (current members via JudicialOffice)
        var offices = await _db.JudicialOffices
            .Include(jo => jo.Person)
            .Where(jo => jo.CourtId == courtId && jo.IsCurrent)
            .ToListAsync(ct);
        foreach (var jo in offices)
        {
            var pId = $"person:{jo.PersonId}";
            nodes.Add(new GraphNodeRaw(pId, "person", jo.Person.DisplayName, court.Name, null));
            edges.Add(new GraphEdgeRaw($"e-mo-{jo.PersonId}-{courtId}", pId, center.Id, "memberOf", null));
        }

        return new GraphNeighborhood(center, DeduplicateNodes(nodes), edges);
    }

    private async Task<GraphNeighborhood> GetPersonNeighborhood(int personId, CancellationToken ct)
    {
        var person = await _db.Persons
            .FirstOrDefaultAsync(p => p.Id == personId, ct)
            ?? throw new ArgumentException("Person not found");

        var currentOffice = await _db.JudicialOffices
            .Include(jo => jo.Court)
            .Where(jo => jo.PersonId == personId && jo.IsCurrent)
            .FirstOrDefaultAsync(ct);

        var center = new GraphNodeRaw($"person:{personId}", "person",
            person.DisplayName, currentOffice?.Court.Name, null);
        var nodes = new List<GraphNodeRaw>();
        var edges = new List<GraphEdgeRaw>();

        if (currentOffice is not null)
        {
            var cId = $"court:{currentOffice.CourtId}";
            nodes.Add(new GraphNodeRaw(cId, "court", currentOffice.Court.Name, currentOffice.Court.JurisdictionArea, null));
            edges.Add(new GraphEdgeRaw($"e-mo-{personId}-{currentOffice.CourtId}", center.Id, cId, "memberOf", null));
        }

        var participations = await _db.RulingParticipations
            .Include(rp => rp.Ruling).ThenInclude(r => r.Court)
            .Where(rp => rp.PersonId == personId)
            .OrderByDescending(rp => rp.Ruling.RulingDate)
            .Take(MaxNeighbors)
            .ToListAsync(ct);
        foreach (var rp in participations)
        {
            var rId = $"ruling:{rp.RulingId}";
            nodes.Add(RulingNode(rp.RulingId, rp.Ruling.CaseTitle, rp.Ruling.RulingDate,
                rp.Ruling.Court.Name, null, null));
            edges.Add(new GraphEdgeRaw($"e-sb-{rp.RulingId}-{personId}", rId, center.Id, "signedBy", rp.Role.ToString()));
        }

        // Proceedings as party
        var procParties = await _db.ProceedingParties
            .Include(pp => pp.JudicialProceeding)
            .Where(pp => pp.PersonId == personId)
            .Take(MaxNeighbors)
            .ToListAsync(ct);
        foreach (var pp in procParties)
        {
            var procId = $"proceeding:{pp.JudicialProceedingId}";
            nodes.Add(new GraphNodeRaw(procId, "proceeding",
                pp.JudicialProceeding.DisplayName ?? pp.JudicialProceeding.CaseNumber, null, null));
            edges.Add(new GraphEdgeRaw($"e-po-{personId}-{pp.JudicialProceedingId}", center.Id, procId, "partyOf", pp.Role.ToString()));
        }

        return new GraphNeighborhood(center, DeduplicateNodes(nodes), edges);
    }

    private async Task<GraphNeighborhood> GetStatuteNeighborhood(int statuteId, CancellationToken ct)
    {
        var statute = await _db.Statutes.FindAsync([statuteId], ct)
            ?? throw new ArgumentException("Statute not found");

        var center = new GraphNodeRaw($"statute:{statuteId}", "statute",
            $"Ley {statute.Number}", statute.Name, null);
        var nodes = new List<GraphNodeRaw>();
        var edges = new List<GraphEdgeRaw>();

        // Rulings citing this statute
        var rulingStatutes = await _db.RulingStatutes
            .Include(rs => rs.Ruling).ThenInclude(r => r.Court)
            .Where(rs => rs.StatuteId == statuteId)
            .OrderByDescending(rs => rs.Ruling.RulingDate)
            .Take(MaxNeighbors)
            .ToListAsync(ct);
        foreach (var rs in rulingStatutes)
        {
            var rId = $"ruling:{rs.RulingId}";
            nodes.Add(RulingNode(rs.RulingId, rs.Ruling.CaseTitle, rs.Ruling.RulingDate,
                rs.Ruling.Court.Name, null, null));
            edges.Add(new GraphEdgeRaw($"e-cs-{rs.RulingId}-{statuteId}", rId, center.Id, "citesStatute", rs.Articles));
        }

        // Norm relations (outbound)
        var outNorms = await _db.NormRelations
            .Include(nr => nr.TargetStatute)
            .Where(nr => nr.SourceStatuteId == statuteId)
            .ToListAsync(ct);
        foreach (var nr in outNorms)
        {
            var tId = $"statute:{nr.TargetStatuteId}";
            nodes.Add(new GraphNodeRaw(tId, "statute", $"Ley {nr.TargetStatute.Number}", nr.TargetStatute.Name, null));
            edges.Add(new GraphEdgeRaw($"e-nr-{nr.Id}", center.Id, tId, "normRelation", nr.RelationType.ToString()));
        }

        // Norm relations (inbound)
        var inNorms = await _db.NormRelations
            .Include(nr => nr.SourceStatute)
            .Where(nr => nr.TargetStatuteId == statuteId)
            .ToListAsync(ct);
        foreach (var nr in inNorms)
        {
            var sId = $"statute:{nr.SourceStatuteId}";
            nodes.Add(new GraphNodeRaw(sId, "statute", $"Ley {nr.SourceStatute.Number}", nr.SourceStatute.Name, null));
            edges.Add(new GraphEdgeRaw($"e-nr-{nr.Id}", sId, center.Id, "normRelation", nr.RelationType.ToString()));
        }

        return new GraphNeighborhood(center, DeduplicateNodes(nodes), edges);
    }

    private async Task<GraphNeighborhood> GetKeywordNeighborhood(int keywordId, CancellationToken ct)
    {
        var keyword = await _db.Keywords.FindAsync([keywordId], ct)
            ?? throw new ArgumentException("Keyword not found");

        var center = new GraphNodeRaw($"keyword:{keywordId}", "keyword", keyword.Description, null, null);
        var nodes = new List<GraphNodeRaw>();
        var edges = new List<GraphEdgeRaw>();

        var rulingKeywords = await _db.RulingKeywords
            .Include(rk => rk.Ruling).ThenInclude(r => r.Court)
            .Where(rk => rk.KeywordId == keywordId)
            .OrderByDescending(rk => rk.Ruling.RulingDate)
            .Take(MaxNeighbors)
            .ToListAsync(ct);
        foreach (var rk in rulingKeywords)
        {
            var rId = $"ruling:{rk.RulingId}";
            nodes.Add(RulingNode(rk.RulingId, rk.Ruling.CaseTitle, rk.Ruling.RulingDate,
                rk.Ruling.Court.Name, null, null));
            edges.Add(new GraphEdgeRaw($"e-hk-{rk.RulingId}-{keywordId}", rId, center.Id, "hasKeyword", null));
        }

        return new GraphNeighborhood(center, DeduplicateNodes(nodes), edges);
    }

    private async Task<GraphNeighborhood> GetProceedingNeighborhood(int proceedingId, CancellationToken ct)
    {
        var proc = await _db.JudicialProceedings
            .Include(p => p.Court)
            .FirstOrDefaultAsync(p => p.Id == proceedingId, ct)
            ?? throw new ArgumentException("Proceeding not found");

        var center = new GraphNodeRaw($"proceeding:{proceedingId}", "proceeding",
            proc.DisplayName ?? proc.CaseNumber, proc.Court?.Name, null);
        var nodes = new List<GraphNodeRaw>();
        var edges = new List<GraphEdgeRaw>();

        if (proc.Court is not null)
        {
            var cId = $"court:{proc.CourtId}";
            nodes.Add(new GraphNodeRaw(cId, "court", proc.Court.Name, proc.Court.JurisdictionArea, null));
            edges.Add(new GraphEdgeRaw($"e-at-{proceedingId}-{proc.CourtId}", center.Id, cId, "adjudicatedAt", null));
        }

        var rulings = await _db.Rulings
            .Where(r => r.JudicialProceedingId == proceedingId)
            .OrderByDescending(r => r.RulingDate)
            .Take(MaxNeighbors)
            .Select(r => new { r.Id, r.CaseTitle, r.RulingDate, CourtName = r.Court.Name })
            .ToListAsync(ct);
        foreach (var r in rulings)
        {
            var rId = $"ruling:{r.Id}";
            nodes.Add(RulingNode(r.Id, r.CaseTitle, r.RulingDate, r.CourtName, null, null));
            edges.Add(new GraphEdgeRaw($"e-bt-{r.Id}-{proceedingId}", rId, center.Id, "belongsTo", null));
        }

        var parties = await _db.ProceedingParties
            .Include(pp => pp.Person)
            .Where(pp => pp.JudicialProceedingId == proceedingId)
            .ToListAsync(ct);
        foreach (var pp in parties)
        {
            var pId = $"person:{pp.PersonId}";
            nodes.Add(new GraphNodeRaw(pId, "person", pp.Person.DisplayName, pp.Role.ToString(), null));
            edges.Add(new GraphEdgeRaw($"e-po-{pp.PersonId}-{proceedingId}", pId, center.Id, "partyOf", pp.Role.ToString()));
        }

        return new GraphNeighborhood(center, DeduplicateNodes(nodes), edges);
    }

    private static GraphNodeRaw RulingNode(Guid id, string caseTitle, DateOnly rulingDate, string courtName,
        string? legalBranch, string? precedentWeight)
    {
        var props = new Dictionary<string, string>();
        if (legalBranch is not null) props["legalBranch"] = legalBranch;
        if (precedentWeight is not null) props["precedentWeight"] = precedentWeight;
        return new GraphNodeRaw($"ruling:{id}", "ruling", caseTitle,
            $"{rulingDate:yyyy-MM-dd} · {courtName}", props.Count > 0 ? props : null);
    }

    private static IReadOnlyList<GraphNodeRaw> DeduplicateNodes(List<GraphNodeRaw> nodes)
    {
        var seen = new HashSet<string>();
        var result = new List<GraphNodeRaw>();
        foreach (var n in nodes)
        {
            if (seen.Add(n.Id))
                result.Add(n);
        }
        return result;
    }
}
