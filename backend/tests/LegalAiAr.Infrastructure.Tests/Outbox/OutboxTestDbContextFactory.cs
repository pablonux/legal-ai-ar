using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LegalAiAr.Infrastructure.Tests.Outbox;

internal static class OutboxTestDbContextFactory
{
    public static OutboxTestDbContext Create(string databaseName)
    {
        var interceptor = new DispatchDomainEventsInterceptor();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .AddInterceptors(interceptor)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new OutboxTestDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
