using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Proceedings.Commands;

public record DeleteProsecutorOpinionsCommand : IRequest<DeleteProsecutorOpinionsResult>;
public record DeleteProsecutorOpinionsResult(int Deleted);

public class DeleteProsecutorOpinionsHandler
    : IRequestHandler<DeleteProsecutorOpinionsCommand, DeleteProsecutorOpinionsResult>
{
    private readonly IRulingRepository _rulings;
    private readonly ILogger<DeleteProsecutorOpinionsHandler> _logger;

    public DeleteProsecutorOpinionsHandler(
        IRulingRepository rulings,
        ILogger<DeleteProsecutorOpinionsHandler> logger)
    {
        _rulings = rulings;
        _logger = logger;
    }

    public async Task<DeleteProsecutorOpinionsResult> Handle(
        DeleteProsecutorOpinionsCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _rulings.DeleteAllProsecutorOpinionsAsync(cancellationToken);

        _logger.LogInformation("Deleted {Count} prosecutor opinions for re-extraction", deleted);

        return new DeleteProsecutorOpinionsResult(deleted);
    }
}
