using System.Text.Json.Serialization;
using LearningPlatform.API.Extensions;
using LearningPlatform.API.Middleware;
using LearningPlatform.API.Swagger;
using LearningPlatform.API.Versioning;
using LearningPlatform.Application;
using LearningPlatform.Infrastructure;
using LearningPlatform.Persistence;
using LearningPlatform.Persistence.Context;
using LearningPlatform.Persistence.Identity.Seed;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Learning Platform API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Layer registrations.
    builder.Services.AddApplicationServices();
    builder.Services.AddPersistenceServices(builder.Configuration);
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Presentation services.
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddApiVersioningServices();
    builder.Services.AddSwaggerServices();
    builder.Services.AddHealthCheckServices(builder.Configuration);

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddJwtAuthentication(builder.Configuration);

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        await IdentitySeeder.SeedAsync(scope.ServiceProvider);
    }

    app.UseSwaggerDocumentation();

    app.UseSerilogRequestLogging();

    app.UseExceptionHandler();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapHealthCheckEndpoints();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Learning Platform API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Exposed so WebApplicationFactory<Program> can bootstrap this API in integration tests.
public partial class Program;
