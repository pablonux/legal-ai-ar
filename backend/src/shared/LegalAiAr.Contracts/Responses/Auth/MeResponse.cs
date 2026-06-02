namespace LegalAiAr.Contracts.Responses.Auth;

public record MeResponse(string Email, string DisplayName, string Role, string Groups);
