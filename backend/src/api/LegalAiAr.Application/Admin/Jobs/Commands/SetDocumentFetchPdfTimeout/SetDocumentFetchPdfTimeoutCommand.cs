using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Admin.Jobs.Commands.SetDocumentFetchPdfTimeout;

public record SetDocumentFetchPdfTimeoutCommand(
    Guid JobId,
    Guid DocumentId,
    int? TimeoutSeconds) : IRequest<SetDocumentFetchPdfTimeoutResultDto>;

public sealed class SetDocumentFetchPdfTimeoutHandler
    : IRequestHandler<SetDocumentFetchPdfTimeoutCommand, SetDocumentFetchPdfTimeoutResultDto>
{
    private readonly IDocumentRepository _documents;

    public SetDocumentFetchPdfTimeoutHandler(IDocumentRepository documents)
    {
        _documents = documents;
    }

    public async Task<SetDocumentFetchPdfTimeoutResultDto> Handle(
        SetDocumentFetchPdfTimeoutCommand request,
        CancellationToken cancellationToken)
    {
        if (request.TimeoutSeconds is int v && (v < 60 || v > 900))
        {
            throw new DomainException("El timeout debe estar entre 60 y 900 segundos, o null para quitarlo.");
        }

        var ok = await _documents.TrySetFetchPdfTimeoutAsync(
            request.JobId,
            request.DocumentId,
            request.TimeoutSeconds,
            cancellationToken);

        if (!ok)
        {
            throw new NotFoundException(
                $"Document {request.DocumentId} not found for job {request.JobId}, or concurrent update.");
        }

        var msg = request.TimeoutSeconds.HasValue
            ? $"Timeout de descarga PDF fijado en {request.TimeoutSeconds.Value}s para este documento. Reencolá el Fetcher para aplicarlo."
            : "Timeout especial eliminado; el Fetcher usará el valor por defecto del worker.";

        return new SetDocumentFetchPdfTimeoutResultDto(msg);
    }
}
