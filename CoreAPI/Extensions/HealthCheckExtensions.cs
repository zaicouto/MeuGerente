using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace CoreAPI.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck("api_alive", () => HealthCheckResult.Healthy("API is running"))
            .AddMongoDb(
                sp => sp.GetRequiredService<IMongoClient>(),
                name: "mongodb",
                timeout: TimeSpan.FromSeconds(5),
                tags: ["db", "nosql"]
            );

        return services;
    }
}
