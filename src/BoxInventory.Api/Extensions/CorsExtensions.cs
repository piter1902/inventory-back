namespace BoxInventory.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (allowedOrigins.Contains("*"))
                    policy.AllowAnyOrigin();
                else
                    policy.WithOrigins(allowedOrigins);

                policy.AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }
}
