using LegalAiAr.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence;

public class OntologyStatsProvider : IOntologyStatsProvider
{
    private readonly AppDbContext _db;

    public OntologyStatsProvider(AppDbContext db) => _db = db;

    public async Task<IReadOnlyDictionary<string, int>> GetEntityCountsAsync(CancellationToken cancellationToken = default)
    {
        var rulings = await _db.Rulings.CountAsync(cancellationToken);
        var courts = await _db.Courts.CountAsync(cancellationToken);
        var persons = await _db.Persons.CountAsync(cancellationToken);
        var statutes = await _db.Statutes.CountAsync(cancellationToken);
        var thesaurus = await _db.ThesaurusTerms.CountAsync(cancellationToken);
        var keywords = await _db.Keywords.CountAsync(cancellationToken);
        var sources = await _db.Sources.CountAsync(cancellationToken);
        var proceedings = await _db.JudicialProceedings.CountAsync(cancellationToken);

        return new Dictionary<string, int>
        {
            ["Sentencia"] = rulings,
            ["Tribunal"] = courts,
            ["Persona"] = persons,
            ["NormaJuridica"] = statutes,
            ["ThesaurusTerm"] = thesaurus,
            ["PalabraClave"] = keywords,
            ["Fuente"] = sources,
            ["ProcesoJudicial"] = proceedings,
        };
    }

    public async Task<IReadOnlyList<OntologyEntityStats>> GetEntityStatsAsync(CancellationToken cancellationToken = default)
    {
        var rulingCount = await _db.Rulings.CountAsync(cancellationToken);
        var rulingsByBranch = await _db.Rulings
            .Where(r => r.LegalBranch != null)
            .GroupBy(r => r.LegalBranch!.Value)
            .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(cancellationToken);
        var rulingsByWeight = await _db.Rulings
            .Where(r => r.PrecedentWeight != null)
            .GroupBy(r => r.PrecedentWeight!.Value)
            .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(cancellationToken);

        var courtCount = await _db.Courts.CountAsync(cancellationToken);
        var courtsByType = await _db.Courts
            .Where(c => c.CourtCategory != null)
            .GroupBy(c => c.CourtCategory!.Value)
            .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(cancellationToken);
        var courtsByFuero = await _db.Courts
            .Where(c => c.Fuero != null)
            .GroupBy(c => c.Fuero!.Value)
            .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(cancellationToken);

        var personCount = await _db.Persons.CountAsync(cancellationToken);
        var personsByCourt = await _db.JudicialOffices
            .Where(jo => jo.IsCurrent)
            .GroupBy(jo => jo.Court.Name)
            .Select(g => new { Code = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(15)
            .ToListAsync(cancellationToken);

        var statuteCount = await _db.Statutes.CountAsync(cancellationToken);
        var statutesByType = await _db.Statutes
            .Where(s => s.NormType != null)
            .GroupBy(s => s.NormType!.Value)
            .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(cancellationToken);
        var statutesByLevel = await _db.Statutes
            .Where(s => s.NormativeLevel != null)
            .GroupBy(s => s.NormativeLevel!.Value)
            .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(cancellationToken);

        var thesaurusCount = await _db.ThesaurusTerms.CountAsync(cancellationToken);
        var thesaurusByBranch = await _db.ThesaurusTerms
            .Where(t => t.BranchName != null && t.IsPreferred)
            .GroupBy(t => t.BranchName!)
            .Select(g => new { Code = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(15)
            .ToListAsync(cancellationToken);

        var keywordCount = await _db.Keywords.CountAsync(cancellationToken);

        var sourceCount = await _db.Sources.CountAsync(cancellationToken);

        return
        [
            new OntologyEntityStats("Sentencia", "Ruling", rulingCount,
            [
                new OntologyTaxonomyBreakdown("LegalBranch", "Rama del derecho",
                    rulingsByBranch.Select(r => new OntologyTaxonomyValueCount(r.Code, r.Code, r.Count)).ToList()),
                new OntologyTaxonomyBreakdown("PrecedentWeight", "Peso precedencial",
                    rulingsByWeight.Select(r => new OntologyTaxonomyValueCount(r.Code, r.Code, r.Count)).ToList()),
            ]),
            new OntologyEntityStats("Tribunal", "Court", courtCount,
            [
                new OntologyTaxonomyBreakdown("CourtType", "Tipo de tribunal",
                    courtsByType.Select(c => new OntologyTaxonomyValueCount(c.Code, c.Code, c.Count)).ToList()),
                new OntologyTaxonomyBreakdown("Fuero", "Fuero",
                    courtsByFuero.Select(c => new OntologyTaxonomyValueCount(c.Code, c.Code, c.Count)).ToList()),
            ]),
            new OntologyEntityStats("Persona", "Person", personCount,
            [
                new OntologyTaxonomyBreakdown("PersonByCourt", "Tribunal actual",
                    personsByCourt.Select(p => new OntologyTaxonomyValueCount(p.Code, p.Code, p.Count)).ToList()),
            ]),
            new OntologyEntityStats("NormaJuridica", "Statute", statuteCount,
            [
                new OntologyTaxonomyBreakdown("NormType", "Tipo de norma",
                    statutesByType.Select(s => new OntologyTaxonomyValueCount(s.Code, s.Code, s.Count)).ToList()),
                new OntologyTaxonomyBreakdown("NormativeLevel", "Nivel normativo",
                    statutesByLevel.Select(s => new OntologyTaxonomyValueCount(s.Code, s.Code, s.Count)).ToList()),
            ]),
            new OntologyEntityStats("ThesaurusTerm", "ThesaurusTerm", thesaurusCount,
            [
                new OntologyTaxonomyBreakdown("ThesaurusBranch", "Rama temática",
                    thesaurusByBranch.Select(t => new OntologyTaxonomyValueCount(t.Code, t.Code, t.Count)).ToList()),
            ]),
            new OntologyEntityStats("PalabraClave", "Keyword", keywordCount, []),
            new OntologyEntityStats("Fuente", "Source", sourceCount, []),
            new OntologyEntityStats("ProcesoJudicial", "JudicialProceeding",
                await _db.JudicialProceedings.CountAsync(cancellationToken), []),
        ];
    }

    public async Task<IReadOnlyDictionary<string, int>> GetTaxonomyCountsAsync(
        string taxonomyId, CancellationToken cancellationToken = default)
    {
        var result = taxonomyId switch
        {
            "LegalBranch" => await _db.Rulings
                .Where(r => r.LegalBranch != null)
                .GroupBy(r => r.LegalBranch!.Value)
                .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
                .ToListAsync(cancellationToken),
            "PrecedentWeight" => await _db.Rulings
                .Where(r => r.PrecedentWeight != null)
                .GroupBy(r => r.PrecedentWeight!.Value)
                .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
                .ToListAsync(cancellationToken),
            "CourtType" => await _db.Courts
                .Where(c => c.CourtCategory != null)
                .GroupBy(c => c.CourtCategory!.Value)
                .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
                .ToListAsync(cancellationToken),
            "Fuero" => await _db.Courts
                .Where(c => c.Fuero != null)
                .GroupBy(c => c.Fuero!.Value)
                .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
                .ToListAsync(cancellationToken),
            "GovernmentLevel" => await _db.Courts
                .Where(c => c.GovernmentLevel != null)
                .GroupBy(c => c.GovernmentLevel!.Value)
                .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
                .ToListAsync(cancellationToken),
            "NormType" => await _db.Statutes
                .Where(s => s.NormType != null)
                .GroupBy(s => s.NormType!.Value)
                .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
                .ToListAsync(cancellationToken),
            "NormativeLevel" => await _db.Statutes
                .Where(s => s.NormativeLevel != null)
                .GroupBy(s => s.NormativeLevel!.Value)
                .Select(g => new { Code = g.Key.ToString(), Count = g.Count() })
                .ToListAsync(cancellationToken),
            _ => []
        };

        return result.ToDictionary(r => r.Code, r => r.Count);
    }

    public async Task<IReadOnlyDictionary<string, int>> GetRelationCountsAsync(CancellationToken cancellationToken = default)
    {
        var citations = await _db.Citations.CountAsync(cancellationToken);
        var rulingStatutes = await _db.RulingStatutes.CountAsync(cancellationToken);
        var rulingParticipations = await _db.RulingParticipations.CountAsync(cancellationToken);
        var rulingKeywords = await _db.RulingKeywords.CountAsync(cancellationToken);
        var rulings = await _db.Rulings.CountAsync(cancellationToken);
        var normRelations = await _db.NormRelations.CountAsync(cancellationToken);
        var keywordsWithThesaurus = await _db.Keywords.Where(k => k.ThesaurusTermId != null).CountAsync(cancellationToken);
        var proceedingRulings = await _db.Rulings.Where(r => r.JudicialProceedingId != null).CountAsync(cancellationToken);
        var prosecutorOpinions = await _db.ProsecutorOpinions.CountAsync(cancellationToken);

        return new Dictionary<string, int>
        {
            ["citaFallo"] = citations,
            ["citaNorma"] = rulingStatutes,
            ["firmadoPor"] = rulingParticipations,
            ["emitidoPor"] = rulings,
            ["tienePalabraClave"] = rulingKeywords,
            ["tieneDescriptor"] = keywordsWithThesaurus,
            ["deroga / modifica"] = normRelations,
            ["provenienteDe"] = rulings,
            ["normalizadoPor"] = keywordsWithThesaurus,
            ["conduceA"] = proceedingRulings,
            ["dictaminadoPor"] = prosecutorOpinions,
        };
    }
}
