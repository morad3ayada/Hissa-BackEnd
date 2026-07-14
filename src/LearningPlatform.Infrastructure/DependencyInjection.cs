using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Authentication.Interfaces;
using LearningPlatform.Infrastructure.Cache;
using LearningPlatform.Infrastructure.Certificates;
using LearningPlatform.Infrastructure.Email;
using LearningPlatform.Infrastructure.FileStorage;
using LearningPlatform.Infrastructure.Services;
using LearningPlatform.Shared.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LearningPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        var cacheSettings = configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>() ?? new CacheSettings();
        services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));

        if (cacheSettings.UseDistributedCache && !string.IsNullOrWhiteSpace(cacheSettings.RedisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
                options.Configuration = cacheSettings.RedisConnectionString);
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddScoped<ICacheService, CacheService>();

        services.AddScoped<IEmailService, LoggingEmailService>();

        // NOTE: ISmsService and IPaymentService are defined as contracts in
        // Application.Common.Interfaces. Concrete (vendor-backed) implementations will be
        // registered here once the corresponding providers are selected. INotificationService
        // is registered in Application's own DI (it's a pure DB-backed service, no vendor).

        services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddSingleton<ICertificatePdfGenerator, QuestPdfCertificateGenerator>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
