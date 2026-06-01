using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LegalAiAr.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for EF Core migrations. Reads connection string from
/// AzureSql__ConnectionString env var or appsettings.
/// </summary>
internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var apiPath = Path.Combine(basePath, "../../api/LegalAiAr.Api");
        if (!Directory.Exists(apiPath))
        {
            apiPath = Path.Combine(basePath, "../LegalAiAr.Api");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration["AzureSql:ConnectionString"]
            ?? Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
            ?? "Server=localhost,1433;Database=LegalAiAr;User Id=sa;Password=Dev_Password123!;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
