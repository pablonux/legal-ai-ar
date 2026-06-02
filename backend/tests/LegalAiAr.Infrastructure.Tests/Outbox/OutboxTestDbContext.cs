using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Tests.Outbox;

/// <summary>
/// Test DbContext that maps <see cref="TestPingAggregate"/> for outbox integration tests only.
/// </summary>
internal sealed class OutboxTestDbContext : AppDbContext
{
    public OutboxTestDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestPingAggregate> TestPingAggregates => Set<TestPingAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestPingAggregate>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Ignore(e => e.DomainEvents);
            builder.ToTable("TestPingAggregates");
        });
    }
}
