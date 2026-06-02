using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Catalogs.Mapping;

public static class PersonListViewParser
{
    public static PersonListView Parse(string? vista) =>
        vista?.Trim().ToLowerInvariant() switch
        {
            "magistrados" => PersonListView.Magistrates,
            "partes" => PersonListView.Parties,
            _ => PersonListView.All
        };
}
