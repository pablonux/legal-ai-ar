using System.Text.Json.Serialization;

namespace LegalAiAr.Infrastructure.Search.Models;

/// <summary>
/// Document model for the statutes Azure AI Search index.
/// </summary>
public class StatuteSearchDocument
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("statuteId")]
    public int StatuteId { get; set; }

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("normType")]
    public string? NormType { get; set; }

    [JsonPropertyName("normativeLevel")]
    public string? NormativeLevel { get; set; }

    [JsonPropertyName("legalBranch")]
    public string? LegalBranch { get; set; }

    [JsonPropertyName("issuingBody")]
    public string? IssuingBody { get; set; }

    [JsonPropertyName("sanctionDate")]
    public DateTimeOffset? SanctionDate { get; set; }

    [JsonPropertyName("publicationDate")]
    public DateTimeOffset? PublicationDate { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("isVigente")]
    public bool IsVigente { get; set; }

    [JsonPropertyName("fullText")]
    public string? FullText { get; set; }

    [JsonPropertyName("saijId")]
    public string? SaijId { get; set; }

    [JsonPropertyName("rulingCount")]
    public int RulingCount { get; set; }
}
