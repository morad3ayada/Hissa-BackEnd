using FluentValidation;
using LearningPlatform.Application.Features.Parents.Commands;

namespace LearningPlatform.Application.Features.Parents.Validators;

public class LinkMyStudentCommandValidator : AbstractValidator<LinkMyStudentCommand>
{
    public LinkMyStudentCommandValidator()
    {
        RuleFor(x => x.StudentEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.RelationshipType)
            .MaximumLength(50)
            .When(x => x.RelationshipType is not null);
    }
}
