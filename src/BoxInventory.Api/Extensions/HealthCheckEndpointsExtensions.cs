using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace BoxInventory.Api.Extensions;

public static class HealthCheckEndpointsExtensions
{
    public static WebApplication MapHealthCheckEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
        });

        return app;
    }
}
