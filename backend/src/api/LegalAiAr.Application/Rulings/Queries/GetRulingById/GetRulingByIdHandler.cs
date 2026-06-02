using LegalAiAr.Application.Common.Mappings;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Rulings;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Rulings.Queries.GetRulingById;

/// <summary>
/// Handles retrieval of full ruling details by ID.
/// </summary>
public class GetRulingByIdHandler : IRequestHandler<GetRulingByIdQuery, RulingDto>
{
    private readonly IRulingRepository _rulingRepository;

    public GetRulingByIdHandler(IRulingRepository rulingRepository)
    {
        _rulingRepository = rulingRepository;
    }

    /// <inheritdoc />
    public async Task<RulingDto> Handle(GetRulingByIdQuery request, CancellationToken cancellationToken)
    {
        var ruling = await _rulingRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Ruling not found.");

        if (ruling.Status == RulingStatus.Reprocessing && !request.AllowReprocessingView)
            throw new NotFoundException("Ruling not found.");

        return RulingMapper.ToDto(ruling);
    }
}
