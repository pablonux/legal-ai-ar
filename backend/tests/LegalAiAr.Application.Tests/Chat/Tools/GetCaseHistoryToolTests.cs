using LegalAiAr.Application.Chat.Tools;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Chat.Tools;

public class GetCaseHistoryToolTests
{
    private static ToolExecutionContext CreateContext(Action<ServiceCollection> configure)
    {
        var services = new ServiceCollection();
        configure(services);
        return new ToolExecutionContext { Services = services.BuildServiceProvider() };
    }

    [Fact]
    public async Task Execute_InvalidId_ReturnsError()
    {
        var tool = new GetCaseHistoryTool();
        var ctx = CreateContext(sc => sc.AddSingleton(Substitute.For<IJudicialProceedingRepository>()));

        var result = await tool.ExecuteAsync("""{"rulingId":"not-a-guid"}""", ctx, CancellationToken.None);

        Assert.Contains("Error", result);
    }

    [Fact]
    public async Task Execute_NoProceeding_ReturnsNotFound()
    {
        var repo = Substitute.For<IJudicialProceedingRepository>();
        var rulingId = Guid.NewGuid();
        repo.GetByRulingIdAsync(rulingId, Arg.Any<CancellationToken>()).Returns((JudicialProceeding?)null);
        var tool = new GetCaseHistoryTool();
        var ctx = CreateContext(sc => sc.AddSingleton(repo));

        var result = await tool.ExecuteAsync($$"""{ "rulingId": "{{rulingId}}" }""", ctx, CancellationToken.None);

        Assert.Contains("no proceeding", result);
    }

    [Fact]
    public async Task Execute_WithProceeding_ReturnsChain()
    {
        var rulingId = Guid.NewGuid();
        var otherRulingId = Guid.NewGuid();
        var court = new Court { Name = "CSJN", InstanceLevel = 3, JurisdictionArea = "Federal", Territory = "Nacional", Instance = "CSJN" };
        var proceeding = new JudicialProceeding
        {
            Id = 1,
            CaseNumber = "CSJ 123/2024",
            RulingCount = 2,
            Rulings = new List<Ruling>
            {
                new() { Id = rulingId, CaseTitle = "Current", RulingDate = new DateOnly(2024, 6, 1), Court = court },
                new() { Id = otherRulingId, CaseTitle = "Previous", RulingDate = new DateOnly(2023, 3, 1), Court = new Court { Name = "Cámara", InstanceLevel = 2, JurisdictionArea = "Federal", Territory = "Nacional", Instance = "Cámara" } },
            }
        };

        var repo = Substitute.For<IJudicialProceedingRepository>();
        repo.GetByRulingIdAsync(rulingId, Arg.Any<CancellationToken>()).Returns(proceeding);

        var tool = new GetCaseHistoryTool();
        var ctx = CreateContext(sc => sc.AddSingleton(repo));

        var result = await tool.ExecuteAsync($$"""{ "rulingId": "{{rulingId}}" }""", ctx, CancellationToken.None);

        Assert.Contains("CSJ 123/2024", result);
        Assert.Contains("CSJN", result);
        Assert.Contains("← current", result);
        Assert.Contains(rulingId, ctx.ResolvedRulingIds);
        Assert.Contains(otherRulingId, ctx.ResolvedRulingIds);
    }

    [Fact]
    public void Tool_HasCorrectNameAndSchema()
    {
        var tool = new GetCaseHistoryTool();
        Assert.Equal("get_case_history", tool.Name);
        Assert.Contains("rulingId", tool.ParametersSchema);
    }
}
