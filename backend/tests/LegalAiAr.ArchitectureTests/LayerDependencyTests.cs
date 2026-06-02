using System.Reflection;
using LegalAiAr.Agents;
using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Extensions;
using LegalAiAr.Core.Entities;
using NetArchTest.Rules;

namespace LegalAiAr.ArchitectureTests;

public class LayerDependencyTests
{
    private static readonly Assembly CoreAssembly = typeof(Ruling).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(ApplicationServiceExtensions).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(LegalAiAr.Infrastructure.ServiceCollectionExtensions).Assembly;
    private static readonly Assembly ApiAssembly = typeof(IEndpoint).Assembly;
    private static readonly Assembly AgentsAssembly = typeof(AgentsAssemblyMarker).Assembly;

    [Fact]
    public void Core_should_not_reference_Application_Infrastructure_or_Api()
    {
        var result = Types.InAssembly(CoreAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LegalAiAr.Application",
                "LegalAiAr.Infrastructure",
                "LegalAiAr.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailingTypes(result));
    }

    [Fact]
    public void Application_should_not_reference_Api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("LegalAiAr.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailingTypes(result));
    }

    [Fact]
    public void Handlers_should_reside_in_Application()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .ResideInNamespace("LegalAiAr.Application")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailingTypes(result));
    }

    [Fact]
    public void Core_entities_should_not_reference_Agents()
    {
        var result = Types.InAssembly(CoreAssembly)
            .That()
            .ResideInNamespace("LegalAiAr.Core.Entities")
            .ShouldNot()
            .HaveDependencyOn("LegalAiAr.Agents")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailingTypes(result));
    }

    [Fact]
    public void Agents_should_not_reference_Infrastructure_or_Api()
    {
        var result = Types.InAssembly(AgentsAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("LegalAiAr.Infrastructure", "LegalAiAr.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailingTypes(result));
    }

    private static string FormatFailingTypes(NetArchTest.Rules.TestResult result) =>
        result.IsSuccessful
            ? string.Empty
            : string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>());
}
