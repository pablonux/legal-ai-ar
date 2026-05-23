using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using LegalAiAr.Infrastructure.Blob;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Messages;
using LegalAiAr.Infrastructure.Crawling;
using LegalAiAr.Infrastructure.Crawling.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Worker.Parser.Parsers;

/// <summary>
/// Fetches metadata from the 8 CSJN sjconsulta REST endpoints and returns CsjnApiMetadata.
/// Implements throttling (R-002) and defensive schema validation (R-001).
/// Blob cache: responses are written through after each successful GET; when <see cref="CsjnApiOptions.PreferBlobApiCacheBeforeHttp"/>
/// is true (default), cached JSON under <c>_cache/csjn/api/</c> is read before HTTP even if the pipeline message has <c>UseCache=false</c>.
/// Post-<c>abrirAnalisis</c> endpoints can run in parallel when <see cref="CsjnApiOptions.PostAbrirParallelEnabled"/> is true,
/// coordinated by <see cref="CsjnApiRequestGate"/> inside <see cref="CsjnSjconsultaJsonTransport"/>.
/// </summary>
public partial class CsjnApiParser
{
    private const int SourceId = 1; // CSJN

    private readonly IOptions<CsjnApiOptions> _options;
    private readonly ILogger<CsjnApiParser> _logger;
    private readonly CsjnSjconsultaJsonTransport _sjconsultaTransport;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public CsjnApiParser(
        IOptions<CsjnApiOptions> options,
        ILogger<CsjnApiParser> logger,
        CsjnSjconsultaJsonTransport sjconsultaTransport)
    {
        _options = options;
        _logger = logger;
        _sjconsultaTransport = sjconsultaTransport;
    }

    /// <summary>
    /// Fetches metadata from all 8 CSJN endpoints and merges into CsjnApiMetadata.
    /// </summary>
    public async Task<CsjnApiMetadata> FetchMetadataAsync(
        string documentId,
        string analysisId,
        CancellationToken cancellationToken = default,
        bool useCache = false,
        DateOnly? rulingDateHint = null,
        string? caseNumberHint = null)
    {
        if (string.IsNullOrWhiteSpace(analysisId))
        {
            throw new ArgumentException("AnalysisId is required for CSJN API calls.", nameof(analysisId));
        }

        var baseUrl = _options.Value.BaseUrl.TrimEnd('/');
        var outbound = _options.Value.OutboundHttpEnabled;

        // 1. abrirAnalisis — case-level metadata (richest endpoint)
        var abrir = await _sjconsultaTransport.GetJsonWithRetryAsync(
            $"{baseUrl}/fallos/abrirAnalisis.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
            documentId,
            CachePathHelper.ApiCacheKey(SourceId, "abrirAnalisis", analysisId),
            useCache,
            outbound,
            cancellationToken);

        var caseTitle = GetString(abrir, "tituloCausa", "caseTitle", "titulo", "caratula")
            ?? GetStringFromFirstRecord(abrir, "tituloCausa", "caseTitle", "titulo", "caratula")
            ?? GetStringFromRecordObject(abrir, "tituloCausa", "caseTitle", "titulo", "caratula")
            ?? GetStringFromValorObject(abrir, "caratula")
            ?? GetStringCaseInsensitive(abrir, "tituloCausa", "caseTitle", "titulo", "caratula", "Caratula", "TituloCausa")
            ?? GetStringFromNestedWrappers(abrir, "tituloCausa", "caseTitle", "titulo", "caratula");

        if (string.IsNullOrWhiteSpace(caseTitle))
        {
            caseTitle = $"[Sin carátula] Doc {documentId}";
            _logger.LogWarning("abrirAnalisis response missing caseTitle for document {DocumentId}, using fallback", documentId);
        }

        // FIX: CaseNumber — read identificacionExpediente (root) then claveRecurso (nested)
        var caseNumber = GetString(abrir, "identificacionExpediente")
            ?? GetStringFromNested(abrir, "recursoExpediente", "claveRecurso", "identificacionExpediente")
            ?? GetStringFromNested(abrir, "expediente", "oldIdentificadorExpediente");
        if (string.IsNullOrWhiteSpace(caseNumber) && !string.IsNullOrWhiteSpace(caseNumberHint))
        {
            caseNumber = caseNumberHint;
            _logger.LogDebug("Using case number hint {CaseNumber} for document {DocumentId}", caseNumber, documentId);
        }

        // FIX: RulingDate — prefer hint, then falloDestacado.fecha, then sumarios fechaFallo (below), then reciboEntrada
        DateOnly rulingDate;
        if (rulingDateHint.HasValue)
        {
            rulingDate = rulingDateHint.Value;
            _logger.LogDebug("Using ruling date hint {Date} for document {DocumentId}", rulingDate, documentId);
        }
        else
        {
            rulingDate = ParseRulingDate(abrir, documentId);
        }

        var jurisdiction = GetString(abrir, "jurisdiccion", "jurisdiction")
            ?? GetStringFromValorObject(abrir, "competencia");
        var resourceType = GetString(abrir, "tipoRecurso", "resourceType")
            ?? GetStringFromValorObject(abrir, "tipoRecurso");
        var rulingDirection = GetString(abrir, "sentido", "rulingDirection")
            ?? GetStringFromValorObject(abrir, "sentidoPronunciamiento");
        var subjectArea = GetString(abrir, "materia", "subjectArea")
            ?? GetStringFromValorObject(abrir, "materiaSecretaria");
        var isUnconstitutional = GetBool(abrir, "inconstitucional", "isUnconstitutional");

        // FIX: Summary — falloDestacado is an OBJECT, extract titulo/cabecilla/resumen
        var summary = ExtractFalloDestacadoSummary(abrir);
        if (string.IsNullOrWhiteSpace(summary))
        {
            summary = GetString(abrir, "sintesis", "featuredRuling", "summary", "resumen");
        }

        // New fields from abrirAnalisis
        var actionType = GetStringFromValorObject(abrir, "tipoAccion");
        var internalSubject = GetStringFromValorObject(abrir, "materia");
        var observations = GetString(abrir, "observaciones");
        var federalQuestion = ExtractFederalQuestion(abrir);
        var proceduralFormula = ExtractFormula(abrir);
        var votes = ParseVotesStructured(abrir);
        var apiStatutes = ParseReferenciasNormativas(abrir);

        var (documentos, sumarios, citas, citantes, dictamenes, sintesisJson, enlacesJson) =
            await _sjconsultaTransport.FetchPostAbrirEndpointsAsync(baseUrl, documentId, analysisId, useCache, outbound, cancellationToken)
                .ConfigureAwait(false);

        if (!ValidateDocumentExists(documentos, documentId))
        {
            _logger.LogWarning("Document {DocumentId} not found in getAllDocumentos response", documentId);
        }

        var officialReference = ExtractOfficialReference(documentos);

        // FIX: Holding — read 'texto' field (the actual headnote), NOT 'holding' (numeric flag)
        var holding = ExtractSumarioTexto(sumarios);

        // Keywords: prefer abrirAnalisis.voces (always populated); fall back to sumarios
        var keywords = ParseVoces(abrir);
        if (keywords.Count == 0)
            keywords = ParseKeywords(sumarios);

        if (string.IsNullOrEmpty(summary))
        {
            summary = ExtractSumarioTexto(sumarios);
        }

        // Extract tomo/pagina from sumarios (higher priority than getAllDocumentos)
        var sumarioRef = ExtractSumarioOfficialReference(sumarios);
        if (!string.IsNullOrEmpty(sumarioRef))
            officialReference = sumarioRef;

        // Extract fechaFallo from sumarios as fallback for ruling date
        if (!rulingDateHint.HasValue)
        {
            var sumarioDate = ExtractSumarioFechaFallo(sumarios);
            if (sumarioDate.HasValue)
            {
                rulingDate = sumarioDate.Value;
            }
        }

        // Extract tieneDictamenes flag from sumarios
        var hasDictamenFromSumarios = ExtractTieneDictamenes(sumarios);

        // Parse full sumarios array (structured)
        var parsedSumarios = ParseSumariosStructured(sumarios);

        var citations = ParseCitations(citas);

        var citedBy = ParseCitedBy(citantes);

        var hasDictamen = hasDictamenFromSumarios || HasDictamenData(dictamenes);
        var parsedDictamenes = ParseDictamenes(dictamenes);

        var syntheses = ParseSintesis(sintesisJson);

        var links = ParseEnlaces(enlacesJson);

        return new CsjnApiMetadata(
            CaseTitle: caseTitle,
            RulingDate: rulingDate,
            CaseNumber: caseNumber,
            Jurisdiction: jurisdiction,
            ResourceType: resourceType,
            RulingDirection: rulingDirection,
            SubjectArea: subjectArea,
            IsUnconstitutional: isUnconstitutional,
            Summary: summary,
            Holding: holding,
            Keywords: keywords,
            Citations: citations,
            CitedBy: citedBy,
            ActionType: actionType,
            InternalSubject: internalSubject,
            OfficialReference: officialReference,
            Observations: observations,
            FederalQuestion: federalQuestion,
            ProceduralFormula: proceduralFormula,
            HasDictamen: hasDictamen,
            Votes: votes,
            ApiStatutes: apiStatutes,
            Sumarios: parsedSumarios,
            Syntheses: syntheses,
            Links: links,
            Dictamenes: parsedDictamenes);
    }

    #region FalloDestacado extraction

    /// <summary>
    /// Extracts summary from falloDestacado object: prefers resumen, then cabecilla (strip HTML), then titulo.
    /// </summary>
    private static string? ExtractFalloDestacadoSummary(JsonElement abrir)
    {
        if (abrir.ValueKind != JsonValueKind.Object) return null;
        if (!abrir.TryGetProperty("falloDestacado", out var fd) || fd.ValueKind != JsonValueKind.Object)
            return null;

        var resumen = GetString(fd, "resumen");
        if (!string.IsNullOrWhiteSpace(resumen)) return resumen;

        var cabecilla = GetString(fd, "cabecilla");
        if (!string.IsNullOrWhiteSpace(cabecilla))
            return StripHtml(cabecilla);

        return GetString(fd, "titulo");
    }

    /// <summary>
    /// Extracts ruling date from falloDestacado.fecha (epoch ms).
    /// </summary>
    private static DateOnly? ExtractFalloDestacadoDate(JsonElement abrir)
    {
        if (abrir.ValueKind != JsonValueKind.Object) return null;
        if (!abrir.TryGetProperty("falloDestacado", out var fd) || fd.ValueKind != JsonValueKind.Object)
            return null;
        if (!fd.TryGetProperty("fecha", out var fechaProp)) return null;

        if (fechaProp.TryGetInt64(out var epochMs) && epochMs > 1000000000000)
        {
            return DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(epochMs).DateTime);
        }
        return null;
    }

    #endregion

    #region Sumarios extraction

    /// <summary>Unwraps CSJN sumarios payload: top-level array or <c>Record</c>/<c>Records</c> array.</summary>
    private static bool TryGetSumariosArray(JsonElement el, out JsonElement arrayRoot)
    {
        if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() > 0)
        {
            arrayRoot = el;
            return true;
        }

        if (el.ValueKind == JsonValueKind.Object)
        {
            if (el.TryGetProperty("Record", out var record) &&
                record.ValueKind == JsonValueKind.Array &&
                record.GetArrayLength() > 0)
            {
                arrayRoot = record;
                return true;
            }

            if (el.TryGetProperty("Records", out var records) &&
                records.ValueKind == JsonValueKind.Array &&
                records.GetArrayLength() > 0)
            {
                arrayRoot = records;
                return true;
            }
        }

        arrayRoot = default;
        return false;
    }

    private static bool TryGetFirstSumarioItem(JsonElement sumarios, out JsonElement first)
    {
        if (!TryGetSumariosArray(sumarios, out var arr) || arr.GetArrayLength() == 0)
        {
            first = default;
            return false;
        }

        first = arr[0];
        return true;
    }

    /// <summary>
    /// Extracts the doctrinal headnote text from getSumariosAnalisis[0].texto.
    /// Skips numeric 'holding' field which is a flag, not text.
    /// </summary>
    private static string? ExtractSumarioTexto(JsonElement sumarios)
    {
        if (!TryGetFirstSumarioItem(sumarios, out var first))
            return null;

        var texto = GetString(first, "texto");
        if (!string.IsNullOrWhiteSpace(texto))
            return StripHtml(texto);

        var considerando = GetString(first, "considerando");
        if (!string.IsNullOrWhiteSpace(considerando))
            return StripHtml(considerando);

        return null;
    }

    private static string? ExtractSumarioOfficialReference(JsonElement sumarios)
    {
        if (!TryGetFirstSumarioItem(sumarios, out var first))
            return null;

        var tomo = GetIntNullable(first, "tomo");
        var pagina = GetIntNullable(first, "pagina");
        if (tomo.HasValue && pagina.HasValue && tomo.Value > 0 && pagina.Value > 0)
            return $"Fallos: {tomo.Value}:{pagina.Value}";

        return null;
    }

    private static DateOnly? ExtractSumarioFechaFallo(JsonElement sumarios)
    {
        if (!TryGetFirstSumarioItem(sumarios, out var first))
            return null;

        if (first.TryGetProperty("fechaFallo", out var prop) && prop.TryGetInt64(out var epochMs) && epochMs > 1000000000000)
        {
            return DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(epochMs).DateTime);
        }
        return null;
    }

    private static bool ExtractTieneDictamenes(JsonElement sumarios)
    {
        if (!TryGetFirstSumarioItem(sumarios, out var first))
            return false;

        if (first.TryGetProperty("tieneDictamenes", out var prop) && prop.ValueKind == JsonValueKind.True)
            return true;
        return false;
    }

    #endregion

    #region New fields from abrirAnalisis

    private static string? ExtractFederalQuestion(JsonElement abrir)
    {
        if (abrir.ValueKind != JsonValueKind.Object) return null;
        if (!abrir.TryGetProperty("cuestionesFederales", out var arr) || arr.ValueKind != JsonValueKind.Array || arr.GetArrayLength() == 0)
            return null;

        var values = new List<string>();
        foreach (var item in arr.EnumerateArray())
        {
            var val = GetStringFromValorObject(item, "tipoCuestionFederal");
            if (!string.IsNullOrWhiteSpace(val))
                values.Add(val);
        }
        return values.Count > 0 ? string.Join("; ", values) : null;
    }

    private static string? ExtractFormula(JsonElement abrir)
    {
        if (abrir.ValueKind != JsonValueKind.Object) return null;
        if (!abrir.TryGetProperty("formulas", out var arr) || arr.ValueKind != JsonValueKind.Array || arr.GetArrayLength() == 0)
            return null;

        var values = new List<string>();
        foreach (var item in arr.EnumerateArray())
        {
            var val = GetStringFromValorObject(item, "tipoFormula");
            if (!string.IsNullOrWhiteSpace(val))
                values.Add(val);
        }
        return values.Count > 0 ? string.Join("; ", values) : null;
    }

    /// <summary>
    /// Parses votosAnalisisDocumental[] into structured vote records.
    /// Reads ministros[] array (with stable id/descripcion) when available, falls back to vocales string.
    /// </summary>
    private static IReadOnlyList<CsjnVoteDto> ParseVotesStructured(JsonElement abrir)
    {
        var list = new List<CsjnVoteDto>();
        if (abrir.ValueKind != JsonValueKind.Object) return list;
        if (!abrir.TryGetProperty("votosAnalisisDocumental", out var arr) || arr.ValueKind != JsonValueKind.Array)
            return list;

        foreach (var item in arr.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) continue;

            var voteType = GetStringFromValorObject(item, "tipoVoto") ?? GetString(item, "tipoVoto");
            var pages = GetString(item, "paginas");
            var nroPagina = item.TryGetProperty("nroPagina", out var np) && np.ValueKind == JsonValueKind.Number
                ? np.GetInt32().ToString()
                : null;

            var ministers = ParseMinistros(item);
            var vocales = GetString(item, "vocales");

            var judgesStr = ministers.Count > 0
                ? string.Join(", ", ministers.Select(m => m.Surname))
                : vocales ?? "";

            if (string.IsNullOrWhiteSpace(judgesStr) && ministers.Count == 0)
                continue;

            list.Add(new CsjnVoteDto(
                voteType ?? "DESCONOCIDO",
                judgesStr,
                pages ?? nroPagina,
                ministers.Count > 0 ? ministers : null));
        }

        return list;
    }

    private static List<CsjnMinistroDto> ParseMinistros(JsonElement voteItem)
    {
        var result = new List<CsjnMinistroDto>();
        if (!voteItem.TryGetProperty("ministros", out var arr) || arr.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var m in arr.EnumerateArray())
        {
            if (m.ValueKind != JsonValueKind.Object) continue;
            if (!m.TryGetProperty("id", out var idProp) || idProp.ValueKind != JsonValueKind.Number) continue;

            var id = idProp.GetInt32();
            var surname = GetString(m, "descripcion") ?? "";
            if (id > 0 && !string.IsNullOrWhiteSpace(surname))
                result.Add(new CsjnMinistroDto(id, surname.Trim()));
        }

        return result;
    }

    /// <summary>
    /// Parses referenciasNormativas[] into statute references.
    /// </summary>
    private static IReadOnlyList<CsjnApiStatuteDto> ParseReferenciasNormativas(JsonElement abrir)
    {
        var list = new List<CsjnApiStatuteDto>();
        if (abrir.ValueKind != JsonValueKind.Object) return list;
        if (!abrir.TryGetProperty("referenciasNormativas", out var arr) || arr.ValueKind != JsonValueKind.Array)
            return list;

        foreach (var item in arr.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) continue;

            string? normType = null, number = null, description = null;
            if (item.TryGetProperty("norma", out var norma) && norma.ValueKind == JsonValueKind.Object)
            {
                normType = GetStringFromValorObject(norma, "tipoNorma");
                number = GetString(norma, "numero");
                description = GetString(norma, "descripcion");
            }
            var article = GetString(item, "articulo");
            var subsection = GetString(item, "inciso");

            if (!string.IsNullOrWhiteSpace(number) || !string.IsNullOrWhiteSpace(description))
            {
                list.Add(new CsjnApiStatuteDto(normType, number, description, article, subsection));
            }
        }

        return list;
    }

    #endregion

    #region getAllDocumentos helpers

    private static string? ExtractOfficialReference(JsonElement documentos)
    {
        if (documentos.ValueKind != JsonValueKind.Array) return null;

        foreach (var item in documentos.EnumerateArray())
        {
            var tipo = GetStringFromValorObject(item, "tipoDocumento");
            if (tipo is not "FALLO") continue;

            var tomo = GetIntNullable(item, "tomo");
            var pagina = GetIntNullable(item, "pagina");
            if (tomo.HasValue && pagina.HasValue && tomo.Value > 0 && pagina.Value > 0)
                return $"Fallos: {tomo.Value}:{pagina.Value}";
        }
        return null;
    }

    #endregion

    #region getDictamenesAnalisis helpers

    private static bool HasDictamenData(JsonElement dictamenes)
    {
        if (dictamenes.ValueKind == JsonValueKind.Array && dictamenes.GetArrayLength() > 0)
            return true;
        return false;
    }

    private static IReadOnlyList<CsjnSumarioDto> ParseSumariosStructured(JsonElement el)
    {
        var list = new List<CsjnSumarioDto>();
        if (!TryGetSumariosArray(el, out var root) || root.GetArrayLength() == 0)
            return list;

        foreach (var item in root.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) continue;
            var texto = GetString(item, "texto")
                        ?? GetString(item, "considerando");
            if (string.IsNullOrWhiteSpace(texto)) continue;

            texto = StripHtml(texto);
            var id = GetIntNullable(item, "id");
            var orden = GetInt(item, "orden");
            var tomo = GetIntNullable(item, "tomo")?.ToString();
            var pagina = GetIntNullable(item, "pagina")?.ToString();

            var keywords = ParseSumarioKeywords(item);
            list.Add(new CsjnSumarioDto(id, texto, tomo, pagina, orden, keywords));
        }
        return list;
    }

    private static IReadOnlyList<CsjnKeywordDto> ParseSumarioKeywords(JsonElement sumario)
    {
        var list = new List<CsjnKeywordDto>();
        if (!sumario.TryGetProperty("vocesSumario", out var arr) || arr.ValueKind != JsonValueKind.Array)
            return list;

        foreach (var item in arr.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) continue;
            if (!item.TryGetProperty("tipoVoz", out var tipoVoz) || tipoVoz.ValueKind != JsonValueKind.Object)
                continue;

            var code = GetInt(tipoVoz, "codigoValor", "codigo");
            var desc = GetString(tipoVoz, "valor", "descripcion");
            if (desc != null)
                list.Add(new CsjnKeywordDto(code, desc));
        }
        return list;
    }

    private static IReadOnlyList<CsjnDictamenDto> ParseDictamenes(JsonElement el)
    {
        var list = new List<CsjnDictamenDto>();
        if (el.ValueKind != JsonValueKind.Array) return list;

        foreach (var item in el.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) continue;
            var title = GetString(item, "titulo");
            var docUrl = GetString(item, "doc", "enlace");
            var docType = GetStringFromValorObject(item, "tipoDocumento")
                ?? GetString(item, "tipoDocumento");

            list.Add(new CsjnDictamenDto(title, docUrl, docType));
        }
        return list;
    }

    #endregion

    #region getSintesisAnalisis parsing

    private static IReadOnlyList<CsjnSintesisDto> ParseSintesis(JsonElement el)
    {
        var list = new List<CsjnSintesisDto>();
        if (el.ValueKind != JsonValueKind.Array) return list;

        var order = 0;
        foreach (var item in el.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) continue;
            var text = GetString(item, "titulo", "texto")
                ?? GetString(item, "doc");
            if (string.IsNullOrWhiteSpace(text)) continue;

            text = StripHtml(text);
            list.Add(new CsjnSintesisDto(text, order++));
        }
        return list;
    }

    #endregion

    #region getEnlacesAnalisis parsing

    private static IReadOnlyList<CsjnEnlaceDto> ParseEnlaces(JsonElement el)
    {
        var list = new List<CsjnEnlaceDto>();
        if (el.ValueKind != JsonValueKind.Array) return list;

        foreach (var item in el.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) continue;
            var url = GetString(item, "enlace");
            if (string.IsNullOrWhiteSpace(url)) continue;

            var description = GetString(item, "descripcion");
            var isInternal = false;
            if (item.TryGetProperty("interno", out var internoProp))
            {
                isInternal = internoProp.ValueKind == JsonValueKind.True
                    || (internoProp.TryGetInt32(out var intVal) && intVal == 1);
            }

            list.Add(new CsjnEnlaceDto(url, description, isInternal));
        }
        return list;
    }

    #endregion

    #region getCitantes HTML parsing

    /// <summary>
    /// Parses getCitantes response. Handles both HTML string format (real API) and JSON array fallback.
    /// </summary>
    private IReadOnlyList<CsjnCitedByDto> ParseCitedBy(JsonElement el)
    {
        // Real API returns a JSON string containing HTML
        if (el.ValueKind == JsonValueKind.String)
        {
            var html = el.GetString();
            if (string.IsNullOrWhiteSpace(html))
                return [];

            return ParseCitedByHtml(html);
        }

        // Fallback: structured JSON array (hypothetical)
        if (el.ValueKind == JsonValueKind.Array)
        {
            return ParseCitedByJsonArray(el);
        }

        if (el.ValueKind == JsonValueKind.Object)
        {
            return ParseCitedByJsonObject(el);
        }

        return [];
    }

    /// <summary>
    /// Parses HTML from getCitantes to extract analysisId and caseNumber from links.
    /// </summary>
    private IReadOnlyList<CsjnCitedByDto> ParseCitedByHtml(string html)
    {
        var list = new List<CsjnCitedByDto>();

        foreach (Match match in CitantesLinkRegex().Matches(html))
        {
            var analysisId = match.Groups["id"].Value;
            var text = match.Groups["text"].Value.Trim();
            text = StripHtml(text);

            if (!string.IsNullOrWhiteSpace(analysisId) && !string.IsNullOrWhiteSpace(text))
            {
                list.Add(new CsjnCitedByDto(analysisId, text));
            }
        }

        _logger.LogDebug("Parsed {Count} citedBy entries from HTML", list.Count);
        return list;
    }

    [GeneratedRegex(@"idAnalisis=(?<id>\d+)[^>]*>(?<text>[^<]+)<", RegexOptions.Compiled)]
    private static partial Regex CitantesLinkRegex();

    private static IReadOnlyList<CsjnCitedByDto> ParseCitedByJsonArray(JsonElement el)
    {
        var list = new List<CsjnCitedByDto>();
        foreach (var item in el.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) continue;
            var analysisId = GetString(item, "idAnalisis", "analysisId", "IdAnalisis");
            var caseNumber = GetString(item, "numeroCausa", "caseNumber", "numero", "identificadorExpediente");
            if (analysisId != null)
                list.Add(new CsjnCitedByDto(analysisId, caseNumber ?? ""));
        }
        return list;
    }

    private static IReadOnlyList<CsjnCitedByDto> ParseCitedByJsonObject(JsonElement el)
    {
        foreach (var arrName in new[] { "Record", "Records", "citantes", "citedBy" })
        {
            if (el.TryGetProperty(arrName, out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                return ParseCitedByJsonArray(arr);
            }
        }
        return [];
    }

    #endregion

    #region JSON helpers

    private static JsonElement? GetFirstArrayElement(JsonElement el)
    {
        if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() > 0)
            return el[0];
        return null;
    }

    private static string? GetString(JsonElement el, params string[] keys)
    {
        if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() > 0)
            return GetString(el[0], keys);

        if (el.ValueKind != JsonValueKind.Object)
            return null;

        foreach (var key in keys)
        {
            if (el.TryGetProperty(key, out var prop))
            {
                var val = GetStringFromElement(prop);
                if (!string.IsNullOrWhiteSpace(val))
                    return val.Trim();
            }
        }
        return null;
    }

    private static string? GetStringFromElement(JsonElement prop)
    {
        if (prop.ValueKind == JsonValueKind.String)
            return prop.GetString();
        if (prop.ValueKind == JsonValueKind.Number)
        {
            if (prop.TryGetInt64(out var n)) return n.ToString();
            if (prop.TryGetDouble(out var d)) return ((long)d).ToString();
        }
        if (prop.ValueKind == JsonValueKind.Object)
            return GetString(prop, "valor", "value", "descripcion", "description", "codigo", "nombre", "texto");
        return null;
    }

    private static string? GetStringCaseInsensitive(JsonElement el, params string[] keys)
    {
        if (el.ValueKind != JsonValueKind.Object)
            return null;
        var keySet = new HashSet<string>(keys, StringComparer.OrdinalIgnoreCase);
        foreach (var prop in el.EnumerateObject())
        {
            if (keySet.Contains(prop.Name))
            {
                var val = GetStringFromElement(prop.Value);
                if (!string.IsNullOrWhiteSpace(val))
                    return val.Trim();
            }
        }
        return null;
    }

    private static string? GetStringFromNestedWrappers(JsonElement el, params string[] keys)
    {
        foreach (var wrapper in new[] { "data", "response", "Record", "Records", "Result", "result", "analisis", "causa", "Analisis", "Causa" })
        {
            if (!el.TryGetProperty(wrapper, out var nested))
                continue;
            if (nested.ValueKind == JsonValueKind.Object)
            {
                var s = GetString(nested, keys) ?? GetStringFromValorObject(nested, "caratula") ?? GetStringCaseInsensitive(nested, keys);
                if (s != null) return s;
            }
            if (nested.ValueKind == JsonValueKind.Array && nested.GetArrayLength() > 0)
            {
                var s = GetString(nested[0], keys) ?? GetStringFromValorObject(nested[0], "caratula") ?? GetStringCaseInsensitive(nested[0], keys);
                if (s != null) return s;
            }
        }
        return null;
    }

    private static string? GetStringFromFirstRecord(JsonElement el, params string[] keys)
    {
        if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() > 0)
            return GetString(el[0], keys);
        if (el.ValueKind != JsonValueKind.Object)
            return null;

        if (el.TryGetProperty("Record", out var record) && record.ValueKind == JsonValueKind.Array && record.GetArrayLength() > 0)
            return GetString(record[0], keys);
        if (el.TryGetProperty("Records", out var records) && records.ValueKind == JsonValueKind.Array && records.GetArrayLength() > 0)
            return GetString(records[0], keys);
        if (el.TryGetProperty("Result", out var result) && result.ValueKind == JsonValueKind.Object)
            return GetString(result, keys);
        return null;
    }

    private static string? GetStringFromRecordObject(JsonElement el, params string[] keys)
    {
        if (el.TryGetProperty("Record", out var record) && record.ValueKind == JsonValueKind.Object)
        {
            var s = GetString(record, keys);
            if (s != null) return s;
        }
        if (el.TryGetProperty("Result", out var result) && result.ValueKind == JsonValueKind.Object)
            return GetString(result, keys);
        return null;
    }

    private static string? GetStringFromNested(JsonElement el, string nestedKey, params string[] keys)
    {
        if (!el.TryGetProperty(nestedKey, out var nested) || nested.ValueKind != JsonValueKind.Object)
            return null;
        foreach (var key in keys)
        {
            if (nested.TryGetProperty(key, out var prop))
            {
                var val = GetStringFromElement(prop);
                if (!string.IsNullOrWhiteSpace(val))
                    return val.Trim();
            }
        }
        return null;
    }

    private static string? GetStringFromValorObject(JsonElement el, string objectKey)
    {
        if (!el.TryGetProperty(objectKey, out var obj) || obj.ValueKind != JsonValueKind.Object)
            return null;
        return GetString(obj, "valor", "value", "descripcion", "description");
    }

    private static bool GetBool(JsonElement el, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (el.TryGetProperty(key, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.True) return true;
                if (prop.ValueKind == JsonValueKind.False) return false;
                var s = GetStringFromElement(prop);
                if (!string.IsNullOrEmpty(s))
                    return s.Equals("true", StringComparison.OrdinalIgnoreCase) || s == "1";
            }
        }
        return false;
    }

    private static int GetInt(JsonElement el, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (el.TryGetProperty(key, out var prop))
            {
                if (prop.TryGetInt32(out var i))
                    return i;
                var s = GetStringFromElement(prop);
                if (s != null && int.TryParse(s, out var parsed))
                    return parsed;
            }
        }
        return 0;
    }

    private static int? GetIntNullable(JsonElement el, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (el.TryGetProperty(key, out var prop))
            {
                if (prop.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
                    return null;
                if (prop.TryGetInt32(out var i))
                    return i;
                var s = GetStringFromElement(prop);
                if (string.IsNullOrEmpty(s))
                    return null;
                if (int.TryParse(s, out var parsed))
                    return parsed;
            }
        }
        return null;
    }

    #endregion

    #region Date parsing

    /// <summary>
    /// Parses ruling date with improved fallback chain:
    /// 1. falloDestacado.fecha (epoch ms) — actual ruling date
    /// 2. reciboEntrada.fechaString (last resort — this is the receipt date, not ruling date)
    /// </summary>
    private DateOnly ParseRulingDate(JsonElement el, string context)
    {
        // Prefer falloDestacado.fecha (the actual ruling date)
        var fdDate = ExtractFalloDestacadoDate(el);
        if (fdDate.HasValue)
            return fdDate.Value;

        // Standard date field search
        var s = GetString(el, "fecha", "rulingDate", "fechaFallido", "date", "fechaSentencia")
            ?? GetStringFromFirstRecord(el, "fecha", "rulingDate", "fechaFallido", "date", "fechaSentencia")
            ?? GetStringFromRecordObject(el, "fecha", "rulingDate", "fechaFallido", "date", "fechaSentencia")
            ?? GetStringFromNested(el, "reciboEntrada", "fechaString", "fecha");
        if (string.IsNullOrWhiteSpace(s))
            throw new CsjnSchemaViolationException("abrirAnalisis: ruling date (fecha) is required", SourceId, context);

        if (long.TryParse(s, out var timestampMs) && timestampMs > 1000000000000)
        {
            return DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).DateTime);
        }

        if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            return d;
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDt))
            return DateOnly.FromDateTime(parsedDt);

        var formats = new[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "yyyy-MM-dd" };
        if (DateOnly.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exactDate))
            return exactDate;

        throw new CsjnSchemaViolationException($"abrirAnalisis: invalid date format '{s}'", SourceId, context);
    }

    #endregion

    #region Voces / Keywords

    private static IReadOnlyList<CsjnKeywordDto> ParseVoces(JsonElement abrir)
    {
        var list = new List<CsjnKeywordDto>();
        if (abrir.ValueKind != JsonValueKind.Object)
            return list;

        if (!abrir.TryGetProperty("voces", out var voces) || voces.ValueKind != JsonValueKind.Array)
            return list;

        foreach (var item in voces.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) continue;
            if (!item.TryGetProperty("tipoVoz", out var tipoVoz) || tipoVoz.ValueKind != JsonValueKind.Object)
                continue;

            var code = GetInt(tipoVoz, "codigoValor", "codigo");
            var desc = GetString(tipoVoz, "valor", "descripcion");
            if (desc != null)
                list.Add(new CsjnKeywordDto(code, desc));
        }

        return list;
    }

    private static IReadOnlyList<CsjnKeywordDto> ParseKeywords(JsonElement el)
    {
        var list = new List<CsjnKeywordDto>();

        if (el.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in el.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object) continue;
                var code = GetInt(item, "codigoValor", "externalCode", "codigo");
                var desc = GetString(item, "descripcion", "description", "valor");
                if (desc != null)
                    list.Add(new CsjnKeywordDto(code, desc));
            }
            return list;
        }

        if (el.ValueKind != JsonValueKind.Object)
            return list;

        foreach (var arrName in new[] { "Record", "Records", "palabrasClave", "keywords", "valores" })
        {
            if (el.TryGetProperty(arrName, out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.EnumerateArray())
                {
                    var code = GetInt(item, "codigoValor", "externalCode", "codigo");
                    var desc = GetString(item, "descripcion", "description", "valor");
                    if (desc != null)
                        list.Add(new CsjnKeywordDto(code, desc));
                }
                break;
            }
        }

        return list;
    }

    #endregion

    #region Citations

    private static IReadOnlyList<CsjnCitationDto> ParseCitations(JsonElement el)
    {
        var list = new List<CsjnCitationDto>();

        if (el.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in el.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object) continue;
                var alias = GetString(item, "alias", "cita", "referencia", "referenciaNormativa");
                if (alias != null)
                {
                    var summaryId = GetIntNullable(item, "idSumario", "summaryId");
                    var falloId = GetIntNullable(item, "idFallo");
                    var citationText = GetString(item, "textoCita");
                    list.Add(new CsjnCitationDto(alias, summaryId, falloId, citationText));
                }
            }
            return list;
        }

        if (el.ValueKind != JsonValueKind.Object)
            return list;

        foreach (var arrName in new[] { "Record", "Records", "citas", "citations" })
        {
            if (el.TryGetProperty(arrName, out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.EnumerateArray())
                {
                    var alias = GetString(item, "alias", "cita", "referencia", "referenciaNormativa");
                    if (alias != null)
                    {
                        var summaryId = GetIntNullable(item, "idSumario", "summaryId");
                        var falloId = GetIntNullable(item, "idFallo");
                        var citationText = GetString(item, "textoCita");
                        list.Add(new CsjnCitationDto(alias, summaryId, falloId, citationText));
                    }
                }
                break;
            }
        }

        return list;
    }

    #endregion

    #region Document validation

    private static bool ValidateDocumentExists(JsonElement el, string documentId)
    {
        if (el.ValueKind != JsonValueKind.Object && el.ValueKind != JsonValueKind.Array)
            return false;

        if (el.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in el.EnumerateArray())
            {
                var codigo = GetString(item, "Codigo", "codigo", "id", "idDocumento");
                if (codigo == documentId)
                    return true;
            }
            return false;
        }

        foreach (var arrName in new[] { "Record", "Records", "Result", "documentos", "Documentos" })
        {
            if (el.TryGetProperty(arrName, out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.EnumerateArray())
                {
                    var codigo = GetString(item, "Codigo", "codigo", "id", "idDocumento");
                    if (codigo == documentId)
                        return true;
                }
            }
        }

        return false;
    }

    #endregion

    #region Utility

    private static string StripHtml(string html)
    {
        if (string.IsNullOrEmpty(html)) return html;
        var text = HtmlTagRegex().Replace(html, " ");
        text = System.Net.WebUtility.HtmlDecode(text);
        text = WhitespaceRegex().Replace(text, " ");
        return text.Trim();
    }

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    #endregion
}
