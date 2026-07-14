using FluentValidation;

namespace LearningPlatform.Application.Features.Authentication.Validators;

public static class PasswordValidatorExtensions
{
    /// <summary>
    /// Mirrors the ASP.NET Identity password policy configured in Persistence.DependencyInjection,
    /// so invalid passwords are rejected by FluentValidation before ever reaching UserManager.
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeAStrongPassword<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
}
