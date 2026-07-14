namespace LearningPlatform.API.Extensions;

public static class HealthCheckServiceExtensions
{
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddHealthChecks()
            .AddSqlServer(
                connectionString!,
                name: "sql-server",
                tags: ["db", "sql", "ready"]);

        return services;
    }
}
