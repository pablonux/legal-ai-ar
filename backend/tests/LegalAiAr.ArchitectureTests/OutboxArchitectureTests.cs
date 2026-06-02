using LegalAiAr.Api.Interfaces;
using LegalAiAr.Core.Domain;
using NetArchTest.Rules;

namespace LegalAiAr.ArchitectureTests;

public class OutboxArchitectureTests
{
    private static readonly System.Reflection.Assembly ApiAssembly = typeof(IEndpoint).Assembly;
    private static readonly System.Reflection.Assembly CoreAssembly = typeof(IDomainEvent).Assembly;

    [Fact]
    public void Api_should_not_define_IDomainEvent_types()
    {
        var domainEventsInApi = Types.InAssembly(ApiAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .GetTypes()
            .ToList();

        Assert.True(
            domainEventsInApi.Count == 0,
            "Domain event types must live in Core or feature domain folders, not in the Api layer. Found: "
            + string.Join(", ", domainEventsInApi.Select(t => t.FullName)));
    }

    [Fact]
    public void IDomainEvent_implementations_in_Core_should_reside_in_Domain_namespace()
    {
        var result = Types.InAssembly(CoreAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .ResideInNamespace("LegalAiAr.Core.Domain")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailingTypes(result));
    }

    private static string FormatFailingTypes(NetArchTest.Rules.TestResult result) =>
        result.IsSuccessful
            ? string.Empty
            : "Failing types: " + string.Join(", ", result.FailingTypeNames ?? []);
}
