using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Rulings.Queries.GetRulingDocument;

public class GetRulingDocumentHandler : IRequestHandler<GetRulingDocumentQuery, RulingDocumentResult?>
{
    private readonly IRulingRepository _rulings;
    private readonly IBlobStorageService _blobStorage;

    public GetRulingDocumentHandler(IRulingRepository rulings, IBlobStorageService blobStorage)
    {
        _rulings = rulings;
        _blobStorage = blobStorage;
    }

    public async Task<RulingDocumentResult?> Handle(
        GetRulingDocumentQuery request, CancellationToken cancellationToken)
    {
        var ruling = await _rulings.GetByIdAsync(request.Id, cancellationToken);
        if (ruling is null || string.IsNullOrEmpty(ruling.BlobPath))
            return null;

        var stream = await _blobStorage.DownloadAsync(ruling.BlobPath, cancellationToken);
        return new RulingDocumentResult(stream, "application/pdf");
    }
}
