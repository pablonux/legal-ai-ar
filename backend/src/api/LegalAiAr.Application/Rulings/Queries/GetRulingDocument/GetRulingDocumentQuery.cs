using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Rulings.Queries.GetRulingDocument;

public record GetRulingDocumentQuery(Guid Id) : IRequest<RulingDocumentResult?>;

public record RulingDocumentResult(Stream Content, string ContentType);
