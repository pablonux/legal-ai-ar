using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Models;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Application.Admin.Jobs.Queries.GetJobAudit;

/// <summary>
/// Read-only audit: job counters vs <c>Documents</c>, DB pause flags, SignalR presence, and pipeline rows flagged for review.
/// </summary>
/// <remarks>
/// Rows in <see cref="IDocumentRepository.GetAuditRiskPipelineDocumentsForJobAsync"/> (especially <c>Processing</c>)
/// are hints, not verdicts: meaning depends on whether workers are connected, queues still have work for this job,
/// and the pipeline is advancing. See that method's documentation for the healthy vs post-incident interpretation.
/// </remarks>
public sealed class GetJobAuditHandler : IRequestHandler<GetJobAuditQuery, JobAuditDto>
{
    private static readonly string[] WorkerTypesOrder =
        ["Discoverer", "Fetcher", "Parser", "Enricher", "Persister", "Indexer"];

    private readonly IIngestionJobRepository _jobs;
    private readonly IDocumentRepository _documents;
    private readonly IWorkerPauseStateRepository _workerPauses;
    private readonly IWorkerSignalRPresenceTracker _presence;
    private readonly IQueueMetricsService _queueMetrics;
    private readonly PipelineQueueNames _queueNames;

    public GetJobAuditHandler(
        IIngestionJobRepository jobs,
        IDocumentRepository documents,
        IWorkerPauseStateRepository workerPauses,
        IWorkerSignalRPresenceTracker presence,
        IQueueMetricsService queueMetrics,
        PipelineQueueNames queueNames)
    {
        _jobs = jobs;
        _documents = documents;
        _workerPauses = workerPauses;
        _presence = presence;
        _queueMetrics = queueMetrics;
        _queueNames = queueNames;
    }

    public async Task<JobAuditDto> Handle(GetJobAuditQuery request, CancellationToken cancellationToken)
    {
        var job = await _jobs.GetByIdAsync(request.JobId, cancellationToken)
            ?? throw new NotFoundException($"IngestionJob {request.JobId} not found.");

        var db = await _documents.GetDocumentStatusCountsForJobAsync(request.JobId, cancellationToken);
        var pauseRows = await _workerPauses.GetAllAsync(cancellationToken);
        var liveCounts = _presence.GetConnectedCountsByWorkerType();
        var riskDocs = await _documents.GetAuditRiskPipelineDocumentsForJobAsync(request.JobId, 100, cancellationToken);

        var mainQueueCounts = await Task.WhenAll(
            _queueNames.AllMain.Select(q => _queueMetrics.GetApproximateMessageCountAsync(q, cancellationToken)));
        var approxMainPipelineQueueTotal = mainQueueCounts.Sum();

        var indexedDelta = job.DocumentsIndexed - db.Completed;
        var failedDelta = job.DocumentsFailed - db.Failed;

        var expected = job.DocumentsDiscovered - job.DocumentsSkipped;
        var satisfiesCompletion = expected > 0 && job.DocumentsIndexed + job.DocumentsFailed >= expected;
        var formulaSatisfiedButStillOutstanding =
            satisfiesCompletion && db.OutstandingPendingOrProcessing > 0;

        var structuralRepairSafety = BuildStructuralRepairSafety(
            db, liveCounts, approxMainPipelineQueueTotal);

        var notes = new List<string>();

        if (job.InfrastructureDegraded)
        {
            notes.Insert(0,
                "Este job está marcado como infra degradada en BD (incidente de worker/colación). Verificá el probe de colas Discoverer/Fetcher más abajo; si ambas responden OK, podés ejecutar «Recuperar pipeline».");
        }

        if (liveCounts.Values.Sum() == 0)
        {
            notes.Add(
                "Ningún proceso worker tiene conexión SignalR activa al API. Los chips «Activo» solo reflejan pausa en BD, no que el proceso esté levantado. Tras reiniciar el API, los workers deben reconectar.");
        }

        if (indexedDelta != 0)
        {
            notes.Add(
                $"El contador del job «Indexados» ({job.DocumentsIndexed}) no coincide con filas «Completed» en Documents ({db.Completed}); delta = {indexedDelta}. Suele indicar desvío tras cortes o reintentos.");
        }

        if (failedDelta != 0)
        {
            notes.Add(
                $"El contador «Fallidos» del job ({job.DocumentsFailed}) no coincide con filas «Failed» ({db.Failed}); delta = {failedDelta}.");
        }

        if (formulaSatisfiedButStillOutstanding)
        {
            notes.Add(
                "La fórmula Indexed+Failed ≥ Discovered−Skipped se cumple, pero siguen filas Pending/Processing: el job puede permanecer en «processing» hasta resolverlas (p. ej. cola perdida en Persister/Indexer).");
        }

        if (db.OutstandingPendingOrProcessing > 0)
        {
            var paused = pauseRows.Where(w => w.IsPaused).Select(w => w.WorkerType).ToList();
            if (paused.Count > 0)
            {
                notes.Add(
                    $"Hay {db.OutstandingPendingOrProcessing} documento(s) Pending/Processing y estos workers están en pausa en BD: {string.Join(", ", paused)}.");
            }
        }

        if (db.Total > 0 && db.Total != job.DocumentsDiscovered)
        {
            notes.Add(
                $"Filas en Documents para este job: {db.Total}; «DocumentsDiscovered» en el job: {job.DocumentsDiscovered}. Revisar reasignación de job o descubrimiento.");
        }

        var pendingTail = riskDocs.Where(d =>
            d.Status == DocumentStatus.Pending
            && (d.CurrentStage == PipelineStage.Persister || d.CurrentStage == PipelineStage.Indexer)).ToList();
        if (pendingTail.Count > 0)
        {
            notes.Add(
                $"{pendingTail.Count} documento(s) en Pending en etapa Persister o Indexer (posible mensaje de cola perdido o skip del worker). Podés usar «Marcar cola final como fallo» para cerrarlos con mensaje de auditoría.");
        }

        var processingRisk = riskDocs.Count(d => d.Status == DocumentStatus.Processing);
        if (processingRisk > 0)
        {
            var workersLive = liveCounts.Values.Sum() > 0;
            if (workersLive)
            {
                notes.Add(
                    $"{processingRisk} documento(s) en Processing: con workers conectados por SignalR suele ser trabajo en curso normal, no un fallo automático. Revisar solo si el job dejó de avanzar y las colas para este trabajo están vacías.");
            }
            else
            {
                notes.Add(
                    $"{processingRisk} documento(s) en Processing: sin workers en SignalR pueden ser restos de caída. Ojo: tras un corte, unos mensajes pueden perderse en vuelo y otros seguir por cola; esos perdidos a veces solo se notan cuando el resto del job ya terminó, las colas están vacías y los workers quietos, pero la fila sigue en Processing.");
            }
        }

        if (structuralRepairSafety.SuggestedAdministrativeSafeWindow)
        {
            notes.Add(
                "Heurística «ventana administrativa»: sin filas Processing para este job en SQL y colas principales del pipeline en ~0 mensajes. Aún así no prueba ausencia de mensajes solo de este job ni trabajo en vuelo; conviene confirmar que no haya otros jobs activos.");
        }

        if (notes.Count == 0)
        {
            notes.Add("No se detectaron señales críticas adicionales (revisión básica).");
        }

        var pauseByType = pauseRows.ToDictionary(w => w.WorkerType, w => w, StringComparer.OrdinalIgnoreCase);
        var workerDtos = WorkerTypesOrder.Select(t =>
        {
            pauseByType.TryGetValue(t, out var ps);
            var n = liveCounts.TryGetValue(t, out var c) ? c : 0;
            return new WorkerAuditDto(t, ps?.IsPaused ?? false, ps?.UpdatedAt, n);
        }).ToList();

        var riskDtos = riskDocs
            .Select(d => new StuckPipelineDocumentDto(d.Id, d.ExternalId, d.CurrentStage.ToString(), d.Status.ToString()))
            .ToList();

        var dbDto = new DocumentStatusCountsDto(
            db.Pending,
            db.Processing,
            db.Completed,
            db.Failed,
            db.Discarded,
            db.Cancelled,
            db.Total,
            db.OutstandingPendingOrProcessing);

        return new JobAuditDto(
            job.Id,
            job.Source?.Name ?? $"Source {job.SourceId}",
            job.Status,
            job.StartedAt,
            job.CompletedAt,
            job.DocumentsDiscovered,
            job.DocumentsSkipped,
            job.DocumentsCrawled,
            job.DocumentsParsed,
            job.DocumentsEnriched,
            job.DocumentsPersisted,
            job.DocumentsIndexed,
            job.DocumentsFailed,
            db.Total,
            dbDto,
            indexedDelta,
            failedDelta,
            satisfiesCompletion,
            formulaSatisfiedButStillOutstanding,
            workerDtos,
            riskDtos,
            structuralRepairSafety,
            job.InfrastructureDegraded,
            job.DegradedSinceUtc,
            job.DegradedReason,
            notes,
            DateTime.UtcNow);
    }

    private static StructuralRepairSafetyDto BuildStructuralRepairSafety(
        DocumentStatusCountSet db,
        IReadOnlyDictionary<string, int> liveCounts,
        int approxMainPipelineQueueTotal)
    {
        var noProcessing = db.Processing == 0;
        var anySignalR = liveCounts.Values.Sum() > 0;
        var suggested = noProcessing && approxMainPipelineQueueTotal == 0;

        var caveats = new List<string>
        {
            "No se puede garantizar «cero mensajes solo para este job» sin leer cuerpos de mensaje: las colas son compartidas y Azure no indexa por IngestionJobId.",
            "Los workers no publican el job/documento en curso: SignalR solo indica si hay proceso conectado por tipo.",
            "Los mensajes en visibilidad tras Receive no suelen figurar en el conteo aproximado de la cola.",
        };

        if (!noProcessing)
        {
            caveats.Add(
                $"Hay {db.Processing} fila(s) Processing para este job en SQL: un worker podría seguir tocándolas; no es el momento más seguro para cambios estructurales en BD.");
        }

        if (approxMainPipelineQueueTotal > 0)
        {
            caveats.Add(
                $"Suma aproximada de mensajes en colas principales del pipeline: {approxMainPipelineQueueTotal} (>0). Puede ser carga de otros jobs o mensajes pendientes de este.");
        }

        if (suggested)
        {
            caveats.Add(
                "Heurística alineada: sin Processing para este job y conteos aproximados de colas principales en 0. Útil como ventana administrativa si validás que no hay otros jobs en curso.");
        }

        return new StructuralRepairSafetyDto(
            noProcessing,
            approxMainPipelineQueueTotal,
            anySignalR,
            suggested,
            caveats);
    }
}
