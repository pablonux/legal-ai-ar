namespace LegalAiAr.Core.Entities;

/// <summary>
/// Catalog of judicial sources supported by the system.
/// CSJN=1, SAIJ=2 (jurisprudencia/legislación web), PJN=3, SCBA=4, SAIJ tesauro API=6 (misma marca SAIJ, vocabulario TemaTres).
/// </summary>
public class Source
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Strategy { get; set; } = string.Empty; // "api-first" | "html-pdf"
    public bool IsActive { get; set; }

    public ICollection<Ruling> Rulings { get; set; } = new List<Ruling>();
    public ICollection<IngestionJob> IngestionJobs { get; set; } = new List<IngestionJob>();
    public ICollection<CrawlerConfig> CrawlerConfigs { get; set; } = new List<CrawlerConfig>();
}
