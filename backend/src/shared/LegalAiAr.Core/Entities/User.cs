namespace LegalAiAr.Core.Entities;

/// <summary>
/// Admin user for Phase 1. Phase 3: sync with Entra ID.
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Role { get; set; } = string.Empty; // admin, lawyer, viewer
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
