using LegalAiAr.Application.Chat.Tools;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LegalAiAr.Application.Tests.Chat.Tools;

public class ToolRegistryTests
{
    private static IChatTool CreateFakeTool(string name, string result = "ok")
    {
        var tool = Substitute.For<IChatTool>();
        tool.Name.Returns(name);
        tool.Description.Returns($"Fake {name}");
        tool.ParametersSchema.Returns("{}");
        tool.ExecuteAsync(Arg.Any<string>(), Arg.Any<ToolExecutionContext>(), Arg.Any<CancellationToken>())
            .Returns(result);
        return tool;
    }

    [Fact]
    public void GetToolDefinitions_ReturnsAllRegisteredTools()
    {
        var tools = new[] { CreateFakeTool("tool_a"), CreateFakeTool("tool_b") };
        var registry = new ToolRegistry(tools, Substitute.For<ILogger<ToolRegistry>>());

        var defs = registry.GetToolDefinitions();

        Assert.Equal(2, defs.Count);
        Assert.Contains(defs, d => d.Name == "tool_a");
        Assert.Contains(defs, d => d.Name == "tool_b");
    }

    [Fact]
    public async Task ExecuteAsync_KnownTool_DelegatesToTool()
    {
        var tool = CreateFakeTool("my_tool", "tool result");
        var registry = new ToolRegistry([tool], Substitute.For<ILogger<ToolRegistry>>());
        var context = new ToolExecutionContext { Services = Substitute.For<IServiceProvider>() };

        var result = await registry.ExecuteAsync("my_tool", "{}", context);

        Assert.Equal("tool result", result);
        await tool.Received(1).ExecuteAsync("{}", context, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_UnknownTool_ReturnsError()
    {
        var registry = new ToolRegistry([], Substitute.For<ILogger<ToolRegistry>>());
        var context = new ToolExecutionContext { Services = Substitute.For<IServiceProvider>() };

        var result = await registry.ExecuteAsync("nonexistent", "{}", context);

        Assert.Contains("Unknown tool", result);
        Assert.Contains("nonexistent", result);
    }

    [Fact]
    public async Task ExecuteAsync_ToolThrows_ReturnsErrorMessage()
    {
        var tool = Substitute.For<IChatTool>();
        tool.Name.Returns("failing_tool");
        tool.Description.Returns("Fails");
        tool.ParametersSchema.Returns("{}");
        tool.ExecuteAsync(Arg.Any<string>(), Arg.Any<ToolExecutionContext>(), Arg.Any<CancellationToken>())
            .Returns<string>(_ => throw new InvalidOperationException("boom"));

        var registry = new ToolRegistry([tool], Substitute.For<ILogger<ToolRegistry>>());
        var context = new ToolExecutionContext { Services = Substitute.For<IServiceProvider>() };

        var result = await registry.ExecuteAsync("failing_tool", "{}", context);

        Assert.Contains("Error executing failing_tool", result);
        Assert.Contains("boom", result);
    }

    [Fact]
    public async Task ExecuteAsync_CaseInsensitive()
    {
        var tool = CreateFakeTool("Search_Rulings", "found");
        var registry = new ToolRegistry([tool], Substitute.For<ILogger<ToolRegistry>>());
        var context = new ToolExecutionContext { Services = Substitute.For<IServiceProvider>() };

        var result = await registry.ExecuteAsync("search_rulings", "{}", context);

        Assert.Equal("found", result);
    }
}
