using System.Reflection;
using FluentValidation;
using LearningPlatform.Application.Common.Behaviors;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Services;
using LearningPlatform.Application.Features.Chat.Interfaces;
using LearningPlatform.Application.Features.Chat.Services;
using LearningPlatform.Application.Features.LiveSessions.Interfaces;
using LearningPlatform.Application.Features.LiveSessions.Services;
using LearningPlatform.Application.Features.Progress.Interfaces;
using LearningPlatform.Application.Features.Progress.Services;
using LearningPlatform.Application.Features.Quizzes.Interfaces;
using LearningPlatform.Application.Features.Quizzes.Services;
using LearningPlatform.Application.Features.Wallet.Interfaces;
using LearningPlatform.Application.Features.Wallet.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LearningPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(cfg => { }, assembly);

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddScoped<ICourseDurationRecalculator, CourseDurationRecalculator>();
        services.AddScoped<ILessonAccessService, LessonAccessService>();
        services.AddScoped<ICourseProgressCalculator, CourseProgressCalculator>();
        services.AddScoped<IQuizAuthorizationService, QuizAuthorizationService>();
        services.AddScoped<IGamificationService, GamificationService>();
        services.AddScoped<ILiveSessionAccessService, LiveSessionAccessService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ICourseCompletionService, CourseCompletionService>();
        services.AddScoped<IChatQueryService, ChatQueryService>();
        services.AddScoped<IWalletService, WalletService>();

        return services;
    }
}
