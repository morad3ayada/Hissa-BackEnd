using FluentValidation;
using LearningPlatform.Application.Features.Authentication.Commands;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Authentication.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
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
            .IsInEnum().WithMessage("Invalid role")
            .Must(role => role is UserRole.Student or UserRole.Instructor or UserRole.Parent)
            .WithMessage("Only Student, Instructor, and Parent roles are allowed for registration.");
    }
}
