using LegalAiAr.Application.Rulings.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Rulings.Queries.GetRulingById;

/// <summary>
/// Query for full ruling details by ID.
/// </summary>
public record GetRulingByIdQuery(Guid Id, bool AllowReprocessingView = false) : IRequest<RulingDto>;
