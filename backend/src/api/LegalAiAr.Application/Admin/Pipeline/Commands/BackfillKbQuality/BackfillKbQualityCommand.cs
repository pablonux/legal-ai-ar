using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Admin.Pipeline.Commands.BackfillKbQuality;

public record BackfillKbQualityCommand : IRequest<BackfillKbQualityResult>;

public record BackfillKbQualityResult(
    int StatutesClassified,
    int StatutesLegalBranchFilled,
    int PersonsCourtUpdated);

public class BackfillKbQualityHandler
    : IRequestHandler<BackfillKbQualityCommand, BackfillKbQualityResult>
{
    private readonly IStatuteRepository _statutes;
    private readonly IPersonRepository _persons;
    private readonly ILogger<BackfillKbQualityHandler> _logger;

    public BackfillKbQualityHandler(
        IStatuteRepository statutes,
        IPersonRepository persons,
        ILogger<BackfillKbQualityHandler> logger)
    {
        _statutes = statutes;
        _persons = persons;
        _logger = logger;
    }

    public async Task<BackfillKbQualityResult> Handle(
        BackfillKbQualityCommand request, CancellationToken cancellationToken)
    {
        var unclassified = await _statutes.GetUnclassifiedAsync(cancellationToken);
        var classified = 0;
        foreach (var statute in unclassified)
        {
            var before = statute.NormType;
            StatuteClassifier.ClassifyIfNeeded(statute);
            if (statute.NormType != before)
                classified++;
        }
        _logger.LogInformation(
            "Statute classification: {Classified}/{Total} statutes classified by name pattern",
            classified, unclassified.Count);

        var legalBranchFilled = await _statutes.BackfillLegalBranchFromRulingsAsync(cancellationToken);
        _logger.LogInformation("LegalBranch propagation: {Updated} statutes updated from linked rulings",
            legalBranchFilled);

        var personsUpdated = await _persons.BackfillCurrentCourtIdAsync(cancellationToken);
        _logger.LogInformation("Person court backfill: {Updated} persons updated from latest ruling",
            personsUpdated);

        return new BackfillKbQualityResult(classified, legalBranchFilled, personsUpdated);
    }
}
