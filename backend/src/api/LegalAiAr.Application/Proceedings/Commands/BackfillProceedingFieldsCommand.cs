using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Proceedings.Commands;

public record BackfillProceedingFieldsCommand : IRequest<BackfillProceedingFieldsResult>;

public record BackfillProceedingFieldsResult(int ProceedingsUpdated);

public class BackfillProceedingFieldsHandler
    : IRequestHandler<BackfillProceedingFieldsCommand, BackfillProceedingFieldsResult>
{
    private readonly IJudicialProceedingRepository _repo;
    private readonly ILogger<BackfillProceedingFieldsHandler> _logger;

    public BackfillProceedingFieldsHandler(
        IJudicialProceedingRepository repo,
        ILogger<BackfillProceedingFieldsHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<BackfillProceedingFieldsResult> Handle(
        BackfillProceedingFieldsCommand request, CancellationToken cancellationToken)
    {
        var updated = await _repo.BackfillProceedingFieldsAsync(cancellationToken);

        _logger.LogInformation(
            "BackfillProceedingFields: {Updated} proceedings updated with ProcessType, CourtId, LegalBranch, Status",
            updated);

        return new BackfillProceedingFieldsResult(updated);
    }
}
