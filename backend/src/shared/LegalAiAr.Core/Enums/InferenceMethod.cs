namespace LegalAiAr.Core.Enums;

/// <summary>
/// How a field value was obtained during ingestion (audit/provenance tracking).
/// </summary>
public enum InferenceMethod
{
    SourceApi,
    AiFallback,
    AiPrimary,
    Heuristic,
    Derived,
    Embedding,
    Manual,
    Migration
}
