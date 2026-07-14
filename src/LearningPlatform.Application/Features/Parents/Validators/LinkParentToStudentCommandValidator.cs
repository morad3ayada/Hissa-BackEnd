using FluentValidation;
using LearningPlatform.Application.Features.Parents.Commands;

namespace LearningPlatform.Application.Features.Parents.Validators;

public class LinkParentToStudentCommandValidator : AbstractValidator<LinkParentToStudentCommand>
{
    public LinkParentToStudentCommandValidator()
    {
        RuleFor(x => x.ParentId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.RelationshipType).MaximumLength(50);
    }
}
