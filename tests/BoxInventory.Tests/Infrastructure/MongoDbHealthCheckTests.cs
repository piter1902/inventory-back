using BoxInventory.Infrastructure.HealthChecks;
using BoxInventory.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace BoxInventory.Tests.Infrastructure;

public class MongoDbHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenPingSucceeds_ReturnsHealthy()
    {
        var dbContext = new Mock<MongoDbContext>(Options.Create(new MongoDbOptions
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "test",
        }))
        { CallBase = true };
        dbContext.Setup(c => c.PingAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var healthCheck = new MongoDbHealthCheck(dbContext.Object);

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenPingThrows_ReturnsUnhealthy()
    {
        var dbContext = new Mock<MongoDbContext>(Options.Create(new MongoDbOptions
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "test",
        }))
        { CallBase = true };
        dbContext.Setup(c => c.PingAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("connection timed out"));

        var healthCheck = new MongoDbHealthCheck(dbContext.Object);

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Exception.Should().BeOfType<TimeoutException>();
    }
}
