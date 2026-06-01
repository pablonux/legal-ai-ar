using LegalAiAr.Agents;

namespace LegalAiAr.AgentEvals;

public class AgentEvalsPlaceholderTests
{
    [Fact]
    public void AgentsAssembly_IsReferenced()
    {
        Assert.Equal("LegalAiAr.Agents", typeof(AgentsAssemblyMarker).Assembly.GetName().Name);
    }
}
