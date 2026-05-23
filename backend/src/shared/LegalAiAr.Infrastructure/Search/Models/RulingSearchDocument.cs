using System.Text.Json.Serialization;

namespace LegalAiAr.Infrastructure.Search.Models;

/// <summary>
/// Document model for the rulings-by-ruling Azure AI Search index.
/// Matches the schema created by LegalAiAr.SetupSearch.
/// </summary>
public class RulingSearchDocument
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("rulingId")]
    public string RulingId { get; set; } = string.Empty;

    [JsonPropertyName("caseTitle")]
    public string CaseTitle { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("holding")]
    public string? Holding { get; set; }

    [JsonPropertyName("rulingDate")]
    public DateTimeOffset? RulingDate { get; set; }

    [JsonPropertyName("jurisdictionArea")]
    public string? JurisdictionArea { get; set; }

    [JsonPropertyName("instance")]
    public string? Instance { get; set; }

    [JsonPropertyName("court")]
    public string? Court { get; set; }

    [JsonPropertyName("courtType")]
    public string? CourtType { get; set; }

    [JsonPropertyName("fuero")]
    public string? Fuero { get; set; }

    [JsonPropertyName("instanceLevel")]
    public int? InstanceLevel { get; set; }

    [JsonPropertyName("keywords")]
    public IList<string> Keywords { get; set; } = new List<string>();

    [JsonPropertyName("caseNumber")]
    public string? CaseNumber { get; set; }

    [JsonPropertyName("rulingDirection")]
    public string? RulingDirection { get; set; }

    [JsonPropertyName("persons")]
    public IList<string> Persons { get; set; } = new List<string>();

    [JsonPropertyName("statutes")]
    public IList<string> Statutes { get; set; } = new List<string>();

    [JsonPropertyName("subjectArea")]
    public string? SubjectArea { get; set; }

    [JsonPropertyName("legalBranch")]
    public string? LegalBranch { get; set; }

    [JsonPropertyName("precedentWeight")]
    public string? PrecedentWeight { get; set; }

    [JsonPropertyName("isPlenario")]
    public bool IsPlenario { get; set; }

    [JsonPropertyName("isLeadingCase")]
    public bool IsLeadingCase { get; set; }

    [JsonPropertyName("resourceType")]
    public string? ResourceType { get; set; }

    [JsonPropertyName("isUnconstitutional")]
    public bool IsUnconstitutional { get; set; }

    [JsonPropertyName("actionType")]
    public string? ActionType { get; set; }

    [JsonPropertyName("officialReference")]
    public string? OfficialReference { get; set; }

    [JsonPropertyName("federalQuestion")]
    public string? FederalQuestion { get; set; }

    [JsonPropertyName("hasDictamen")]
    public bool HasDictamen { get; set; }

    [JsonPropertyName("embedding")]
    public IReadOnlyList<float>? Embedding { get; set; }
}
