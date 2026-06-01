namespace LegalAiAr.Core.Enums;

/// <summary>
/// How an entity is mentioned within a text chunk (for GraphRAG local search).
/// </summary>
public enum MentionType
{
    Named,
    Applied,
    Discussed,
    Referenced
}
