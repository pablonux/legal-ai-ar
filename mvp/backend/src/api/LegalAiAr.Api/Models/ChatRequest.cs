namespace LegalAiAr.Api.Models;

/// <summary>
/// Request body for POST /api/chat.
/// </summary>
/// <param name="Query">User's legal question.</param>
public record ChatRequest(string? Query);
