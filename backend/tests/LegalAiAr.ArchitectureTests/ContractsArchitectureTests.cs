using System.Reflection;
using LegalAiAr.Contracts.Responses.Auth;

namespace LegalAiAr.ArchitectureTests;

public class ContractsArchitectureTests
{
    private static readonly Assembly ContractsAssembly = typeof(MeResponse).Assembly;

    [Fact]
    public void Contracts_should_not_reference_other_LegalAiAr_assemblies()
    {
        var illegalReferences = ContractsAssembly
            .GetReferencedAssemblies()
            .Select(a => a.Name)
            .Where(name => name is not null && name.StartsWith("LegalAiAr", StringComparison.Ordinal))
            .ToList();

        Assert.True(
            illegalReferences.Count == 0,
            $"LegalAiAr.Contracts must not reference other LegalAiAr projects. Found: {string.Join(", ", illegalReferences)}");
    }
}
