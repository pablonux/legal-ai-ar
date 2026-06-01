namespace LegalAiAr.Core.Entities;

public class WorkerPauseState
{
    public string WorkerType { get; set; } = string.Empty;
    public bool IsPaused { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
