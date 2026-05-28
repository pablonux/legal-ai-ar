using LegalAiAr.Application.Admin.RulingReprocess.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Admin.RulingReprocess.Queries.ListRulingReprocessRequests;

public sealed class ListRulingReprocessRequestsHandler
    : IRequestHandler<ListRulingReprocessRequestsQuery, RulingReprocessListResult>
{
    private readonly IRulingReprocessRequestRepository _requests;

    public ListRulingReprocessRequestsHandler(IRulingReprocessRequestRepository requests) =>
        _requests = requests;

    public async Task<RulingReprocessListResult> Handle(
        ListRulingReprocessRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var total = await _requests.CountAsync(request.Status, cancellationToken);
        var rows = await _requests.ListAsync(request.Status, skip, pageSize, cancellationToken);

        var items = rows.Select(r => new RulingReprocessRequestDto(
            r.Id,
            r.RulingId,
            r.DocumentId,
            r.Status.ToString(),
            r.UseCache,
            r.RequestedBy,
            r.RequestedAt,
            r.StartedAt,
            r.CompletedAt,
            r.ErrorMessage,
            r.RetryCount,
            r.Ruling.CaseTitle,
            r.Ruling.ExternalId)).ToList();

        return new RulingReprocessListResult(items, total);
    }
}
