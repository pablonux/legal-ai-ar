namespace LegalAiAr.Core.Models;

/// <summary>
/// Result of persisting an entity to the Knowledge Base.
/// </summary>
/// <param name="EntityId">The persisted entity's primary key (Ruling.Id or Statute.Id).</param>
/// <param name="IsNew">True if the entity was created, false if updated.</param>
public record PersistResult(Guid EntityId, bool IsNew);
