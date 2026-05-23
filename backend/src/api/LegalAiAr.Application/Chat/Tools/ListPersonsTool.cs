using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

public sealed class ListPersonsTool : IChatTool
{
    private const int MaxResults = 50;

    public string Name => "list_persons";

    public string Description =>
        "List persons who have participated in rulings (judges, prosecutors, etc.). Use when the user asks about people involved in rulings.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "courtName": { "type": "string", "description": "Filter by court name." }
          }
        }
        """;

    public async Task<string> ExecuteAsync(
        string argumentsJson, ToolExecutionContext context, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var root = doc.RootElement;

        var courtName = GetOptionalString(root, "courtName");

        var personRepo = context.Services.GetRequiredService<IPersonRepository>();
        var persons = await personRepo.ListWithRulingCountAsync(courtName, MaxResults, cancellationToken);

        return FormatResults(persons);
    }

    private static string FormatResults(IReadOnlyList<PersonWithCount> persons)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[list_persons: {persons.Count} persons]");

        if (persons.Count == 0)
        {
            sb.AppendLine("No persons found matching the criteria.");
            return sb.ToString();
        }

        sb.AppendLine();
        for (var i = 0; i < persons.Count; i++)
        {
            var p = persons[i];
            sb.AppendLine($"{i + 1}. {p.FirstName} {p.LastName} | Rulings: {p.RulingCount}");
        }

        return sb.ToString();
    }

    private static string? GetOptionalString(JsonElement root, string property) =>
        root.TryGetProperty(property, out var el) && el.ValueKind == JsonValueKind.String
            ? el.GetString()
            : null;
}
