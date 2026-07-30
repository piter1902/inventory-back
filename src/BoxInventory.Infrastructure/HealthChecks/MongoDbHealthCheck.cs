using BoxInventory.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BoxInventory.Infrastructure.HealthChecks;

public class MongoDbHealthCheck(MongoDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.PingAsync(cancellationToken);
            return HealthCheckResult.Healthy("MongoDB connection is healthy.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB connection failed.", ex);
        }
    }
}
