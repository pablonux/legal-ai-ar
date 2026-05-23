using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.Models;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Proceedings.Commands;

public record LinkProceedingsCommand : IRequest<LinkProceedingsResult>;

public class LinkProceedingsHandler : IRequestHandler<LinkProceedingsCommand, LinkProceedingsResult>
{
    private readonly IJudicialProceedingRepository _repo;
    private readonly ILogger<LinkProceedingsHandler> _logger;

    public LinkProceedingsHandler(IJudicialProceedingRepository repo, ILogger<LinkProceedingsHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<LinkProceedingsResult> Handle(LinkProceedingsCommand request, CancellationToken cancellationToken)
    {
        var (created, linked) = await _repo.LinkUnlinkedRulingsAsync(cancellationToken);

        _logger.LogInformation("LinkProceedings complete: {Created} proceedings, {Linked} rulings linked",
            created, linked);

        return new LinkProceedingsResult(created, linked);
    }
}
