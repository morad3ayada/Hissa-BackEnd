using FluentValidation;
using LearningPlatform.Application.Features.Authentication.Commands;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Authentication.Validators;

public class AdminCreateUserCommandValidator : AbstractValidator<AdminCreateUserCommand>
{
    private static readonly UserRole[] AllowedRoles = [UserRole.Instructor, UserRole.Student, UserRole.Parent];

    public AdminCreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256);

        RuleFor(x => x.Password).MustBeAStrongPassword();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Role)
            .IsInEnum()
            .Must(role => AllowedRoles.Contains(role))
            .WithMessage($"Role must be one of: {string.Join(", ", AllowedRoles)}.");
    }
}
