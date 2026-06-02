using LegalAiAr.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;

namespace LegalAiAr.ArchitectureTests;

public class EndpointArchitectureTests
{
    private static readonly System.Reflection.Assembly ApiAssembly = typeof(IEndpoint).Assembly;

    [Fact]
    public void Endpoints_should_reside_in_Endpoints_namespace()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .ImplementInterface(typeof(IEndpoint))
            .Should()
            .ResideInNamespace("LegalAiAr.Api.Endpoints")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailingTypes(result));
    }

    [Fact]
    public void Api_should_not_define_ControllerBase_types()
    {
        var controllers = Types.InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(ControllerBase))
            .GetTypes()
            .ToList();

        Assert.True(
            controllers.Count == 0,
            "MVC controllers are removed; use IEndpoint in LegalAiAr.Api.Endpoints. Found: "
            + string.Join(", ", controllers.Select(t => t.Name)));
    }

    private static string FormatFailingTypes(NetArchTest.Rules.TestResult result) =>
        result.IsSuccessful
            ? string.Empty
            : string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>());
}
