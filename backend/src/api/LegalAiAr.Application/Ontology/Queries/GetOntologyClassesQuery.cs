using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Ontology.Models;

namespace LegalAiAr.Application.Ontology.Queries;

public record GetOntologyClassesQuery : IRequest<OntologyClassesResponse>;

public class GetOntologyClassesHandler : IRequestHandler<GetOntologyClassesQuery, OntologyClassesResponse>
{
    private readonly OntologyModelProvider _model;

    public GetOntologyClassesHandler(OntologyModelProvider model) => _model = model;

    public Task<OntologyClassesResponse> Handle(GetOntologyClassesQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new OntologyClassesResponse(_model.GetClasses()));
    }
}
