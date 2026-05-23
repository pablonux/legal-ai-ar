using System.Text.Json;
using LegalAiAr.Core.Exceptions;

namespace LegalAiAr.Infrastructure.Crawling.Sources;

/// <summary>
/// Parses CSJN paginarFallos.html JSON response into document records.
/// Implements defensive versioning (R-001): throws <see cref="CsjnSchemaViolationException"/>
/// when required fields (Codigo, idAnalisis) are missing or have wrong type.
/// </summary>
public static class CsjnPaginationParser
{
    private const int CsjnSourceId = 1;

    /// <summary>
    /// Parses the pagination response (JSON with Records/records array) and returns document records with discovery metadata.
    /// </summary>
    /// <param name="pageSource">Raw page source (typically JSON from paginarFallos.html).</param>
    /// <param name="pageIndex">Zero-based page index, for error context.</param>
    /// <returns>List of discovered records with metadata. Empty if not JSON or no Records.</returns>
    /// <exception cref="CsjnSchemaViolationException">When a record in Records has missing required fields (Codigo, idAnalisis) or wrong types.</exception>
    /// <exception cref="CsjnSessionTimeoutException">When CSJN returns session timeout ("Ha excedido el tiempo de inactividad").</exception>
    public static IReadOnlyList<DiscoveredRecord> ParseRecordsWithMetadata(string pageSource, int pageIndex = 0)
    {
        var results = new List<DiscoveredRecord>();

        var trimmed = (pageSource ?? string.Empty).Trim();

        // Detect session timeout (HTML/XML or JSON with Result=ERROR)
        if (ContainsSessionTimeoutError(trimmed))
        {
            throw new CsjnSessionTimeoutException(
                "CSJN session timeout: 'Ha excedido el tiempo de inactividad'. Re-submit search and retry.");
        }

        if (!trimmed.StartsWith('{') && !trimmed.StartsWith('['))
        {
            return results;
        }

        try
        {
            using var doc = JsonDocument.Parse(pageSource ?? string.Empty);
            var root = doc.RootElement;

            // Check for Result=ERROR (session timeout or other error)
            if (root.TryGetProperty("Result", out var resultEl) || root.TryGetProperty("result", out resultEl))
            {
                var result = resultEl.GetString() ?? string.Empty;
                if (result.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                {
                    var msg = root.TryGetProperty("Message", out var msgEl) || root.TryGetProperty("message", out msgEl)
                        ? msgEl.GetString() ?? string.Empty
                        : string.Empty;
                    if (msg.Contains("inactividad", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new CsjnSessionTimeoutException(
                            "CSJN session timeout: 'Ha excedido el tiempo de inactividad'. Re-submit search and retry.");
                    }
                }
            }

            if (!root.TryGetProperty("Records", out var recordsEl))
            {
                root.TryGetProperty("records", out recordsEl);
            }

            if (recordsEl.ValueKind == JsonValueKind.Array)
            {
                var index = 0;
                foreach (var record in recordsEl.EnumerateArray())
                {
                    if (!TryParseRecord(record, out var docId, out var analysisId, out var violationReason))
                    {
                        throw new CsjnSchemaViolationException(
                            $"CSJN pagination schema violation at page {pageIndex}, record {index}: {violationReason}",
                            CsjnSourceId,
                            $"page={pageIndex}, record={index}");
                    }
                    var caseNumber = GetStringOrEmpty(record, "identificadorExpediente");
                    var subjectCode = GetStringOrEmpty(record, "materiaSecretaria");
                    var rulingYear = GetStringOrEmpty(record, "anioFallo");
                    var hasVotes = GetStringOrEmpty(record, "tieneVotos").Equals("S", StringComparison.OrdinalIgnoreCase);
                    results.Add(new DiscoveredRecord(docId, analysisId, caseNumber, subjectCode, rulingYear, hasVotes));
                    index++;
                }
            }
        }
        catch (JsonException)
        {
            // Malformed JSON: return empty (transient or non-pagination response)
        }

        return results;
    }

    /// <summary>
    /// Backward-compatible: returns only (DocumentId, AnalysisId) tuples.
    /// </summary>
    public static IReadOnlyList<(string DocumentId, string AnalysisId)> ParseRecords(string pageSource, int pageIndex = 0)
    {
        return ParseRecordsWithMetadata(pageSource, pageIndex)
            .Select(r => (r.DocumentId, r.AnalysisId))
            .ToList();
    }

    private static bool TryParseRecord(JsonElement record, out string documentId, out string analysisId, out string? violationReason)
    {
        documentId = string.Empty;
        analysisId = string.Empty;
        violationReason = null;

        if (!record.TryGetProperty("Codigo", out var codigoEl) && !record.TryGetProperty("codigo", out codigoEl))
        {
            violationReason = "missing required field Codigo";
            return false;
        }

        if (!record.TryGetProperty("idAnalisis", out var idAnalisisEl) && !record.TryGetProperty("IdAnalisis", out idAnalisisEl))
        {
            violationReason = "missing required field idAnalisis";
            return false;
        }

        documentId = GetStringOrNumber(codigoEl);
        analysisId = GetStringOrEmpty(idAnalisisEl);

        if (string.IsNullOrEmpty(documentId))
        {
            violationReason = "Codigo is null, empty, or wrong type (expected string or number)";
            return false;
        }

        if (string.IsNullOrEmpty(analysisId))
        {
            violationReason = "idAnalisis is null, empty, or wrong type (expected string)";
            return false;
        }

        return true;
    }

    private static string GetStringOrEmpty(JsonElement el)
    {
        if (el.ValueKind != JsonValueKind.String)
            return string.Empty;
        return el.GetString() ?? string.Empty;
    }

    private static string GetStringOrEmpty(JsonElement parent, string propertyName)
    {
        if (parent.TryGetProperty(propertyName, out var el))
            return GetStringOrEmpty(el);
        return string.Empty;
    }

    /// <summary>
    /// Gets string value, or converts number to string. CSJN returns Codigo as int in JSON.
    /// </summary>
    private static string GetStringOrNumber(JsonElement el)
    {
        return el.ValueKind switch
        {
            JsonValueKind.String => el.GetString() ?? string.Empty,
            JsonValueKind.Number => el.TryGetInt32(out var i) ? i.ToString() : el.GetRawText(),
            _ => string.Empty
        };
    }

    /// <summary>
    /// Detects CSJN session timeout in response (HTML/XML or JSON).
    /// </summary>
    private static bool ContainsSessionTimeoutError(string content)
    {
        if (string.IsNullOrEmpty(content))
            return false;

        var lower = content.ToLowerInvariant();
        return lower.Contains("ha excedido el tiempo de inactividad")
            || (lower.Contains("<result>error</result>") && lower.Contains("inactividad"));
    }
}

/// <summary>
/// Record discovered from paginarFallos.html with optional metadata fields.
/// </summary>
public record DiscoveredRecord(
    string DocumentId,
    string AnalysisId,
    string CaseNumber,
    string SubjectCode,
    string RulingYear,
    bool HasVotes);
