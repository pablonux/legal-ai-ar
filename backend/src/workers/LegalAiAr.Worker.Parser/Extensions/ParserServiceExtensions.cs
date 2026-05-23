using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Crawling;
using LegalAiAr.Infrastructure.Pipeline;
using LegalAiAr.Worker.Parser.Parsers;
using LegalAiAr.Worker.Parser.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Parser.Extensions;

public static class ParserServiceExtensions
{
    public static IServiceCollection AddParserServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCsjnSjconsultaApiClient(configuration);

        services.AddSingleton<ITextNormalizer, PdfTextNormalizer>();
        services.AddScoped<PdfTextExtractor>();
        services.AddScoped<IBlobPdfExtractor, BlobPdfExtractor>();

        services.AddScoped<CsjnApiParser>();

        services.AddScoped<IStrategyResolver<IParseStrategy>>(sp =>
        {
            var blobStorage = sp.GetRequiredService<IBlobStorageService>();
            var builder = new StrategyResolverBuilder<IParseStrategy>();

            builder.Register(EntityType.Ruling, 1,
                new CsjnRulingParseStrategy(
                    sp.GetRequiredService<CsjnApiParser>(),
                    sp.GetRequiredService<IBlobPdfExtractor>(),
                    blobStorage,
                    sp.GetRequiredService<ILogger<CsjnRulingParseStrategy>>()));

            builder.Register(EntityType.Statute, 2,
                new SaijLegislationParseStrategy(
                    blobStorage,
                    sp.GetRequiredService<ILogger<SaijLegislationParseStrategy>>()));

            builder.Register(EntityType.Ruling, 3,
                new SaijRulingParseStrategy(
                    blobStorage,
                    sp.GetRequiredService<ILogger<SaijRulingParseStrategy>>()));

            return builder.Build();
        });

        // Backward compat: IParserProcessor still used by old tools/requeue handlers
        services.AddScoped<SaijLegislationParser>();
        services.AddScoped<SaijRulingParser>();
        services.AddScoped<IParserProcessor, ParserProcessor>();

        return services;
    }
}
