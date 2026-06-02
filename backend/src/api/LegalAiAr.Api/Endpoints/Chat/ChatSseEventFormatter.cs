using LegalAiAr.Core.Models;

namespace LegalAiAr.Api.Endpoints.Chat;

internal static class ChatSseEventFormatter
{
    public static string? Format(ChatStreamEvent evt) =>
        evt switch
        {
            ChatTextChunk text =>
                "data: " + text.Text.Replace("\n", "\ndata: ") + "\n\n",
            ChatToolStart toolStart =>
                $"event: tool_start\ndata: {{\"tool\":\"{Escape(toolStart.ToolName)}\"}}\n\n",
            ChatToolEnd toolEnd =>
                $"event: tool_end\ndata: {{\"tool\":\"{Escape(toolEnd.ToolName)}\",\"resultCount\":{toolEnd.ResultCount}}}\n\n",
            ChatValidationEvent validation =>
                $"event: validation\ndata: {{\"status\":\"{Escape(validation.Status)}\",\"citationsChecked\":{validation.CitationsChecked},\"valid\":{validation.Valid},\"warnings\":{validation.Warnings},\"details\":[{string.Join(",", validation.Details.Select(d => $"\"{Escape(d)}\""))}]}}\n\n",
            ChatNormalizedResponse normalized =>
                "event: normalized\ndata: " + normalized.Text.Replace("\n", "\ndata: ") + "\n\n",
            _ => null
        };

    private static string Escape(string value) =>
        value.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
