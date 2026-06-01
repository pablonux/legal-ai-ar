using System.Diagnostics;
using System.Text.Json;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Persister.Strategies;

/// <summary>
/// Persists a statute entity from PersisterPayload.StatutePayloadData.
/// Uses IStatuteRepository for GetOrCreate + update pattern.
/// </summary>
public sealed class StatutePersistStrategy : IPersistStrategy
{
    private readonly IStatuteRepository _statuteRepository;
    private readonly IBlobStorageService _blobStorage;
    private readonly ILogger<StatutePersistStrategy> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public StatutePersistStrategy(
        IStatuteRepository statuteRepository,
        IBlobStorageService blobStorage,
        ILogger<StatutePersistStrategy> logger)
    {
        _statuteRepository = statuteRepository;
        _blobStorage = blobStorage;
        _logger = logger;
    }

    public async Task<PersistResult> PersistAsync(
        PersisterMessage message,
        CancellationToken cancellationToken = default)
    {
        var hashPrefix = message.ContentHash is { Length: > 0 }
            ? message.ContentHash[..Math.Min(8, message.ContentHash.Length)]
            : "";

        var sw = Stopwatch.StartNew();
        var payload = await DownloadPayloadAsync(message.PayloadBlobPath!, cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister statute phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix}",
            "BlobJsonPayload", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix);

        if (payload.Statute is null)
            throw new InvalidOperationException("PersisterPayload.Statute is null for a Statute entity");

        var data = payload.Statute;

        sw.Restart();
        var statute = await _statuteRepository.GetOrCreateAsync(data.Number, data.Name, cancellationToken);
        var isNew = statute.SaijId is null;

        statute.Name = data.Name;
        statute.SaijId = data.SaijId ?? message.DocumentId.ToString();
        statute.IssuingBodyName = data.IssuingBody;
        statute.IssuingBody = data.IssuingBody;
        statute.FullText = data.FullText;
        statute.OfficialUrl = data.OfficialUrl;

        if (data.NormType != null && Enum.TryParse<NormType>(data.NormType, true, out var nt))
            statute.NormType = nt;
        if (data.NormativeLevel != null && Enum.TryParse<NormativeLevel>(data.NormativeLevel, true, out var nl))
            statute.NormativeLevel = nl;
        if (data.Status != null && Enum.TryParse<StatuteStatus>(data.Status, true, out var st))
            statute.Status = st;
        if (data.SanctionDate.HasValue)
            statute.SanctionDate = data.SanctionDate.Value;
        if (data.PublicationDate.HasValue)
            statute.PublicationDate = data.PublicationDate.Value;

        await _statuteRepository.SaveChangesAsync(cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister statute phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix} StatuteId={StatuteId} IsNew={IsNew}",
            "GetOrCreateAndSave", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix, statute.Id, isNew);

        _logger.LogInformation("Persisted Statute Id={StatuteId} Number={Number} Name={Name}",
            statute.Id, statute.Number, statute.Name);

        return new PersistResult(
            EntityId: GuidFromInt(statute.Id),
            IsNew: isNew);
    }

    private static Guid GuidFromInt(int id)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(id).CopyTo(bytes, 0);
        return new Guid(bytes);
    }

    private async Task<PersisterPayload> DownloadPayloadAsync(string blobPath, CancellationToken ct)
    {
        await using var stream = await _blobStorage.DownloadAsync(blobPath, ct);
        var payload = await JsonSerializer.DeserializeAsync<PersisterPayload>(stream, JsonOptions, ct);
        return payload ?? throw new InvalidOperationException($"Failed to deserialize PersisterPayload from {blobPath}");
    }
}
