using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Statutes;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Statutes.Queries.GetStatutePyramid;

public class GetStatutePyramidHandler : IRequestHandler<GetStatutePyramidQuery, IReadOnlyList<PyramidLevelDto>>
{
    private static readonly Dictionary<NormativeLevel, string> Labels = new()
    {
        [NormativeLevel.CONSTITUTIONAL] = "Constitucional",
        [NormativeLevel.SUPRALEGAL] = "Supralegal (Tratados)",
        [NormativeLevel.LEGAL] = "Legal (Leyes)",
        [NormativeLevel.REGULATORY] = "Reglamentario (Decretos / Resoluciones)",
        [NormativeLevel.INDIVIDUAL] = "Individual (Actos Administrativos)",
    };

    private readonly IStatuteRepository _repo;

    public GetStatutePyramidHandler(IStatuteRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<PyramidLevelDto>> Handle(
        GetStatutePyramidQuery request, CancellationToken cancellationToken)
    {
        var countsTask = _repo.GetCountsByNormativeLevelAsync(cancellationToken);
        var vigenteTask = _repo.GetVigenteCountsByNormativeLevelAsync(cancellationToken);
        await Task.WhenAll(countsTask, vigenteTask);

        var counts = countsTask.Result;
        var vigente = vigenteTask.Result;

        return Enum.GetValues<NormativeLevel>()
            .OrderBy(l => (int)l)
            .Select(level => new PyramidLevelDto(
                level.ToString(),
                Labels.GetValueOrDefault(level, level.ToString()),
                counts.GetValueOrDefault(level, 0),
                vigente.GetValueOrDefault(level, 0)))
            .ToList();
    }
}
