using BoxInventory.Application.Common.Interfaces;
using BoxInventory.Domain.Interfaces;
using BoxInventory.Infrastructure.Persistence;
using BoxInventory.Infrastructure.Repositories;
using BoxInventory.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BoxInventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.SectionName));

        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IBoxRepository, BoxRepository>();
        services.AddScoped<IZoneRepository, ZoneRepository>();
        services.AddScoped<IImageCompressionService, ImageCompressionService>();
        services.AddScoped<IExcelReaderService, ExcelReaderService>();
        services.AddScoped<IItemMovementLogRepository, ItemMovementLogRepository>();

        return services;
    }
}
